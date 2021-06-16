using System;
using System.Collections.Generic;

namespace Hishop.Weixin.Pay.Notify
{
    /// <summary>
    /// 告警通知实体类
    /// </summary>
    public class AlarmNotify : NotifyObject
    {
        /// <summary>
        /// 公众号Id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 告警通知错误类型
        /// </summary>
        //public AlarmErrorType ErrorType { get; set; }
        public int ErrorType { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 错误详情
        /// </summary>
        public string AlarmContent { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        //public DateTime TimeStamp { get; set; }
        public long TimeStamp { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string AppSignature { get; set; }

        /// <summary>
        /// 签名方法
        /// </summary>
        public string SignMethod { get; set; }
    }

    /// <summary>
    /// 告警通知错误类型
    /// </summary>
    public enum AlarmErrorType
    {
        发货超时 = 1001
    }
}
