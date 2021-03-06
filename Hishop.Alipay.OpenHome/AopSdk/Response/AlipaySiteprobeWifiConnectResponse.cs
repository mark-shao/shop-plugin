using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipaySiteprobeWifiConnectResponse.
    /// </summary>
    public class AlipaySiteprobeWifiConnectResponse : AopResponse
    {
        /// <summary>
        /// 结果码
        /// </summary>
        [XmlElement("code")]
        public long Code { get; set; }

        /// <summary>
        /// 结果描述
        /// </summary>
        [XmlElement("msg")]
        public string Msg { get; set; }
    }
}
