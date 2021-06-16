using Hishop.Alipay.OpenHome.Utility;
using System;
using System.Xml.Serialization;

namespace Hishop.Alipay.OpenHome.Model
{
    [XmlRoot("xml")]
    public class Message
    {
        
        string toUserId;
        string agreementId;
        string appId;
        string msgType;



        [XmlElement("ToUserId",typeof(CData))]
        public CData ToUserId { get { return toUserId; } set { toUserId = value; } }

        [XmlElement("AgreementId", typeof(CData))]
        public CData AgreementId { get { return agreementId; } set { agreementId = value; } }

        [XmlElement("AppId", typeof(CData))]
        public CData AppId { get { return appId; } set { appId = value; } }

        [XmlElement("CreateTime")]
        public string CreateTime { get; set; }

        [XmlElement("MsgType", typeof(CData))]
        public CData MsgType { get { return msgType; } set { msgType = value; } }

        [XmlElement("ArticleCount")]
        public int ArticleCount { get; set; }

        [XmlElement("Articles")]
        public Articles Articles { get; set; }


    }
}
