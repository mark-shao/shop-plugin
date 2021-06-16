using System.Collections.Specialized;
using System.Web;
using System.Globalization;
using System.Xml;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.Alipay
{
    /// <summary>
    /// 通知验证查询
    /// MD5摘要在验证在商户系统中完成
    /// </summary>
    public class AlipayNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        private const string InputCharset = "utf-8";

        public AlipayNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
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
        public override void VerifyNotify(int timeout, string configXml)
        {
            bool isValid;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);

            // 通知验证
            try
            {
                isValid = bool.Parse(GetResponse(CreateUrl(doc.FirstChild), timeout));
            }
            catch
            {
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
            isValid = isValid && (parameters["sign"].Equals(Globals.GetMD5(prestr, InputCharset)));

            if (isValid)
            {
                switch (parameters["trade_status"])
                {
                    // 支付宝的买家已付款表示买家已付款给支付宝
                    case "WAIT_SELLER_SEND_GOODS":
                        OnPayment();
                        break;

                    // 支付宝的交易结束就是打款到商家账户上了
                    case "TRADE_FINISHED":
                        if (parameters["payment_type"] == "1")
                        OnFinished(false);
                        else
                            OnFinished(true);
                        break;

                    //// 交易中途关闭
                    //case "TRADE_CLOSED":
                    //    OnClosed();
                    //    break;

                    //// 卖家发货
                    //case "WAIT_BUYER_CONFIRM_GOODS":
                    //    OnSendGoods();
                    //    break;
                }
            }
            else
                OnNotifyVerifyFaild();
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
            return decimal.Parse(parameters["total_fee"]);
        }

        public override string GetOrderId()
        {
            return parameters["out_trade_no"];
        }

        public override string GetGatewayOrderId()
        {
            return parameters["trade_no"];
        }

    }
}