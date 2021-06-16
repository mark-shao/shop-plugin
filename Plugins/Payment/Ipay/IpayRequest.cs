using System;
using System.Text;
using System.Globalization;
using System.Web.Security;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.Ipay
{
    [Plugin("中国在线支付网")]
    public class IpayRequest : PaymentRequest
    {

        public IpayRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            v_oid = orderId;
            v_amount = amount.ToString("F", CultureInfo.InvariantCulture);
            v_email = buyerEmail;
            v_url = returnUrl;
        }

        public IpayRequest()
        {
        }

        #region 常量
        private const string Gateway = "http://www.ipay.cn/4.0/bank.shtml";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// 商户编号,数字组成,由IPAY分配
        /// </summary>
        [ConfigElement("商户号", Nullable = false)]
        public string Vmid { get; set; }

        /// <summary>
        /// 私钥
        /// </summary>
        [ConfigElement("私钥", Nullable = false)]
        public string Vkey { get; set; }

        /// <summary>
        /// 商户订单编号,由数字0至9组成,一天内不重复
        /// Max(14)
        /// </summary>
        private readonly string v_oid = "";

        /// <summary>
        /// 订单金额,数字组成,小数点后最多保留二位
        /// </summary>
        private readonly string v_amount = "";

        /// <summary>
        /// 消费者EMAIL，凭此查询订单
        /// </summary>
        private readonly string v_email = "";

        /// <summary>
        /// 支付完毕后返回地址，优先级大于静态返回URL
        /// </summary>
        private readonly string v_url = "";

        public override void SendRequest()
        {
            // 生成签名
            string sign =
                FormsAuthentication.HashPasswordForStoringInConfigFile(
                    Vmid + v_oid + v_amount + v_email + Vkey, "MD5").ToLower(CultureInfo.InvariantCulture);

            StringBuilder sb = new StringBuilder();

            sb.Append(CreateField("v_mid", Vmid));
            sb.Append(CreateField("v_oid", v_oid));
            sb.Append(CreateField("v_amount", v_amount));
            sb.Append(CreateField("v_email", v_email));
            sb.Append(CreateField("v_mobile", ""));
            sb.Append(CreateField("v_md5", sign));
            sb.Append(CreateField("v_url", v_url));

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