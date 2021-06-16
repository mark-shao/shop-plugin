using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayTrustUserBasicinfoVerifyGetResponse.
    /// </summary>
    public class AlipayTrustUserBasicinfoVerifyGetResponse : AopResponse
    {
        /// <summary>
        /// {       姓名：1/0 （1：匹配上 0：没匹配上 2:支付宝没有收录该信息）       身份证信息:  1/0  （用户没传的字段，不匹配，也不在结果中显示该字段）  }
        /// </summary>
        [XmlElement("verify_info")]
        public string VerifyInfo { get; set; }
    }
}
