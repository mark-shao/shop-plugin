using System;
using System.Collections.Generic;

namespace Hishop.Weixin.Pay.Notify
{
    /// <summary>
    /// 反馈通知实体类
    /// </summary>
    public class FeedBackNotify : NotifyObject
    {
        /// <summary>
        /// 公众号Id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        //public DateTime TimeStamp { get; set; }
        public long TimeStamp { get; set; }

        /// <summary>
        /// 支付该笔订单的用户Id
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string AppSignature { get; set; }

        /// <summary>
        /// 反馈通知类型
        /// </summary>
        //public FeedBackType MsgType { get; set; }
        public string MsgType { get; set; }

        /// <summary>
        /// 投诉单号
        /// </summary>
        public string FeedBackId { get; set; }

        /// <summary>
        /// 交易订单号
        /// </summary>
        public string TransId { get; set; }

        /// <summary>
        /// 用户投诉原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 用户希望的解决方案
        /// </summary>
        public string Solution { get; set; }

        /// <summary>
        /// 备注信息+电话
        /// </summary>
        public string ExtInfo { get; set; }
    }

    /// <summary>
    /// 反馈通知类型
    /// </summary>
    public enum FeedBackType
    {
        /// <summary>
        /// 用户提交投诉
        /// </summary>
        Request,

        /// <summary>
        /// 用户确认消除投诉
        /// </summary>
        Confirm,

        /// <summary>
        /// 用户拒绝消除投诉
        /// </summary>
        Reject
    }
}
