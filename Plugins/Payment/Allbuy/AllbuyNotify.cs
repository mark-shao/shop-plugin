using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Globalization;
using System.Xml;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.Allbuy
{
    public class AllbuyNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;

        public AllbuyNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            string merchant = parameters["merchant"];
            string billno = parameters["billno"];
            string vPstring = parameters["v_pstring"];
            string amount = parameters["amount"];
            string success = parameters["success"];
            string remark = parameters["remark"];
            string sign = parameters["sign"];

            // 检查参数
            if (merchant == null || billno == null || vPstring == null || amount == null || success == null || remark == null || sign == null)
            {
                OnNotifyVerifyFaild();
                return;
            }

            // 状态检查
            if (!success.Equals("Y")) {
                OnNotifyVerifyFaild();
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);

            // 签名检查
            string s = merchant + billno + amount + success + doc.FirstChild.SelectSingleNode("Key").InnerText;
            if (!sign.Equals(FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5").ToLower(CultureInfo.InvariantCulture)))
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
            return decimal.Parse(parameters["amount"]);
        }

        public override string GetOrderId()
        {
            return parameters["billno"];
        }

        public override string GetGatewayOrderId()
        {
            return string.Empty;
        }

    }
}