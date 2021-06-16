using System.Collections.Specialized;
using System.Web;
using System.Globalization;
using System.Xml;
using Hishop.Plugins;
using System.Collections.Generic;
using System.Data;
using System;
using System.Text;

namespace Hishop.Plugins.Payment.AlipayCrossborderMobilePayment
{
    /// <summary>
    /// 通知验证查询
    /// MD5摘要在验证在商户系统中完成
    /// </summary>
    public class AlipayCrossbordermobilepaymentNotify : PaymentNotify
    {

        private readonly NameValueCollection parameters;
        private const string InpuitCharset = "utf-8";
        private string notify_data = "", seller_email = "", trade_no = "", buyer_email = "", notify_type = "", out_trade_no = "", seller_id = "", trade_status = "", buyer_id, notify_id;
        DateTime notify_time;
        private int quantity = 0;
        decimal price = 0, total_fee = 0;
        bool IsNotify = true;
        public AlipayCrossbordermobilepaymentNotify(NameValueCollection parameters)
        {
            if (parameters["IsReturn"] != null && parameters["IsReturn"].ToLower() == "true")
            {
                parameters.Remove("IsReturn");
                IsNotify = false;
            }
            this.parameters = parameters;
            if (IsNotify)
            {
                notify_data = parameters["notify_data"];
                // WriteError("进入通知类", parameters);
                LoadNotifydata(notify_data);
            }
            else
            {
                LoadReturndata();
            }
        }

        public void LoadReturndata()
        {
            trade_no = this.parameters["trade_no"];
            out_trade_no = this.parameters["out_trade_no"];
            trade_status = this.parameters["trade_status"];
        }


        //<notify><payment_type>1</payment_type><subject>订单支付</subject><trade_no>2015011500001000820040771017</trade_no><buyer_email>rxyhj@aliyun.com</buyer_email><gmt_create>2015-01-15 16:58:37</gmt_create><notify_type>trade_status_sync</notify_type>
        //<quantity>1</quantity><out_trade_no>201501150439097</out_trade_no><notify_time>2015-01-15 16:58:53</notify_time><seller_id>2088701375176746</seller_id><trade_status>TRADE_SUCCESS</trade_status><is_total_fee_adjust>N</is_total_fee_adjust><total_fee>0.01</total_fee><gmt_payment>2015-01-15 16:58:52</gmt_payment><seller_email>hishop@live.cn</seller_email><price>0.01</price><buyer_id>2088002126230824</buyer_id><notify_id>caf16cffb8b61a0138d9df936d0b68e16k</notify_id><use_coupon>N</use_coupon></notify>
        public void LoadNotifydata(string notifydata)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.XmlResolver = null;
                xmlDoc.LoadXml(notifydata);
                XmlNode resultnode = xmlDoc.SelectSingleNode("notify/trade_no");
                if (resultnode != null)
                    trade_no = resultnode.InnerText;
                resultnode = xmlDoc.SelectSingleNode("notify/out_trade_no");
                if (resultnode != null)
                    out_trade_no = resultnode.InnerText;
                resultnode = xmlDoc.SelectSingleNode("notify/seller_email");
                if (resultnode != null)
                    seller_email = resultnode.InnerText;
                resultnode = xmlDoc.SelectSingleNode("notify/quantity");
                if (resultnode != null)
                    int.TryParse(resultnode.InnerText, out quantity);
                resultnode = xmlDoc.SelectSingleNode("notify/price");
                if (resultnode != null)
                    decimal.TryParse(resultnode.InnerText, out price);
                resultnode = xmlDoc.SelectSingleNode("notify/total_fee");
                if (resultnode != null)
                    decimal.TryParse(resultnode.InnerText, out total_fee);
                resultnode = xmlDoc.SelectSingleNode("notify/trade_status");
                if (resultnode != null)
                    trade_status = resultnode.InnerText;
                resultnode = xmlDoc.SelectSingleNode("notify/buyer_email");
                if (resultnode != null)
                    buyer_email = resultnode.InnerText;
                resultnode = xmlDoc.SelectSingleNode("notify/seller_email");
                if (resultnode != null)
                    seller_email = resultnode.InnerText;
                resultnode = xmlDoc.SelectSingleNode("notify/buyer_id");
                if (resultnode != null)
                    buyer_id = resultnode.InnerText;
            }
            catch (Exception ex)
            {
                WriteError(ex.Message + "---" + notifydata, this.parameters);
            }

        }
        public void WriteError(string Error, NameValueCollection parameters)
        {
            DataTable dt = new DataTable();
            dt.TableName = "alipay";
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
            if (IsNotify)
                dt.WriteXml(HttpContext.Current.Request.MapPath("/alipaynotify_.xml"));
            else
                dt.WriteXml(HttpContext.Current.Request.MapPath("/alipayreturn_.xml"));
        }
        /// <summary>
        ///  验证消息是否是支付宝发出的合法消息，验证callback
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <param name="sign">支付宝生成的签名结果</param>
        /// <returns>验证结果</returns>
        public void VerifyReturn(int timeout, string configXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);
            Dictionary<string, string> pay_param = new Dictionary<string, string>();
            foreach (string s in parameters.AllKeys)
            {
                if (s != "sign" && s != "sign_type")
                {
                    pay_param.Add(s, parameters[s]);
                }
            }
            //获取返回时的签名验证结果
            string prestr = BuildQuery(pay_param, false) + doc.FirstChild.SelectSingleNode("Key").InnerText;
            string strSign = Globals.GetMD5(prestr, InpuitCharset);

            //写日志记录（若要调试，请取消下面两行注释）
            //string sWord = "isSign=" + isSign.ToString() + "\n 返回回来的参数：" + GetPreSignStr(inputPara) + "\n ";
            //Core.LogResult(sWord);
            string sign = parameters["sign"] == null ? "" : parameters["sign"].ToLower();
            //判断isSign是否为true
            //isSign不是true，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关
            if (strSign != sign)//验证成功
            {
                WriteError("验签失败" + sign + "----" + strSign + "---" + doc.FirstChild.SelectSingleNode("Key").InnerText + "---" + prestr, this.parameters);
                OnNotifyVerifyFaild();
                return;
            }

            if (trade_status == "TRADE_SUCCESS" || trade_status == "TRADE_FINISHED" || trade_status == "SUCCESS")
            {
                OnFinished(false);
            }
            else
            {

                WriteError("获取结果失败", this.parameters);
                OnNotifyVerifyFaild();
            }
            //比较result值是否为success
            //if (trade_status.ToLower() != "success")
            //{
            //    WriteError("获取结果失败", this.parameters);
            //    OnNotifyVerifyFaild();
            //}
            //else
            //{
            //    OnFinished(false);
            //}
        }
        /// <summary>
        /// 通知验证
        /// </summary>
        /// <returns>成功，返回true，失败，返回false</returns>
        public override void VerifyNotify(int timeout, string configXml)
        {
            if (!IsNotify)
            {
                VerifyReturn(timeout, configXml);
                return;
            }
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);

            //获取所有get请求的参数
            //SortedDictionary<string, string> sArrary = GetRequestGet();
            parameters.Remove("HIGW");
            //生成本地签名sign
            string[] requestarr = parameters.AllKeys;
            Dictionary<string, string> pay_param = new Dictionary<string, string>();
            foreach (string s in parameters.AllKeys)
            {
                if (s != "sign" && s != "sign_type")
                {
                    pay_param.Add(s, parameters[s]);
                }
            }
            string prestr = "service=" + parameters["service"] + "&v=" + parameters["v"] + "&sec_id=" + parameters["sec_id"] + "&notify_data=" + parameters["notify_data"] + doc.FirstChild.SelectSingleNode("Key").InnerText;
            //prestr = prestr + doc.FirstChild.SelectSingleNode("Key").InnerText;
            string strSign = Globals.GetMD5(prestr, InpuitCharset);
            //   string keystrsign = Function.Sign(prestr + doc.FirstChild.SelectSingleNode("Key").InnerText, "MD5", InpuitCharset);
            //Function.BuildMysign(sArrary, doc.FirstChild.SelectSingleNode("Key").InnerText, "MD5", InpuitCharset);
            //获取支付宝返回sign
            string sign = parameters["sign"] == null ? "" : parameters["sign"].ToLower();


            //验签对比
            if (!(sign == strSign))
            {
                //验签失败
                WriteError("验签失败" + sign + "----" + strSign + "---" + doc.FirstChild.SelectSingleNode("Key").InnerText + "---" + prestr, this.parameters);
                OnNotifyVerifyFaild();
                return;
            }

            //比较result值是否为success
            if (trade_status.ToLower() != "trade_success")
            {
                //   WriteError("获取结果失败", this.parameters);
                OnNotifyVerifyFaild();
            }
            else
            {
                OnFinished(false);
            }
        }

        public static string BuildQuery(IDictionary<string, string> dict, bool encode)
        {
            //排序字典，默认为升序
            SortedDictionary<string, string> param = new SortedDictionary<string, string>(dict);

            StringBuilder query = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = param.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(value))
                {
                    if (hasParam)
                        query.Append("&");

                    query.Append(name);
                    query.Append("=");
                    if (encode && name.ToLower() != "service" && name.ToLower() != "_input_charset")
                        query.Append(System.Web.HttpUtility.UrlEncode(value, Encoding.UTF8));
                    else
                        query.Append(value);

                    hasParam = true;
                }
            }

            return query.ToString();
        }
        /// <summary>
        /// 获取Get请求的所有参数
        /// </summary>
        /// <returns>请求参数字符串</returns>
        public SortedDictionary<string, string> GetRequestGet()
        {
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            //对url第一个字符？过滤
            string query = HttpContext.Current.Request.Url.Query.Replace("?", "");
            if (!string.IsNullOrEmpty(query))
            {
                //根据&符号分隔成数组
                string[] coll = query.Split('&');
                //定义临时数组
                string[] temp = { };
                //循环各数组
                for (int i = 0; i < coll.Length; i++)
                {
                    //根据=号拆分
                    temp = coll[i].Split('=');
                    //把参数名和值分别添加至SortedDictionary数组
                    sArray.Add(temp[0], temp[1]);
                }
            }
            return sArray;
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
