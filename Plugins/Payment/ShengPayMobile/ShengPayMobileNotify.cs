using System.Collections.Specialized;
using System.Web.Security;
using System.Xml;
using System.Web;
using System.Text;
using System;

namespace Hishop.Plugins.Payment.ShengPayMobile
{
    public class ShengPayMobileNotify : PaymentNotify
    {
        private const string publicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC69veKW1X9GETEFr49gu9PN8w7H6alWec8wmF8SoP3tqQLAflZp8g83UZPX2UWhClnm53P5ZwesaeSTHkXkSI0iSjwd27N07bc8puNgB5BAGhJ80KYqTv3Zovl04C8AepVmxy9iFniJutJSYYtsRcnHYyUNoJai4VXhJsp5ZRMqwIDAQAB";
        private readonly NameValueCollection parameters;

        public ShengPayMobileNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }
        public ShengPayMobileNotify() { }

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
                outMemberId = parameters["outMemberId"],
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
            string key = "";

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);

            if (doc != null && doc.FirstChild != null && doc.FirstChild.SelectSingleNode("SellerKey") != null)
            {
                key = doc.FirstChild.SelectSingleNode("SellerKey").InnerText;
            }
            //Origin = Name +|+ Version +|+ Charset +|+ TraceNo +|+ MsgSender +|+ SendTime +|+ InstCode +|+ OrderNo +|+ OrderAmount +|+ TransNo +|+ TransAmount +|+ TransStatus +|+ TransType +|+ TransTime +|+ MerchantNo +|+ ErrorCode +|+ ErrorMsg +|+ Ext1 +|+ SignType +|;
            string origin = string.IsNullOrEmpty(Name) ? "" : Name + "|";
            origin += string.IsNullOrEmpty(Version) ? "" : Version + "|";
            origin += string.IsNullOrEmpty(Charset) ? "" : Charset + "|";
            origin += string.IsNullOrEmpty(TraceNo) ? "" : TraceNo + "|";
            origin += string.IsNullOrEmpty(MsgSender) ? "" : MsgSender + "|";
            origin += string.IsNullOrEmpty(outMemberId) ? "" : outMemberId + "|";
            origin += string.IsNullOrEmpty(SendTime) ? "" : SendTime + "|";
            origin += string.IsNullOrEmpty(InstCode) ? "" : InstCode + "|";
            origin += string.IsNullOrEmpty(OrderNo) ? "" : OrderNo + "|";
            origin += string.IsNullOrEmpty(OrderAmount) ? "" : OrderAmount + "|";
            origin += string.IsNullOrEmpty(TransNo) ? "" : TransNo + "|";
            origin += string.IsNullOrEmpty(TransAmount) ? "" : TransAmount + "|";
            origin += string.IsNullOrEmpty(TransStatus) ? "" : TransStatus + "|";
            origin += string.IsNullOrEmpty(TransType) ? "" : TransType + "|";
            origin += string.IsNullOrEmpty(TransTime) ? "" : TransTime + "|";
            origin += string.IsNullOrEmpty(MerchantNo) ? "" : MerchantNo + "|";
            origin += string.IsNullOrEmpty(ErrorCode) ? "" : ErrorCode + "|";
            origin += string.IsNullOrEmpty(ErrorMsg) ? "" : ErrorMsg + "|";
            origin += string.IsNullOrEmpty(Ext1) ? "" : Ext1 + "|";
            origin += string.IsNullOrEmpty(Ext2) ? "" : Ext2 + "|";
            origin += string.IsNullOrEmpty(SignType) ? "" : SignType + "|";
            string sign = "";
            bool isSuccessed = false;
            if (SignType.ToLower() == "MD5")
            {
                sign = FormsAuthentication.HashPasswordForStoringInConfigFile(origin + key, "MD5").ToLower();
                if (sign == remoteSign.ToLower())
                {
                    isSuccessed = true;
                }
            }
            else
            {
                try
                {
                    isSuccessed = RSAFromPkcs8.verify(origin, remoteSign, publicKey);
                }
                catch (Exception ex)
                {
                    PayLog.WriteExpectionLog_Page(ex, parameters, LogType.ShengpayMobile);
                }
            }
            //string sign = FormsAuthentication.HashPasswordForStoringInConfigFile(origin + key, "MD5").ToLower();

            if ((TransStatus != "01") || !isSuccessed)
            {
                PayLog.writeLog_Collection(parameters, origin, sign, "签名验证失败或者状态不正确 ", LogType.ShengpayMobile);
                OnNotifyVerifyFaild();
            }
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
