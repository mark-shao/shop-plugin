using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Weixin.Pay.Domain
{
    /// <summary>
    /// 退款信息
    /// </summary>
    public class RefundInfo
    {
        /// <summary>
        /// 退款信息构造函数
        /// </summary>
        public RefundInfo()
        {
            BankType = "WX";
            FeeType = "1";
            InputCharset = "UTF-8";
        }

        /// <summary>
        /// 银行通道类型
        /// </summary>
        public string BankType { get; private set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public string nonce_str { get; set; }

        /// <summary>
        /// 操作员，默认为商户号（mchid）
        /// </summary>
        public string op_user_id { get; private set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 商户退款单号
        /// </summary>
        public string out_refund_no { get; set; }
        /// <summary>
        /// 支付币种
        /// </summary>
        public string FeeType { get; private set; }

        /// <summary>
        /// 通知URL
        /// </summary>
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal? TotalFee { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal? RefundFee { get; set; }
        /// <summary>
        /// 退款ID
        /// </summary>
        public string RefundID { get; set; }
        /// <summary>
        /// 传入参数字符编码
        /// </summary>
        public string InputCharset { get; private set; }
    }
}
