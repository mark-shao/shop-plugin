using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Security.Cryptography;

namespace Hishop.Plugins.Payment.AlipayCrossborderMobilePayment
{
    public class AliPay
    {
        public static string GetMD5(string s)
        {

            /// <summary>
            /// 与ASP兼容的MD5加密算法
            /// </summary>

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(s));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        public static string[] BubbleSort(string[] r)
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

        public string CreatUrl(
            string gateway,
            string service,
            string partner,
            string sign_type,
            string out_trade_no,
            string subject,
            string body,
            string currency,
            string total_fee,
            string key,
            string return_url,
            string notify_url,
            string rmb
            )
        {
            /// <summary>
            /// created by sunzhizhi 2006.5.21,sunzhizhi@msn.com。
            /// </summary>
            int i;
            //https://mapi.alipay.com/gateway.do?body=订单号-201510203829844&currency=USD&notify_url=http://luoxiansheng.xm.kuaidiantong.cn/pay/wap_alipay_forex_notify_url.aspx&out_trade_no=201510203829844&partner=2088611221573217&return_url=http://luoxiansheng.xm.kuaidiantong.cn/pay/wap_alipay_forex_return_url.aspx&rmb_fee=0.10&service=create_forex_trade&subject=订单支付&sign=929d61c9cbde38506d95f796089077f9&sign_type=MD5
            //构造数组；
            string[] Oristr ={
                "service="+service,
                "partner=" + partner,
                "_input_charset=UTF-8",
                "product_code=NEW_WAP_OVERSEAS_SELLER",
                "subject=" + subject,
                "body=" + body,
                "out_trade_no=" + out_trade_no,
                 (rmb == "1" ?"rmb_fee=" + total_fee:"total_fee=" + total_fee),

                "currency=" + currency,
                "notify_url=" + notify_url,
                "return_url=" + return_url
            };

            //进行排序；
            string[] Sortedstr = BubbleSort(Oristr);


            //构造待md5摘要字符串 ；

            StringBuilder prestr = new StringBuilder();

            for (i = 0; i < Sortedstr.Length; i++)
            {
                if (i == Sortedstr.Length - 1)
                {
                    prestr.Append(Sortedstr[i]);

                }
                else
                {

                    prestr.Append(Sortedstr[i] + "&");
                }

            }

            prestr.Append(key);

            //生成Md5摘要；
            string sign = GetMD5(prestr.ToString());

            //构造支付Url；
            StringBuilder parameter = new StringBuilder();
            parameter.Append(gateway);
            for (i = 0; i < Sortedstr.Length; i++)
            {
                parameter.Append(Sortedstr[i] + "&");
            }

            parameter.Append("sign=" + sign + "&sign_type=" + sign_type);


            //返回支付Url；
            return parameter.ToString();

        }
    }
}
