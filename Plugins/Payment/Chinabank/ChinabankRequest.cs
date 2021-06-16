using System;
using System.Text;
using System.Web.Security;
using System.Globalization;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.Chinabank
{
    [Plugin("网银在线")]
    public class ChinabankRequest : PaymentRequest
    {

        public ChinabankRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            v_oid = orderId;
            v_amount = amount.ToString("F", CultureInfo.InvariantCulture);
            v_url = returnUrl;
            remark2 = "[url:=" + notifyUrl + "]";
        }

        public ChinabankRequest()
        {
        }

        #region 常量
        private const string Gateway = "https://pay3.chinabank.com.cn/PayGate";
        private const string v_moneytype = "CNY";
        private const string Remark1 = "Chinabank";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// 商户编号
        /// </summary>
        [ConfigElement("商户号", Nullable = false)]
        public string Vmid { get; set; }

        /// <summary>
        /// 商户密钥
        /// </summary>
        [ConfigElement("商户密钥", Nullable = false)]
        public string Key { get; set; }              //账户的支付宝安全校验码

        /// <summary>
        /// 订单编号
        /// </summary>
        private readonly string v_oid = "";

        /// <summary>
        /// 订单总金额
        /// </summary>
        private readonly string v_amount = "";
        
        /// <summary>
        /// 返回URL
        /// </summary>
        private readonly string v_url = "";

        private readonly string remark2 = "";

        public override void SendRequest()
        {
            // 生成签名
            string sign =
                FormsAuthentication.HashPasswordForStoringInConfigFile(
                    v_amount + v_moneytype + v_oid + Vmid + v_url + Key, "MD5").ToUpper(CultureInfo.InvariantCulture);

            StringBuilder sb = new StringBuilder();

            sb.Append(CreateField("v_mid", Vmid));
            sb.Append(CreateField("v_oid", v_oid));
            sb.Append(CreateField("v_amount", v_amount));
            sb.Append(CreateField("v_moneytype", v_moneytype));
            sb.Append(CreateField("v_url", v_url));
            sb.Append(CreateField("remark1", Remark1));
            sb.Append(CreateField("remark2", remark2));
            sb.Append(CreateField("v_md5info", sign));

            SubmitPaymentForm(CreateForm(sb.ToString(), Gateway));
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