using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayTrustUserDatareceivedSendResponse.
    /// </summary>
    public class AlipayTrustUserDatareceivedSendResponse : AopResponse
    {
        /// <summary>
        /// 当值为T时，表示回流成功
        /// </summary>
        [XmlElement("is_successful")]
        public string IsSuccessful { get; set; }
    }
}
