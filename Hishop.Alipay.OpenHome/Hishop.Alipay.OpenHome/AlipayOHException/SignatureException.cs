using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Alipay.OpenHome.AlipayOHException
{
    /// <summary>
    /// 签名异常
    /// </summary>
    public class SignatureException:AlipayOpenHomeException
    {
        /// <summary>
        /// 签名异常
        /// </summary>
        public SignatureException() { }

        /// <summary>
        /// 签名异常
        /// </summary>
        public SignatureException(string message):base(message) { }

        /// <summary>
        /// 签名异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SignatureException(string message, Exception innerException) : base(message, innerException) { }
    }
}
