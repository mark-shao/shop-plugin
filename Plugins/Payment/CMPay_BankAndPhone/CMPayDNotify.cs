using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Xml;
using Hishop.Plugins;
using Com.HisunCmpay;
using System.Collections;

namespace Hishop.Plugins.Payment.CMPay_D
{
    public class CMPayDNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;

        public CMPayDNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            String reqData = IPosMUtil.keyValueToString(parameters);
            Hashtable ht = IPosMUtil.parseStringToMap(reqData);
            
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);

            // 取得singKey值
            String signKey = doc.FirstChild.SelectSingleNode("Key").InnerText;
            //支付结果通知
            String merchantId = (String)ht["merchantId"];
            String payNo = (String)ht["payNo"];
            String returnCode = (String)ht["returnCode"];
            String message = (String)ht["message"];
            String signType = (String)ht["signType"];
            String type = (String)ht["type"];
            String version = (String)ht["version"];
            String amount = (String)ht["amount"];
            String amtItem = (String)ht["amtItem"];
            String bankAbbr = (String)ht["bankAbbr"];
            String mobile = (String)ht["mobile"];
            String orderId = (String)ht["orderId"];
            String payDate = (String)ht["payDate"];
            String accountDate = (String)ht["accountDate"];
            String reserved1 = (String)ht["reserved1"];
            String reserved2 = (String)ht["reserved2"];
            String status = (String)ht["status"];
            String orderDate = (String)ht["orderDate"];
            String fee = (String)ht["fee"];
            String hmac = (String)ht["hmac"];

            //进行验签的原文
            String signData = merchantId + payNo + returnCode + message + signType
                  + type + version + amount + amtItem + bankAbbr + mobile
                  + orderId + payDate + accountDate + reserved1 + reserved2 + status
                  + orderDate + fee;

            if (!"000000".Equals(returnCode))
            {
                OnNotifyVerifyFaild();
                return;
            }

            if ("MD5".Equals(GlobalParam.getInstance().signType))
            {
                if (!SignUtil.verifySign(signData, signKey, hmac))
                {
                    OnNotifyVerifyFaild();
                    return;
                }
            }

            OnFinished(false);
        }

        private static String AppendParam(String returnStr, String paramId, String paramValue)
        {
            if (returnStr != "")
            {
                if (paramValue != "")
                {

                    returnStr += "&" + paramId + "=" + paramValue;
                }
            }
            else
            {
                if (paramValue != "")
                {
                    returnStr = paramId + "=" + paramValue;
                }
            }

            return returnStr;
        }

        public override void WriteBack(HttpContext context, bool success)
        {
            if (context == null)
                return;

            context.Response.Clear();
            context.Response.Write(success ? "success" : "fail");
            context.Response.End();
        }

        public override decimal GetOrderAmount()
        {
            String reqData = IPosMUtil.keyValueToString(parameters);
            Hashtable ht = IPosMUtil.parseStringToMap(reqData);
            String amount = (String)ht["amount"];

            return decimal.Parse(amount) / 100;
        }

        public override string GetOrderId()
        {
            String reqData = IPosMUtil.keyValueToString(parameters);
            Hashtable ht = IPosMUtil.parseStringToMap(reqData);
            String orderId = (String)ht["orderId"];

            return orderId;
        }

        public override string GetGatewayOrderId()
        {

            String reqData = IPosMUtil.keyValueToString(parameters);
            Hashtable ht = IPosMUtil.parseStringToMap(reqData);
            String payNo = (String)ht["payNo"];

            return payNo;
        }

    }
}