using System.Collections.Specialized;
using System.Web;
using System.Globalization;
using System.Xml;
using Hishop.Plugins;
using System.Data;
using System;

namespace Hishop.Plugins.Payment.AlipayAssure
{
    /// <summary>
    /// 通知验证查询
    /// MD5摘要在验证在商户系统中完成
    /// </summary>
    public class AlipayAssureNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        private const string InpuitCharset = "utf-8";

        public AlipayAssureNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
            //PayLog.AppendLog_Collection(parameters, "", "", "进入通知页面", LogType.Alipay_Assure);
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
                configNode.SelectSingleNode("Partner").InnerText.Trim(), parameters["notify_id"]
                );
        }

        /// <summary>
        /// 通知验证
        /// </summary>
        /// <returns>成功，返回true，失败，返回false</returns>
        public override void VerifyNotify(int timeout, string configXml)
        {

            bool isValid;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);
            // 通知验证
            try
            {
                isValid = bool.Parse(GetResponse(CreateUrl(doc.FirstChild), timeout));
                //isValid = parameters["is_success"] == "T";
               // writeXML("验证成功1", isValid.ToString() + "-" + CreateUrl(doc.FirstChild));
            }
            catch (Exception ex)
            {
                //writeXML("验证失败1---" + parameters["is_success"], ex.Message);
                PayLog.writeLog_Collection(parameters, "", HttpContext.Current.Request.Url.ToString(), ex.Message, LogType.Alipay_Direct);
                isValid = false;
            }

            if(!isValid)
            {
                PayLog.AppendLog_Collection(parameters, "", HttpContext.Current.Request.Url.ToString(), "is_success值错了", LogType.Alipay_Direct);
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
            string validSign = Globals.GetMD5(prestr, InpuitCharset);
            isValid = isValid && (parameters["sign"].Equals(Globals.GetMD5(prestr, InpuitCharset)));

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
                    case "SUCCESS":
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
            {
                PayLog.writeLog_Collection(parameters, Globals.GetMD5(prestr, InpuitCharset), HttpContext.Current.Request.Url.ToString(), "签名错误" + "---" + doc.FirstChild.SelectSingleNode("Key").InnerText, LogType.Alipay_Direct);
                OnNotifyVerifyFaild();
            }
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
        public void writeXML(string Data, string ErrorMsg)
        {
            DataTable dt = new DataTable();
            dt.TableName = "Error";
            dt.Columns.Add("DateTime");
            dt.Columns.Add("Error");
            dt.Columns.Add("Data");
            DataRow dr = dt.NewRow();
            dr["DateTime"] = DateTime.Now;
            dr["Error"] = ErrorMsg;
            dr["Data"] = Data;
            dt.Rows.Add(dr);
            dt.WriteXml(HttpContext.Current.Request.MapPath("/PayLog.xml"));


        }

        public void WriteError(string Error, NameValueCollection parameters, string configXml = "")
        {
            DataTable dt = new DataTable();
            dt.TableName = "QRCodealipay";
            dt.Columns.Add(new DataColumn("OperTime"));
            dt.Columns.Add(new DataColumn("msg"));
            //IEnumerator myParam = parameters.GetEnumerator();
            foreach (string s in parameters.AllKeys)
            {
                dt.Columns.Add(new DataColumn(s));
            }
            DataRow dr = dt.NewRow();
            dr["OperTime"] = DateTime.Now;
            dr["msg"] = Error;
            foreach (string s in parameters.AllKeys)
            {
                dr[s] = parameters[s];
            }
            dt.Columns.Add(new DataColumn("configxml"));
            dr["configxml"] = configXml;
            dt.Rows.Add(dr);
            dt.WriteXml(HttpContext.Current.Request.MapPath("/alipynotify.xml"));
        }
    }
}