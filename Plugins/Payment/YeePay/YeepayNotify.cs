using System.Collections.Specialized;
using System.Web;
using System.Xml;
using Hishop.Plugins;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDK.yop.utils;

namespace Hishop.Plugins.Payment.YeePay
{
    /// <summary>
    /// 易宝通知返回
    /// </summary>
    public class YeepayNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        private string responseStr;
        public YeepayNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
            responseStr = HttpContext.Current.Request.Params["response"];//支付成功异步请求过来的数据
            //Buy.AppendLog("response111:" + responseStr);
        }


        public override void VerifyNotify(int timeout, string configXml)
        {
            #region 以前易宝支付的验证
            //string r0_Cmd = parameters["r0_Cmd"];
            //string p1_MerId = parameters["p1_MerId"];
            //string r1_Code = parameters["r1_Code"];
            //string r2_TrxId = parameters["r2_TrxId"];
            //string r3_Amt = parameters["r3_Amt"];
            //string r4_Cur = parameters["r4_Cur"];
            //string r5_Pid = parameters["r5_Pid"];
            //string r6_Order = parameters["r6_Order"];
            //string r7_Uid = parameters["r7_Uid"];
            //string r8_MP = parameters["r8_MP"];
            //string r9_BType = parameters["r9_BType"];
            //string hmac = parameters["hmac"];

            //if (r1_Code != "1")
            //{
            //    OnNotifyVerifyFaild();
            //    return;
            //}

            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(configXml);

            //if (!Buy.VerifyCallback(
            //    p1_MerId, doc.FirstChild.SelectSingleNode("KeyValue").InnerText, r0_Cmd, r1_Code, 
            //    r2_TrxId, r3_Amt, r4_Cur, r5_Pid, r6_Order, r7_Uid, r8_MP, r9_BType, hmac)
            //    )
            //{
            //    OnNotifyVerifyFaild();
            //    return;
            //}

            //OnFinished(false);
            #endregion

            //Buy.AppendLog("timeout:" + timeout + "configXml:" + configXml);

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);

            string privatekey = doc.FirstChild.SelectSingleNode("KeyValue").InnerText;//私钥

            JObject obj = GetParamsObject(privatekey);
            if (obj != null && obj["status"] != null && obj["status"].ToString() == "SUCCESS")
            {
                //Buy.AppendLog("ok了:");
                OnFinished(false);//验证通过
                return;
            }
            else
            {
                OnNotifyVerifyFaild();
                return;
            }

            //Buy.AppendLog("privatekey:" + privatekey);
            //Buy.AppendLog("sourceData:" + sourceData);
        }

        private JObject GetParamsObject(string privatekey)
        {
            if (string.IsNullOrEmpty(privatekey))
            {
                privatekey = Buy.GetPrivateKeyTxt();
                //Buy.AppendLog("getcookie--privatekey:" + privatekey);
            }
            if (string.IsNullOrEmpty(privatekey))
            {
                return null;
            }
                        
            if (string.IsNullOrEmpty(responseStr))
                return null;

            string sourceData = SHA1withRSA.NoticeDecrypt(responseStr, privatekey, Buy.YopPublicKey);

            //Buy.AppendLog("sourceData:" + sourceData);
            JObject obj = (JObject)JsonConvert.DeserializeObject(sourceData);//序列化

            return obj;
        }

        public override void WriteBack(System.Web.HttpContext context, bool success)
        {
        }

        public override decimal GetOrderAmount()
        {
            decimal result = 0;
            string payAmount = parameters["r3_Amt"];
            if (string.IsNullOrEmpty(payAmount))
            {
                decimal.TryParse(payAmount, out result);
                return result;
            }

            JObject obj = GetParamsObject(string.Empty);
            if (obj != null && obj["payAmount"] != null)
            {
                decimal.TryParse(obj["payAmount"].ToString(), out result);
            }
            return result;
        }

        public override string GetOrderId()
        {
            string r6_Order = parameters["r6_Order"];
            if (string.IsNullOrEmpty(r6_Order))
            {
                r6_Order = parameters["OrderId"];
            }

            if (string.IsNullOrEmpty(r6_Order))
            {
                JObject obj = GetParamsObject(string.Empty);
                if (obj != null && obj["orderId"] != null)
                {
                    r6_Order = obj["orderId"].ToString();
                }
                //Buy.AppendLog("r6_Order:" + r6_Order);
            }

            return r6_Order;
        }

        public override string GetGatewayOrderId()
        {
            return parameters["r2_TrxId"];
        }

    }
}