using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

namespace Hishop.Plugins.Payment.GoPay
{
    internal static class Globals
    {

        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string GetMD5(string str)
        {
            byte[] b = Encoding.Default.GetBytes(str);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }

        /// <summary>
        /// 获取国付宝服务器时间 用于时间戳
        /// </summary>
        /// <returns>格式YYYYMMDDHHMMSS</returns>
        public static string GetGopayServerTime()
        {
            return Get_Http("https://mertest.gopay.com.cn/PGServer/time", 10000);
            //如果方法报错，请修改参数为http://mertest.gopay.com.cn/PGServer/time
        }

        /// <summary>
        /// 获取远程服务器返回结果
        /// </summary>
        /// <param name="strUrl">指定URL路径地址</param>
        /// <param name="timeout">超时时间设置</param>
        /// <returns>服务器返回结果</returns>
        public static string Get_Http(string strUrl, int timeout)
        {
            string strResult;
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                myReq.Timeout = timeout;
                HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }

                strResult = strBuilder.ToString();
            }
            catch (Exception exp)
            {
                strResult = "错误：" + exp.Message;
            }

            return strResult;
        }

        public static string getRealIp()
        {
            string UserIP;
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null) //得到穿过代理服务器的ip地址
            {
                UserIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                UserIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            return UserIP;
        }
    }
}
