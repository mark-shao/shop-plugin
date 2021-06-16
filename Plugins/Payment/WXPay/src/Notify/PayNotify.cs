using System;
using System.Collections.Generic;
using Hishop.Weixin.Pay.Domain;

namespace Hishop.Weixin.Pay.Notify
{
    /// <summary>
    /// 支付通知实体类
    /// </summary>
    public class PayNotify : NotifyObject
    {
        /// <summary>
        /// 公众号Id
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 银行类型
        /// </summary>
        public string bank_type { get; set; }
        /// <summary>
        /// 支付币种
        /// </summary>
        public int cash_fee { get; set; }
        public string fee_type { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string mch_id { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 商户生成的随机字符串
        /// </summary>
        public string nonce_str { get; set; }

        /// <summary>
        /// 支付该笔订单的用户Id
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }

        /// <summary>
        /// 用户是否关注了公众号
        /// </summary>
        public string is_subscribe { get; set; }
        /// <summary>
        /// 结果代码
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 返回代码
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string time_end { get; set; }
        /// <summary>
        /// 子商户ID
        /// </summary>
        public string sub_mch_id { get; set; }

        /// <summary>
        /// 子商户ID
        /// </summary>
        public string sub_appid { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public int total_fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PayInfo PayInfo { get; set; }

        public string sub_openid { get; set; }

        public string sub_is_subscribe { get; set; }

        public string attach { get; set; }

        public string prepay_id { get; set; }
        /// <summary>
        /// 优惠券数量
        /// </summary>
        public string coupon_count { get; set; }
        /// <summary>
        /// 优惠总金额
        /// </summary>
        public string coupon_fee { get; set; }
        /// <summary>
        /// 优惠券1金额
        /// </summary>
        public string coupon_fee_0 { get; set; }
        /// <summary>
        /// 优惠券2金额
        /// </summary>
        public string coupon_fee_1 { get; set; }
        /// <summary>
        /// 优惠券1ID
        /// </summary>
        public string coupon_id_0 { get; set; }
        /// <summary>
        /// 优惠券2ID
        /// </summary>
        public string coupon_id_1 { get; set; }
        /// <summary>
        /// 应结订单金额
        /// </summary>
        public int settlement_total_fee { get; set; }
        /// <summary>
        /// 优惠券类型
        /// </summary>
        public string coupon_type_0 { get; set; }
    }
}
