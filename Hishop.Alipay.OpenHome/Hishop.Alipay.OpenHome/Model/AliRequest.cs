using System.Xml.Serialization;

namespace Hishop.Alipay.OpenHome.Model
{
    /// <summary>
    /// 阿里请求
    /// </summary>
    [XmlRoot("XML")]
    public class AliRequest
    {
        /// <summary>
        /// 为开发者的公众号标识appId
        /// </summary>
        [XmlElement("AppId")]
        public string AppId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("FromUserId")]
        public string FromUserId { get; set; }

        /// <summary>
        /// 消息创建时间
        /// </summary>
        [XmlElement("CreateTime")]
        public long CreateTime{get;set;}

        [XmlElement("MsgType")]
        public string MsgType { get; set; }

        /// <summary>
        /// 消息体的子类型
        /// </summary>
        [XmlElement("EventType")]
        public string EventType { get; set; }

        [XmlElement("ActionParam")]
        public string ActionParam { get; set; }

        [XmlElement("AgreementId")]
        public string AgreementId { get; set; }

        [XmlElement("AccountNo")]
        public string AccountNo { get; set; }

    }
}
