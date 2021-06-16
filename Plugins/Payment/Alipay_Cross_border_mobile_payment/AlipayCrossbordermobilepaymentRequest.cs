using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Hishop.Plugins.Payment.AlipayCrossborderMobilePayment
{
    [Plugin("支付宝境外支付Wap移动手机版接口")]
    public class AlipaycrossbordermobilepaymentRequest : PaymentRequest
    {
        public AlipaycrossbordermobilepaymentRequest() { }
        public AlipaycrossbordermobilepaymentRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            this.body = body;
            outTradeNo = orderId;
            this.subject = subject;
            totalFee = amount.ToString("F", CultureInfo.InvariantCulture);
            this.returnUrl = returnUrl;
            this.notifyUrl = notifyUrl;
            this.showUrl = showUrl;
            
        }
        #region 常量
        private static string GATEWAY = "https://mapi.alipay.com/gateway.do?";
        //private static string GATEWAY = "https://openapi.alipaydev.com/gateway.do?";
        private const string Service = "create_forex_trade_wap";
        private static string SignType = "MD5";
        // private const string PaymentType = "1";                  //支付类型	
        private static string InputCharset = "utf-8";
        //private const string Agent = "C4335302345904805116";
        //private const string extend_param = "isv^yf31";
        #endregion

        [ConfigElement("安全校验码(Key)", Nullable = false)]
        public string Key { get; set; }              //账户的支付宝安全校验码
        [ConfigElement("收款支付宝账号", Nullable = false)]
        public string SellerEmail { get; set; } //卖家email

        [ConfigElement("合作者身份(PID)", Nullable = false)]
        public string Partner { get; set; }

        [ConfigElement("是否人民币收款", Nullable = false)]
        public string RMB { get; set; }
        [ConfigElement("币种", Nullable = false)]
        public string Currency { get; set; }

        private readonly string returnUrl;
        private readonly string notifyUrl;
        private readonly string outTradeNo;
        private readonly string subject;	//subject		商品名称
        private readonly string body;		//body			商品描述
        private readonly string totalFee;                      //总金额					0.01～50000.00
        private readonly string showUrl;

        public override void SendRequest()
        {
            AliPay ap = new AliPay();
            string aliay_url = ap.CreatUrl(
                GATEWAY,
                Service,
                Partner,
                SignType,
                outTradeNo,
                subject,
                body,
                Currency,
                totalFee,
                Key,
                returnUrl,
                notifyUrl,
                RMB
                );

            RedirectToGateway(aliay_url);


            //RedirectToGateway(
            //    Globals.CreatDirectUrl(GATEWAY, Service, Partner, SignType, outTradeNo, subject,
            //                           body,
            //    //PaymentType,
            //                           totalFee, showUrl, SellerEmail, Key, returnUrl,
            //                           InputCharset, notifyUrl, RMB, Currency//, Agent, extend_param
            //        ));
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

        protected override bool NeedProtect
        {
            get { return true; }
        }

        public override string ShortDescription
        {
            get { return string.Empty; }
        }
    }
}
