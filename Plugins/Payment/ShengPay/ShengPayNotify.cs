using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml;

namespace Hishop.Plugins.Payment.ShengPay
{
    public class ShengPayNotify : PaymentNotify
    {
        private readonly NameValueCollection _parameters;

        public ShengPayNotify(NameValueCollection parameters)
        {
            _parameters = parameters;
            //PayLog.AppendLog_Collection(parameters, "", "", "盛付通回调进入", LogType.ShengPay);
        }

        public override string GetGatewayOrderId()
        {
            return _parameters["TransNo"];
        }

        public override decimal GetOrderAmount()
        {
            return decimal.Parse(_parameters["TransAmount"]);
        }

        public override string GetOrderId()
        {
            return _parameters["OrderNo"];
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            string Name = _parameters["Name"],
                   Version = _parameters["Version"],
                   Charset = _parameters["Charset"],
                   TraceNo = _parameters["TraceNo"],
                   MsgSender = _parameters["MsgSender"],
                   SendTime = _parameters["SendTime"],
                   InstCode = _parameters["InstCode"],
                   OrderNo = _parameters["OrderNo"],
                   OrderAmount = _parameters["OrderAmount"],
                   TransNo = _parameters["TransNo"],
                   TransAmount = _parameters["TransAmount"],
                   TransStatus = _parameters["TransStatus"],
                   TransType = _parameters["TransType"],
                   TransTime = _parameters["TransTime"],
                   MerchantNo = _parameters["MerchantNo"],
                   PaymentNo = _parameters["PaymentNo"],
                   ErrorCode = _parameters["ErrorCode"],
                   ErrorMsg = _parameters["ErrorMsg"],
                   PayableFee = _parameters["PayableFee"],
                   ReceivableFee = _parameters["ReceivableFee"],
                   PayChannel = _parameters["PayChannel"],
                   Ext1 = _parameters["Ext1"],
                   BankSerialNo = _parameters["BankSerialNo"],
                   SignType = _parameters["SignType"],
                   mac = _parameters["SignMsg"];
            try
            {
                IDictionary<string, string> param = new Dictionary<string, string>();
                foreach (string s in _parameters.AllKeys)
                {
                    param.Add(s, _parameters[s]);
                }
                string data = "";
                string error = "";
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.LoadXml(configXml);
                string key = doc.FirstChild.SelectSingleNode("Key").InnerText;
                string origin = Name + Version + Charset + TraceNo + MsgSender + SendTime + InstCode + OrderNo + OrderAmount + TransNo
                    + TransAmount + TransStatus + TransType + TransTime + MerchantNo + ErrorCode + ErrorMsg + Ext1 + BankSerialNo + SignType;


                StringBuilder sbOriginStr = new StringBuilder();
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(Name) ? "" : Name + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(Version) ? "" : Version + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(Charset) ? "" : Charset + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(TraceNo) ? "" : TraceNo + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(MsgSender) ? "" : MsgSender + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(SendTime) ? "" : SendTime + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(InstCode) ? "" : InstCode + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(OrderNo) ? "" : OrderNo + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(OrderAmount) ? "" : OrderAmount + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(TransNo) ? "" : TransNo + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(TransAmount) ? "" : TransAmount + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(TransStatus) ? "" : TransStatus + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(TransType) ? "" : TransType + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(TransTime) ? "" : TransTime + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(MerchantNo) ? "" : MerchantNo + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(ErrorCode) ? "" : ErrorCode + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(ErrorMsg) ? "" : ErrorMsg + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(Ext1) ? "" : Ext1 + "|");
                //sbOriginStr.AppendFormat(string.IsNullOrEmpty(BankSerialNo) ? "" : BankSerialNo + "|");
                sbOriginStr.AppendFormat(string.IsNullOrEmpty(SignType) ? "" : SignType + "|");

                data = sbOriginStr.ToString();

                string sign = FormsAuthentication.HashPasswordForStoringInConfigFile(sbOriginStr.ToString() + key, "MD5");

                data += "|status:" + TransStatus + "|sign:" + sign + "|mac:" + mac + "";
                if ((TransStatus != "01") || (mac != sign))
                {
                    error = "回调签名错误";
                    PayLog.AppendLog(param, sign, error, data, LogType.ShengPay);
                    OnNotifyVerifyFaild();
                }
                else
                    OnFinished(false);
            }
            catch (Exception ex)
            {
                PayLog.WriteExpectionLog_Page(ex, _parameters, LogType.ShengPay);
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