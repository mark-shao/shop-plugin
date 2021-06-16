using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Xml;
using Jayrock.Json.Conversion;
using System.Text.RegularExpressions;
using JDCloudSDK.Ias.Model;

namespace Hishop.Plugins.OpenId.JDCloud
{
    public class JDCloudNotify : OpenIdNotify
    {
        private const string ReUrl = "ReturnUrl";
        private readonly SortedDictionary<string, string> parameters;
        private NameValueCollection paramets;
        public JDCloudNotify(NameValueCollection _parameters)
        {
            paramets = _parameters;
            parameters = new SortedDictionary<string, string>();
            string[] requestItem = _parameters.AllKeys;

            for (int i = 0; i < requestItem.Length; i++)
            {
                parameters.Add(requestItem[i], _parameters[requestItem[i]]);
            }
            parameters.Remove("HIGW");
            parameters.Remove("HITO");
        }

        public override void Verify(int timeout, string configXml)
        {
            bool isValid = false;
            string username = string.Empty;
            string msg = "验证失败";
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);
            string openId = string.Empty;
            if (!string.IsNullOrEmpty(parameters["code"]))
            {
                HttpCookie requrl = HttpContext.Current.Request.Cookies[ReUrl];
                if (requrl == null)
                    requrl = new HttpCookie(ReUrl);
                string returnurl = requrl.Value;
                string token = string.Empty;
                string verifier = parameters["code"];
                var state = parameters["state"];
                var client_id = parameters["client_id"];

                string key = doc.FirstChild.SelectSingleNode("Partner").InnerText;
                string secret = doc.FirstChild.SelectSingleNode("Key").InnerText;
                WebUtils util = new WebUtils();
                try
                {
                    IDictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("appid", key);
                    param.Add("appkey", secret);
                    param.Add("code", verifier);
                    param.Add("client_id", client_id);
                    param.Add("state", state);

                    JDCloudService cloudService = new JDCloudService();
                    cloudService.Partner = key;
                    cloudService.Key = secret;

                    if (string.IsNullOrEmpty(client_id))
                    {
                        string reurl = returnurl.Replace("http://", "").Replace("https://", "");
                        if (reurl.IndexOf("/") != -1)
                        {
                            reurl = reurl.Substring(0, reurl.IndexOf("/"));
                        }
                        ApplicationRes res = util.GetAppDefault(cloudService, reurl);//获取已创建的clientid应用
                        if (res != null)
                            client_id = res.ClientId;
                    }
                    string tokenurl = string.Format("https://oauth2.jdcloud.com/token?client_id={0}&client_secret={1}&grant_type=authorization_code&code={2}"
                        , client_id, state, verifier.Trim());//注意client_secret值要与前面state值是一致的

                    string tokenstr = util.GetRequestPost(tokenurl);//获取第三步根据code获取token

                    token = util.GetJsonOneNameValue(tokenstr, "access_token");//从json中获取token值
                    if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(tokenstr) && tokenstr.IndexOf("远程服务器返回错误") == -1)
                    {
                        string strobj = util.GetRequestPostHeader("https://oauth2.jdcloud.com/userinfo", token);//第四步根据token获取用户信息
                        string straccount = util.GetJsonOneNameValue(strobj, "account");//从json中获取token值
                        if (!string.IsNullOrEmpty(straccount))
                            openId = username = straccount;

                        if (!string.IsNullOrEmpty(username))
                        {
                            HttpCookie name = HttpContext.Current.Request.Cookies["NickName"];
                            if (name == null)
                                name = new HttpCookie("NickName");
                            name.Value = System.Web.HttpContext.Current.Server.UrlEncode(username);
                            name.Expires = DateTime.Now.AddHours(1);//过期时间是1小时
                            HttpContext.Current.Response.Cookies.Add(name);
                            isValid = true;
                        }
                        else
                        {
                            OpenIdLog.AppendLog(param, strobj, "1", "2获取用户信息：" + strobj, LogType.JDCloud);
                        }
                    }
                    else
                    {
                        OpenIdLog.AppendLog(param, tokenstr, "3", "4token：" + tokenstr, LogType.JDCloud);
                    }
                }
                catch (Exception ex)
                {
                    isValid = false;
                    msg += "获取授权失败！" + ex.Message;
                    IDictionary<string, string> iParam = new Dictionary<string, string>();
                    if (iParam == null)
                    {
                        iParam = new Dictionary<string, string>();
                    }
                    if (ex is System.Threading.ThreadAbortException)
                    {
                        return;
                    }
                    iParam.Add("ErrorMessage", ex.Message);
                    iParam.Add("StackTrace", ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        iParam.Add("InnerException", ex.InnerException.ToString());
                    }
                    if (ex.GetBaseException() != null)
                        iParam.Add("BaseException", ex.GetBaseException().Message);
                    if (ex.TargetSite != null)
                        iParam.Add("TargetSite", ex.TargetSite.ToString());
                    iParam.Add("ExSource", ex.Source);
                    OpenIdLog.AppendLog(iParam, "5", "6", msg, LogType.OpenIdNotify);
                }
            }
            else
            {
                OpenIdLog.AppendLog(new Dictionary<string, string>(), "code为空", "7", "8", LogType.OpenIdNotify);
            }
            if (isValid)
                OnAuthenticated(openId);
            else
                OnFailed(msg);
        }
    }
}
