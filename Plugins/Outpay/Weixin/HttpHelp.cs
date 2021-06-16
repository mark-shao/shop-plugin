using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hishop.Plugins.Outpay.Weixin
{
    public class HttpHelp
    {/// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public string DoPost(string url, IDictionary<string, string> parameters)
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

        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="value">请求数据</param>
        /// <returns>HTTP响应</returns>
        public string DoPost(string url, string value, string cerPassword = null, string cerPath = null)
        {
            HttpWebRequest req = GetWebRequest(url, "POST", cerPassword, cerPath);
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            byte[] postData = Encoding.UTF8.GetBytes(value);
            System.IO.Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

            return GetResponseAsString(rsp, Encoding.UTF8);
        }

        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public string DoGet(string url, IDictionary<string, string> parameters)
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

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   //直接确认，否则打不开
            return true;
        }



        public HttpWebRequest GetWebRequest(string url, string method, string cerPassword = null, string cerPath = null)
        {
            cerPath = GetphysicsPath(cerPath);
            HttpWebRequest req = null;
            if (url.Contains("https"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

                req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                if (cerPassword != null)
                {
                    try
                    {
                        //加载证书
                        string password = cerPassword;//"1260132201";
                        FileStream fileStream = new FileStream(cerPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] bytes = new byte[fileStream.Length];
                        fileStream.Read(bytes, 0, bytes.Length);
                        fileStream.Close();
                        X509Certificate2 cert = new X509Certificate2(bytes, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                        req.ClientCertificates.Add(cert);
                    }
                    catch (Exception ex)
                    {
                        IDictionary<string, string> param = new Dictionary<string, string>();
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
                        PayLog.writeLog(param, cerPath, cerPassword, "证书验证", LogType.Weixin);
                    }
                    // System.IO.File.WriteAllText(@"D:\1.txt", "证书验证" + cerPath + "-" + password);
                }
                else
                {
                    // System.IO.File.WriteAllText(@"D:\2.txt", "证书验证密码为空" + cerPath );
                    //PayLog.writeLog(null, cerPath, cerPassword, "证书验证密码为空", LogType.Weixin);
                }

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
