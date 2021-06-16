using System;
using System.Text;
using Hishop.Plugins;
using System.Globalization;

namespace Hishop.Plugins.Payment.Paypal
{
    [Plugin("贝宝中国")]
    public class PaypalRequest : PaymentRequest
    {

        public PaypalRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            this.amount = amount.ToString("F", CultureInfo.InvariantCulture);
            invoice = orderId;
            itemNumber = orderId;
            this.returnUrl = returnUrl;
        }

        public PaypalRequest()
        {
        }

        #region 常量
        private const string GatewayUrl = "https://www.paypal.com/cgi-bin/webscr";
        private const string Cmd = "_xclick";
        private const string Quantity = "1"; // 物品数量
        private const string UndefinedQuantity = "0"; // 如果设置为 1，则允许买家修改数量，不允许修改数量
        private const string NoShipping = "1"; // 不要求客户输入收货地址
        private const string Rm = "2";//客户的浏览器由 POST 方法返回至返回URL，同时将所有可用交易变量发送至该 URL
        private const string CurrencyCode = "CNY";
        private const string Custom = "PayPalStandard";
        private const string Charset = "utf-8";
        private const string NoNote = "1";
        #endregion

        protected override bool NeedProtect
        {
            get { return false; }
        }

        // 必填项
        private readonly string amount;// 购物价格或金额，不包括运费、手续费或税费
        private readonly string invoice; // 可以用于识别此次购物的帐单号码的传递变量。用于保存订单编号
        private readonly string returnUrl;

        [ConfigElement("商户号", Nullable = false)]
        public string Business { get; set; }// 商家账号

        private readonly string itemNumber;

        public override void SendRequest()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(CreateField("cmd", Cmd));
            sb.Append(CreateField("amount", amount));
            sb.Append(CreateField("invoice", invoice));
            sb.Append(CreateField("quantity", Quantity));
            sb.Append(CreateField("undefined_quantity", UndefinedQuantity));
            sb.Append(CreateField("no_shipping", NoShipping));
            sb.Append(CreateField("return", returnUrl));
            sb.Append(CreateField("rm", Rm));
            sb.Append(CreateField("currency_code", CurrencyCode));
            sb.Append(CreateField("custom", Custom));
            sb.Append(CreateField("business", Business));
            sb.Append(CreateField("charset", Charset));
            sb.Append(CreateField("no_note", NoNote));
            sb.Append(CreateField("item_number", itemNumber));

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