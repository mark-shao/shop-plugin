using System;
using System.Collections.Generic;

namespace Hishop.Weixin.Pay.Domain
{
    /// <summary>
    /// 支付请求实体类
    /// </summary>
    public class PayRequestInfo
    {
        /// <summary>
        /// 公众号Id
        /// </summary>
        public string appId { get; set; }

        /// <summary>
        /// 订单详情扩展字符串
        /// </summary>
        public string package { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string timeStamp { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string nonceStr { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string paySign { get; set; }
        public string prepayid { get; set; }
        /// <summary>
        /// 签名方式
        /// </summary>
        public string signType {
            get { return "MD5"; }
        }
        /// <summary>
        /// 获取h5支付链接
        /// </summary>
        public string mweb_url { get; set; }
    }
}
