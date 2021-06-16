using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml;
using System.Web.Caching;
using System.Collections;

namespace Hishop.Plugins
{
    public abstract class PluginContainer
    {

        protected static volatile Cache pluginCache = HttpRuntime.Cache;

        protected abstract string PluginLocalPath { get; }
        protected abstract string PluginVirtualPath { get; }
        protected abstract string IndexCacheKey { get; }
        protected abstract string TypeCacheKey { get; }
        
        protected PluginContainer()
        {
            // 重新创建某类插件的实例时，需清空此类插件的缓存
            pluginCache.Remove(IndexCacheKey);
            pluginCache.Remove(TypeCacheKey);
        }

        protected void VerifyIndex()
        {
            if (pluginCache.Get(IndexCacheKey) == null)
            {
                XmlDocument catalog = new XmlDocument();
                XmlNode mapNode = catalog.CreateElement("Plugins");

                BuildIndex(catalog, mapNode);
                catalog.AppendChild(mapNode);

                pluginCache.Insert(IndexCacheKey, catalog, new CacheDependency(PluginLocalPath));
            }
        }

        private void BuildIndex(XmlDocument catalog, XmlNode mapNode)
        {
            if (!Directory.Exists(PluginLocalPath))
                return;

            string[] pluginDlls = Directory.GetFiles(PluginLocalPath, "*.dll", SearchOption.AllDirectories);
            string interfaceName = typeof(IPlugin).FullName;

            foreach (string file in pluginDlls)
            {
                Assembly assembly = Assembly.Load(LoadPlugin(file));
                foreach (Type t in assembly.GetExportedTypes())
                {
                    if (CheckIsPlugin(t, interfaceName))
                    {
                        AddPlugin(t, file, catalog, mapNode);
                    }
                }
            }
        }

        private Type GetPlugin(string baseName, string name, string attname)
        {
            // 先从缓存中查找
            Hashtable lookupTable = GetPluginCache();
            name = name.ToLower();
            Type pluginType = lookupTable[name] as Type;

            if (pluginType == null)
            {
                // 如果缓存中没有找到，则从文件中加载
                if (pluginCache.Get(IndexCacheKey) == null)
                    return null;

                // 查找索引
                XmlDocument catalog = pluginCache.Get(IndexCacheKey) as XmlDocument;
                XmlNode node = catalog.DocumentElement.SelectSingleNode("//" + baseName + "/item[@" + attname + "='" + name + "']");

                if (node == null || !File.Exists(node.Attributes["file"].Value))
                    return null;

                Assembly assembly = Assembly.Load(LoadPlugin(node.Attributes["file"].Value));
                pluginType = assembly.GetType(node.Attributes["identity"].Value, false, true);

                // 如果从文件加载成功，则将类型加入缓存
                if (pluginType != null)
                    lookupTable[name] = pluginType;
            }

            return pluginType;
        }

        internal virtual Type GetPlugin(string baseName, string name)
        {
            return GetPlugin(baseName, name, "identity");
        }

        internal virtual Type GetPluginWithNamespace(string baseName, string name)
        {
            return GetPlugin(baseName, name, "namespace");
        }

        private Hashtable GetPluginCache()
        {
            Hashtable lookupTable = pluginCache.Get(TypeCacheKey) as Hashtable;
            if (lookupTable == null)
            {
                lookupTable = new Hashtable();
                pluginCache.Insert(TypeCacheKey, lookupTable, new CacheDependency(PluginLocalPath));
            }
            return lookupTable;
        }

        #region build helper
        private void AddPlugin(Type t, string filename, XmlDocument catalog, XmlNode mapNode)
        {
            XmlNode pluginType = mapNode.SelectSingleNode(t.BaseType.Name);
            if (pluginType == null)
            {
                pluginType = catalog.CreateElement(t.BaseType.Name);
                mapNode.AppendChild(pluginType);
            }

            XmlNode pluginNode = catalog.CreateElement("item");

            XmlAttribute identityAtt = catalog.CreateAttribute("identity");
            identityAtt.Value = t.FullName.ToLower();
            pluginNode.Attributes.Append(identityAtt);

            XmlAttribute fileAtt = catalog.CreateAttribute("file");
            fileAtt.Value = filename;
            pluginNode.Attributes.Append(fileAtt);

            PluginAttribute att = (PluginAttribute)Attribute.GetCustomAttribute(t, typeof(PluginAttribute));

            if (att != null)
            {
                XmlAttribute nameAtt = catalog.CreateAttribute("name");
                nameAtt.Value = att.Name;
                pluginNode.Attributes.Append(nameAtt);

                XmlAttribute seqAtt = catalog.CreateAttribute("seq");
                seqAtt.Value = (att.Sequence > 0) ? att.Sequence.ToString(CultureInfo.InvariantCulture) : "0";
                pluginNode.Attributes.Append(seqAtt);

                ConfigablePlugin cp = Activator.CreateInstance(t) as ConfigablePlugin;

                XmlAttribute logoAtt = catalog.CreateAttribute("logo");
                if (string.IsNullOrEmpty(cp.Logo) || cp.Logo.Trim().Length == 0)
                    logoAtt.Value = "";
                else
                    logoAtt.Value = PluginVirtualPath + "/images/" + cp.Logo.Trim();
                pluginNode.Attributes.Append(logoAtt);

                XmlAttribute saAtt = catalog.CreateAttribute("shortDescription");
                saAtt.Value = cp.ShortDescription;
                pluginNode.Attributes.Append(saAtt);

                XmlAttribute desAtt = catalog.CreateAttribute("description");
                desAtt.Value = cp.Description;
                pluginNode.Attributes.Append(desAtt);
            }

            XmlAttribute spaceAtt = catalog.CreateAttribute("namespace");
            spaceAtt.Value = t.Namespace.ToLower();
            pluginNode.Attributes.Append(spaceAtt);

            if (att != null && att.Sequence > 0)
            {
                XmlNode node = FindNode(pluginType.ChildNodes, att.Sequence);
                if (node == null)
                    pluginType.AppendChild(pluginNode);
                else
                    pluginType.InsertBefore(pluginNode, node);
            }
            else
            {
                pluginType.AppendChild(pluginNode);
            }
        }

        private static XmlNode FindNode(XmlNodeList nodeList, int sequence)
        {
            if (nodeList == null || nodeList.Count == 0 || sequence <= 0)
                return null;

            for (int index = 0; index < nodeList.Count; index++)
            {
                if (int.Parse(nodeList[index].Attributes["seq"].Value) > sequence)
                    return nodeList[index];
            }

            return null;
        }

        private static byte[] LoadPlugin(string filename)
        {
            byte[] buffer;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                buffer = new byte[(int)fs.Length];
                fs.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }

        private static bool CheckIsPlugin(Type t, string interfaceName)
        {
            try
            {
                if (t == null
                 || !t.IsClass
                 || !t.IsPublic
                 || t.IsAbstract
                 || t.GetInterface(interfaceName) == null)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        public abstract PluginItemCollection GetPlugins();

        public abstract PluginItem GetPluginItem(string fullName);

        protected PluginItem GetPluginItem(string baseName, string fullName)
        {
            PluginItem item = null;
            XmlDocument catalog = pluginCache.Get(IndexCacheKey) as XmlDocument;
            XmlNode node = catalog.SelectSingleNode("//" + baseName + "/item[@identity='" + fullName + "']");

            if (node != null)
            {
                item = new PluginItem
                {
                    FullName = node.Attributes["identity"].Value,
                    DisplayName = node.Attributes["name"].Value,
                    Logo = node.Attributes["logo"].Value,
                    ShortDescription = node.Attributes["shortDescription"].Value,
                    Description = node.Attributes["description"].Value
                };
            }

            return item;
        }

        protected PluginItemCollection GetPlugins(string baseName)
        {
            PluginItemCollection plugins = new PluginItemCollection();

            XmlDocument catalog = pluginCache.Get(IndexCacheKey) as XmlDocument;
            XmlNodeList nodes = catalog.SelectNodes("//" + baseName + "/item");

            if (nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    PluginItem item = new PluginItem
                    {
                        FullName = node.Attributes["identity"].Value,
                        DisplayName = node.Attributes["name"].Value,
                        Logo = node.Attributes["logo"].Value,
                        ShortDescription = node.Attributes["shortDescription"].Value,
                        Description = node.Attributes["description"].Value
                    };
                    plugins.Add(item);
                }
            }

            return plugins;
        }

    }
}