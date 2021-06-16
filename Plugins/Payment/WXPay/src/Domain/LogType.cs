using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Weixin.Pay.Domain
{
    /// <summary>
    /// 日志类型枚举
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 在线支付
        /// </summary>
        Pay = 0,
        /// <summary>
        /// 支付通知
        /// </summary>
        PayNotify = 1,
        /// <summary>
        /// 原生支付
        /// </summary>
        NativePay,
        /// <summary>
        /// 原生支付通知
        /// </summary>
        NativePayNotify,
        /// <summary>
        /// 扫码支付
        /// </summary>
        MicroPay,
        /// <summary>
        /// 扫码支付通知
        /// </summary>
        MicroPayNotify,
        /// <summary>
        /// 退款支付
        /// </summary>
        Refund,
        /// <summary>
        /// 退款支付通知
        /// </summary>
        RefundNotify,
        /// <summary>
        /// 退款查询
        /// </summary>
        RefundQuery,
        /// <summary>
        /// 订单支付
        /// </summary>
        OrderQuery,
        /// <summary>
        /// 下载对帐单
        /// </summary>
        DownLoadBill,
        /// <summary>
        /// 关闭订单,撤销订单
        /// </summary>
        CloseOrder,
        /// <summary>
        /// 获取Token和OpenID
        /// </summary>
        GetTokenOrOpenID,
        /// <summary>
        /// 获取或者编辑收货地址
        /// </summary>
        GetOrEditAddress,
        /// <summary>
        /// 转换成短地址
        /// </summary>
        ShortUrl,
        /// <summary>
        /// 统一下订单
        /// </summary>
        UnifiedOrder,
        /// <summary>
        /// 接口信息上报
        /// </summary>
        Report,
        /// <summary>
        /// 错误信息
        /// </summary>
        Error,
        /// <summary>
        ///统一下单时获取PrepayId
        /// </summary>
        GetPrepayID,
        
    }
}
