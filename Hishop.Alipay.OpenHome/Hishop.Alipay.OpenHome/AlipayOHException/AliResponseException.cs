using System;

namespace Hishop.Alipay.OpenHome.AlipayOHException
{
    public class AliResponseException : AlipayOpenHomeException
    {
        public string ResponseCode{set;get;}

        /// <summary>
        /// 支付宝服务窗响应异常
        /// </summary>
        public AliResponseException() { }

        /// <summary>
        /// 支付宝服务窗响应异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public AliResponseException(string code, string message, Exception innerException)
            : base(message, innerException)
        {
            ResponseCode = code;
        }


        /// <summary>
        /// 支付宝服务窗响应异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public AliResponseException(string code, string message)
            : base(message)
        {
            ResponseCode = code;
        }

    }
}
