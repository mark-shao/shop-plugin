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



namespace Hishop.Plugins.Payment.GoPay
{
    [Plugin("国付宝")]
    public class GoPayRequest : PaymentRequest
    {

        public GoPayRequest()
        { }
        public GoPayRequest(
              string orderId, decimal amount,
              string subject, string body, string buyerEmail, DateTime date,
              string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            this.buyerEmail = buyerEmail;
            this.body = body;
            this.subject = subject;
            this.frontMerUrl = returnUrl;
            this.backgroundMerUrl = notifyUrl;
            merOrderNum = orderId;
            tranAmt = amount.ToString("F", CultureInfo.InvariantCulture);
            tranDateTime = date.ToString("yyyyMMddHHmmss");
        }
        //private const string GatewayUrl = "https://mertest.gopay.com.cn/PGServer/Trans/WebClientAction.do";
        private const string GatewayUrl = "https://gateway.gopay.com.cn/Trans/WebClientAction.do";
        //https://gatewaymer.gopay.com.cn/Trans/WebClientAction.do
	   // private const string merchantID = "0000003358";
        private const string version="2.1";
        private const string charset = "2"; //1GBK  ,2:UTF-8
        private const string language = "1";
        private const string signType = "1";// 1 MD5 2  SHA
	    private const string tranCode = "8888";
        private const string currencyType= "156";//币种 

        private readonly string merOrderNum = "";//订单号
        private readonly string tranAmt= "";//交易金额  
	    private readonly string  feeAmt = "0";//手续费    
        private const string currencyTyp = "156";
        private readonly string frontMerUrl = "";//商户URL 
        private readonly string backgroundMerUrl = "";
	    private readonly string tranDateTime ="";//交易时间  
        //private readonly string virCardNoIn = "0000000001000000584";//国付宝转入帐户
        private readonly string tranIP =Globals. getRealIp();//用户浏览器IP
	    private readonly string msgExt="";//附加信息
        private readonly string bankCode = "";//银行代码
	    private readonly string userType = "";//用户类型
        private string gopayServerTime = Globals.GetGopayServerTime();
        private string sign = "";
        private string subject = ""; //商品名称
        private string body = "";
        private string buyerEmail = "";
	    //private readonly string VerficationCode="12345678";//商户验证码

        [ConfigElement("商户号", Nullable = false)]
        public string merchantID { get; set; }

        [ConfigElement("商户密钥", Nullable = false)]
        public string VerficationCode { get; set; }

        [ConfigElement("国付宝账号", Nullable = false)]
        public string virCardNoIn { get; set; }

        public override void SendRequest()
        {
           
            StringBuilder sb = new StringBuilder();
            sb.Append(CreateField("version",version));
            sb.Append(CreateField("charset",charset));
            sb.Append(CreateField("language", language));
            sb.Append(CreateField("signType",signType));
            sb.Append(CreateField("tranCode",tranCode));
            sb.Append(CreateField("merchantID",merchantID));
            sb.Append(CreateField("merOrderNum",merOrderNum));
            sb.Append(CreateField("tranAmt",tranAmt));
            sb.Append(CreateField("feeAmt",feeAmt));

        //    sb.Append(CreateField("orgtranDateTime",""));
            //sb.Append(CreateField("orgOrderNum",""));
           // sb.Append(CreateField("authID", ""));
           // sb.Append(CreateField("orgtranAmt", ""));

            sb.Append(CreateField("currencyType",currencyType));
            sb.Append(CreateField("frontMerUrl", frontMerUrl));
            sb.Append(CreateField("backgroundMerUrl",backgroundMerUrl));
            sb.Append(CreateField("tranDateTime",tranDateTime));
            sb.Append(CreateField("virCardNoIn", virCardNoIn));
            sb.Append(CreateField("tranIP",tranIP));
            sb.Append(CreateField("isRepeatSubmit", "1")); //是否允许重复提交
            sb.Append(CreateField("goodsName",subject));
            sb.Append(CreateField("goodsDetail",body));
            sb.Append(CreateField("buyerName",""));
            sb.Append(CreateField("buyerContact",buyerEmail));
            sb.Append(CreateField("merRemark1",""));
            sb.Append(CreateField("merRemark2",""));
            sb.Append(CreateField("gopayServerTime",gopayServerTime));
            //sb.Append(CreateField("isLocked",""));
            //sb.Append(CreateField("virCardNo", ""));
            //sb.Append(CreateField("orgTxnStat",""));
            //sb.Append(CreateField("orgTxnType", ""));
            //sb.Append(CreateField("customerEMail", ""));

            sb.Append(CreateField("bankCode",bankCode));
            sb.Append(CreateField("userType", userType));
            sb.Append(CreateField("VerficationCode", VerficationCode));

            
            string plain = "version=[" + version + "]tranCode=[" + tranCode + "]merchantID=[" + merchantID + "]merOrderNum=[" + merOrderNum + "]tranAmt=[" + tranAmt + "]feeAmt=[" + feeAmt + "]tranDateTime=[" + tranDateTime + "]frontMerUrl=[" + frontMerUrl + "]backgroundMerUrl=[" + backgroundMerUrl + "]orderId=[]gopayOutOrderId=[]tranIP=[" + tranIP + "]respCode=[]gopayServerTime=[" + gopayServerTime + "]VerficationCode=[" + VerficationCode + "]";
             sign=Globals.GetMD5(plain);
   sb.Append(CreateField("signValue", sign));
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
