using System;
using System.Text;
using System.Globalization;
using System.Web.Security;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.IPSExpress
{
    [Plugin("环迅易捷支付")]
    public class IpsExpressRequest : PaymentRequest
    {

        public IpsExpressRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            BillNo = orderId;
            Amount = amount.ToString("F", CultureInfo.InvariantCulture);
            BackUrl = returnUrl;
        }

        public IpsExpressRequest()
        {
        }

        #region 常量
        private const string PostUrl = "http://express.ips.com.cn/pay/payment.asp";
        private const string Remark = "IPSExpress";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }

        [ConfigElement("商户号", Nullable = false)]
        public string Merchant { get; set; }

        private readonly string BillNo;
        private readonly string Amount;
        private readonly string BackUrl;

        [ConfigElement("商户密钥", Nullable = false)]
        public string MerPassword { get; set; }

        public override void SendRequest()
        {
            // 生成签名
            string s = Merchant + BillNo + Amount + Remark + MerPassword;
            string sign = FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5").ToLower(CultureInfo.InvariantCulture);

            StringBuilder sb = new StringBuilder();

            //构建支付参数
            sb.Append(CreateField("Merchant", Merchant));
            sb.Append(CreateField("BillNo", BillNo));
            sb.Append(CreateField("Amount", Amount));
            sb.Append(CreateField("Remark", Remark));
            sb.Append(CreateField("BackUrl", BackUrl));
            sb.Append(CreateField("Sign", sign));

            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00018\" checked>中国工商银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00021\">招商银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00003\">中国建设银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00017\">中国农业银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00013\">民生银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00030\">光大银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00016\">兴业银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00111\">中国银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00211\">交通银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00311\">交通银行上海" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00411\">广东发展银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00023\">深圳发展银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00032\">浦东发展银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00511\">中信实业银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00611\">广州商业银行" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00711\">邮政储蓄" + System.Environment.NewLine);
            sb.Append("<input type=\"radio\" name=\"PayBank\" value=\"00811\">华夏银行" + System.Environment.NewLine);

            SubmitPaymentForm(CreateForm(sb.ToString(), PostUrl));
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