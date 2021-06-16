using System;

namespace Hishop.Alipay.OpenHome.AlipayOHException
{
    /// <summary>
    /// 支付宝服务窗基础异常
    /// </summary>
    public class AlipayOpenHomeException:ApplicationException
    {

        public AlipayOpenHomeException() { }

        public AlipayOpenHomeException(string message):base(message) { }

        public AlipayOpenHomeException(string message, Exception innerException) : base(message, innerException) { }

    }
}
