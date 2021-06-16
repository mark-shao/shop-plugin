using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Xml;
using System.Web.Security;
using System.Globalization;
using System.Collections.Specialized;

namespace Hishop.Plugins.Payment.BankUnion
{
    public class BankUnionNotify:PaymentNotify
    {
        private readonly NameValueCollection parameters;
        public BankUnionNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            String[] resArr = new String[QuickPayConf.notifyVo.Length];
            for (int i = 0; i < QuickPayConf.notifyVo.Length; i++)
            {
                resArr[i] = parameters[QuickPayConf.notifyVo[i]];
            }
            
            String signature = parameters[QuickPayConf.signature];
            String signMethod = parameters[QuickPayConf.signMethod];

            // 参数检查
            if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(signMethod))
            {
                OnNotifyVerifyFaild();
                return;
            }
            // 状态检查
            if (!resArr[10].Equals("00"))
            {
                OnNotifyVerifyFaild();
                return;
            }
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);


            QuickPayConf.securityKey = doc.FirstChild.SelectSingleNode("Key").InnerText;
            bool signatureCheck = new QuickPayUtils().checkSign(resArr, signMethod, signature);
            // 签名检查
            if (!signatureCheck)
            {
                OnNotifyVerifyFaild();
                return;
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
            return decimal.Parse(parameters[QuickPayConf.notifyVo[6]])/100;
        }

        public override string GetOrderId()
        {
            return parameters[QuickPayConf.notifyVo[8]];
        }

        public override string GetGatewayOrderId()
        {
            return parameters[QuickPayConf.notifyVo[16]]; ;
        }

    }
}
