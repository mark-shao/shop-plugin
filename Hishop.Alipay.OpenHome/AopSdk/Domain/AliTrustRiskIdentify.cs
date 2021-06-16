using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AliTrustRiskIdentify Data Structure.
    /// </summary>
    [Serializable]
    public class AliTrustRiskIdentify : AopObject
    {
        /// <summary>
        /// 当有风险时,为"T";无风险识别是为"F"
        /// </summary>
        [XmlElement("is_risk")]
        public string IsRisk { get; set; }

        /// <summary>
        /// 描述风险标签
        /// </summary>
        [XmlElement("risk_tag")]
        public string RiskTag { get; set; }
    }
}
