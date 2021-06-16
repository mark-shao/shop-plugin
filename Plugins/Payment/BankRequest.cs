using System;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment
{
    [Plugin("线下支付")]
    public class BankRequest : PaymentRequest
    {

        private readonly string url;

        public BankRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            url = showUrl;
        }

        public BankRequest()
        {
        }

        public override void SendRequest()
        {
            RedirectToGateway(url);
        }

        protected override bool NeedProtect
        {
            get { return false; }
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