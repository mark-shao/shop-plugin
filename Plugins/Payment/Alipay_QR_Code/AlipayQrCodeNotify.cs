using System.Collections.Specialized;
using System.Web;
using System.Globalization;
using System.Xml;
using Hishop.Plugins;
using System;
using System.Data;
using System.Collections.Generic;

namespace Hishop.Plugins.Payment.AlipayQrCode
{
    /// <summary>
    /// 通知验证查询
    /// MD5摘要在验证在商户系统中完成
    /// </summary>
    public class AlipayQrCodeNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        private const string InputCharset = "utf-8";
        private string notify_data = "", seller_email = "", subject = "", trade_no = "", buyer_email = "", partner = "", out_trade_no = "", qrcode = "", trade_status = "";
        decimal total_fee = 0;
        DateTime gmt_create, gmt_payment;
        public AlipayQrCodeNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;

            notify_data = parameters["notify_data"];
            if (notify_data != null && notify_data != "")
                LoadNotifydata(notify_data);
        }
        public void LoadNotifydata(string notifydata)
        {
            //try
            //{
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(notifydata);
            XmlNode resultnode = xmlDoc.SelectSingleNode("notify/trade_no");
            if (resultnode != null)
                trade_no = resultnode.InnerText;
            resultnode = xmlDoc.SelectSingleNode("notify/out_trade_no");
            if (resultnode != null)
                out_trade_no = resultnode.InnerText;
            resultnode = xmlDoc.SelectSingleNode("notify/total_fee");
            if (resultnode != null)
                decimal.TryParse(resultnode.InnerText, out total_fee);
            resultnode = xmlDoc.SelectSingleNode("notify/trade_status");
            if (resultnode != null)
                trade_status = resultnode.InnerText;
            resultnode = xmlDoc.SelectSingleNode("notify/buyer_email");
            if (resultnode != null)
                buyer_email = resultnode.InnerText;
            resultnode = xmlDoc.SelectSingleNode("notify/subject");
            if (resultnode != null)
                subject = resultnode.InnerText;
            resultnode = xmlDoc.SelectSingleNode("notify/partner");
            if (resultnode != null)
                partner = resultnode.InnerText;
            resultnode = xmlDoc.SelectSingleNode("notify/qrcode");
            if (resultnode != null)
                qrcode = resultnode.InnerText;
            //}
            //catch (Exception ex)
            //{
            //    WriteError(ex.Message + "---参数获取错误----" + notifydata, this.parameters);
            //}

        }

        public void WriteError(string Error, NameValueCollection parameters)
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
            dt.Rows.Add(dr);
            dt.WriteXml(HttpContext.Current.Request.MapPath("/QRCodealipaynotify.xml"));
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
        public override void VerifyNotify(int timeout, string configXml)
        {
            bool isValid;
            //try
            //{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);

            isValid = (trade_status.ToLower() == "trade_success" || trade_status.ToLower() == "success");
            // 通知验证
            //try
            //{
            //    isValid = bool.Parse(GetResponse(CreateUrl(doc.FirstChild), timeout));
            //}
            //catch
            //{
            //    isValid = false;
            //}

            parameters.Remove("HIGW");
            string[] requestarr = parameters.AllKeys;
            Dictionary<string, string> pay_param = new Dictionary<string, string>();
            foreach (string s in parameters.AllKeys)
            {
                if (s != "sign" && s != "sign_type")
                {
                    pay_param.Add(s, parameters[s]);
                }
            }

            // 构造待md5摘要字符串
            string prestr = Globals.BuildQuery(pay_param, false);
            prestr = prestr + doc.FirstChild.SelectSingleNode("Key").InnerText;
            string sign = parameters["sign"] == null ? "" : parameters["sign"].ToLower();

            isValid = isValid && (sign == System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(prestr, "MD5").ToLower());
            if (isValid)
            {
                //WriteError("签名正确,完成支付" + sign + "----" + prestr + "---" + doc.FirstChild.SelectSingleNode("Key").InnerText, parameters);
                // 支付宝的交易结束就是打款到商家账户上了
                OnFinished(false);
            }
            else
            {
                //WriteError("签名错误" + sign + "----" + prestr + "---" + doc.FirstChild.SelectSingleNode("Key").InnerText, parameters);
                OnNotifyVerifyFaild();

            }
            //}
            //catch (Exception ex) { WriteError("签名信息错误" + ex.Message, parameters); }
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
            return total_fee;
        }

        public override string GetOrderId()
        {
            return out_trade_no;
        }

        public override string GetGatewayOrderId()
        {
            return trade_no;
        }

    }
}