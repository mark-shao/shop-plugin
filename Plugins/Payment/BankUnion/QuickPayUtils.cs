using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.IO;

namespace Hishop.Plugins.Payment.BankUnion
{
    public class QuickPayUtils
    {
        /// <summary>
        /// 生成发送银联报文页面
        /// </summary>
        /// <param name="valueVo"></param>
        /// <returns></returns>
        public string createPayHtml(String[] valueVo)
        {
            return createPayHtml(valueVo, null);
        }

        /// <summary>
        /// 直接跳转银行支付页面
        /// </summary>
        /// <param name="valueVo"></param>
        /// <param name="bank"></param>
        /// <returns></returns>
        public string createPayHtml(String[] valueVo, String bank)
        {

            SortedDictionary<string, string> map = new SortedDictionary<string, string>(StringComparer.Ordinal);
            for (int i = 0; i < QuickPayConf.reqVo.Length; i++)
            {
                map.Add(QuickPayConf.reqVo[i], valueVo[i]);
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<script language=\"javascript\">window.onload=function(){document.pay_form.submit();}</script>");
            sb.Append("<form id=\"pay_form\" name=\"pay_form\" action=\"").Append(QuickPayConf.gateWay).Append("\" method=\"post\">");
            foreach (KeyValuePair<string, string> enrty in map)
            {
                //if (enrty.Key.Equals("merAbbr"))
                //{
                //    sb.Append("<input type=\"hidden\" name=\"" + enrty.Key + "\" id=\"" + enrty.Key + "\" value=\"" + HttpUtility.UrlEncode(enrty.Value, Encoding.GetEncoding(QuickPayConf.charset)) + "\" />");
                //}
                //else
                //{
                    sb.Append("<input type=\"hidden\" name=\"" + enrty.Key + "\" id=\"" + enrty.Key + "\" value=\"" + enrty.Value + "\" />");
                //}
            }
            sb.Append("<input type=\"hidden\" name=\"signature\" id=\"signature\" value=\"" + signMap(map, QuickPayConf.signType) + "\">");
            sb.Append("<input type=\"hidden\" name=\"signMethod\" id=\"signMethod\" value=\"" + QuickPayConf.signType + "\" />");
            if (bank != null && bank != "")
            {
                sb.Append("<input type=\"hidden\" name=\"t\" id=\"t\" value=\"5\" />");
                sb.Append("<input type=\"hidden\" name=\"bank\" id=\"bank\" value=\"" + bank + "\" />");
            }
            sb.Append("</form>");
            return sb.ToString();
        }

        public string createBackStr(String[] valueVo, String[] keyVo)
        {
            SortedDictionary<string, string> map = new SortedDictionary<string, string>(StringComparer.Ordinal);
            for (int i = 0; i < keyVo.Length; i++)
            {
                map.Add(keyVo[i], valueVo[i]);
            }
            SortedDictionary<string, string> map2 = new SortedDictionary<string, string>(StringComparer.Ordinal);
            for (int i = 0; i < keyVo.Length; i++)
            {
                map2.Add(keyVo[i], HttpUtility.UrlEncode(valueVo[i]));
            }
            map2.Add("signature", signMap(map, QuickPayConf.signType));
            map2.Add("signMethod", QuickPayConf.signType);
            return joinMapValue(map2, '&');
        }

        /// <summary>
        /// 查询验证签名
        /// </summary>
        /// <param name="valueVo"></param>
        /// <returns>0:验证失败 1验证成功 2没有签名信息（报文格式不对）</returns>
        public int checkSecurity(string[] valueVo)
        {
            SortedDictionary<string, string> map = new SortedDictionary<string, string>(StringComparer.Ordinal);
            for (int i = 0; i < valueVo.Length; i++)
            {
                string[] keyValue = valueVo[i].Split('=');
                map.Add(keyValue[0], keyValue.Length >= 2 ? valueVo[i].Substring(keyValue[0].Length + 1) : "");
            }
            if (map["signature"] == "") return 2;
            if (String.Compare(QuickPayConf.signType, map["signMethod"], true) == 0)
            {
                string signature = map["signature"];
                map.Remove("signature");
                map.Remove("signMethod");
                if (String.Compare(signature, (md5(joinMapValue(map, '&') + md5(QuickPayConf.securityKey)))) == 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 生成加密钥
        /// </summary>
        /// <param name="map"></param>
        /// <param name="signMethod"></param>
        /// <returns></returns>
        public string signMap(SortedDictionary<string, string> map, string signMethod)
        {
            if (String.Compare(QuickPayConf.signType, signMethod, true) == 0)
            {
                return md5(joinMapValue(map, '&') + md5(QuickPayConf.securityKey));
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="valueVo"></param>
        /// <param name="signMethod"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public bool checkSign(string[] valueVo, string signMethod, string signature)
        {
            SortedDictionary<string, string> map = new SortedDictionary<string, string>(StringComparer.Ordinal);
            for (int i = 0; i < QuickPayConf.notifyVo.Length; i++)
            {
                map.Add(QuickPayConf.notifyVo[i], valueVo[i]);
            }
            if (signature == null) return false;
            if (String.Compare(QuickPayConf.signType, signMethod, true) == 0)
            {
                return String.Compare(signature, md5(joinMapValue(map, '&') + md5(QuickPayConf.securityKey))) == 0;
            }
            else
            {
                return false;
            }
        }

        private string joinMapValue(SortedDictionary<string, string> map, char connector)
        {
            StringBuilder b = new StringBuilder();
            foreach (KeyValuePair<string, string> entry in map)
            {
                b.Append(entry.Key);
                b.Append('=');
                if (entry.Value != null)
                {
                    b.Append(entry.Value);
                }
                b.Append(connector);
            }
            return b.ToString();
        }

        /// <summary>
        /// get the md5 hash of a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string md5(string str)
        {
            if (str == null)
            {
                return null;
            }
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            System.Text.Encoding e = System.Text.Encoding.GetEncoding(QuickPayConf.charset);
            byte[] fromData = e.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;
            for (int i = 0; i < targetData.Length; i++)
            {
                string strHex = targetData[i].ToString("X2").ToLower();
                byte2String += strHex;
            }
            return byte2String;
        }

        public string[] getResArr(string str)
        {
            string ReservedKey = "cupReserved=";
            int ReservedStartIndex = str.IndexOf(ReservedKey);
            int LeftBigBraketIndex = str.IndexOf('{', ReservedStartIndex + ReservedKey.Length);
            int RightBigBraketIndex = str.IndexOf('}', LeftBigBraketIndex);
            string ReservedValue = str.Substring(LeftBigBraketIndex, RightBigBraketIndex - LeftBigBraketIndex + 1);
            str = str.Replace(ReservedValue, "");
            string[] resArr = str.Split('&');
            for (int i = 0; i < resArr.Length; i++)
            {
                if (String.Compare(resArr[i], "cupReserved=") == 0)
                {
                    resArr[i] += ReservedValue;
                }
            }
            return resArr;
        }

        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="strURL"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        public string doPostQueryCmd(string strURL, string req)
        {
            HttpWebRequest send = (HttpWebRequest)WebRequest.Create(strURL);
            System.Text.Encoding e = System.Text.Encoding.GetEncoding(QuickPayConf.charset);
            byte[] requestBytes = e.GetBytes(req);
            send.Method = "POST";
            send.ContentType = "application/x-www-form-urlencoded";
            send.ContentLength = requestBytes.Length;
            Stream requestStream = send.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Flush();
            requestStream.Close();

            HttpWebResponse receive = (HttpWebResponse)send.GetResponse();
            StreamReader sr = new StreamReader(receive.GetResponseStream(), e);
            string backstr = sr.ReadToEnd();
            sr.Close();
            receive.Close();
            return backstr;
        }
    }
}
