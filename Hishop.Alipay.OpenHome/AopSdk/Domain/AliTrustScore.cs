using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AliTrustScore Data Structure.
    /// </summary>
    [Serializable]
    public class AliTrustScore : AopObject
    {
        /// <summary>
        /// ่้บปๅ
        /// </summary>
        [XmlElement("score")]
        public long Score { get; set; }
    }
}
