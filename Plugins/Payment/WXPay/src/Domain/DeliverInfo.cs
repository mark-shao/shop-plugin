using System;
using System.Collections.Generic;

namespace Hishop.Weixin.Pay.Domain
{
    /// <summary>
    /// 发货信息
    /// </summary>
    public class DeliverInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DeliverInfo()
        {
            TimeStamp = DateTime.Now;
            Status = true;
            Message = "ok";
            SignMethod = "sha1";
        }
        /// <summary>
        /// 带参数构造函数
        /// </summary>
        public DeliverInfo(string openId, string transId, string outTradeNo)
            : this()
        {
            OpenId = openId;
            TransId = transId;
            OutTradeNo = outTradeNo;
        }

        /// <summary>
        /// 公众号Id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 支付该笔订单的用户Id
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 交易订单号
        /// </summary>
        public string TransId { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// 发货状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 发货状态信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string AppSignature { get; set; }

        /// <summary>
        /// 签名方法
        /// </summary>
        public string SignMethod { get; private set; }
    }
}
