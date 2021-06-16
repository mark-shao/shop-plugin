using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web.Script.Serialization;
using Hishop.Weixin.MP.Domain;

namespace Hishop.Weixin.MP.Api
{
    public class TokenApi
    {
        public string AppId
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("AppId");
            }
        }

        public string AppSecret
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("AppSecret");
            }
        }

        public static string GetToken_Message(string appid, string secret)
        {
            string url = String.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, secret);

            string response = new Util.WebUtils().DoGet(url, null);
            if (response.Contains("access_token"))
                response = new JavaScriptSerializer().Deserialize<Token>(response).access_token;
            return response;
        }


        public static string GetToken(string appid, string secret)
        {
            string url = String.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, secret);

            return new Util.WebUtils().DoGet(url, null);
        }
    }
}
