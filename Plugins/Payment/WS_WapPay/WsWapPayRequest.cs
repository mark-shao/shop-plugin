using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Hishop.Plugins;
using System.Globalization;
using System.Web.Security;
using System.Security.Cryptography;
using System.Web;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using System.Collections;
using System.Collections.Generic;

namespace Hishop.Plugins.Payment.WS_WapPay
{
    [Plugin("支付宝手机网站支付")]
    public class WsWapPayRequest : PaymentRequest
    {
        public WsWapPayRequest(string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            this.Out_trade_no = orderId;
            this.Total_fee = amount.ToString("F", CultureInfo.InvariantCulture);
            this.Subject = subject;
            this.Merchant_url = showUrl;
            this.Call_back_url = returnUrl;
            this.Notify_url = notifyUrl;
        }

        public WsWapPayRequest() { }

        #region 支付宝参数，必须按照以下值传递
        private const string Req_url = "https://openapi.alipay.com/gateway.do";//请求地址

        private const string V = "1.0";//版本
        private const string SignType = "RSA2";//签名类型
        private const string Format = "json";//请求参数格式
        private const string Input_charset_UTF8 = "UTF-8";//编码格式UTF-8
        #endregion

        #region 商户需要手动配置  设置开放平台的RSA2支付宝密钥使用rsa_public_key.pem文件中的密钥设置


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

        ///// <summary>
        ///// 卖家账户名称
        ///// </summary>
        //[ConfigElement("收款账号", Nullable = false)]
        //public string Seller_account_name { set; get; }

        private string Req_id = System.DateTime.Now.ToString();//请求ID 请随机生成
        private string Out_trade_no = "";// 外部交易号(由商户创建，请不要重复)
        private string Subject = "";//订单标题
        private string Total_fee = "";//订单价格
        private string Out_user = "";//外部用户唯一标识

        //三个返回URL
        private string Call_back_url = "";//用户付款成功同步返回URL
        private string Notify_url = "";//服务端接收通知URL
        private string Merchant_url = "";// 用户付款中途退出返回URL


        #endregion


        public override void SendRequest()
        {
            //IDictionary<string,string> param = new Dictionary<string, string>();
            //param.Add("Req_url", Req_url);
            //param.Add("Partner", Partner);
            //param.Add("Key", Key);
            //param.Add("Format", Format);
            //param.Add("V", V);
            //param.Add("SignType", SignType);
            //param.Add("PublicKey", PublicKey);
            //param.Add("Out_trade_no", Out_trade_no);
            //param.Add("Subject", Subject);
            //param.Add("Total_fee", Total_fee);
            //param.Add("Merchant_url", Merchant_url);
            //param.Add("Call_back_url", Call_back_url);
            //param.Add("Notify_url", Notify_url);

            //PayLog.writeLog(param, "", "", "", LogType.WS_WapPay);

            DefaultAopClient client = new DefaultAopClient(Req_url, Partner, Key, Format, V, SignType, PublicKey, Input_charset_UTF8, false);
            // 组装业务参数model
            AlipayTradeWapPayModel model = new AlipayTradeWapPayModel();
            model.Body = Out_trade_no;
            model.Subject = Subject;
            model.TotalAmount = Total_fee;
            model.OutTradeNo = Out_trade_no;
            model.ProductCode = "QUICK_WAP_PAY";
            //model.QuitUrl = Merchant_url;
            model.EnablePayChannels = "balance,moneyFund,coupon,pcredit,pcreditpayInstallment,creditCardCartoon,credit_group,creditCard,creditCardExpress,promotion,bankPay,debitCardExpress";
            AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
            // 设置支付完成同步回调地址
            request.SetReturnUrl(Call_back_url);
            // 设置支付完成异步通知接收地址
            request.SetNotifyUrl(Notify_url);
            // 将业务model载入到request
            request.SetBizModel(model);
            AlipayTradeWapPayResponse response = null;
            try
            {
                response = client.pageExecute(request, null, "post");
                HttpContext.Current.Response.Write(response.Body);
            }
            catch (Exception exp)
            {

                IDictionary<string, string> param = new Dictionary<string, string>();
                param.Add("Gateway", Req_url);
                param.Add("Partner", Partner);
                param.Add("Key", Key);
                param.Add("Format", Format);
                param.Add("V", V);
                param.Add("SignType", SignType);
                param.Add("PublicKey", PublicKey);
                param.Add("Out_trade_no", Out_trade_no);
                param.Add("Subject", Subject);
                param.Add("Total_fee", Total_fee);
                param.Add("returnUrl", Call_back_url);
                param.Add("notifyUrl", Notify_url);

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

        protected override bool NeedProtect
        {
            get { return true; }
        }

        public override string ShortDescription
        {
            get { return "mobile"; }
        }
    }

}
