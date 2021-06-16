using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Xml;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.Cncard
{
    public class CncardNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        public CncardNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            string c_mid = parameters["c_mid"];
            string c_order = parameters["c_order"];
            string c_orderamount = parameters["c_orderamount"];
            string c_ymd = parameters["c_ymd"];
            string c_moneytype = parameters["c_moneytype"];
            string c_transnum = parameters["c_transnum"];
            string c_succmark = parameters["c_succmark"];
            string c_cause = parameters["c_cause"];
            string c_memo1 = parameters["c_memo1"];
            string c_memo2 = parameters["c_memo2"];
            string c_signstr = parameters["c_signstr"];

            // 参数检查
            if (c_mid == null || c_order == null || c_orderamount == null || c_ymd == null || c_moneytype == null || c_transnum == null ||
                c_succmark == null || c_cause == null || c_memo1 == null || c_memo2 == null || c_signstr == null
                )
            {
                OnNotifyVerifyFaild();
                return;
            }

            // 交易状态检查
            if (!c_succmark.Equals("Y"))
            {
                OnNotifyVerifyFaild();
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);

            //校验商户编号
            if (c_mid != doc.FirstChild.SelectSingleNode("Cmid").InnerText)
            {
                OnNotifyVerifyFaild();
                return;
            }

            // 签名校验
            string s = c_mid + c_order + c_orderamount + c_ymd + c_transnum + c_succmark + c_moneytype + c_memo1 +
                       c_memo2 + doc.FirstChild.SelectSingleNode("Cpass").InnerText;
            if (!c_signstr.Equals(FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5").ToLower()))
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
            return decimal.Parse(parameters["c_orderamount"]);
        }

        public override string GetOrderId()
        {
            return parameters["c_order"];
        }

        public override string GetGatewayOrderId()
        {
            return parameters["c_transnum"];
        }

    }
}