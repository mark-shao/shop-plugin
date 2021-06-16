using System;
using System.Xml.Serialization;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayTrustUserBasicinfoGetResponse.
    /// </summary>
    public class AlipayTrustUserBasicinfoGetResponse : AopResponse
    {
        /// <summary>
        /// 用户基本信息(已废弃)
        /// </summary>
        [XmlElement("ali_trust_basicinfo")]
        public AliTrustBasicInfo AliTrustBasicinfo { get; set; }

        /// <summary>
        /// 用户基本信息
        /// </summary>
        [XmlElement("ali_trust_user_basic_info")]
        public AliTrustBasicInfo AliTrustUserBasicInfo { get; set; }
    }
}
