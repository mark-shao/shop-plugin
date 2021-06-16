using System;
using System.Collections.Generic;

namespace Hishop.Weixin.Pay.Domain
{
    /// <summary>
    /// 提供商模式
    /// </summary>
    public enum ProviderModes
    {
        /// <summary>
        /// 普通模式，用户自己申请支付接口
        /// </summary>
        NormalMode = 0,
        /// <summary>
        /// 服务商模式，由服务代理商家申请
        /// </summary>
        ServiceMode = 1
    }
    /// <summary>
    /// 数据包信息
    /// </summary>
    public class PackageInfo
    {
        public PackageInfo()
        {
            BankType = "WX";
            FeeType = "1";
            InputCharset = "UTF-8";
            SpbillCreateIp = "127.0.0.1";
            sub_appid = "";
            sub_openid = "";
        }
        /// <summary>
        /// 申请模式
        /// </summary>
        private ProviderModes ProviderMode
        {
            get
            {
                return string.IsNullOrEmpty(sub_mch_id) ? ProviderModes.NormalMode : ProviderModes.ServiceMode;
            }
        }
        /// <summary>
        /// 子商户公众账号ID，可为空参数
        /// </summary>
        public string sub_appid
        {
            get;
            set;
        }
        /// <summary>
        /// 用户子标识,可为空，当此参数不为空时sub_appid不能为空
        /// </summary>
        public string sub_openid
        {
            get;
            set;
        }
        /// <summary>
        /// 子商户号，用于服务商模式申请的帐号使用
        /// </summary>
        public string sub_mch_id { get; set; }
        /// <summary>
        /// 银行通道类型
        /// </summary>
        public string BankType { get; private set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public string Attach { get; set; }

        /// <summary>
        /// 商户号（财付通商户号mchid）
        /// </summary>
        public string Partner { get; private set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal TotalFee { get; set; }

        /// <summary>
        /// 支付币种
        /// </summary>
        public string FeeType { get; private set; }

        /// <summary>
        /// 通知URL
        /// </summary>
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 订单生成的机器IP
        /// </summary>
        public string SpbillCreateIp { get; set; }

        /// <summary>
        /// 交易起始时间
        /// </summary>
        public DateTime? TimeStart { get; set; }

        /// <summary>
        /// 交易结束时间
        /// </summary>
        public DateTime? TimeExpire { get; set; }

        /// <summary>
        /// 物流费用
        /// </summary>
        public decimal? TransportFee { get; set; }

        /// <summary>
        /// 商品费用
        /// </summary>
        public decimal? ProductFee { get; set; }

        /// <summary>
        /// 商品标记
        /// </summary>
        public string GoodsTag { get; set; }

        /// <summary>
        /// 传入参数字符编码
        /// </summary>
        public string InputCharset { get; private set; }
        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }
    }
}
