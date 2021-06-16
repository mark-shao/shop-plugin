using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Web;
namespace Hishop.Plugins.Payment.ShengPayWeiXinH5
{
    public class Ext2Info
    {
        /// <summary>
        /// 请求来源，ANDROID_APP, IOS_APP, WAP
        /// </summary>
        public string requestFrom { get; set; }
        /// <summary>
        /// app名称，请求来源为ANDROID_APP和IOS_APP时填写
        /// </summary>
        public string app_name { get; set; }
        /// <summary>
        /// IOS应用唯一标识，请求来源为IOS_APP
        /// </summary>
        public string bundle_id { get; set; }
        /// <summary>
        /// 包名，请求来源为ANDROID_APP时填写
        /// </summary>
        public string package_name { get; set; }
        /// <summary>
        /// WAP首页URL地址  请求来源为WAP时填写
        /// </summary>
        public string wap_url { get; set; }
        /// <summary>
        /// WAP网站名 请求来源为WAP时填写
        /// </summary>
        public string wap_name { get; set; }
        /// <summary>
        /// 为商户自定义的跟本次交易有关的参数
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 可以为空，或者为任何自己想要卡网关回传的校验类型的数据。
        /// </summary>
        public string attach { get; set; }
    }
    [Plugin("盛付通微信H5支付")]
    public class ShengPayWeiXinH5Request : PaymentRequest
    {

        #region 属性

        /// <summary>
        /// 支付接口
        /// </summary>
        //const string GatewayUrl = "http://api.shengpay.com/html5-gateway/pay.htm?page=mobile";//老版地址
        const string GatewayUrl = "https://cardpay.shengpay.com/mobile-acquire-channel/cashier.htm";//新版地址
        // const string GatewayUrl = "http://61.152.90.48:8088/html5-gateway-general/pay.htm?page=mobile";

        /// <summary>
        /// 版本名称
        /// </summary>
        const string Name = "B2CPayment";

        /// <summary>
        /// 版本号
        /// </summary>
        const string Version = "V4.1.1.1.1";

        /// <summary>
        /// 字符集
        /// </summary>
        string Charset = "UTF-8";

        /// <summary>
        /// 请求序列号
        /// </summary>
        string TraceNo;

        /// <summary>
        /// 发送方标识
        /// </summary>
        [ConfigElement("发送方标识", Nullable = false)]
        public string SenderId { set; get; }//后面的版本修改名称为MsgSender

        /// <summary>
        /// 发送支付请求时间
        /// </summary>
        string SendTime;

        /// <summary>
        /// 商户订单号
        /// </summary>
        string OrderNo;

        /// <summary>
        /// 支付金额
        /// </summary>
        string OrderAmount;

        /// <summary>
        /// 商户订单提交时间
        /// </summary>
        string OrderTime;

        /// <summary>
        /// 币种
        /// </summary>
        const string Currency = "CNY";

        /// <summary>
        /// 支付类型
        /// </summary>
        string PayType = "PT312";
        /// <summary>
        /// 支付频道
        /// </summary>
        string PayChannel = "hw";
        /// <summary>
        /// 支付机构
        /// </summary>
        string InstCode;

        /// <summary>
        /// 浏览器回调地址
        /// </summary>
        string PageUrl;

        /// <summary>
        /// 商户后台回调地址
        /// </summary>
        string NotifyUrl;

        /// <summary>
        /// 商品编号
        /// </summary>
        string ProductId;

        /// <summary>
        /// 商品名称
        /// </summary>
        string ProductName;

        /// <summary>
        /// 商品数量
        /// </summary>
        string ProductNum;

        /// <summary>
        /// 单价
        /// </summary>
        string UnitPrice;

        /// <summary>
        /// 商品描述
        /// </summary>
        string ProductDesc;

        /// <summary>
        /// 商品Url
        /// </summary>
        string ProductUrl;

        /// <summary>
        /// 卖方标识
        /// </summary>
        string SellerId;

        /// <summary>
        /// 买方标识
        /// </summary>
        string BuyerId;

        /// <summary>
        /// 支付人联系方式
        /// </summary>
        string BuyerContact;

        /// <summary>
        /// 买家IP地址
        /// </summary>
        string BuyerIP;

        /// <summary>
        /// 买家姓名
        /// </summary>
        string BuyerName;

        /// <summary>
        /// 付款方手机号
        /// </summary>
        string PayerMobileNo;

        /// <summary>
        /// 付款方认证凭证
        /// </summary>
        string PayerAuthTicket;

        /// <summary>
        /// 扩展1
        /// </summary>
        string Ext1;

        /// <summary>
        /// 扩展2
        /// </summary>
        string Ext2;

        /// <summary>
        /// 签名类型
        /// </summary>
        string SignType = "MD5";

        string ExpireTime;
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
        public ShengPayWeiXinH5Request(string orderId, decimal amount, String subject, String body, string buyerEmail, DateTime orderTime,
                               string showUrl, string returnUrl, string notifyUrl, String attach)
        {
            OrderNo = orderId;
            OrderAmount = amount.ToString("F2");
            PageUrl = returnUrl;
            NotifyUrl = notifyUrl;
            BuyerIP = GetUserIP();
            TraceNo = Guid.NewGuid().ToString("N");
            ProductUrl = showUrl;
            ProductName = body;
            ProductDesc = body;
            OrderTime = orderTime.ToString("yyyyMMddHHmmss");
            SendTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            Ext1 = "";
            Ext2 = attach;
            //string.Format("{\"requestFrom\":\"WAP\",\"app_name\":\"\",\"bundle_id\":\"\",\"package_name\":\"\",\"wap_url\":\"{0}\",\"wap_name\":\"{1}\",\"note\":\"{2}\",\"attach\":\"{3}\"}", showUrl, "", orderId, attach);
        }

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

        public ShengPayWeiXinH5Request() { }
        /// <summary>
        /// 发送请求
        /// </summary>
        public override void SendRequest()
        {
//            Name+Version+charset+traceNo+MsgSender+sendTime+orderNo+orderAmount+orderTime+expireTime+currency+payType+
//payChannel+instCode+cardValue+language+pageUrl+BackUrl+notifyUrl+SharingInfo+SharingNotifyUrl+productId+productName+productNum+unitPrice+
//productDesc+productUrl+sellerId+buyerName+buyerId+buyerContact+buyerIp+payeeId+depositId+depositIdType+payerId+ext1+ext2+signType+MD5key

            StringBuilder sbOriginStr = new StringBuilder();
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(Name) ? "" : Name);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(Version) ? "" : Version);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(Charset) ? "" : Charset);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(TraceNo) ? "" : TraceNo);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(SenderId) ? "" : SenderId);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(SendTime) ? "" : SendTime);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(OrderNo) ? "" : OrderNo);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(OrderAmount) ? "" : OrderAmount);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(OrderTime) ? "" : OrderTime);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(ExpireTime) ? "" : ExpireTime);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(Currency) ? "" : Currency);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(PayType) ? "" : PayType);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(PayChannel) ? "" : PayChannel);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(PageUrl) ? "" : PageUrl);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(NotifyUrl) ? "" : NotifyUrl);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(ProductId) ? "" : ProductId);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(ProductName) ? "" : ProductName);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(ProductNum) ? "" : ProductNum);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(UnitPrice) ? "" : UnitPrice);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(ProductDesc) ? "" : ProductDesc);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(ProductUrl) ? "" : ProductUrl);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(SellerId) ? "" : SellerId);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(BuyerName) ? "" : BuyerName);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(BuyerId) ? "" : BuyerId);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(BuyerContact) ? "" : BuyerContact);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(BuyerIP) ? "" : BuyerIP);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(Ext1) ? "" : Ext1);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(Ext2) ? "" : Ext2);
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(SignType) ? "" : SignType);

            string originStr = sbOriginStr.ToString();

            //string originStr = ServiceCode + Version + Charset + TraceNo + SenderId + SendTime +
            //                   OrderNo + OrderAmount + OrderTime + Currency + PageUrl + NotifyUrl + ProductId +
            //                   ProductName + ProductNum + UnitPrice + ProductDesc + ProductUrl + SellerId +
            //                   BuyerName + BuyerId + BuyerContact + BuyerIP + PayerMobileNo + PayerAuthTicket +
            //                   Ext1 + Ext2 + SignType;
            string sign = FormsAuthentication.HashPasswordForStoringInConfigFile((originStr + SellerKey), SignType);

            StringBuilder sb = new StringBuilder();

            sb.Append(CreateField("Name", Name));
            sb.Append(CreateField("Version", Version));
            sb.Append(CreateField("Charset", Charset));
            sb.Append(CreateField("TraceNo", TraceNo));
            sb.Append(CreateField("MsgSender", SenderId));
            sb.Append(CreateField("SendTime", SendTime));
            sb.Append(CreateField("OrderNo", OrderNo));
            sb.Append(CreateField("OrderAmount", OrderAmount));
            sb.Append(CreateField("OrderTime", OrderTime));
            sb.Append(CreateField("Currency", Currency));
            sb.Append(CreateField("PayType", PayType));
            sb.Append(CreateField("PayChannel", PayChannel));
            sb.Append(CreateField("InstCode", InstCode));
            sb.Append(CreateField("PageUrl", PageUrl));
            sb.Append(CreateField("NotifyUrl", NotifyUrl));
            sb.Append(CreateField("ProductId", ProductId));
            sb.Append(CreateField("ProductName", ProductName));
            sb.Append(CreateField("ProductNum", ProductNum));
            sb.Append(CreateField("ProductDesc", ProductDesc));
            sb.Append(CreateField("ProductUrl", ProductUrl));
            sb.Append(CreateField("SellerId", SellerId));
            sb.Append(CreateField("BuyerName", BuyerName));
            sb.Append(CreateField("BuyerId", BuyerId));
            sb.Append(CreateField("BuyerContact", BuyerContact));
            sb.Append(CreateField("BuyerIp", BuyerIP));
            sb.Append(CreateField("PayerMobileNo", PayerMobileNo));
            sb.Append(CreateField("PayerAuthTicket", PayerAuthTicket));
            sb.Append(CreateField("Ext1", Ext1));
            sb.Append(CreateField("Ext2", Ext2));
            sb.Append(CreateField("SignType", SignType));
            sb.Append(CreateField("SignMsg", sign));

            IDictionary<string, string> para = new Dictionary<string, string>();
            para.Add("Name", Name);
            para.Add("version", Version);
            para.Add("charset", Charset);
            para.Add("TraceNo", TraceNo);
            para.Add("MsgSender", SenderId);
            para.Add("sendTime", SendTime);
            para.Add("orderNo", OrderNo);
            para.Add("orderAmount", OrderAmount);
            para.Add("orderTime", OrderTime);
            para.Add("currency", Currency);
            para.Add("payType", PayType);
            para.Add("instCode", InstCode);
            para.Add("pageUrl", PageUrl);
            para.Add("notifyUrl", NotifyUrl);
            para.Add("productId", ProductId);
            para.Add("productName", ProductName);
            para.Add("productNum", ProductNum);
            para.Add("productDesc", ProductDesc);
            para.Add("productUrl", ProductUrl);
            para.Add("sellerId", SellerId);
            para.Add("buyerName", BuyerName);
            para.Add("buyerId", BuyerId);
            para.Add("buyerContact", BuyerContact);
            para.Add("buyerIp", BuyerIP);
            para.Add("payerMobileNo", PayerMobileNo);
            para.Add("payerAuthTicket", PayerAuthTicket);
            para.Add("ext1", Ext1);
            para.Add("ext2", Ext2);
            para.Add("signType", SignType);
            para.Add("SignMsg", sign);
            para.Add("SellerKey", SellerKey);

            PayLog.AppendLog(para, sign, CreateForm(sb.ToString(), GatewayUrl), "支付日志", LogType.ShengpayMobile);

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
