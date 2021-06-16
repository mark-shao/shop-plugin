using System;
using System.Text;
using Hishop.Plugins;
using System.Globalization;

namespace Hishop.Plugins.Payment.Xpay
{
    [Plugin("易付通")]
    public class XpayRequest : PaymentRequest
    {

        public XpayRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            bid = orderId;
            prc = amount.ToString("F", CultureInfo.InvariantCulture);
            pdt = subject;
            url = returnUrl;
        }

        public XpayRequest()
        {
        }

        #region 常量
        private const string GatewayUrl = "http://pay.xpay.cn/pay.aspx";
        private const string Card = "bank";
        private const string Scard = "bank,Unicom,xpay,ebilling,ibank";
        private const string ActionCode = "sell";
        private const string Ver = "2.0";
        private const string Remark1 = "xpay";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// 商户交易号
        /// </summary>
        [ConfigElement("商户号", Nullable = false)]
        public string Tid { get; set; }

        [ConfigElement("商户密钥", Nullable = false)]
        public string Key { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        private readonly string bid = "";
        /// <summary>
        /// 订单总金额
        /// </summary>
        private readonly string prc = "";
        
        /// <summary>
        /// 支付交易完成后返回到该url
        /// </summary>
        private readonly string url = "";

        /// <summary>
        /// 产品名称或交易说明
        /// </summary>
        private readonly string pdt = "";

        public override void SendRequest()
        {
            // 生成签名
            string s = Key + ":" + prc + "," + bid + "," + Tid + "," +
                       Card + "," + Scard + "," + ActionCode + "," + "," + Ver;
            string md = Globals.GetXpayMD5(s);

            StringBuilder sb = new StringBuilder();

            sb.Append(CreateField("tid", Tid));
            sb.Append(CreateField("bid", bid));
            sb.Append(CreateField("prc", prc));
            sb.Append(CreateField("card", Card));
            sb.Append(CreateField("scard", Scard));
            sb.Append(CreateField("actioncode", ActionCode));
            sb.Append(CreateField("actionparameter", ""));
            sb.Append(CreateField("ver", Ver));
            sb.Append(CreateField("url", url));
            sb.Append(CreateField("pdt", pdt));
            sb.Append(CreateField("remark1", Remark1));
            sb.Append(CreateField("md", md));

            SubmitPaymentForm(CreateForm(sb.ToString(), GatewayUrl));
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