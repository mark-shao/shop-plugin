using System;
using System.Text;
using System.Globalization;
using System.Web.Security;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.Cncard
{
    [Plugin("云网支付")]
    public class CncardRequest : PaymentRequest
    {

        public CncardRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            c_order = orderId;
            c_orderamount = amount.ToString("F", CultureInfo.InvariantCulture);
            c_ymd = date.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
            c_returl = returnUrl;
        }

        public CncardRequest()
        {
        }

        #region 常量
        private const string GatewayUrl = "https://www.cncard.net/purchase/getorder.asp";

        /// <summary>
        /// 支付币种
        /// </summary>
        private const string c_moneytype = "0";

        /// <summary>
        /// 商户参数一
        /// </summary>
        private const string c_memo1 = "Cncard";

        /// <summary>
        /// 返回标识
        /// </summary>
        private const string c_retflag = "1";

        /// <summary>
        /// 支付结果通知方式
        /// </summary>
        private const string notifytype = "0";

        private const string c_language = "0";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// 商户编号
        /// </summary>
        [ConfigElement("商户号", Nullable = false)]
        public string Cmid { get; set; }

        /// <summary>
        /// 支付密钥，请登录商户管理后台，在账户信息->基本信息->安全信息中的支付密钥项
        /// </summary>
        [ConfigElement("支付密钥", Nullable = false)]
        public string Cpass { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        private readonly string c_order = "";

        /// <summary>
        /// 订单总金额
        /// </summary>
        private readonly string c_orderamount = "";

        /// <summary>
        /// 订单产生日期
        /// </summary>
        private readonly string c_ymd = "";
        
        /// <summary>
        /// 支付结果页面URL
        /// </summary>
        private readonly string c_returl = "";

        public override void SendRequest()
        {
            // 生成签名
            string srcStr = Cmid + c_order + c_orderamount + c_ymd + c_moneytype + c_retflag + c_returl +
                            c_memo1 + notifytype + c_language + Cpass;
            string c_signstr = FormsAuthentication.HashPasswordForStoringInConfigFile(srcStr, "MD5").ToLower();

            StringBuilder sb = new StringBuilder();

            sb.Append(CreateField("c_mid", Cmid));
            sb.Append(CreateField("c_order", c_order));
            sb.Append(CreateField("c_orderamount", c_orderamount));
            sb.Append(CreateField("c_ymd", c_ymd));
            sb.Append(CreateField("c_moneytype", c_moneytype));
            sb.Append(CreateField("c_retflag", c_retflag));
            sb.Append(CreateField("c_returl", c_returl));
            sb.Append(CreateField("c_paygate", ""));
            sb.Append(CreateField("c_memo1", c_memo1));
            sb.Append(CreateField("c_memo2", ""));
            sb.Append(CreateField("c_language", c_language));
            sb.Append(CreateField("notifytype", notifytype));
            sb.Append(CreateField("c_signstr", c_signstr));
            
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