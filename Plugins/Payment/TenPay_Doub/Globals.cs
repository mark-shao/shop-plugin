using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Net.Security;
using System.Web;
namespace Hishop.Plugins.Payment.TenPay_Doub
{
    internal static class Globals
    {
        /// <summary>
        /// 获取大写的MD5签名结果
        /// </summary>
        /// <param name="encypStr"></param>
        /// <returns></returns>
        internal static string GetMD5(string encypStr)
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


        /** 对字符串进行URL编码 */
        public static string UrlEncode(string instr, string charset)
        {
            //return instr;
            if (instr == null || instr.Trim() == "")
                return "";
            else
            {
                string res;

                try
                {
                    res = HttpUtility.UrlEncode(instr, Encoding.UTF8);

                }
                catch (Exception ex)
                {
                    res = HttpUtility.UrlEncode(instr, Encoding.UTF8);
                }


                return res;
            }
        }


        public static  bool DoPost(string url,string method,out string result)
        {
            StreamReader sr = null;
            HttpWebResponse wr = null;

            HttpWebRequest hp = null;
            try
            {
                string postData = null;
                if (method.ToUpper() == "POST")
                {
                    string[] sArray = System.Text.RegularExpressions.Regex.Split(url, "\\?");

                    hp = (HttpWebRequest)WebRequest.Create(sArray[0]);

                    if (sArray.Length >= 2)
                    {
                        postData = sArray[1];
                    }
                }
                else
                {
                    hp = (HttpWebRequest)WebRequest.Create(url);
                }

                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
             //   ServicePointManager.Expect100Continue = false;
                //if (this.certFile != "")
                //{
                //    hp.ClientCertificates.Add(new X509Certificate2(this.certFile, this.certPasswd));
                //}
                hp.Timeout = 120 * 1000;

                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("gb2312");
                if (postData != null)
                {
                    byte[] data = encoding.GetBytes(postData);

                    hp.Method = "POST";

                    hp.ContentType = "application/x-www-form-urlencoded";

                    hp.ContentLength = data.Length;

                    Stream ws = hp.GetRequestStream();

                    // 发送数据

                    ws.Write(data, 0, data.Length);
                    ws.Close();
                }
                wr = (HttpWebResponse)hp.GetResponse();
                sr = new StreamReader(wr.GetResponseStream(), encoding);
               result = sr.ReadToEnd();
                sr.Close();
                wr.Close();
            }
            catch (Exception exp)
            {
                //this.errInfo += exp.Message;
                //if (wr != null)
                //{
                //    this.responseCode = Convert.ToInt32(wr.StatusCode);
                //}
                result = "erro";
                return false;
            }

            //this.responseCode = Convert.ToInt32(wr.StatusCode);

            return true;
        }


        //验证服务器证书
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        /// <summary>
        /// 添加参数,惹参数值不为空串,则添加。反之,不添加。
        /// </summary>
        internal static StringBuilder AddParameter(StringBuilder buf, String parameterName, String parameterValue)
        {
            if (null == parameterValue || "".Equals(parameterValue))
            {
                return buf;
            }

            if ("".Equals(buf.ToString()))
            {
                buf.Append(parameterName);
                buf.Append("=");
                buf.Append(parameterValue);
            }
            else
            {
                buf.Append("&");
                buf.Append(parameterName);
                buf.Append("=");
                buf.Append(parameterValue);
            }
            return buf;
        }
    }
}