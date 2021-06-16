using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Globalization;
using System.Xml;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.Chinabank
{
    public class ChinabankNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        public ChinabankNotify(NameValueCollection parameters)
        {
           // PayLog.AppendLog_Collection(parameters, "", "0", "进入通知页面", LogType.ChinaBank);
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            try
            {
                string v_oid = parameters["v_oid"];
                string v_pstatus = parameters["v_pstatus"];
                string v_pstring = parameters["v_pstring"];
                string v_pmode = parameters["v_pmode"];
                string v_md5str = parameters["v_md5str"];
                string v_amount = parameters["v_amount"];
                string v_moneytype = parameters["v_moneytype"];
                string remark1 = parameters["remark1"];

                // 参数检查
                if (
                    (v_oid == null) ||
                    (v_pstatus == null) ||
                    (v_pstring == null) ||
                    (v_pmode == null) ||
                    (v_md5str == null) ||
                    (v_amount == null) ||
                    (remark1 == null) ||
                    (v_moneytype == null)
                    )
                {
                    PayLog.AppendLog_Collection(parameters, "", "1", configXml, LogType.ChinaBank);
                    OnNotifyVerifyFaild();
                    return;
                }

                // 状态检查
                if (!v_pstatus.Equals("20"))
                {
                    PayLog.AppendLog_Collection(parameters, "", "2", configXml, LogType.ChinaBank);
                    OnNotifyVerifyFaild();
                    return;
                }

                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.LoadXml(configXml);

                string sign =
                    FormsAuthentication.HashPasswordForStoringInConfigFile(
                        v_oid + v_pstatus + v_amount + v_moneytype + doc.FirstChild.SelectSingleNode("Key").InnerText, "MD5").
                        ToUpper(CultureInfo.InvariantCulture);
                // 签名检查
                if (!v_md5str.Equals(sign))
                {
                    PayLog.AppendLog_Collection(parameters, sign, "3", configXml, LogType.ChinaBank);
                    OnNotifyVerifyFaild();
                    return;
                }
            }
            catch (System.Exception ex)
            {
                PayLog.AppendLog_Collection(parameters, "", "3", configXml + "---" + ex.Message, LogType.ChinaBank);
            }

            OnFinished(false);
        }

        public override void WriteBack(HttpContext context, bool success)
        {
            if (context == null)
                return;

            context.Response.Clear();
            context.Response.Write(success ? "ok" : "error");
            context.Response.End();
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