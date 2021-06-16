using System;
using System.Text;
using System.Globalization;
using System.Web.Security;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.IPS
{
    [Plugin("环迅(IPS v3.0)")]
    public class IpsRequest : PaymentRequest
    {

        public IpsRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            Merchanturl = returnUrl;
            ServerUrl = notifyUrl;
            Billno = orderId;
            Amount = amount.ToString("F2", CultureInfo.InvariantCulture);
            Date = date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public IpsRequest()
        {
        }

        #region 常量
        private const string PostUrl = "https://pay.ips.com.cn/ipayment.aspx";
        private const string Currency_Type = "RMB";
        private const string Gateway_Type = "01";
        private const string Attach = "IPS";
        private const string OrderEncodeType = "5";
        private const string RetEncodeType = "17";
        private const string Rettype = "1";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// 商户号
        /// </summary>
        [ConfigElement("商户号", Nullable = false)]
        public string Mer_code { get; set; }

        /// <summary>
        /// 证书[商户密钥]
        /// </summary>
        [ConfigElement("商户密钥", Nullable = false)]
        public string Cert { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        private readonly string Billno = "";

        private readonly string ServerUrl = "";

        /// <summary>
        /// 金额
        /// </summary>
        private readonly string Amount = "";

        /// <summary>
        /// 订单日期,规则：YYYYMMDD
        /// </summary>
        private readonly string Date = "";
        
        /// <summary>
        /// 商户返回地址
        /// </summary>
        private readonly string Merchanturl = "";

        public override void SendRequest()
        {
            // 生成签名
            StringBuilder sbsign=new StringBuilder();
            sbsign.AppendFormat("billno{0}currencytype{1}amount{2}date{3}", Billno, Currency_Type, Amount, Date);
            sbsign.AppendFormat("orderencodetype{0}{1}",OrderEncodeType,Cert);
          //  string sign = FormsAuthentication.HashPasswordForStoringInConfigFile(sbsign.ToString(), "MD5").ToLower(CultureInfo.InvariantCulture);
            string sign = Globals.GetMD5(sbsign.ToString()).ToLower(CultureInfo.InvariantCulture);
            StringBuilder sb = new StringBuilder();

            //构建支付参数
            sb.Append(CreateField("mer_code", Mer_code));
            sb.Append(CreateField("Billno", Billno));
            sb.Append(CreateField("Amount", Amount));
            sb.Append(CreateField("Date", Date));
            sb.Append(CreateField("Currency_Type", Currency_Type));
            sb.Append(CreateField("Gateway_type", Gateway_Type));
            sb.Append(CreateField("Merchanturl", Merchanturl));
            sb.Append(CreateField("Attach", Attach));
            sb.Append(CreateField("OrderEncodeType", OrderEncodeType));// MD5加密
            sb.Append(CreateField("RetEncodeType", RetEncodeType));// 支付返回用MD5加密验证
            sb.Append(CreateField("RetType", Rettype));
            sb.Append(CreateField("ServerUrl",ServerUrl));
            sb.Append(CreateField("SignMD5", sign));

            SubmitPaymentForm(CreateForm(sb.ToString(), PostUrl));
        }

        public override bool IsMedTrade
        {
            get { return false; }
        }

        public override void SendGoods(string tradeno, string logisticsName, string invoiceno, string transportType)
        {

        }

        public override string Description
        {
            get { return string.Empty; }
        }

        public override string Logo
        {
            get { return string.Empty; }
        }

        public override string ShortDescription
        {
            get { return string.Empty; }
        }

    }
}