using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Plugins.Refund.AlipayDirect
{
    public class config
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        public static string gatewayUrl = "https://openapi.alipay.com/gateway.do";
        /// <summary>
        /// 版本号
        /// </summary>
        public static string version = "1.0";
        /// <summary>
        /// 应用appid
        /// </summary>
        public static string app_id { get; set; }
        /// <summary>
        /// 商户私钥
        /// </summary>
        public static string private_key { get; set; }
        private string _sign_type = "RSA2";
        /// <summary>
        /// 签名方式
        /// </summary>
        public static string sign_type = "RSA2";
        
        /// <summary>
        /// 支付宝公钥
        /// </summary>
        public static string alipay_public_key { get; set; }
        /// <summary>
        /// 编码格式
        /// </summary>
        public static string charset = "UTF-8";

    }
}
