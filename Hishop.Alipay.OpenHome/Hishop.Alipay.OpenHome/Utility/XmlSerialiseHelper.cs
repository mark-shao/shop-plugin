using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Hishop.Alipay.OpenHome.Utility
{
    public class XmlSerialiseHelper
    {
        /// <summary>
        /// 序列化当前对象
        /// </summary>
        /// <returns></returns>
        public static string Serialise<T>(T t)
        {
            MemoryStream ms = new MemoryStream();
            //using Encoding
            StreamWriter sw = new StreamWriter(ms, Encoding.GetEncoding("GBK"));
            XmlSerializer xs = new XmlSerializer(typeof(T));
            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            //empty namespaces
            xsn.Add(String.Empty, String.Empty);
            xs.Serialize(sw, t, xsn);
            string text = Encoding.GetEncoding("GBK").GetString(ms.ToArray()).Replace("\r", "").Replace("\n", "");
            while (text.Contains(" <"))
            {
                text = text.Replace(" <", "<");
            }
            text = text.Substring(text.IndexOf("?>") + 2);
            return text;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml)
        {
            System.IO.StringReader stringReader = new System.IO.StringReader(xml);
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(stringReader);  
        }
    }
}
