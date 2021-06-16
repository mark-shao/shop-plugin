using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Hishop.Open.Api
{
    public static class OpenApiSign
    {
          public static Dictionary<string, string> Parameterfilter(SortedDictionary<string, string> dicArrayPre)
        {
            Dictionary<string, string> dicArray = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                if (temp.Key.ToLower() != "sign" && temp.Key.ToLower() != "sign_type"&&temp.Value!="" && temp.Value != null)
                {
                    dicArray.Add(temp.Key.ToLower(), temp.Value);
                }
            }

            return dicArray;
        }

        public static string BuildSign(Dictionary<string, string> dicArray,string appSecret,string sign_type, string _input_charset)
        {
            string prestr = CreateLinkstring(dicArray);  //把数组所有元素，按照“参数参数值”的模式将字符拼接成字符串
            prestr =prestr+appSecret;
            string mysign = Sign(prestr, sign_type, _input_charset);	//把最终的字符串签名，获得签名结果
            
            return mysign;
        }

        public static string CreateLinkstring(Dictionary<string, string> dicArray)
        {
            SortedDictionary<string, string> sortArray = new SortedDictionary<string, string>(dicArray);
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in sortArray)
            {
                if (!string.IsNullOrEmpty(temp.Key) && !string.IsNullOrEmpty(temp.Value))
                {
                    prestr.Append(temp.Key+ temp.Value);
                }
            }

            return prestr.ToString();
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="prestr">签名字符串</param>
        /// <param name="sign_type">签名</param>
        /// <param name="_input_charset"></param>
        /// <returns></returns>
        public static string Sign(string prestr, string sign_type, string _input_charset)
        {
            StringBuilder sb = new StringBuilder(32);
            if (sign_type.ToUpper() == "MD5")
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(prestr));
                for (int i = 0; i < t.Length; i++)
                {
                    sb.Append(t[i].ToString("x").PadLeft(2, '0'));
                }
            }
            return sb.ToString().ToUpper();
        }

        public static string PostData(string url, string postData)
        {
            string result = string.Empty;
            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                Encoding encoding = Encoding.UTF8;
                byte[] bytes = encoding.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
               
                using (Stream writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }

                #region 读取服务器返回信息

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        Encoding _encodingResponse = Encoding.UTF8;
                        //if(response)
                        Stream decompress = responseStream;
                        //decompress
                        if (response.ContentEncoding.ToLower() == "gzip")
                        {
                            decompress = new GZipStream(responseStream, CompressionMode.Decompress);
                        }
                        else if (response.ContentEncoding.ToLower() == "deflate")
                        {
                            decompress = new DeflateStream(responseStream, CompressionMode.Decompress);
                        }
                        using (StreamReader readStream = new StreamReader(decompress, _encodingResponse))
                        {
                            result = readStream.ReadToEnd();
                        }
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                result = string.Format("获取信息错误：{0}", e.Message);

            }

            return result;
        }
        /// <summary>
        /// 以Get/post方法获取远程GET方法数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetData(string url, string method = "GET")
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = method;
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();
            reader.Close();
            return content;
        }

        //验证签名
        public static bool CheckSign(SortedDictionary<string, string> tmpParas,string appSecret,ref string message)
        {
            Dictionary<string, string> paras =Parameterfilter(tmpParas);
            bool tag = BuildSign(paras, appSecret, "MD5", "utf-8") == tmpParas["sign"];
            message = tag ? "" : OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Signature, "sign");
            return tag;
        }
        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="tmpParas"></param>
        /// <param name="keycode"></param>
        /// <returns></returns>
        public static string GetSign(SortedDictionary<string, string> tmpParas, string keycode)
        {
            Dictionary<string, string> paras =Parameterfilter(tmpParas);
            string newSign = BuildSign(paras, keycode, "MD5", "utf-8");
            //HttpContext.Current.Response.Write(newSign);
            return newSign;
        }

        /// <summary>
        /// 检验时间戳在有效期范围内
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static bool CheckTimeStamp(string timestamp)
        {
            DateTime dttimestamp = DateTime.Parse(timestamp);

            return ((DateTime.Now - dttimestamp).TotalMinutes <= 10);

        }
    }
    
}