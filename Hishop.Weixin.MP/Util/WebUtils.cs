using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Hishop.Weixin.MP.Util
{
    /// <summary>
    /// 网络工具类。
    /// </summary>
    public sealed class WebUtils
    {
        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public string DoPost(string url, IDictionary<string, string> parameters)
        {
            try
            {
                HttpWebRequest req = GetWebRequest(url, "POST");
                req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

                byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters));
                System.IO.Stream reqStream = req.GetRequestStream();
                reqStream.Write(postData, 0, postData.Length);
                reqStream.Close();

                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

                return GetResponseAsString(rsp, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                parameters.Add("url", url);
                AppendLog(parameters, ex, "GetWebRequest");
                return "";
            }
        }

        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="value">请求数据</param>
        /// <returns>HTTP响应</returns>
        public string DoPost(string url, string value)
        {
            try
            {
                HttpWebRequest req = GetWebRequest(url, "POST");
                req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

                byte[] postData = Encoding.UTF8.GetBytes(value);
                System.IO.Stream reqStream = req.GetRequestStream();
                reqStream.Write(postData, 0, postData.Length);
                reqStream.Close();

                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

                return GetResponseAsString(rsp, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("url", url);
                parameters.Add("value", value);
                AppendLog(parameters, ex, "GetWebRequest");
                return "";
            }
        }

        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public string DoGet(string url, IDictionary<string, string> parameters)
        {
            try
            {
                if (parameters != null && parameters.Count > 0)
                {
                    if (url.Contains("?"))
                    {
                        url = url + "&" + BuildQuery(parameters);
                    }
                    else
                    {
                        url = url + "?" + BuildQuery(parameters);
                    }
                }

                HttpWebRequest req = GetWebRequest(url, "GET");
                req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

                return GetResponseAsString(rsp, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                parameters.Add("url", url);
                AppendLog(parameters, ex, "GetWebRequest");
                return "";
            }
        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   //直接确认，否则打不开
            return true;
        }

        public HttpWebRequest GetWebRequest(string url, string method)
        {
            try
            {
                HttpWebRequest req = null;
                if (url.Contains("https"))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                }
                else
                {
                    req = (HttpWebRequest)WebRequest.Create(url);
                }
                req.ServicePoint.Expect100Continue = false;
                req.Method = method;
                req.KeepAlive = true;
                req.UserAgent = "Hishop";
                return req;
            }
            catch (Exception ex)
            {
                IDictionary<string, string> iParam = new Dictionary<string, string>();
                iParam.Add("url", url);
                iParam.Add("method", method);
                AppendLog(iParam, ex, "GetWebRequest");
                return null;
            }

        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        public string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            System.IO.Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }

        /// <summary>
        /// 组装GET请求URL。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>带参数的GET请求URL</returns>
        public string BuildGetUrl(string url, IDictionary<string, string> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildQuery(parameters);
                }
                else
                {
                    url = url + "?" + BuildQuery(parameters);
                }
            }
            return url;
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string BuildQuery(IDictionary<string, string> parameters)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");
                    postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

        /// <summary>
        /// 追加日志
        /// </summary>
        /// <param name="param"></param>
        /// <param name="sign"></param>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="logtype"></param>
        public static void AppendLog(IDictionary<string, string> param, Exception ex = null, string logPath = "")
        {
            if (param == null)
            {
                param = new Dictionary<string, string>();
            }
            if (ex != null)
            {
                if (ex is System.Threading.ThreadAbortException)
                {
                    return;
                }
                param.Add("ErrorMessage", ex.Message);
                param.Add("StackTrace", ex.StackTrace);
                if (ex.InnerException != null)
                {
                    param.Add("InnerException", ex.InnerException.ToString());
                }
                if (ex.GetBaseException() != null)
                    param.Add("BaseException", ex.GetBaseException().Message);
                if (ex.TargetSite != null)
                    param.Add("TargetSite", ex.TargetSite.ToString());
                param.Add("ExSource", ex.Source);
            }
            var objlock = new Object();
            lock (objlock)
            {
                if (string.IsNullOrEmpty(logPath))
                {
                    logPath = "/log/error_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";
                }
                else
                {
                    if (logPath.IndexOf('/') == -1 && logPath.IndexOf('.') == -1)
                    {
                        logPath = "/log/" + logPath + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";
                    }
                }
                string LogFile = GetphysicsPath(logPath);
                CreatePath(LogFile);
                using (StreamWriter fs = File.AppendText(LogFile))
                {
                    fs.WriteLine("时间：" + DateTime.Now.ToString());
                    if (param != null)
                    {
                        foreach (KeyValuePair<string, string> key in param)
                        {
                            fs.WriteLine(key.Key + ":" + key.Value);
                        }
                    }
                    fs.WriteLine("");
                    fs.WriteLine("");
                }
            }
        }
        /// <summary>
        /// 创建目录，根据指定的路径来创建目录，可多级创建，发生异常则返回false
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CreatePath(string path)
        {
            bool Success = true;
            path = GetphysicsPath(path).ToLower();
            var basePath = AppDomain.CurrentDomain.BaseDirectory.ToLower();
            path = path.Replace(basePath, "");
            string[] pathArr = path.Split(Path.DirectorySeparatorChar);
            string tempPath = pathArr[0];
            tempPath = basePath + tempPath;
            try
            {
                for (int i = 1; i < pathArr.Length - 1; i++)
                {
                    tempPath = tempPath + Path.DirectorySeparatorChar + pathArr[i];
                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Success = false;
            }
            return Success;
        }
        /// <summary>
        /// 获取指定虚拟路径的真实路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetphysicsPath(string path)
        {
            string physicsPath = "";
            try
            {
                if (HttpContext.Current == null)
                {
                    string strPath = path.Replace("/", "\\");
                    if (strPath.StartsWith("\\"))
                    {
                        strPath = strPath.TrimStart('\\');
                    }
                    physicsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
                }
                else
                {
                    physicsPath = HttpContext.Current.Request.MapPath(path);
                }
            }
            catch { physicsPath = path; }
            return physicsPath;
        }

    }


}
