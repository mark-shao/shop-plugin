using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Xml;
using Jayrock.Json.Conversion;
using System.Text.RegularExpressions;

namespace Hishop.Plugins.OpenId.QQ
{
    public class QQNotify : OpenIdNotify
    {
        private const string ReUrl = "ReturnUrl";
        private readonly SortedDictionary<string, string> parameters;
        private NameValueCollection paramets;
        public QQNotify(NameValueCollection _parameters)
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
                string key = doc.FirstChild.SelectSingleNode("Partner").InnerText;
                string secret = doc.FirstChild.SelectSingleNode("Key").InnerText;
                WebUtils util = new WebUtils();
                try
                {
                    IDictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("grant_type", "authorization_code");
                    param.Add("client_id", key);   
                    param.Add("client_secret", secret);
                    param.Add("code", verifier);
                    param.Add("redirect_uri",returnurl);
                    param.Add("state", "hishop");
                   // string url = "https://graph.qq.com/oauth2.0/token?grant_type=authorization_code&client_id=" + key + "&client_secret=" + secret + "&code=" + verifier + "&redirect_uri=" + returnurl + "&state=hishop";
                    var tokenstr = util.DoPost("https://graph.qq.com/oauth2.0/token", param);
                   // var tokenstr = GetResponse(url, 1000000);
                    token = util.GetParameters(tokenstr,"access_token");
                    if (!string.IsNullOrEmpty(token))
                    {
                        IDictionary<string, string> param2 = new Dictionary<string, string>();
                        param2.Add("access_token", token);
                        var openidstr = util.DoPost("https://graph.qq.com/oauth2.0/me", param2);
                        string regstr = "\"openid\":\"(?<openid>[\\w]+)\"";
                       Regex reg = new Regex(regstr, RegexOptions.IgnoreCase);
                        MatchCollection mcollection = reg.Matches(openidstr);
                        openId = mcollection[0].Groups["openid"].Value;
                       IDictionary<string, string> param3 = new Dictionary<string, string>();
                       param3.Add("access_token", token);
                        param3.Add("oauth_consumer_key", key);
                        param3.Add("openid", openId);
                       var userinfostr = util.DoPost("https://graph.qq.com/user/get_user_info", param3);
                        
                       msg += userinfostr;
                       //   \"nickname\":\"(?<nickname>.+)\"\s*,\s*\"figureurl\":
                       string regstr2 = "\"nickname\":\\s*\"(?<nickname>.+)\"\\s*,\\s*\"gender\"";
                        Regex reg2 = new Regex(regstr2, RegexOptions.IgnoreCase);
                       MatchCollection mcollection2 = reg2.Matches(userinfostr);
                        username = mcollection2[0].Groups["nickname"].Value;
                        msg += username;
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
                            OpenIdLog.AppendLog(param, userinfostr, "1", "2" + userinfostr, LogType.OpenIdNotify);
                        }
                    }
                    else
                    {
                        OpenIdLog.AppendLog(param, tokenstr, "3", "4" + tokenstr, LogType.OpenIdNotify);
                    }
                }
                catch(Exception ex)
                {
                    isValid = false;
                    msg+= "获取授权失败！"+ex.Message;
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
