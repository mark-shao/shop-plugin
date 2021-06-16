using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Hishop.Plugins.Payment.WXQRCode
{
    public class WXQRCodeNotify : PaymentNotify
    {
        private string appid = "", bank_type = "", cash_fee = "", fee_type = "", is_subscribe = "", mch_id = "", nonce_str = "", openid = "", out_trade_no = "", result_code = "", return_code = "", sign = "", time_end = "", total_fee = "", trade_type = "", transaction_id = "";
        private string sub_mch_id = "", sub_appid = "", sub_is_subscribe = "", sub_openid = "", coupon_count = "", coupon_fee = "", coupon_fee_0 = "", coupon_id_0 = "";
        private readonly NameValueCollection parameters;
        private Dictionary<string, string> nofifyData = new Dictionary<string, string>();
        public WXQRCodeNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
            LoadNotifydata(parameters["notify_data"]);
        }

        public void LoadNotifydata(string notifydata)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            xmlDoc.LoadXml(notifydata);
            XmlNode resultnode = xmlDoc.SelectSingleNode("xml/out_trade_no");
            if (resultnode != null)
            {
                out_trade_no = resultnode.InnerText;
                nofifyData.Add("out_trade_no", out_trade_no);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/total_fee");
            if (resultnode != null)
            {
                total_fee = resultnode.InnerText;
                nofifyData.Add("total_fee", total_fee);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/appid");
            if (resultnode != null)
            {
                appid = resultnode.InnerText;
                nofifyData.Add("appid", appid);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/bank_type");
            if (resultnode != null)
            {
                bank_type = resultnode.InnerText;
                nofifyData.Add("bank_type", bank_type);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/cash_fee");
            if (resultnode != null)
            {
                cash_fee = resultnode.InnerText;
                nofifyData.Add("cash_fee", cash_fee);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/fee_type");
            if (resultnode != null)
            {
                fee_type = resultnode.InnerText;
                nofifyData.Add("fee_type", fee_type);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/is_subscribe");
            if (resultnode != null)
            {
                is_subscribe = resultnode.InnerText;
                nofifyData.Add("is_subscribe", is_subscribe);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/mch_id");
            if (resultnode != null)
            {
                mch_id = resultnode.InnerText;
                nofifyData.Add("mch_id", mch_id);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/nonce_str");
            if (resultnode != null)
            {
                nonce_str = resultnode.InnerText;
                nofifyData.Add("nonce_str", nonce_str);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/openid");
            if (resultnode != null)
            {
                openid = resultnode.InnerText;
                nofifyData.Add("openid", openid);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/result_code");
            if (resultnode != null)
            {
                result_code = resultnode.InnerText;
                nofifyData.Add("result_code", result_code);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/return_code");
            if (resultnode != null)
            {
                return_code = resultnode.InnerText;
                nofifyData.Add("return_code", return_code);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/sign");
            if (resultnode != null)
            {
                sign = resultnode.InnerText;
                // PayLog.AppendLog(null, resultnode.InnerText + resultnode.InnerText.Length, "1", xmlDoc.InnerXml, LogType.WXQRCode);
            }
            else
            {
                // PayLog.AppendLog(null, "签名数据获取错误", "2", xmlDoc.InnerXml, LogType.WXQRCode);
            }
            resultnode = xmlDoc.SelectSingleNode("xml/time_end");
            if (resultnode != null)
            {
                time_end = resultnode.InnerText;
                nofifyData.Add("time_end", time_end);
            }

            resultnode = xmlDoc.SelectSingleNode("xml/trade_type");
            if (resultnode != null)
            {
                trade_type = resultnode.InnerText;
                nofifyData.Add("trade_type", trade_type);
            }
            resultnode = xmlDoc.SelectSingleNode("xml/sub_mch_id");
            if (resultnode != null)
            {
                sub_mch_id = resultnode.InnerText;
                nofifyData.Add("sub_mch_id", sub_mch_id);
            }
            resultnode = xmlDoc.SelectSingleNode("xml/sub_appid");
            if (resultnode != null)
            {
                sub_appid = resultnode.InnerText;
                nofifyData.Add("sub_appid", sub_appid);
            }
            resultnode = xmlDoc.SelectSingleNode("xml/sub_is_subscribe");
            if (resultnode != null)
            {
                sub_is_subscribe = resultnode.InnerText;
                nofifyData.Add("sub_is_subscribe", sub_is_subscribe);
            }
            resultnode = xmlDoc.SelectSingleNode("xml/sub_openid");
            if (resultnode != null)
            {
                sub_openid = resultnode.InnerText;
                nofifyData.Add("sub_openid", sub_openid);
            }
            resultnode = xmlDoc.SelectSingleNode("xml/transaction_id");
            if (resultnode != null)
            {
                transaction_id = resultnode.InnerText;
                nofifyData.Add("transaction_id", transaction_id);
            }
            resultnode = xmlDoc.SelectSingleNode("xml/coupon_count");
            if (resultnode != null)
            {
                coupon_count = resultnode.InnerText;
                nofifyData.Add("coupon_count", coupon_count);
            }
            resultnode = xmlDoc.SelectSingleNode("xml/coupon_fee");
            if (resultnode != null)
            {
                coupon_fee = resultnode.InnerText;
                nofifyData.Add("coupon_fee", coupon_fee);
            }
            resultnode = xmlDoc.SelectSingleNode("xml/coupon_fee_0");
            if (resultnode != null)
            {
                coupon_fee_0 = resultnode.InnerText;
                nofifyData.Add("coupon_fee_0", coupon_fee_0);
            }
            resultnode = xmlDoc.SelectSingleNode("xml/coupon_id_0");
            if (resultnode != null)
            {
                coupon_id_0 = resultnode.InnerText;
                nofifyData.Add("coupon_id_0", coupon_id_0);
            }
            //  PayLog.AppendLog(null, sign, "", xmlDoc.InnerXml, LogType.WXQRCode);
        }

        public string CoverDictionaryToString(Dictionary<string, string> data)
        {
            //如果不加stringComparer.Ordinal，排序方式和java treemap有差异
            SortedDictionary<string, string> treeMap = new SortedDictionary<string, string>(StringComparer.Ordinal);

            foreach (KeyValuePair<string, string> kvp in data)
            {
                if (string.IsNullOrEmpty(kvp.Value)) continue;
                treeMap.Add(kvp.Key, kvp.Value);
            }

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> element in treeMap)
            {
                builder.Append(element.Key + "=" + element.Value + "&");
            }

            return builder.ToString().Substring(0, builder.Length - 1);
        }

        public override string GetGatewayOrderId()
        {
            return transaction_id;
        }

        public override decimal GetOrderAmount()
        {
            return decimal.Parse(total_fee) / 100;
        }

        public override string GetOrderId()
        {
            return out_trade_no;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            // PayLog.AppendLog(null, null, null, "configXml:" + configXml, LogType.WXQRCode);
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);
            var key = doc.FirstChild.SelectSingleNode("AppSecret").InnerText;
            //PayLog.AppendLog(null, sign, null, "configXml:" + configXml + "key：" + key + "sign:" + CoverDictionaryToString(nofifyData) + "&key=" + key, LogType.WXQRCode);
            var mysign = GetMD5String(CoverDictionaryToString(nofifyData) + "&key=" + key).ToUpper();
            // PayLog.AppendLog(null, null, null, "mysign:" + mysign, LogType.WXQRCode);
            if (mysign.Equals(sign))
            {
                //PayLog.AppendLog(null, null, "3", "sign:" + sign + "-" + sign.Length, LogType.WXQRCode);
                OnFinished(false);
            }
            else
            {
                PayLog.AppendLog_Collection(parameters, configXml, "4", "sign:" + sign + "-" + sign.Length, LogType.WXQRCode);
                OnNotifyVerifyFaild();
            }
        }

        private string GetMD5String(string encypStr)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            //inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
            inputBye = Encoding.UTF8.GetBytes(encypStr);

            outputBye = m5.ComputeHash(inputBye);

            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }

        public override void WriteBack(System.Web.HttpContext context, bool success)
        {
            if (context == null)
                return;

            context.Response.Clear();
            context.Response.ContentType = "text/xml";
            context.Response.Write("<xml>");
            context.Response.Write(string.Format("<return_code><![CDATA[{0}]]></return_code>", (success ? "SUCCESS" : "FAIL")));
            context.Response.Write(string.Format("<return_msg><![CDATA[OK]]></return_msg>"));
            context.Response.Write("<xml>");
            context.Response.End();
        }
    }
}
