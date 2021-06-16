using System.Collections.Specialized;
using System.Web;
using System.Globalization;
using System.Xml;
using Hishop.Plugins;
using System.Collections.Generic;
using System.Data;
using System;
using System.Text;
using Aop.Api.Util;

namespace Hishop.Plugins.Payment.AlipayWX
{
    /// <summary>
    /// 服务器异步通知页面
    /// </summary>
    public class AlipayWXNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        private string trade_no = "", out_trade_no = "", trade_status = "";
        decimal total_fee = 0;
        bool IsNotify = true;
        public AlipayWXNotify(NameValueCollection parameters)
        {
            if (parameters["IsReturn"] != null && parameters["IsReturn"].ToLower() == "true")
            {
                //   PayLog.writeLog_Collection(parameters, "", "", "进入回调页面", LogType.WS_WapPay);
                parameters.Remove("IsReturn");
                IsNotify = false;
                //   IsNotify = false;
                WriteError("进入返回页面", parameters);
            }
            else
            {
                WriteError("进入回调页面", parameters);
            }
            this.parameters = parameters;
            LoadNotifydata();
        }


        public void LoadNotifydata()
        {
            try
            {
                if (parameters["trade_no"] != null)
                    trade_no = parameters["trade_no"];
                else
                    trade_no = "";
                if (parameters["out_trade_no"] != null)
                    out_trade_no = parameters["out_trade_no"];
                else
                    out_trade_no = "";
                if (parameters["trade_status"] != null)
                    trade_status = parameters["trade_status"];
                else
                    trade_status = "";

                if (parameters["total_amount"] != null)
                    decimal.TryParse(parameters["total_amount"], out total_fee);
            }
            catch (Exception ex)
            {
                PayLog.writeLog_Collection(parameters, "", "", "加载notifydata错误：" + ex.Message, LogType.WS_WapPay);
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
                dt.WriteXml(HttpContext.Current.Request.MapPath("/log/alipaynotify_.xml"));
            else
                dt.WriteXml(HttpContext.Current.Request.MapPath("/log/alipayreturn_.xml"));
        }

        /// <summary>
        /// 通知验证
        /// </summary>
        /// <returns>成功，返回true，失败，返回false</returns>
        public override void VerifyNotify(int timeout, string configXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);
            if (doc != null)
            {
                if (doc.FirstChild.SelectSingleNode("Key") != null)
                    config.private_key = doc.FirstChild.SelectSingleNode("Key").InnerText;
                if (doc.FirstChild.SelectSingleNode("PublicKey") != null)
                    config.alipay_public_key = doc.FirstChild.SelectSingleNode("PublicKey").InnerText;
                if (doc.FirstChild.SelectSingleNode("Partner") != null)
                    config.app_id = doc.FirstChild.SelectSingleNode("Partner").InnerText;
            }
            Dictionary<string, string> sArray = GetRequestPost(parameters);
            if (sArray.Count != 0)
            {
                bool flag = AlipaySignature.RSACheckV1(sArray, config.alipay_public_key, config.charset, config.sign_type, false);
                if (flag)
                {
                    //注意：
                    //退款日期超过可退款期限后（如三个月可退款），支付宝系统发送该交易状态通知
                    OnFinished(false);

                }
                else
                {
                    PayLog.writeLog(sArray, config.alipay_public_key, config.charset, config.sign_type, LogType.WS_WapPay_Notify);
                    OnNotifyVerifyFaild();
                }
            }
        }



        public Dictionary<string, string> GetRequestPost(NameValueCollection coll)
        {
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            //coll = Request.Form;
            String[] requestItem = coll.AllKeys;
            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], coll[requestItem[i]]);
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
