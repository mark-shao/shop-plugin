using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Globalization;
using System.Xml;
using Hishop.Plugins;
using System.Text;

namespace Hishop.Plugins.Payment.IPS
{
    public class IpsNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        public IpsNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            //string mercode = parameters["mercode"];
            string billno = parameters["billno"];
            string amount = parameters["amount"];
            string mydate = parameters["date"];
            string ipsbillno = parameters["ipsbillno"];
            string succ = parameters["succ"];
            string retEncodeType = parameters["retEncodeType"];
            //string msg = parameters["msg"];
            string currency_type = parameters["currency_type"];
            //string attach = parameters["attach"];
            string signature = parameters["signature"];

            // 参数检查
            if (
                billno == null ||
                amount == null ||
                mydate == null ||
                ipsbillno == null ||
                succ == null ||
                retEncodeType == null ||
                currency_type == null ||
                signature == null
                )
            {
                OnNotifyVerifyFaild();
                return;
            }

            // 标示位检查
            if (!succ.Equals("Y"))
            {
                OnNotifyVerifyFaild();
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);

            // 签名检查


            // 生成签名
            StringBuilder sbsign = new StringBuilder();
            sbsign.AppendFormat("billno{0}currencytype{1}amount{2}date{3}", billno, currency_type,amount, mydate);
            sbsign.AppendFormat("succ{0}ipsbillno{1}retencodetype{2}{3}", succ, ipsbillno, retEncodeType, doc.FirstChild.SelectSingleNode("Cert").InnerText);
          // string sign = FormsAuthentication.HashPasswordForStoringInConfigFile(sbsign.ToString(), "MD5").ToLower(CultureInfo.InvariantCulture);
            string sign = Globals.GetMD5(sbsign.ToString()).ToLower(CultureInfo.InvariantCulture);
            if (!signature.Equals(sign))
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