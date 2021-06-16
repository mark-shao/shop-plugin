using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Hishop.Alipay.OpenHome.Utility
{


    public class CData : IXmlSerializable
    {
        private string m_Value;

        public CData()
        {
        }

        public CData(string p_Value)
        {
            m_Value = p_Value;
        }

        public string Value
        {
            get
            {
                return m_Value;
            }
        }

        public void ReadXml(XmlReader reader)
        {
            m_Value = reader.ReadElementContentAsString();
        }

        public void WriteXml(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(m_Value))
                writer.WriteCData(m_Value);
            else
                writer.WriteString(m_Value);
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        public override string ToString()
        {
            return m_Value;
        }

        public static implicit operator string(CData element)
        {
            return (element == null) ? null : element.m_Value;
        }

        public static implicit operator CData(string text)
        {
            return new CData(text);
        }

    }
}

