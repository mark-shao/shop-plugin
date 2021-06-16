using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Globalization;

namespace Hishop.Plugins
{
    public class PluginItemCollection
    {
        private Dictionary<string, PluginItem> plugins = new Dictionary<string, PluginItem>();

        public void Add(PluginItem item)
        {
            if (!plugins.ContainsKey(item.FullName))
                plugins.Add(item.FullName, item);
        }

        public void Remove(string fullName)
        {
            plugins.Remove(fullName);
        }

        public bool ContainsKey(string fullName)
        {
            return plugins.ContainsKey(fullName);
        }

        public int Count
        {
            get { return plugins.Count; }
        }

        public PluginItem[] Items
        {
            get
            {
                PluginItem[] items = new PluginItem[plugins.Count];
                plugins.Values.CopyTo(items, 0);
                return items;
            }
        }

        public virtual string ToJsonString()
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");

            jsonBuilder.AppendFormat("\"qty\":{0}", plugins.Count.ToString(CultureInfo.InvariantCulture));
            if (plugins.Count > 0)
            {
                jsonBuilder.Append(",\"items\":[");
                foreach (string fullName in plugins.Keys)
                {
                    PluginItem item = plugins[fullName];
                    jsonBuilder.Append("{");
                    jsonBuilder.AppendFormat(
                        "\"FullName\":\"{0}\",\"DisplayName\":\"{1}\",\"Logo\":\"{2}\",\"ShortDescription\":\"{3}\",\"Description\":\"{4}\"",
                        item.FullName, item.DisplayName, item.Logo, item.ShortDescription, item.Description);
                    jsonBuilder.Append("},");
                }

                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("]");
            }

            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        public virtual string ToXmlString()
        {
            StringBuilder xmlBuilder = new StringBuilder();
            xmlBuilder.Append("<xml>");
            xmlBuilder.AppendFormat("<qty>{0}</qty>", plugins.Count.ToString(CultureInfo.InvariantCulture));

            foreach (string fullName in plugins.Keys)
            {
                PluginItem item = plugins[fullName];
                xmlBuilder.Append("<item>");
                xmlBuilder.AppendFormat(
                    "<FullName>{0}</FullName><DisplayName>{1}</DisplayName><Logo>{2}</Logo>" +
                    "<ShortDescription>{3}</ShortDescription><Description>{4}</Description>",
                    item.FullName, item.DisplayName, item.Logo, item.ShortDescription, item.Description);
                xmlBuilder.Append("</item>");
            }

            xmlBuilder.Append("</xml>");
            return xmlBuilder.ToString();
        }

    }
}