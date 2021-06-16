using System.Collections.Specialized;
using System.Web;
using System.Globalization;
using System.Xml;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.Tenpay
{
    public class TenpayNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        public TenpayNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            string cmdno = parameters["cmdno"];
            string pay_result = parameters["pay_result"];
            string pay_info = UrlDecode(parameters["pay_info"]);
            string date = parameters["date"];
            string bargainor_id = parameters["bargainor_id"];
            string transaction_id = parameters["transaction_id"];
            string sp_billno = parameters["sp_billno"];
            string total_fee = parameters["total_fee"];
            string fee_type = parameters["fee_type"];
            string attach = parameters["attach"];
            string sign = parameters["sign"];

            // 参数检查
            if (cmdno == null || pay_result == null || pay_info == null || date == null || bargainor_id == null || transaction_id == null ||
                sp_billno == null || total_fee == null || fee_type == null || attach == null || sign == null
                )
            {
                OnNotifyVerifyFaild();
                return;
            }

            // 交易状态
            if (!pay_result.Equals("0"))
            {
                OnNotifyVerifyFaild();
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);

            // 签名验证
            string s = "cmdno=" + cmdno + "&pay_result=" + pay_result + "&date=" + date + "&transaction_id=" +
                       transaction_id
                       + "&sp_billno=" + sp_billno + "&total_fee=" + total_fee + "&fee_type=" + fee_type + "&attach=" +
                       attach + "&key=" + doc.FirstChild.SelectSingleNode("Key").InnerText;

            if (!sign.Equals(Globals.GetMD5(s)))
            {
                OnNotifyVerifyFaild();
                return;
            }

            OnFinished(false);
        }

        /// <summary>
        /// 对字符串进行URL解码
        /// </summary>
        /// <param name="instr">待解码的字符串</param>
        /// <returns>解码结果</returns>
        private string UrlDecode(string instr)
        {
            if (instr == null || instr.Trim() == "")
                return "";

            return instr.Replace("%3d", "=").Replace("%26", "&").Replace("%22", "\"").Replace("%3f", "?")
                .Replace("%27", "'").Replace("%20", " ").Replace("%25", "%");
        }

        public override void WriteBack(HttpContext context, bool success)
        {
        }

        public override decimal GetOrderAmount()
        {
            return decimal.Parse(parameters["total_fee"], CultureInfo.InvariantCulture) / 100;
        }

        public override string GetOrderId()
        {
            return parameters["sp_billno"];
        }

        public override string GetGatewayOrderId()
        {
            return parameters["transaction_id"];
        }

    }
}