using System.Text;
using System.Web;
using System.Security.Cryptography;
using System;
using System.Data;
using System.Collections.Generic;
namespace Hishop.Plugins.Payment.AlipayQrCode
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
        /// <summary>
        /// 排序字典
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static SortedDictionary<string, string> SortParam(IDictionary<string, string> param)
        {
            SortedDictionary<string, string> dict = new SortedDictionary<string, string>(param);
            return dict;
        }
        /// <summary>
        /// 创建提交字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
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

        internal static string CreatDirectUrl(
            string gateway,
            string service,//接口名称 
            string partner,//合作者身份ID
            string _input_charset,//字符编码
            string sign_type,//签名类型   md5
            string method,//动作 add 增加二维码  modify 修改二维码  stop 暂停二维码  restart重启二维码
            string timestamp,//时间戳
            string qrcode,//二维码，当method=add时可空否则不可空
            string biz_type,//业务类型 10 商品码，商户码，11 链接码 12 链接码（预授权业务） 当 method=add/modify时不可空
            string biz_data,//业务数据Json格式
            string key
            )
        {
            /// <summary>
            /// created by sunzhizhi 2006.5.21,sunzhizhi@msn.com。
            /// </summary>
            int i;
            Dictionary<string, string> pay_param = new Dictionary<string, string>();
            pay_param.Add("service", service);
            pay_param.Add("partner", partner);
            pay_param.Add("_input_charset", _input_charset);
            pay_param.Add("timestamp", timestamp);
            pay_param.Add("method", method);
            pay_param.Add("qrcode", qrcode);
            pay_param.Add("biz_type", biz_type);
            pay_param.Add("biz_data", biz_data);

            ////构造数组；
            //string[] Oristr ={ 
            //    "service="+service, 
            //    "partner=" + partner, 
            //    "_input_charset="+_input_charset,
            //    "timestamp="+timestamp,
            //    "method="+method,
            //    "qrcode="+qrcode,
            //    "biz_type="+biz_type,
            //    "biz_data="+biz_data

            //    };
            //char[] delimiterChars = { '=' };
            ////进行排序；
            //string[] Sortedstr = BubbleSort(Oristr);

            ////构造待md5摘要字符串 ；
            //StringBuilder prestr = new StringBuilder();

            //for (i = 0; i < Sortedstr.Length; i++)
            //{
            //    if (Sortedstr[i].Split(delimiterChars)[1].Trim() != "")
            //        if (i == Sortedstr.Length - 1)
            //        {
            //            prestr.Append(Sortedstr[i]);
            //        }
            //        else
            //        {
            //            prestr.Append(Sortedstr[i] + "&");
            //        }
            //}
            string query = BuildQuery(pay_param, false);
            //prestr.Append(key);

            //生成Md5摘要；

            string sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(query + key, "MD5").ToLower();
            //string[] param = Sortedstr;

            //构造支付Url；

            StringBuilder parameter = new StringBuilder();
            parameter.Append(gateway);
            parameter.Append(BuildQuery(pay_param, true));
            //for (i = 0; i < Sortedstr.Length; i++)
            //{
            //    if (Sortedstr[i].Split(delimiterChars)[1].Trim() != "")
            //        parameter.Append(Sortedstr[i].Split(delimiterChars)[0] + "=" + HttpUtility.UrlEncode(Sortedstr[i].Split(delimiterChars)[1]) + "&");
            //}

            parameter.Append("&sign=" + sign + "&sign_type=" + sign_type);
          //  writeLog(pay_param, sign, parameter.ToString(), BuildQuery(pay_param, false));
            //返回支付Url；
            return parameter.ToString();
        }



        internal static void writeLog(IDictionary<string, string> param, string sign, string url, string msg)
        {
            DataTable dt = new DataTable();
            dt.TableName = "log";
            dt.Columns.Add(new DataColumn("OperTime"));
            foreach (KeyValuePair<string, string> key in param)
            {
                dt.Columns.Add(new DataColumn(key.Key));
            }
            dt.Columns.Add(new DataColumn("Msg"));
            dt.Columns.Add(new DataColumn("Sign"));
            dt.Columns.Add(new DataColumn("Url"));
            DataRow dr = dt.NewRow();
            dr["OperTime"] = DateTime.Now;
            foreach (KeyValuePair<string, string> key in param)
            {
                dr[key.Key] = key.Value;
            }
            dr["Msg"] = msg;
            dr["Sign"] = sign;
            dr["Url"] = url;
            dt.Rows.Add(dr);
            dt.WriteXml(HttpContext.Current.Server.MapPath("/log.xml"));
        }
    }


}