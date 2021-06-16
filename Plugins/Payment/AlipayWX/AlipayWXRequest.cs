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
using System.Collections.Generic;

namespace Hishop.Plugins.Payment.AlipayWX
{
    [Plugin("支付宝微信端支付接口", Sequence = 5)]
    public class AlipayWXRequest : PaymentRequest
    {
        public AlipayWXRequest(string orderId, decimal amount,
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

        public AlipayWXRequest() { }

        #region 支付宝参数，必须按照以下值传递
        private const string Req_url = "https://openapi.alipay.com/gateway.do";//请求地址

        private const string V = "1.0";//版本
        private const string SignType = "RSA2";//签名类型
        private const string Format = "json";//请求参数格式
        private const string Input_charset_UTF8 = "UTF-8";//编码格式UTF-8
        #endregion

        #region 商户需要手动配置

        [ConfigElement("商户公钥", Nullable = false)]
        public string PublicKey { get; set; } //卖家email

        [ConfigElement("商户号/AppId", Nullable = false)]
        public string Partner { get; set; }

        /// <summary>
        /// 商户私钥
        /// </summary>
        [ConfigElement("商户私钥", Nullable = false)]
        public string Key { set; get; }


        ///// <summary>
        ///// 卖家账户名称
        ///// </summary>
        //[ConfigElement("收款账号", Nullable = false)]
        //public string SellerEmail { set; get; }

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


        public override void SendRequest() { }


        public override object SendRequest_Ret()
        {
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
                return response.Body;
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
                return "error:" + exp.Message;
            }
            ////创建交易接口
            //Service ali = new Service();
            //string token = ali.alipay_wap_trade_create_direct(
            //   Req_url, Subject, Out_trade_no, Total_fee, SellerEmail, Notify_url,
            //   Out_user, Merchant_url, Call_back_url, Service_Create, Sec_id, Partner, Req_id, Format, V, Input_charset_UTF8, Req_url, Key, Sec_id);

            ////PayLog.AppendLog(null, null, "", "支付宝支付----------token:" + token + "--------------Req_url:" + Req_url + "---------------Subject:" + Subject + "-------------Out_trade_no:" + Out_trade_no + "---------------Total_fee:" + Total_fee + "----------------Seller_account_name:" + SellerEmail + "------------------Notify_url:" + Notify_url + "--------------Out_user:" + Out_user + "--------------Merchant_url:" + Merchant_url + "-----------------Call_back_url:" + Call_back_url + "------------------Service_Create:" + Service_Create + "----------------Sec_id:" + Sec_id + "-------------------Partner:" + Partner + "-----------Req_id:" + Req_id + "------------------Format:" + Format + "---------V:" + V + "----------------Input_charset_UTF8:" + Input_charset_UTF8 + "----------------------Req_url:" + Req_url + "-------------Key:" + Key + "------------Sec_id:" + Sec_id, LogType.WxAliPay);

            ////构造，重定向URL
            //string url = ali.alipay_Wap_Auth_AuthAndExecute(Req_url, Sec_id, Partner, Call_back_url, Format, V, Service_Auth, token, Input_charset_UTF8, Req_url, Key, Sec_id);
            //PayLog.AppendLog(null, url, "", "", LogType.WxAliPay);
            //跳转收银台支付页面
            //RedirectToGateway(url);
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
