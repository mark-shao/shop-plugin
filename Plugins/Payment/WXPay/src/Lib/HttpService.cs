using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Hishop.Weixin.Pay.Domain;
using System.Threading;

namespace Hishop.Weixin.Pay.Lib
{
    /// <summary>
    /// http连接基础类，负责底层的http通信
    /// </summary>
    public class HttpService
    {
        /// <summary>
        /// 验证结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>


        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }
        /// <summary>
        /// 提交请求
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="url"></param>
        /// <param name="isUseCert"></param>
        /// <param name="config"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string Post(string xml, string url, bool isUseCert, PayConfig config, int timeout)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            string result = "";//返回结果

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            IDictionary<string, string> logDict = new Dictionary<string, string>();
            logDict.Add("xml", xml);
            logDict.Add("url", url);
            logDict.Add("isUseCert", isUseCert.ToString());
            logDict.Add("timeout", timeout.ToString());
            logDict.Add("SSLCERT_PATH", config.SSLCERT_PATH);
            logDict.Add("SSLCERT_PASSWORD", config.SSLCERT_PASSWORD);
            logDict.Add("PROXY_URL", config.PROXY_URL);
            //logDict.Add("Key", config.Key);
            
            long len = 0;
            try
            {
                //设置最大连接数  
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.Timeout = Timeout.Infinite;

                //设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(config.PROXY_URL);              //网关服务器端口:端口
                //request.Proxy = proxy;

                //设置POST的数据类型和长度
                request.ContentType = "text/xml";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                request.ContentLength = data.Length;

                //是否使用证书
                if (isUseCert)
                {
                    
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    FileStream fileStream = new FileStream(path + config.SSLCERT_PATH, FileMode.Open, FileAccess.Read, FileShare.Read);
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, bytes.Length);
                    fileStream.Close();
                    X509Certificate2 cert = new X509Certificate2(bytes, config.SSLCERT_PASSWORD, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                    //X509Certificate2 cert = new X509Certificate2(path + config.SSLCERT_PATH, config.SSLCERT_PASSWORD, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                    request.ClientCertificates.Add(cert);
                }

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();
                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                //len = response.GetResponseStream().Length;
                WxPayLog.AppendLog(logDict, "", url + "---0", result, LogType.OrderQuery);
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                //WxPayLog.writeLog(logDict, xml, url, e.Message, LogType.Error);
                WxPayLog.AppendLog(logDict, xml, url + "---1", result, LogType.OrderQuery);
                System.Threading.Thread.ResetAbort();
                return "";
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    logDict.Add("StatusCode", ((HttpWebResponse)e.Response).StatusCode.ToString());
                    logDict.Add("StatusCode", ((HttpWebResponse)e.Response).StatusDescription);
                }
                // WxPayLog.writeLog(logDict, xml, url, e.Message, LogType.Error);
                WxPayLog.AppendLog(logDict, xml, url + "---2---" + len + "---" + e.Message, result, LogType.OrderQuery);
                //throw new WxPayException(e.ToString());
                return "";
            }
            catch (Exception e)
            {
                logDict.Add("HttpService", e.ToString());
                // WxPayLog.writeLog(logDict, "", url, e.Message, LogType.Error);
                WxPayLog.AppendLog(logDict, xml, url + "---3", result, LogType.OrderQuery);
                //throw new WxPayException(e.ToString());
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

        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <param name="PROXY_URL">代理url</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url, string PROXY_URL = "")
        {
            IDictionary<string, string> logDict = new Dictionary<string, string>();
            logDict.Add("PROXY_URL", PROXY_URL);
            System.GC.Collect();
            string result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                //设置代理
                if (!string.IsNullOrEmpty(PROXY_URL))
                {
                    WebProxy proxy = new WebProxy();
                    proxy.Address = new Uri(PROXY_URL);
                    request.Proxy = proxy;
                }
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
                WxPayLog.writeLog(logDict, "", url, "", LogType.Error);
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
                WxPayLog.writeLog(logDict, "", url, "", LogType.Error);
                return "";
            }
            catch (Exception e)
            {
                logDict.Add("HttpService", e.ToString());
                WxPayLog.writeLog(logDict, "", url, "", LogType.Error);
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
    }
}