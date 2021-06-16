using System;
using System.Collections.Generic;

namespace Hishop.Weixin.Pay.Domain
{
    /// <summary>
    /// 支付信息实体类
    /// </summary>
    public class PayInfo
    {
        public PayInfo()
        {
            SignType = "MD5";
            ServiceVersion = "1.0";
            InputCharSet = "GBK";
            SignKeyIndex = 1;
            TimeEnd = DateTime.Now;
        }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 商品标签
        /// </summary>
        public string GoodsTag { get; set; }
        /// <summary>
        /// 授权码
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// 签名方式
        /// </summary>
        public string SignType { get; set; }

        /// <summary>
        /// 接口版本
        /// </summary>
        public string ServiceVersion { get; set; }

        /// <summary>
        /// 字符集
        /// </summary>
        public string InputCharSet { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 密钥序列
        /// </summary>
        public int SignKeyIndex { get; set; }

        //以上为协议参数，以下为业务参数

        /// <summary>
        /// 交易模式
        /// </summary>
        public int TradeMode { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public int TradeState { get; set; }

        /// <summary>
        /// 支付结果信息
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string Partner { get; set; }

        /// <summary>
        /// 付款银行类型
        /// </summary>
        public string BankType { get; set; }

        /// <summary>
        /// 银行订单号
        /// </summary>
        public string BankBillNo { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalFee { get; set; }

        /// <summary>
        /// 币种(默认值1是人民币)
        /// </summary>
        public int FeeType { get; set; }

        /// <summary>
        /// 通知Id
        /// </summary>
        public string NotifyId { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 商户数据包
        /// </summary>
        public string Attach { get; set; }

        /// <summary>
        /// 支付完成时间
        /// </summary>
        public DateTime TimeEnd { get; set; }

        /// <summary>
        /// 物流费用
        /// </summary>
        public decimal TransportFee { get; set; }

        /// <summary>
        /// 物品费用
        /// </summary>
        public decimal ProductFee { get; set; }

        /// <summary>
        /// 折扣价格
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// 买家别名
        /// </summary>
        public string BuyerAlias { get; set; }
    }
}
