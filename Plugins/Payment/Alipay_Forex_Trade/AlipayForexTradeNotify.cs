using System.Collections.Specialized;
using System.Web;
using System.Globalization;
using System.Xml;
using Hishop.Plugins;
using System;
using System.Text;
using System.IO;

namespace Hishop.Plugins.Payment.AlipayForexTrade
{
    /// <summary>
    /// 通知验证查询
    /// MD5摘要在验证在商户系统中完成
    /// </summary>
    public class AlipayForexTradeNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        private const string InputCharset = "utf-8";
        bool IsNotify = true;
        public AlipayForexTradeNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
            if (this.parameters["IsReturn"] != null && this.parameters["IsReturn"].ToLower() == "true")
            {
                this.parameters.Remove("IsReturn");
                IsNotify = false;
            }
        }



        /// <summary>
        /// 创建验证Url
        /// </summary>
        /// <returns></returns>
        private string CreateUrl(XmlNode configNode)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "https://mapi.alipay.com/gateway.do?service=notify_verify&partner={0}&notify_id={1}",
                configNode.SelectSingleNode("Partner").InnerText, parameters["notify_id"]
                );
        }

        public override string GetGatewayOrderId()
        {
            return parameters["trade_no"];
        }

        public override decimal GetOrderAmount()
        {
            return decimal.Parse(parameters["total_fee"]);
        }

        public override string GetOrderId()
        {
            return parameters["out_trade_no"];
        }

        public override void WriteBack(HttpContext context, bool success)
        {
            if (context == null)
                return;

            context.Response.Clear();
            context.Response.Write(success ? "success" : "fail");
            context.Response.End();
        }


        /// <summary>
        /// 通知验证
        /// </summary>
        /// <returns>成功，返回true，失败，返回false</returns>
        public override void VerifyNotify(int timeout, string configXml)
        {
            bool isValid = true;
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);
            #region 通知的时候会出现2次验证 第2次会返回false

            if (IsNotify)
            {
                //// 通知验证
                try
                {
                    isValid = bool.Parse(GetResponse(CreateUrl(doc.FirstChild), timeout));
                    //PayLog.writeLog_Collection(parameters, GetResponse(CreateUrl(doc.FirstChild), timeout) + "-" + CreateUrl(doc.FirstChild), HttpContext.Current.Request.Url.ToString(), isValid.ToString(), LogType.Alipay_Forex);
                }
                catch (Exception ex)
                {
                    PayLog.writeLog_Collection(parameters, GetResponse(CreateUrl(doc.FirstChild), timeout) + "-" + CreateUrl(doc.FirstChild), HttpContext.Current.Request.Url.ToString(), ex.Message, LogType.Alipay_Forex);
                    isValid = false;
                }
            }
            else
            {
                isValid = true;
            }
            #endregion
            string sign = parameters["sign"];
            int i;
            parameters.Remove("HIGW");
            string[] requestarr = parameters.AllKeys;

            // 参数排序
            string[] sortedstr = Globals.BubbleSort(requestarr);

            // 构造待md5摘要字符串
            string prestr = "";
            for (i = 0; i < sortedstr.Length; i++)
            {
                if (
                    !string.IsNullOrEmpty(parameters[sortedstr[i]]) &&
                    (sortedstr[i] != "sign") &&
                    (sortedstr[i] != "sign_type")
                    )
                {
                    if (i == sortedstr.Length - 1)
                    {
                        prestr = prestr + sortedstr[i] + "=" + parameters[sortedstr[i]];
                    }
                    else
                    {
                        prestr = prestr + sortedstr[i] + "=" + parameters[sortedstr[i]] + "&";
                    }
                }
            }

            prestr = prestr + doc.FirstChild.SelectSingleNode("Key").InnerText;
            string createSign = Globals.GetMD5(prestr, InputCharset);
            isValid = isValid && (sign == createSign);
            string stradeStatus = parameters["trade_status"];

            if (isValid && (stradeStatus == "TRADE_SUCCESS" || stradeStatus == "TRADE_FINISHED" || stradeStatus == "SUCCESS"))
            {
                //PayLog.AppendLog_Collection(parameters, parameters["sign"], Globals.GetMD5(prestr, InputCharset), "签名验证成功", LogType.Alipay_Forex);
                // 支付宝的交易结束就是打款到商家账户上了
                OnFinished(false);
            }
            else
            {
                PayLog.AppendLog_Collection(parameters
                    , sign + '-' + createSign
                    , GetResponse(CreateUrl(doc.FirstChild), timeout) + "---" + CreateUrl(doc.FirstChild)
                    , "签名验证失败"
                    , LogType.Alipay_Forex);
                OnNotifyVerifyFaild();
            }
        }
    }
}
