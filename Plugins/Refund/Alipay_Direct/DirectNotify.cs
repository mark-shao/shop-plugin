using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;

namespace Hishop.Plugins.Refund.AlipayDirect
{
    public class DirectNotify : Hishop.Plugins.RefundNotify
    {
        private readonly NameValueCollection parameters;
        private const string InputCharset = "utf-8";

        public DirectNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
            //RefundLog.AppendLog_Collection(parameters, "", HttpContext.Current.Request.Url.ToString(), "进入退款通知页面", LogType.Alipay_Direct);
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

        /// <summary>
        /// 通知验证
        /// </summary>
        /// <returns>成功，返回true，失败，返回false</returns>
        public override bool VerifyNotify(int timeout, string configXml)
        {
            timeout = 60000;
            bool isValid;
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);

            // 通知验证
            try
            {
                string responseStr = GetResponse(CreateUrl(doc.FirstChild), timeout);
                if (responseStr == "Error:The operation has timed out")
                {
                    isValid = true;
                }
                else
                {
                    bool.TryParse(responseStr, out isValid);
                }
                if (!isValid)
                {
                    RefundLog.AppendLog_Collection(parameters, configXml, CreateUrl(doc.FirstChild), "退款通知服务器验证" + (isValid ? "成功" : "失败:" + responseStr), LogType.Alipay_Direct);
                }
            }
            catch (Exception ex)
            {
                RefundLog.AppendLog_Collection(parameters, configXml, CreateUrl(doc.FirstChild), "退款通知服务器验证异常，错误：" + ex.Message, LogType.Alipay_Direct);
                isValid = false;
            }

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
            string clientSign = Globals.GetMD5(prestr, InputCharset);
            isValid = isValid && (parameters["sign"].Equals(clientSign));
            if (!isValid)
            {
                RefundLog.AppendLog_Collection(parameters, Globals.GetMD5(prestr, InputCharset), HttpContext.Current.Request.Url.ToString(), "退款通知签名验证失败,新签名：" + clientSign, LogType.Alipay_Direct);
            }
            // string stradeStatus = parameters["trade_status"];
            return isValid;
            //if (isValid)
            //{
            //    // 支付宝的交易结束就是打款到商家账户上了
            //    OnRefund();
            //}
            //else
            //    OnNotifyVerifyFaild();
        }

        public override void WriteBack(HttpContext context, bool success)
        {
            if (context == null)
                return;

            context.Response.Clear();
            context.Response.Write(success ? "success" : "fail");
            context.Response.End();
        }

        public override string GetOrderId()
        {
            return parameters["out_trade_no"];
        }

    }
}
