using System;

namespace Hishop.Alipay.OpenHome.AlipayOHException
{
    /// <summary>
    /// 支付宝服务窗响应异常
    /// </summary>
    public class ResponseException : AlipayOpenHomeException
    {
        /// <summary>
        /// 支付宝服务窗响应异常
        /// </summary>
        public ResponseException() { }

        /// <summary>
        /// 支付宝服务窗响应异常
        /// </summary>
        public ResponseException(string message):base(message) { }

        /// <summary>
        /// 支付宝服务窗响应异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ResponseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
