using System.Collections.Generic;
using System.Xml;

namespace Hishop.Plugins
{
    /// <summary>
    /// 插件配置信息实体类
    /// </summary>
    public class ConfigData
    {

        private readonly XmlDocument doc;
        private readonly XmlNode root;

        public ConfigData()
        {
            IsValid = true;
            ErrorMsgs = new List<string>();
            doc = new XmlDocument();
            root = doc.CreateElement("xml");
            doc.AppendChild(root);
        }

        public ConfigData(string xml)
        {
            IsValid = true;
            ErrorMsgs = new List<string>();
            doc = new XmlDocument();
            doc.LoadXml(xml);
            root = doc.FirstChild;
        }

        public bool IsValid { get; internal set; }

        public IList<string> ErrorMsgs { get; private set; }

        public bool NeedProtect { get; internal set; }

        internal XmlNodeList AttributeNodes
        {
            get { return root.ChildNodes; }
        }

        public string SettingsXml
        {
            get { return root.OuterXml; }
        }

        internal void Add(string attributeName, string val)
        {
            if (
                !string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(val) && 
                attributeName.Trim().Length > 0 && val.Length > 0
                )
            {
                XmlNode node = doc.CreateElement(attributeName);
                node.InnerText = val;
                root.AppendChild(node);
            }
        }

    }
}