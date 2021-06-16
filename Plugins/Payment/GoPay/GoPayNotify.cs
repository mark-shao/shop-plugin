using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Globalization;
using System.Xml;
using Hishop.Plugins;
using System.Security.Cryptography;
using System.Text;
namespace Hishop.Plugins.Payment.GoPay
{
    public class GoPayNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        public GoPayNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            string version = parameters["version"];
            string charset = parameters["charset"];
            string language = parameters["language"];
            string signType = parameters["signType"];
            string tranCode = parameters["tranCode"];//交易代码,指明了交易的类型
            string merchantID = parameters["merchantID"];//收网关分配给商户的代码，符合国付宝规范要求
            string merOrderNum = parameters["merOrderNum"];//商户产生的订单号，与交易时间组合唯一标识一笔交易
            string tranAmt = parameters["tranAmt"];//交易金额
            string feeAmt = parameters["feeAmt"];//手续费
            string frontMerUrl = parameters["frontMerUrl"];
            string backgroundMerUrl = parameters["backgroundMerUrl"];
            string tranDateTime = parameters["tranDateTime"];
            string tranIP = parameters["tranIP"];
            string respCode = parameters["respCode"];
            string msgExt = parameters["msgExt"];
            string orderId = GetGatewayOrderId();
            string gopayOutOrderId = parameters["gopayOutOrderId"];
            string bankCode = parameters["bankCode"];
            string tranFinishTime = parameters["tranFinishTime"];
            string merRemark1 = parameters["merRemark1"];
            string merRemark2 = parameters["merRemark2"];
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);
            string VerficationCode = doc.FirstChild.SelectSingleNode("VerficationCode").InnerText;
            string signValueFromGopay = parameters["signValue"];

            // 组织加密明文
            string plain = "version=[" + version + "]tranCode=[" + tranCode + "]merchantID=[" + merchantID + "]merOrderNum=[" + merOrderNum + "]tranAmt=[" + tranAmt + "]feeAmt=[" + feeAmt + "]tranDateTime=[" + tranDateTime + "]frontMerUrl=[" + frontMerUrl + "]backgroundMerUrl=[" + backgroundMerUrl + "]orderId=[" + orderId + "]gopayOutOrderId=[" + gopayOutOrderId + "]tranIP=[" + tranIP + "]respCode=[" + respCode + "]gopayServerTime=[]VerficationCode=[" + VerficationCode + "]";

            string signValue = Globals.GetMD5(plain);

            if (signValue.Equals(signValueFromGopay) && respCode.Equals("0000"))
            {
                OnFinished(false);
            }
            else
            {
                OnNotifyVerifyFaild();
            }
        }

        public override string GetGatewayOrderId()
        {
            return parameters["orderid"];
        }

        public override decimal GetOrderAmount()
        {
            return decimal.Parse(parameters["tranAmt"]);
        }

        public override string GetOrderId()
        {
            return parameters["merOrderNum"];
        }

        public override void WriteBack(HttpContext context, bool success)
        {
            if (context == null || !success)
                return;

            context.Response.Clear();
            context.Response.Write("OK");
            context.Response.End();
        }
    }
}
