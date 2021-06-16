using System.Collections.Specialized;
using System.Web;
using System.Xml;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.Xpay
{
    public class XpayNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        public XpayNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            string tid = parameters["tid"];
            string bid = parameters["bid"];
            string sid = parameters["sid"];
            string prc = parameters["prc"];
            string actionCode = parameters["actionCode"];
            string actionParameter = parameters["actionParameter"];
            string card = parameters["card"];
            string success = parameters["success"];
            //string bankcode = parameters["bankcode"];
            //string remark1 = parameters["remark1"];
            //string username = parameters["username"];
            string md = parameters["md"];

            // 交易状态
            if (!success.Equals("true"))
            {
                OnNotifyVerifyFaild();
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);
            // 签名
            string s = doc.FirstChild.SelectSingleNode("Key").InnerText + ":" + bid + "," + sid + "," + prc + "," +
                       actionCode + "," + actionParameter + "," + tid + "," + card + "," + success;

            if (!md.Equals(Globals.GetXpayMD5(s)))
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
            return decimal.Parse(parameters["prc"]);
        }

        public override string GetOrderId()
        {
            return parameters["bid"];
        }

        public override string GetGatewayOrderId()
        {
            return parameters["sid"];
        }

    }
}