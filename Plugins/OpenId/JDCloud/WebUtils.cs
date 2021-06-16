using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using JDCloudSDK.Ias.Client;
using JDCloudSDK.Core.Auth;
using JDCloudSDK.Ias.Apis;
using JDCloudSDK.Core.Http;
using JDCloudSDK.Ias.Model;

namespace Hishop.Plugins.OpenId.JDCloud
{
    /// <summary>
    /// 网络工具类。
    /// </summary>
    public sealed class WebUtils
    {
        private int _timeout = 10000000;

        /// <summary>
        /// 请求与响应的超时时间
        /// </summary>
        public int Timeout
        {
            get { return this._timeout; }
            set { this._timeout = value; }
        }
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }
        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public string DoPost(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest req = GetWebRequest(url, "POST");
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters));
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            return GetResponseAsString(rsp, encoding);
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
            Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
            return GetResponseAsString(rsp, encoding);
        }

       

        public HttpWebRequest GetWebRequest(string url, string method)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.Method = method;
            req.KeepAlive = true;
            //req.UserAgent = "Top4Net";
            req.Timeout = this._timeout;
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
            Stream stream = null;
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
        public string BuildQuery(IDictionary<string, string> parameters)
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
                    postData.Append(Uri.EscapeDataString(value));
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

        /// <summary>
        /// 解析回调地址中top_parameters中的值
        /// </summary>
        /// <param name="key">取值关键词</param>
        /// <returns></returns>
        public string GetParameters(string parameters, string key)
        {
            string ret = string.Empty;
            try
            {
                string[] param = parameters.Split('&');
                for (int i = 0; i < param.Length; i++)
                {
                    string[] info = param[i].Split('=');
                    if (info[0].ToLower() == key.ToLower())
                    {
                        ret = info[1];
                        break;
                    }
                }
            }
            catch
            {
                //
            }
            return ret;
        }


        /// <summary>
        /// 从json字符串数据里，根据名获取它对应的值(这是方便如不引用Json转换，快速获取一个值)
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public string GetJsonOneNameValue(string jsonStr, string name)
        {
            string value = string.Empty;

            if (!string.IsNullOrEmpty(jsonStr) && !string.IsNullOrEmpty(name))
            {
                var strreqs = jsonStr.Replace("{", "").Replace("}", "").Split(new string[] { "," }, StringSplitOptions.None);

                foreach (var item in strreqs)
                {
                    var namevalue = item.Split(new string[] { ":" }, StringSplitOptions.None);
                    if (namevalue.Length > 1)
                    {
                        if (namevalue[0].Replace("\"", "") == name)
                        {
                            value = namevalue[1].Replace("\"", "");
                            break;
                        }
                    }
                }
            }

            return value;
        }


        #region 应用操作
        /// <summary>
        /// 初始应用创建
        /// </summary>
        /// <param name="JDCloudService"></param>
        /// <returns></returns>
        public IasClient GetIasClient(JDCloudService cloudService)
        {
            //1. 设置accessKey和secretKey
            CredentialsProvider credentialsProvider = new StaticCredentialsProvider(cloudService.Partner, cloudService.Key);

            //2. 创建XXXClient
            IasClient iasClient = new IasClient.DefaultBuilder()
                     .CredentialsProvider(credentialsProvider)
                     .HttpRequestConfig(new HttpRequestConfig(Protocol.HTTP, 10))
                     .Build();

            return iasClient;
        }

        /// <summary>
        /// 获取全部的app应用<既ClientId>（app应用它现在是最多只有20个）
        /// </summary>
        /// <returns></returns>
        public GetAppsResponse GetAllApps(JDCloudService cloudService)
        {
            //2. 创建XXXClient
            IasClient iasClient = GetIasClient(cloudService);

            //3. 设置请求参数
            GetAppsRequest request = new GetAppsRequest();
            request.RegionId = "cn-north-1";
            request.Version = "";

            //4. 执行请求
            //return iasClient.GetApps(request).Result;
            return iasClient.GetApps(request);
            //var bbb = iasClient.GetApps(request);
            ////Himall.Core.Log.Error("--b3:");
            //try
            //{
            //    bbb.RunSynchronously();
            //    var ccc222 = bbb.Result;

            //}
            //catch(Exception ex)
            //{

            //}  var ccc = bbb.Result;
            ////ccc.Result.Apps
            //Himall.Core.Log.Error("--b4:");
            //return ccc;
        }

        /// <summary>
        /// 获取当前地址url的app应用
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ApplicationRes GetAppDefault(JDCloudService cloudService, string reurl)
        {
            ApplicationRes res = null;

            if (cloudService.DelOldClientId == "1")
            {
                DeleteAppPart(cloudService, reurl);//删除不包含当前url的应用
                cloudService.DelOldClientId = string.Empty;

                //SaveConfig(cloudService);
            }
            else if (cloudService.DelOldClientId == "2")
            {
                DeleteAppAll(cloudService);//删除之前所有的应用
                cloudService.DelOldClientId = string.Empty;

                //SaveConfig(cloudService);

                return null;
            }

            //Himall.Core.Log.Error("--a0:");
            GetAppsResponse appsResponse = GetAllApps(cloudService);
            //Himall.Core.Log.Error("--a1:");
            if (appsResponse != null && appsResponse.Result != null)
            {
                reurl = reurl.ToLower();
                foreach (var item in appsResponse.Result.Apps)
                {
                    if (item.ClientUri.ToLower().IndexOf(reurl) != -1)
                    {
                        res = item;
                        break;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// 新创建一个应用
        /// </summary>
        /// <param name="reurl"></param>
        /// <returns></returns>
        public CreateAppResponse GetCreateApp(JDCloudService cloudService, string reurl)
        {
            //2. 创建XXXClient
            IasClient iasClient = GetIasClient(cloudService);

            //3. 设置请求参数
            CreateAppRequest request = new CreateAppRequest();
            request.RegionId = "cn-north-1";
            request.ClientName = "client_test";
            request.TokenEndpointAuthMethod = "client_secret_post";
            request.Secret = "12341234";
            request.GrantTypes = "authorization_code,implicit,refresh_token";
            request.RedirectUris = reurl;
            request.ClientUri = reurl;//http://clientUri.com/abc
            request.LogoUri = reurl;//http://logoUri.com/abc
            request.TosUri = reurl;//http://tosUri.com/abc
            request.PolicyUri = reurl;//http://policyUri.com/abc
            request.Scope = "openid";
            request.JwksUri = reurl;//http://jwksUri.com/abc
            request.Jwks = "jwks";
            request.Contacts = "13112341234";
            request.Extension = "hello";
            request.AccessTokenValiditySeconds = 1000;
            request.RefreshTokenValiditySeconds = 5184000;
            request.MultiTenant = true;
            request.UserType = "root";

            //4. 执行请求
            //var response = iasClient.CreateApp(request).Result;
            //return response;
            return iasClient.CreateApp(request);
        }


        /// <summary>
        /// 删除所有应用
        /// </summary>
        /// <returns></returns>
        public bool DeleteAppAll(JDCloudService cloudService)
        {
            bool yn = false;

            //4. 执行请求
            var response = GetAllApps(cloudService);

            if (response != null && response.Result != null)
            {
                //2. 新的创建XXXClient
                IasClient iasClient = GetIasClient(cloudService);

                //StringBuilder strs = new StringBuilder();
                foreach (var item in response.Result.Apps)
                {
                    DeleteAppRequest deleteApp = new DeleteAppRequest();
                    deleteApp.RegionId = response.RequestId;
                    deleteApp.ClientId = item.ClientId;
                    deleteApp.Version = "";

                    var deleteresponse = iasClient.DeleteApp(deleteApp).Result;
                    yn = (deleteresponse != null && deleteresponse.Count > 0);
                }
            }

            return yn;
        }

        /// <summary>
        /// 删除不属于当前url的其他应用
        /// </summary>
        /// <returns></returns>
        public bool DeleteAppPart(JDCloudService cloudService, string reurl)
        {
            bool yn = false;

            //4. 执行请求
            var response = GetAllApps(cloudService);

            if (response != null && response.Result != null)
            {
                //2. 新的创建XXXClient
                IasClient iasClient = GetIasClient(cloudService);

                reurl = reurl.ToLower();
                foreach (var item in response.Result.Apps)
                {
                    if (!string.IsNullOrEmpty(item.RedirectUris) && item.RedirectUris.ToLower().IndexOf(reurl) == -1)
                    {
                        DeleteAppRequest deleteApp = new DeleteAppRequest();
                        deleteApp.RegionId = response.RequestId;
                        deleteApp.ClientId = item.ClientId;
                        deleteApp.Version = "";

                        var deleteresponse = iasClient.DeleteApp(deleteApp).Result;
                        yn = (deleteresponse != null && deleteresponse.Count > 0);
                    }
                }
            }

            return yn;
        }
        #endregion

        #region web——Request请求

        public string GetRequestPost(string requestUrl)
        {
            //提交请求
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "Post";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = 10000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetRequestPostHeader(string requestUrl, string requestHeader)
        {
            //提交请求
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "Post";
                request.ContentType = "text/html;charset=UTF-8;";

                if (!string.IsNullOrEmpty(requestHeader))
                {
                    request.Headers.Add("Authorization", "Bearer " + requestHeader);
                }

                request.Timeout = 10000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}
