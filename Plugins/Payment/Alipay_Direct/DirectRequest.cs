using System;
using Hishop.Plugins;
using System.Globalization;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using System.Web;
using System.Collections.Generic;

namespace Hishop.Plugins.Payment.AlipayDirect
{
    [Plugin("支付宝即时到帐交易", Sequence=2)]
    public class DirectRequest : PaymentRequest
    {

        public DirectRequest(
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

        public DirectRequest()
        {
        }

        #region 常量
        private const string Gateway = "https://openapi.alipay.com/gateway.do";	//'支付接口
        private const string V = "1.0";//版本
        private const string SignType = "RSA2";//签名类型
        private const string Format = "json";//请求参数格式
        private const string Input_charset_UTF8 = "UTF-8";//编码格式UTF-8
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }
        

      

        private readonly string subject;	//subject		商品名称
        private readonly string body;		//body			商品描述
        private readonly string totalFee;                      //总金额					0.01～50000.00
        private readonly string showUrl;

        /// <summary>
        /// 商户号ID
        /// </summary>
        [ConfigElement("商户号/AppId", Nullable = false)]
        public string Partner { set; get; }

        /// <summary>
        /// 商户私钥,读取rsa_private_key.pem的文件内容
        /// </summary>
        [ConfigElement("商户私钥", Nullable = false)]
        public string Key { set; get; }
        /// <summary>
        /// 商户私钥，获取开放平台密钥中的支付宝公钥(SHA256withRsa)
        /// </summary>
        [ConfigElement("商户公钥", Nullable = true)]
        public string PublicKey { set; get; }



        private readonly string returnUrl;
        private readonly string notifyUrl;
        private readonly string outTradeNo;

        public override void SendRequest()
        {
            DefaultAopClient client = new DefaultAopClient(Gateway, Partner, Key, Format, V, SignType, PublicKey, Input_charset_UTF8, false);

            // 组装业务参数model
            AlipayTradePagePayModel model = new AlipayTradePagePayModel();
            model.Body = outTradeNo;
            model.Subject = subject;
            model.TotalAmount = totalFee;
            model.OutTradeNo = outTradeNo;
            model.ProductCode = "FAST_INSTANT_TRADE_PAY";
            //model.QuitUrl = Merchant_url;
            model.EnablePayChannels = "balance,moneyFund,coupon,pcredit,pcreditpayInstallment,creditCardCartoon,credit_group,creditCard,creditCardExpress,promotion,bankPay,debitCardExpress";
                                       
            //ExtendParams exParam = new ExtendParams();
            //exParam.HbFqNum = "3";
            //exParam.HbFqSellerPercent = "0";
            //model.ExtendParams = exParam;
            AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();
            // 设置同步回调地址
            request.SetReturnUrl(returnUrl);
            // 设置异步通知接收地址
            request.SetNotifyUrl(notifyUrl);
            // 将业务model载入到request
            request.SetBizModel(model);

            AlipayTradePagePayResponse response = null;
            try
            {
                response = client.pageExecute(request, null, "post");
                HttpContext.Current.Response.Write(response.Body);
            }
            catch (Exception exp)
            {
                IDictionary < string,string> param = new Dictionary<string, string>();
                param.Add("Gateway", Gateway);
                param.Add("Partner", Partner);
                param.Add("Key", Key);
                param.Add("Format", Format);
                param.Add("V", V);
                param.Add("SignType", SignType);
                param.Add("PublicKey", PublicKey);
                param.Add("Out_trade_no", outTradeNo);
                param.Add("Subject", subject);
                param.Add("Total_fee", totalFee);
                param.Add("returnUrl", returnUrl);
                param.Add("notifyUrl", notifyUrl);

                PayLog.writeLog(param, exp.Message, "", "", LogType.WS_WapPay);
            }
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