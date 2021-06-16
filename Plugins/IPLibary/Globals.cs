using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hishop.Plugins.IPLibary
{
    public class Globals
    {
        public static string IPAddress
        {
            get
            {
                string userIP;

                HttpRequest Request = HttpContext.Current.Request;
                // 如果使用代理，获取真实IP
                if (string.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
                    userIP = Request.ServerVariables["REMOTE_ADDR"];
                else
                    userIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(userIP))
                    userIP = Request.UserHostAddress;

                // WriteLog("/iplog.txt", string.Format("{0}:{1}:{2}", DateTime.Now.ToString(), userIP, HttpContext.Current.Request.Url.ToString()));
                return userIP;
            }
        }
        /// <summary>
        /// 验证结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static bool CheckValidationResult(object sender, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }
        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url)
        {
            IDictionary<string, string> logDict = new Dictionary<string, string>();
            System.GC.Collect();
            string result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                ////设置https验证方式
                //if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                //{
                //    ServicePointManager.ServerCertificateValidationCallback =
                //            new RemoteCertificateValidationCallback(CheckValidationResult);
                //}

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                logDict.Add("HttpService", "Thread - caught ThreadAbortException - resetting.");
                logDict.Add("Exception message: {0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                logDict.Add("HttpService", e.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    logDict.Add("HttpService", "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    logDict.Add("HttpService", "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                return "";
            }
            catch (Exception e)
            {
                logDict.Add("HttpService", e.ToString());
                return "";
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        public static string LibPath
        {
            get
            {
                return "/IPLibrary/qqwry.dat";
            }
        }
        /// <summary>
        /// 获取物理路径 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetphysicsPath(string path)
        {
            string physicsPath = string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    return HttpContext.Current.Server.MapPath(path);
                }
                catch { };
            }
            return string.Empty;
        }
        private static string LogPath
        {
            get
            {
                string path = HttpContext.Current.Server.MapPath("/log/");
                if (!Directory.Exists(path))//如果日志目录不存在就创建
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        /// <summary>
        /// 追加日志
        /// </summary>
        /// <param name="param"></param>
        /// <param name="sign"></param>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="logtype"></param>
        public static void AppendLog(IDictionary<string, string> param, string sign, string url, string msg)
        {
            using (StreamWriter fs = File.AppendText(LogPath + "RequestIP.txt"))
            {
                fs.WriteLine("时间：" + DateTime.Now.ToString());
                if (param != null && param.Count > 0)
                {
                    foreach (KeyValuePair<string, string> key in param)
                    {
                        fs.WriteLine(key.Key + ":" + key.Value);
                    }
                }
                fs.WriteLine("Url:" + url);
                fs.WriteLine("msg:" + msg);
                fs.WriteLine("sign:" + sign);
                fs.WriteLine("");
                fs.WriteLine("");
                fs.WriteLine("");
            }
        }
    }
}
