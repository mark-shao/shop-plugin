using Hishop.Plugins.OpenId.WeiXin.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Xml;

namespace Hishop.Plugins.OpenId.WeiXin
{
    public class WeiXinNotify : OpenIdNotify
    {
        private const string ReUrl = "ReturnUrl";
        private readonly SortedDictionary<string, string> parameters;
        private NameValueCollection paramets;
        public WeiXinNotify(NameValueCollection _parameters)
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
            string key = doc.FirstChild.SelectSingleNode("Partner").InnerText;
            string secret = doc.FirstChild.SelectSingleNode("Key").InnerText;

            OAuthWXConfigInfo oAuthWXConfigInfo = new OAuthWXConfigInfo();
            oAuthWXConfigInfo.AppId = key;
            oAuthWXConfigInfo.AppSecret = secret;

            WeiXinTool weiXinTool = new WeiXinTool(oAuthWXConfigInfo);

            try
            {
                weiXinTool.CheckCanEnable();
                OAuthUserInfo oAuthUserInfo = weiXinTool.GetUserInfo(this.paramets);
                openId = oAuthUserInfo.OpenId;

                if (!string.IsNullOrEmpty(oAuthUserInfo.NickName))
                {
                    username = oAuthUserInfo.NickName;
                    msg += username;
                }

                if (!string.IsNullOrEmpty(username))
                {
                    HttpCookie name = HttpContext.Current.Request.Cookies["NickName"];
                    if (name == null)
                        name = new HttpCookie("NickName");
                    name.Value = System.Web.HttpContext.Current.Server.UrlEncode(username);
                    name.Expires = DateTime.Now.AddHours(1);
                    HttpContext.Current.Response.Cookies.Add(name);
                    isValid = true;
                }

            }
            catch (Exception ex)
            {
                isValid = false;


                IDictionary<string, string> iParam = new Dictionary<string, string>();
                iParam.Add("ConfigXml", configXml);
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

                OpenIdLog.AppendLog(iParam, "", "", HttpContext.Current.Request.Url.ToString(), LogType.Weixin);
                msg += "获取授权失败！" + ex.Message;
            }

            if (isValid)
            {
                OnAuthenticated(openId);
            }
            else
            {
                OnFailed(msg);
            }
        }
    }
}
