using System.Collections.Specialized;
using System.Web;
using System.Globalization;
using System.Web.Security;
using System.Xml;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.IPSExpress
{
    public class IpsExpressNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        public IpsExpressNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            string Merchant = parameters["Merchant"];
            string BillNo = parameters["BillNo"];
            string Amount = parameters["Amount"];
            string Success = parameters["Success"];
            string Remark = parameters["Remark"];
            string Sign = parameters["Sign"];

            // 参数检查
            if (
                Merchant == null ||
                BillNo == null ||
                Amount == null ||
                Success == null ||
                Remark == null ||
                Sign == null
                )
            {
                OnNotifyVerifyFaild();
                return;
            }

            // 标示位检查
            if (!Success.Equals("Y"))
            {
                OnNotifyVerifyFaild();
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);

            // 签名检查
            string s = Merchant + BillNo + Amount + Remark + Success +
                       doc.FirstChild.SelectSingleNode("MerPassword").InnerText;
            if (!Sign.Equals(FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5").ToLower(CultureInfo.InvariantCulture)))
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
            return decimal.Parse(parameters["Amount"]);
        }

        public override string GetOrderId()
        {
            return parameters["BillNo"];
        }

        public override string GetGatewayOrderId()
        {
            return string.Empty;
        }

    }
}