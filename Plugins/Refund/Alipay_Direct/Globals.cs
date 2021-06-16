using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace Hishop.Plugins.Refund.AlipayDirect
{
    internal static class Globals
    {
        internal static string GetMD5(string s, string _input_charset)
        {

            /// <summary>
            /// 与ASP兼容的MD5加密算法
            /// </summary>

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(s));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        internal static string[] BubbleSort(string[] r)
        {
            /// <summary>
            /// 冒泡排序法
            /// </summary>

            int i, j; //交换标志 
            string temp;

            bool exchange;

            for (i = 0; i < r.Length; i++) //最多做R.Length-1趟排序 
            {
                exchange = false; //本趟排序开始前，交换标志应为假

                for (j = r.Length - 2; j >= i; j--)
                {
                    if (System.String.CompareOrdinal(r[j + 1], r[j]) < 0)　//交换条件
                    {
                        temp = r[j + 1];
                        r[j + 1] = r[j];
                        r[j] = temp;

                        exchange = true; //发生了交换，故将交换标志置为真 
                    }
                }

                if (!exchange) //本趟排序未发生交换，提前终止算法 
                {
                    break;
                }

            }
            return r;
        }


        internal static string CreatRefundUrl(
           string gateway,
           string service,
           string partner,
           string sign_type,
           string seller_email,
           string key,
           string return_url,
           string _input_charset,
           string notify_url,
           string refund_date,
           string batch_no,
            string batch_num,
            string detail_data
           )
        {
            /// <summary>
            /// created by sunzhizhi 2006.5.21,sunzhizhi@msn.com。
            /// </summary>
            int i;

            //构造数组；

            SortedDictionary<string, string> refundparam = new SortedDictionary<string, string>();
            refundparam.Add("service", service);
            refundparam.Add("partner", partner);
            refundparam.Add("refund_date", refund_date);
            refundparam.Add("batch_no", batch_no);
            refundparam.Add("batch_num", batch_num);
            refundparam.Add("detail_data", detail_data);
            refundparam.Add("seller_email", seller_email);
            refundparam.Add("notify_url", notify_url);
            refundparam.Add("_input_charset", _input_charset.ToLower());
            string[] Oristr ={ 
                "service="+service, 
                "partner=" + partner, 
                "refund_date=" + refund_date,
                "batch_no=" + batch_no,
                "batch_num=" + batch_num,
                "detail_data=" + detail_data, 
                "seller_email=" + seller_email, 
                "notify_url=" + notify_url,
                "_input_charset="+_input_charset.ToLower(),          
                };

            //进行排序；

            string[] Sortedstr = BubbleSort(Oristr);
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("service", service);
            param.Add("partner", partner);
            param.Add("refund_date", refund_date);
            param.Add("batch_no", batch_no);
            param.Add("batch_num", batch_num);
            param.Add("detail_data", detail_data);
            param.Add("seller_email", seller_email);
            param.Add("notify_url", notify_url);


            //构造待md5摘要字符串 ；
            StringBuilder prestr = new StringBuilder();
            int keyIndex = 0;
            foreach (KeyValuePair<string, string> item in refundparam)
            {
                if (keyIndex == 0)
                {
                    prestr.Append(item.Key + "=" + item.Value);
                }
                else
                {
                    prestr.Append("&" + item.Key + "=" + item.Value);
                }
                keyIndex++;
            }

            prestr.Append(key);
            param.Add("prestr", prestr.ToString());
            //生成Md5摘要；
            string sign = GetMD5(prestr.ToString(), _input_charset);

            refundparam.Add("sign", sign);
            refundparam.Add("sign_type", sign_type.ToUpper());
            StringBuilder sbHtml = new StringBuilder();
            // sbHtml.Append("<iframe name='hideframe' id='hideframe' style='display:none;width:0px;height:0px;' />");
            sbHtml.Append("<form id='refundsubmit' name='refundsubmit' target='_self' action='" + gateway + "_input_charset=" + _input_charset + "' method='get'>");
            foreach (KeyValuePair<string, string> item in refundparam)
            {
                sbHtml.Append("<input type='hidden' name='" + item.Key + "' value='" + item.Value + "'/>");

            }

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='退款提交' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['refundsubmit'].submit();</script>");
            //构造支付Url；
            StringBuilder parameter = new StringBuilder();
            parameter.Append(gateway);
            foreach (KeyValuePair<string, string> item in refundparam)
            {
                if (item.Key != "_input_charset")
                    parameter.Append(item.Key + "=" + HttpUtility.UrlEncode(item.Value) + "&");
            }

            parameter.Append("sign=" + sign + "&sign_type=" + sign_type);

            param.Add("parameter", parameter.ToString());
         //   RefundLog.AppendLog(param, sign, HttpContext.Current.Request.Url.ToString(), "提交签名");
            //返回支付Url；
            // return parameter.ToString();
            return sbHtml.ToString();

        }

    }
}