using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;

namespace Hishop.Plugins
{
    public abstract class ConfigablePlugin
    {

        protected ConfigablePlugin()
        {
        }

        /// <summary>
        /// 获取当前插件的配置信息元数据
        /// </summary>
        /// <returns></returns>
        internal virtual XmlDocument GetMetaData()
        {
            XmlDocument metaDoc = new XmlDocument();
            metaDoc.LoadXml("<xml></xml>");
            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                ConfigElementAttribute att =
                    (ConfigElementAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigElementAttribute));
                if (att != null)
                {
                    AppendAttrubiteNode(metaDoc, att, property);
                }
            }

            return metaDoc;
        }

        private const string RequiredMsg = "{0}为必填项";
        private const string CastMsg = "{0}的输入格式不正确，请按正确格式输入";
        /// <summary>
        /// 获取文件上传的控件名列表
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public virtual IList<string> GetFileConfigNames()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            IList<string> FileList = new List<string>();
            foreach (PropertyInfo property in properties)
            {
                ConfigElementAttribute att =
                    (ConfigElementAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigElementAttribute));
                if (att != null)
                {
                    if (att.InputType == InputType.File)
                    {
                        FileList.Add(property.Name);
                    }
                }

            }

            return FileList;
        }
        /// <summary>
        /// 获取隐藏部门显示控件名称列表
        /// </summary>
        /// <returns></returns>
        public virtual IList<string> GetHiddenPartConfigNames()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            IList<string> HiddenList = new List<string>();
            foreach (PropertyInfo property in properties)
            {
                ConfigElementAttribute att =
                    (ConfigElementAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigElementAttribute));
                if (att != null)
                {
                    if (att.HiddenPart == true)
                    {
                        HiddenList.Add(property.Name);
                    }
                }

            }

            return HiddenList;
        }
        /// <summary>
        /// 获取首个文件上传控件的名称
        /// </summary>
        /// <returns></returns>
        public virtual string GetFirstFileConfigName()
        {
            IList<string> FileList = new List<string>();
            FileList = GetFileConfigNames();
            if (FileList == null || FileList.Count == 0)
                return string.Empty;
            else
                return FileList[0];
        }
        /// <summary>
        /// 获取当前插件的配置信息结构
        /// </summary>
        /// <returns></returns>
        public virtual ConfigData GetConfigData(NameValueCollection form)
        {
            ConfigData data = new ConfigData();
            data.NeedProtect = NeedProtect;
            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                string propertyValue = form[property.Name] ?? "false";
                ConfigElementAttribute att =
                    (ConfigElementAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigElementAttribute));
                if (att != null)
                {
                    if (!att.Nullable && (string.IsNullOrEmpty(propertyValue) || propertyValue.Length == 0))
                    {
                        // 检查必填项
                        data.IsValid = false;
                        data.ErrorMsgs.Add(string.Format(RequiredMsg, att.Name));
                        continue;
                    }

                    if (!string.IsNullOrEmpty(propertyValue))
                    {
                        try
                        {
                            // 检查数据类型是否正确
                            Convert.ChangeType(propertyValue, property.PropertyType);
                        }
                        catch (FormatException)
                        {
                            data.IsValid = false;
                            data.ErrorMsgs.Add(string.Format(CastMsg, att.Name));
                            continue;
                        }
                        data.Add(property.Name, propertyValue);
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// 使用指定的配置信息初始化插件
        /// </summary>
        /// <param name="configXml">配置信息，结构和GetConfigSchema()方法返回的结构一样</param>
        protected virtual void InitConfig(XmlNode configXml)
        {
            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                ConfigElementAttribute att =
                    (ConfigElementAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigElementAttribute));
                if (att != null)
                {
                    XmlNode attNode = configXml.SelectSingleNode(property.Name);
                    if (attNode != null && !string.IsNullOrEmpty(attNode.InnerText) && attNode.InnerText.Length > 0)
                    {
                        property.SetValue(this, Convert.ChangeType(attNode.InnerText, property.PropertyType), null);
                    }
                }
            }
        }

        private static void AppendAttrubiteNode(XmlDocument doc, ConfigElementAttribute att, PropertyInfo property)
        {
            XmlNode attNode = doc.CreateElement("att");

            XmlAttribute attname = doc.CreateAttribute("Property");
            attname.Value = property.Name;
            attNode.Attributes.Append(attname);

            XmlAttribute nameAtt = doc.CreateAttribute("Name");
            nameAtt.Value = string.IsNullOrEmpty(att.Name) ? property.Name : att.Name;
            attNode.Attributes.Append(nameAtt);

            XmlAttribute desAtt = doc.CreateAttribute("Description");
            desAtt.Value = att.Description;
            attNode.Attributes.Append(desAtt);

            XmlAttribute nullAtt = doc.CreateAttribute("Nullable");
            nullAtt.Value = att.Nullable.ToString();
            attNode.Attributes.Append(nullAtt);

            XmlAttribute inputAtt = doc.CreateAttribute("InputType");
            inputAtt.Value = ((int)att.InputType).ToString();
            attNode.Attributes.Append(inputAtt);

            if (att.Options != null && att.Options.Length > 0)
            {
                XmlNode optionNode = doc.CreateElement("Options");
                foreach (string option in att.Options)
                {
                    XmlNode valueNode = doc.CreateElement("Item");
                    valueNode.InnerText = option;
                    optionNode.AppendChild(valueNode);
                }
                attNode.AppendChild(optionNode);
            }

            doc.SelectSingleNode("xml").AppendChild(attNode);
        }

        /// <summary>
        /// 获取插件配置信息是否需要加密
        /// </summary>
        protected abstract bool NeedProtect { get; }
        public abstract string Logo { get; }
        public abstract string ShortDescription { get; }
        public abstract string Description { get; }

    }
}