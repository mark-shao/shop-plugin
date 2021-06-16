using System;

namespace Hishop.Alipay.OpenHome.AlipayOHException
{
    /// <summary>
    /// 支付宝服务窗请求异常
    /// </summary>
    public class RequestException : AlipayOpenHomeException
    {
        /// <summary>
        /// 支付宝服务窗请求异常
        /// </summary>
        public RequestException() { }

        /// <summary>
        /// 支付宝服务窗请求异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public RequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
