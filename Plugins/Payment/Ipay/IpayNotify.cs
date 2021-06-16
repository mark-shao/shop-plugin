using System.Collections.Specialized;
using System.Web;
using System.Globalization;
using System.Web.Security;
using System.Xml;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.Ipay
{
    public class IpayNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        public IpayNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            string v_date = parameters["v_date"];
            string v_mid = parameters["v_mid"];
            string v_oid = parameters["v_oid"];
            string v_amount = parameters["v_amount"];
            string v_status = parameters["v_status"];
            string v_md5 = parameters["v_md5"];

            // 参数检查
            if (
                (v_date == null) ||
                (v_mid == null) ||
                (v_oid == null) ||
                (v_amount == null) ||
                (v_status == null) ||
                (v_md5 == null)
                )
            {
                OnNotifyVerifyFaild();
                return;
            }

            // 检查状态
            if (!v_status.Equals("00"))
            {
                OnNotifyVerifyFaild();
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);

            // 检查签名
            string sign =
                FormsAuthentication.HashPasswordForStoringInConfigFile(
                    v_date + v_mid + v_oid + v_amount + v_status + doc.FirstChild.SelectSingleNode("Vkey").Value, "MD5").
                    ToLower(CultureInfo.InvariantCulture);

            if (!v_md5.Equals(sign))
            {
                OnNotifyVerifyFaild();
                return;
            }

            OnFinished(false);
        }

        public override void WriteBack(HttpContext context, bool success)
        {
        }

        public override decimal GetOrderAmount()
        {
            return decimal.Parse(parameters["v_amount"]);
        }

        public override string GetOrderId()
        {
            return parameters["v_oid"];
        }

        public override string GetGatewayOrderId()
        {
            return string.Empty;
        }

    }
}