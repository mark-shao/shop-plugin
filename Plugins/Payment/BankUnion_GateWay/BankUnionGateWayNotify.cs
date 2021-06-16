using Hishop.Plugins.Payment.BankUnionGateWay.sdk;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;

namespace Hishop.Plugins.Payment.BankUnionGateWay
{
    public class BankUnionGateWayNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        public BankUnionGateWayNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
            parameters.Remove("HIGW");
        }

        private void AddDictionary(Dictionary<string, string> resData, string paramname)
        {
            string value = parameters[paramname];
            if (!string.IsNullOrEmpty(value))
                resData.Add(paramname, value);
        }

        public override void VerifyNotify(int timeout, string configXml)
        {            
            // 使用Dictionary保存参数
            Dictionary<string, string> resData = new Dictionary<string, string>();
            string[] requestItem = parameters.AllKeys;
            for (int i = 0; i < requestItem.Length; i++)
            {
                resData.Add(requestItem[i], parameters[requestItem[i]]);
            }
               
            // 返回报文中不包含UPOG,表示Server端正确接收交易请求,则需要验证Server端返回报文的签名
            if (SDKUtil.Validate(resData, Encoding.UTF8))
            {
                OnFinished(false);
            }
            else
            {
                OnNotifyVerifyFaild();
                return;
            }
        }

        /// <summary>
        /// 将Dictionary内容排序后输出为键值对字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CoverDictionaryToString(Dictionary<string, string> data)
        {
            //如果不加stringComparer.Ordinal，排序方式和java treemap有差异
            SortedDictionary<string, string> treeMap = new SortedDictionary<string, string>(StringComparer.Ordinal);

            foreach (KeyValuePair<string, string> kvp in data)
            {
                treeMap.Add(kvp.Key, kvp.Value);
            }

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> element in treeMap)
            {
                builder.Append(element.Key + "=" + element.Value + "&");
            }

            return builder.ToString().Substring(0, builder.Length - 1);
        }

        public override void WriteBack(HttpContext context, bool success)
        {
            if (context == null)
                return;

            context.Response.Clear();
            context.Response.Write(success ? "ok" : "error");
            context.Response.End();
        }

        public override decimal GetOrderAmount()
        {
            return decimal.Parse(parameters["txnAmt"]) / 100;
        }

        public override string GetOrderId()
        {
            return parameters["orderId"];
        }

        public override string GetGatewayOrderId()
        {
            return string.Empty;
        }
    }
}
