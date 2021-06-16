using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Hishop.Plugins;
using System.Globalization;

namespace Hishop.Plugins.Payment.AlipayAssure
{
    /// <summary>
    /// 支付宝实物交易请求
    /// </summary>
    [Plugin("支付宝担保交易", Sequence = 1)]
    public class AssureRequest : PaymentRequest
    {

        public AssureRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            this.returnUrl = returnUrl;
            this.notifyUrl = notifyUrl;
            this.body = body;
            outTradeNo = orderId;
            this.subject = subject;
            price = amount.ToString("F", CultureInfo.InvariantCulture);
            this.showUrl = showUrl;
            token = attach;
        }

        public AssureRequest()
        {
        }

        #region 常量
        //private const string Gateway = "https://mapi.alipay.com/gateway.do?";	//'支付接口
        private const string Gateway = "https://mapi.alipay.com/gateway.do?";
        private const string Service = "create_partner_trade_by_buyer";
        private const string SignType = "MD5";
        private const string PaymentType = "1"; //支付类型 1-商品购买
        private const string Quantity = "1"; // 购买数量
        private const string InputCharset = "utf-8";
        private const string LogisticsType = "EXPRESS";
        private const string LogisticsFee = "0.00";
        private const string LogisticsPayment = "BUYER_PAY";
        private const string Agent = "C4335302345904805116";
        private const string extend_param = "isv^yf31";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }
        [ConfigElement("收款支付宝账号", Nullable = false)]
        public string SellerEmail { get; set; } //卖家email

        [ConfigElement("合作者身份(PID)", Nullable = false)]
        public string Partner { get; set; }

        private readonly string subject;	// 商品名称
        private readonly string body; //	商品描述
        private readonly string price; // 商品单价
        private readonly string showUrl;



        [ConfigElement("安全校验码(Key)", Nullable = false)]
        public string Key { get; set; }              //账户的支付宝安全校验码

        private readonly string returnUrl; //服务器通知返回接口
        private readonly string notifyUrl;
        private readonly string outTradeNo;
        private string token;

        public override void SendRequest()
        {
            RedirectToGateway(
                Globals.CreatUrl(
                    Gateway, Service, Partner, SignType, outTradeNo, subject, body, PaymentType, price,
                    showUrl, SellerEmail, Key, returnUrl, InputCharset, notifyUrl, LogisticsType, LogisticsFee,
                    LogisticsPayment, Quantity, Agent, extend_param, token
                    ));
        }

        public override bool IsMedTrade
        {
            get { return true; }
        }

        public override void SendGoods(string tradeno, string logisticsName, string invoiceno, string transportType)
        {
            string url = Globals.CreateSendGoodsUrl(Partner, tradeno, logisticsName, invoiceno, transportType, Key,
                                                    InputCharset);

            XmlDocument doc = SendGoodsRequest(url);
            if (doc == null)
                return;

            XmlNode root = doc.SelectSingleNode("alipay");
            if (root == null || root.SelectSingleNode("is_success").InnerText == "T")
            {
                return;
            }
            XmlNode node = root.SelectSingleNode("error");
            if (node != null && node.InnerText == "RETRY")
            {
                int count = 1;
                while (count <= 3)
                {
                    doc = SendGoodsRequest(url);
                    if (doc == null || doc.SelectSingleNode("alipay").SelectSingleNode("is_success").InnerText == "T")
                        return;

                    count++;
                }
            }
        }

        private static XmlDocument SendGoodsRequest(string url)
        {
            try
            {
                HttpWebRequest smsRequest = (HttpWebRequest)WebRequest.Create(url);
                smsRequest.Timeout = 5000;
                HttpWebResponse response = (HttpWebResponse)smsRequest.GetResponse();
                StringBuilder strBuilder = new StringBuilder();

                using (Stream myStream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(myStream, Encoding.Default))
                    {
                        while (-1 != sr.Peek())
                        {
                            strBuilder.Append(sr.ReadLine());
                        }
                    }
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strBuilder.ToString());
                return doc;
            }
            catch (Exception ex)
            {
                PayLog.AppendLog(null, "", url, ex.Message, LogType.Alipay_Assure);
                return null;
            }
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