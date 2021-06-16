using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Web;
namespace Hishop.Plugins.Payment.ShengPayMobile
{
    [Plugin("盛付通移动支付")]
    public class ShengPayMobileRequest : PaymentRequest
    {

        #region 属性

        /// <summary>
        /// 支付接口
        /// </summary>
        const string GatewayUrl = "https://api.shengpay.com/html5-gateway/express.htm?page=mobile";
        // const string GatewayUrl = "http://61.152.90.48:8088/html5-gateway-general/express.htm?page=mobile";
        /// <summary>
        /// 字符集
        /// </summary>
        string Charset = "UTF-8";


        /// <summary>
        /// 发送方标识
        /// </summary>
        [ConfigElement("商户号", Nullable = false)]
        public string SenderId { set; get; }

        /// <summary>
        /// 发送支付请求时间
        /// </summary>
        string requestTime;

        /// <summary>
        /// 商户订单号
        /// </summary>
        string merchantOrderNo;

        /// <summary>
        /// 支付金额
        /// </summary>
        string amount;

        /// <summary>
        /// 币种
        /// </summary>
        const string Currency = "CNY";

        /// <summary>
        /// 浏览器回调地址
        /// </summary>
        string PageUrl;

        /// <summary>
        /// 商户后台回调地址
        /// </summary>
        string NotifyUrl;


        /// <summary>
        /// 商品名称
        /// </summary>
        string ProductName;

        /// <summary>
        /// 商品描述
        /// </summary>
        string ProductDesc;

        /// <summary>
        /// 商品Url
        /// </summary>
        string ProductUrl;

        /// <summary>
        /// 买家IP地址
        /// </summary>
        string userIP;

        /// <summary>
        /// 扩展字段
        /// </summary>
        string exts;



        /// <summary>
        /// 签名类型
        /// </summary>
        string SignType = "MD5";

        /// <summary>
        /// 商户密钥
        /// </summary>
        [ConfigElement("商户密钥", Nullable = false)]
        public string SellerKey { get; set; }

        #endregion

        #region 重写父类属性

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

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="amount"></param>
        /// <param name="productId"></param>
        /// <param name="productDescription"></param>
        /// <param name="buyerEmail"></param>
        /// <param name="orderTime"></param>
        /// <param name="showUrl"></param>
        /// <param name="returnUrl"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="attach"></param>
        public ShengPayMobileRequest(string orderId, decimal amount, String subject, String body, string buyerEmail, DateTime orderTime,
                               string showUrl, string returnUrl, string notifyUrl, String attach)
        {
            merchantOrderNo = orderId;
            this.amount = amount.ToString("f2");
            PageUrl = returnUrl;
            NotifyUrl = notifyUrl;
            userIP = GetUserIP();
            ProductUrl = showUrl;
            ProductName = body;
            ProductDesc = body;
            requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                UserInfo = JsonHelper.ParseFormJson<PayUserInfo>(attach);
                exts = UserInfo.attach;
            }
            catch (Exception ex)
            {
                IDictionary<string, string> param = new Dictionary<string, string>();
                param.Add("attch", attach);
                PayLog.WriteExpectionLog(ex, param, LogType.ShengpayMobile);
            }
        }
        private PayUserInfo UserInfo = null;
        /// <summary>
        ///获取用户的IP地址获取客户IP地址
        /// </summary>
        /// <returns></returns>
        public static String GetUserIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            if (result.Length >= 20) { result = ""; }
            return result;

        }

        public ShengPayMobileRequest() { }
        /// <summary>
        /// 发送请求
        /// </summary>
        public override void SendRequest()
        {
            StringBuilder sbOriginStr = new StringBuilder();
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(SenderId) ? "" : SenderId + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(Charset) ? "" : Charset + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(requestTime) ? "" : requestTime + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(UserInfo.outMemberId) ? "" : UserInfo.outMemberId + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(UserInfo.outMemberRegisterTime) ? "" : UserInfo.outMemberRegisterTime + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(UserInfo.outMemberRegistIP) ? "" : UserInfo.outMemberRegistIP + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(UserInfo.outMemberVerifyStatus.ToString()) ? "" : UserInfo.outMemberVerifyStatus.ToString() + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(UserInfo.outMemberName) ? "" : UserInfo.outMemberName + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(UserInfo.outMemberMobile) ? "" : UserInfo.outMemberMobile + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(merchantOrderNo) ? "" : merchantOrderNo + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(ProductName) ? "" : ProductName + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(ProductDesc) ? "" : ProductDesc + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(Currency) ? "" : Currency + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(amount) ? "" : amount + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(PageUrl) ? "" : PageUrl + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(NotifyUrl) ? "" : NotifyUrl + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(userIP) ? "" : userIP + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(exts) ? "" : exts + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(SignType) ? "" : SignType + "|");

            string originStr = sbOriginStr.ToString();

            //string originStr = ServiceCode + Version + Charset + TraceNo + SenderId + SendTime +
            //                   OrderNo + OrderAmount + OrderTime + Currency + PageUrl + NotifyUrl + ProductId +
            //                   ProductName + ProductNum + UnitPrice + ProductDesc + ProductUrl + SellerId +
            //                   BuyerName + BuyerId + BuyerContact + BuyerIP + PayerMobileNo + PayerAuthTicket +
            //                   Ext1 + Ext2 + SignType;
            string sign = FormsAuthentication.HashPasswordForStoringInConfigFile((originStr + SellerKey), SignType);

            StringBuilder sb = new StringBuilder();

            sb.Append(CreateField("merchantNo", SenderId));
            sb.Append(CreateField("charset", Charset));
            sb.Append(CreateField("requestTime", requestTime));
            sb.Append(CreateField("outMemberId", UserInfo.outMemberId));
            sb.Append(CreateField("outMemberRegistTime", UserInfo.outMemberRegisterTime));
            sb.Append(CreateField("outMemberRegistIP", UserInfo.outMemberRegistIP));
            sb.Append(CreateField("outMemberVerifyStatus", UserInfo.outMemberVerifyStatus.ToString()));
            sb.Append(CreateField("outMemberName", UserInfo.outMemberName));
            sb.Append(CreateField("outMemberMobile", UserInfo.outMemberMobile));
            sb.Append(CreateField("merchantOrderNo", merchantOrderNo));
            sb.Append(CreateField("productName", ProductName));
            sb.Append(CreateField("productDesc", ProductDesc));
            sb.Append(CreateField("currency", Currency));
            sb.Append(CreateField("amount", amount));
            sb.Append(CreateField("pageUrl", ProductUrl));
            sb.Append(CreateField("notifyUrl", NotifyUrl));
            sb.Append(CreateField("userIP", userIP));
            sb.Append(CreateField("bankCardType", ""));
            sb.Append(CreateField("bankCode", ""));
            sb.Append(CreateField("exts", exts));
            sb.Append(CreateField("jsCallBack", ""));
            sb.Append(CreateField("backUrl", ""));
            sb.Append(CreateField("signType", SignType));
            sb.Append(CreateField("signMsg", sign));

            IDictionary<string, string> para = new Dictionary<string, string>();
            para.Add("merchantNo", SenderId);
            para.Add("charset", Charset);
            para.Add("requestTime", requestTime);
            para.Add("outMemberId", UserInfo.outMemberId);
            para.Add("outMemberRegistTime", UserInfo.outMemberRegisterTime);
            para.Add("outMemberRegistIP", UserInfo.outMemberRegistIP);
            para.Add("outMemberVerifyStatus", UserInfo.outMemberVerifyStatus.ToString());
            para.Add("outMemberName", UserInfo.outMemberName);
            para.Add("outMemberMobile", UserInfo.outMemberMobile);
            para.Add("merchantOrderNo", merchantOrderNo);
            para.Add("productName", ProductName);
            para.Add("pageUrl", PageUrl);
            para.Add("notifyUrl", NotifyUrl);
            para.Add("productDesc", ProductDesc);
            para.Add("currency", Currency);
            para.Add("productUrl", ProductUrl);
            para.Add("userIP", userIP);
            para.Add("bankCardType", "");
            para.Add("bankCode", "");
            para.Add("exts", exts);
            para.Add("jsCallBack", "");
            para.Add("backUrl", "");
            para.Add("signType", SignType);
            para.Add("SignMsg", sign);

            PayLog.AppendLog(para, sign, PageUrl, "支付日志", LogType.ShengpayMobile);

            SubmitPaymentForm(CreateForm(sb.ToString(), GatewayUrl));

        }



        public override bool IsMedTrade
        {
            get { throw new NotImplementedException(); }
        }

        public override void SendGoods(string tradeno, string logisticsName, string invoiceno, string transportType)
        {
            throw new NotImplementedException();
        }
    }
}
