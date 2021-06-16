using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.Plugins.Refund
{
    public class ResponseResult
    {
        /// <summary>
        /// 响应状态
        /// </summary>
        public ResponseStatus Status { get; set; }
        /// <summary>
        /// 响应编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 响应子编码
        /// </summary>
        public string SubCode { get; set; }
        /// <summary>
        /// 返回子信息
        /// </summary>
        public string SubMsg { get; set; }
        /// <summary>
        /// 交易订单号
        /// </summary>
        public string TradeNo { get; set; }
        /// <summary>
        /// 原始返回结果
        /// </summary>
        public string OriginalResult { get; set; }
        /// <summary>
        /// 退款手续费
        /// </summary>
        public decimal RefundCharge { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OutTradeNo { get; set; }

    }

    public enum ResponseStatus
    {
        /// <summary>
        /// 请求失败
        /// </summary>
        Failed = 0,
        /// <summary>
        /// 请求成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 请求异常
        /// </summary>
        Error = 2
    }
}
