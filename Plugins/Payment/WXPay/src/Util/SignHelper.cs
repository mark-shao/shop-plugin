using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.Weixin.Pay.Util
{
    /// <summary>
    /// 签名助手
    /// </summary>
    internal class SignHelper
    {
        #region public static string BuildQuery(IDictionary<string, string> dict, bool encode)

        /// <summary>
        /// 创建键值对格式的字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string BuildQuery(IDictionary<string, string> dict, bool encode)
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>(dict);

            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = param.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(value) && name.ToLower() != "sign" && name.ToLower() != "payinfo")
                {
                    if (name.ToLower() == "settlement_total_fee" && value == "0")
                        continue;//settlement_total_fee金额为0，则需忽略

                    if (hasParam)
                        postData.Append("&");

                    postData.Append(name);
                    postData.Append("=");
                    if (encode)
                        postData.Append(System.Web.HttpUtility.UrlEncode(value, Encoding.UTF8));
                    else
                        postData.Append(value);

                    hasParam = true;
                }
            }

            return postData.ToString();
        }
        #endregion
        #region 将idcct转换为xml
        public static string BuildXml(IDictionary<string, string> dict, bool encode)
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>(dict);

            StringBuilder postData = new StringBuilder();
            postData.AppendLine("<xml>");
            IEnumerator<KeyValuePair<string, string>> dem = param.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(value))
                {
                    decimal dValue = 0;
                    Boolean IsText = false;
                    if (!decimal.TryParse(value, out dValue))
                        IsText = true;

                    if (encode)
                        postData.AppendLine("<" + name + ">" + (IsText ? "<![CDATA[" : "") + System.Web.HttpUtility.UrlEncode(value, Encoding.UTF8) + (IsText ? "]]>" : "") + "</" + name + ">");
                    else
                        postData.AppendLine("<" + name + ">" + (IsText ? "<![CDATA[" : "") + value + (IsText ? "]]>" : "") + "</" + name + ">");
                }
            }
            postData.AppendLine("</xml>");
            return postData.ToString();
        }
        #endregion
        #region public static String SignPackage(IDictionary<string, string> parameters, string partnerKey)

        /// <summary>
        /// 签名Package
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="partnerKey"></param>
        /// <returns></returns>
        public static String SignPackage(IDictionary<string, string> parameters, string partnerKey)
        {
            //第一步: 参数按字典排序后生成URL键值对格式
            string signStr = BuildQuery(parameters, false);

            //第二步: 拼接PartnerKey
            signStr += String.Format("&key={0}", partnerKey);

            //第三步: MD5运算并转换成大写
            return System.Web.Security.FormsAuthentication
                .HashPasswordForStoringInConfigFile(signStr, "MD5")
                .ToUpper();
        }
        #endregion

        #region public static string SignPay(IDictionary<string, string> parameters)

        /// <summary>
        /// 签名支付
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string SignPay(IDictionary<string, string> parameters, string key = "")
        {
            string signStr = BuildQuery(parameters, false);
            //第二步: 拼接PartnerKey
            signStr += String.Format("&key={0}", key);
            // Hishop.Weixin.Pay.Domain.WxPayLog.writeLog(parameters, System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(signStr, "MD5").ToUpper(), "", signStr, Hishop.Weixin.Pay.Domain.LogType.Pay);
            return System.Web.Security.FormsAuthentication
                .HashPasswordForStoringInConfigFile(signStr, "MD5")
                .ToUpper();
        }
        #endregion
    }
}
