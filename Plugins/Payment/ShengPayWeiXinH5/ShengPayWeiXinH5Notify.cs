using System.Collections.Specialized;
using System.Web.Security;
using System.Xml;
using System.Web;
using System.Text;

namespace Hishop.Plugins.Payment.ShengPayWeiXinH5
{
    public class ShengPayWeiXinH5Notify : PaymentNotify
    {
        private readonly NameValueCollection parameters;

        public ShengPayWeiXinH5Notify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }
        public ShengPayWeiXinH5Notify() { }

        public override string GetGatewayOrderId()
        {
            return parameters["TraceNo"];
        }

        public override decimal GetOrderAmount()
        {
            return decimal.Parse(parameters["OrderAmount"]);
        }

        public override string GetOrderId()
        {
            return parameters["OrderNo"];
        }


        public override void VerifyNotify(int timeout, string configXml)
        {
            string Name = parameters["Name"],
                Version = parameters["Version"],
                Charset = parameters["Charset"],
                TraceNo = parameters["TraceNo"],
                MsgSender = parameters["MsgSender"],
                SendTime = parameters["SendTime"],
                InstCode = parameters["InstCode"],
                OrderNo = parameters["OrderNo"],
                OrderAmount = parameters["OrderAmount"],
                TransNo = parameters["TransNo"],
                TransAmount = parameters["TransAmount"],
                TransStatus = parameters["TransStatus"],
                TransType = parameters["TransType"],
                TransTime = parameters["TransTime"],
                MerchantNo = parameters["MerchantNo"],
                ErrorCode = parameters["ErrorCode"],
                ErrorMsg = parameters["ErrorMsg"],
                Ext1 = parameters["Ext1"],
                Ext2 = parameters["Ext2"],
                SignType = parameters["SignType"],
                remoteSign = parameters["SignMsg"];
            //Name+Version+Charset+TraceNo+MsgSender+SendTime+InstCode+OrderNo+OrderAmount+ TransNo+TransAmount+TransStatus+TransType+TransTime+MerchantNo+ErrorCode+ErrorMsg +Ext1+Ext2+SignType+merchantKey 
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);
            string key = doc.FirstChild.SelectSingleNode("SellerKey").InnerText;
            string origin = Name + Version + Charset + TraceNo + MsgSender + SendTime + InstCode + OrderNo + OrderAmount + TransNo + TransAmount + TransStatus + TransType + TransTime + MerchantNo + ErrorCode + ErrorMsg + Ext1 + Ext2 + SignType;
            //StringBuilder sbOriginStr = new StringBuilder();
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(Name) ? "" : Name + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(Version) ? "" : Version + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(Charset) ? "" : Charset + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(TraceNo) ? "" : TraceNo + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(MsgSender) ? "" : MsgSender + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(SendTime) ? "" : SendTime + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(InstCode) ? "" : InstCode + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(OrderNo) ? "" : OrderNo + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(OrderAmount) ? "" : OrderAmount + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(TransNo) ? "" : TransNo + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(TransAmount) ? "" : TransAmount + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(TransStatus) ? "" : TransStatus + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(TransType) ? "" : TransType + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(TransTime) ? "" : TransTime + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(MerchantNo) ? "" : MerchantNo + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(ErrorCode) ? "" : ErrorCode + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(ErrorMsg) ? "" : ErrorMsg + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(Ext1) ? "" : Ext1 + "|");
            ////sbOriginStr.AppendFormat(string.IsNullOrEmpty(BankSerialNo) ? "" : BankSerialNo + "|");
            //sbOriginStr.AppendFormat(string.IsNullOrEmpty(SignType) ? "" : SignType + "|");
            string sign = FormsAuthentication.HashPasswordForStoringInConfigFile(origin + key, "MD5");

            if ((TransStatus != "01") || (remoteSign != sign))
                OnNotifyVerifyFaild();
            else
            {
                OnFinished(false);
            }
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
