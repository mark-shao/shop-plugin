using System;
using System.Text;
using System.Globalization;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.Allbuy
{
    [Plugin("AllBuy")]
    public class AllbuyRequest : PaymentRequest
    {
        public AllbuyRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            billNo = orderId;
            this.amount = amount.ToString("F", CultureInfo.InvariantCulture);
            this.date = date.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
            backUrl = returnUrl;
        }

        public AllbuyRequest()
        {
        }

        #region 常量
        private const string GatewayUrl = "http://www.allbuy.cn/newpayment/payment.asp";
        private const string Remark = "Allbuy";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }
        
        /// <summary>
        /// 商户编号
        /// </summary>
        [ConfigElement("商户号", Nullable=false)]
        public string Merchant { get; set; }

        [ConfigElement("商户密钥", Nullable = false)]
        public string Key { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        private readonly string billNo = "";

        /// <summary>
        /// 订单金额
        /// </summary>
        private readonly string amount = "";

        /// <summary>
        /// 交易日期
        /// </summary>
        private readonly string date = "";
        
        /// <summary>
        /// 支付成功返回地址
        /// </summary>
        private readonly string backUrl = "";

        public override void SendRequest()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(CreateField("merchant", Merchant));
            sb.Append(CreateField("BillNo", billNo));
            sb.Append(CreateField("Amount", amount));
            sb.Append(CreateField("Date", date));
            sb.Append(CreateField("Remark", Remark));
            sb.Append(CreateField("BackUrl", backUrl));

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