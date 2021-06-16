using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Xml;
using System.Web;
using System.Security.Cryptography;
using NetDimension.Weibo;

namespace Hishop.Plugins.OpenId.Sina
{
    
    public class SinaNotify:OpenIdNotify
    {
        private const string ReUrl = "ReturnUrl";
        private readonly SortedDictionary<string, string> parameters;
        private NameValueCollection paramets;
        public SinaNotify(NameValueCollection _parameters)
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
            string msg = "验证失败";
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);
            string openId = string.Empty;
            try
            {
                //验证
                if (parameters["code"] != null)
                {
                    Client Sina = null;
                    string AppKey = doc.FirstChild.SelectSingleNode("AppKey").InnerText;
                    string AppSecret = doc.FirstChild.SelectSingleNode("AppSecret").InnerText;
                    HttpCookie requrl = HttpContext.Current.Request.Cookies[ReUrl];
                    if (requrl == null)
                        requrl = new HttpCookie(ReUrl);
                    string returnurl = requrl.Value;
                    OAuth oauth = new OAuth(AppKey, AppSecret, returnurl);
                    AccessToken token = oauth.GetAccessTokenByAuthorizationCode(parameters["code"]);
                    if (!string.IsNullOrEmpty(token.Token))
                    {
                        Sina = new Client(new OAuth(AppKey, AppSecret, token.Token, null)); //用cookie里的accesstoken来实例化OAuth，这样OAuth就有操作权限了
                        string user_id = Sina.API.Account.GetUID();
                        NetDimension.Weibo.Entities.user.Entity user = Sina.API.Users.Show(user_id, null);
                        HttpContext.Current.Request.Cookies.Remove(ReUrl);
                        //何军修改openid返回值为user_id
                        //openId = user.ScreenName;

                        if (!string.IsNullOrEmpty(user.ScreenName))
                        {
                            HttpCookie name = HttpContext.Current.Request.Cookies["SinaNickName"];
                            if (name == null)
                                name = new HttpCookie("SinaNickName");
                            name.Value = System.Web.HttpContext.Current.Server.UrlEncode(user.ScreenName);
                            name.Expires = DateTime.Now.AddHours(2);//过期时间是1小时
                            HttpContext.Current.Response.Cookies.Add(name);
                        }

                        parameters.Add("nickname", user.ScreenName);
                        openId = user_id;
                        isValid = true;
                    }
                    else
                    {
                        msg = "AccessToken参数值不存在！";
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            if (isValid)
                OnAuthenticated(openId);
            else
                OnFailed(msg);
        }
    }
}
