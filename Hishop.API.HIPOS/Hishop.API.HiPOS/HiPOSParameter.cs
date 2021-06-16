using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.API.HiPOS
{
    public sealed class HiPOSParameter
    {
        /// <summary>
        /// 获取商户API密钥
        /// </summary>
        public readonly static string GETAUTH = "http://openapi.hishop.com.cn:10800/openapi/auth/hishop";

        /// <summary>
        /// 获取Access Token
        /// </summary>
        public readonly static string GETTOKEN = "http://openapi.hishop.com.cn:10800/openapi/token";

        /// <summary>
        /// 更新商户资料
        /// </summary>
        public readonly static string UPDATEMERCHANTS = "http://openapi.hishop.com.cn:10800/openapi/merchants/{0}";

        /// <summary>
        /// 获取支付宝开发者公钥
        /// </summary>
        public readonly static string ALIPAYKEY = "http://openapi.hishop.com.cn:10800/openapi/merchants/{0}/alipaykey";

        /// <summary>
        /// 更新 HiShopO2O 功能接口设置
        /// </summary>
        public readonly static string HISHOPO2O = "http://openapi.hishop.com.cn:10800/openapi/merchants/{0}/hishopo2o";

        /// <summary>
        /// 更新支付方式
        /// </summary>
        public readonly static string PAYMENTS = "http://openapi.hishop.com.cn:10800/openapi/merchants/{0}/payments";

        /// <summary>
        /// 请求生成设备授权码
        /// </summary>
        public readonly static string AUTHCODE = "http://openapi.hishop.com.cn:10800/openapi/merchants/{0}/hishop/authcode";

        /// <summary>
        /// 查询交易统计
        /// </summary>
        public readonly static string HISHOPTRADES = "http://openapi.hishop.com.cn:10800/openapi/merchants/{0}/hishop/trades";

        /// <summary>
        /// 查询交易详情
        /// </summary>
        public readonly static string STOREDETAIL = "http://openapi.hishop.com.cn:10800/openapi/merchants/{0}/hishop/trades/store/{1}/detail";
        public struct HttpMethod
        {
            public static readonly string GET = "GET";
            public static readonly string POST = "POST";
            public static readonly string PUT = "PUT";            
        }
    }
}
