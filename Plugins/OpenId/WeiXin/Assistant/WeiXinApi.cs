using Hishop.Plugins.OpenId.WeiXin.Exceptions;
using Hishop.Plugins.OpenId.WeiXin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Plugins.OpenId.WeiXin.Assistant
{
    public class WeiXinApi
    {
        public static UserInfo GetUserInfo(string code, string appid, string appsecret)
        {
            UserInfo userinfo = null;
            OAuthRule rule = new OAuthRule();
            if (string.IsNullOrEmpty(rule.GetTokenUrl))
                throw new System.MissingFieldException("未设置微信接口地址:GetTokenUrl");
            if (string.IsNullOrEmpty(rule.GetUserInfoUrl))
                throw new System.MissingFieldException("未设置微信接口地址:GetTokenUrl");
            string url = string.Format(rule.GetTokenUrl, appid, appsecret, code);
            HttpHandler.ClientRequest clientRequest = new HttpHandler.ClientRequest(url);
            clientRequest.HttpMethod = "get";
            ErrResult err = new ErrResult();
            TokenResult tokenResult = HttpHandler.GetResponseResult<TokenResult, ErrResult>(clientRequest, err);
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("Code", code);
            param.Add("appid", appid);
            param.Add("appsecret", appsecret);
            //OpenIdLog.AppendLog(param, Newtonsoft.Json.JsonConvert.SerializeObject(tokenResult), Newtonsoft.Json.JsonConvert.SerializeObject(err), "", LogType.Weixin);
            if (err.errcode > 0)
            {
                throw new PluginException("微信登录接口GetToken出错: " + err.errmsg);
            }
            if (string.IsNullOrEmpty(tokenResult.access_token))
            {
                throw new PluginException("微信登录接口返回access_Token为空");
            }

            url = string.Format(rule.GetUserInfoUrl, tokenResult.access_token, tokenResult.openid);
            clientRequest = new HttpHandler.ClientRequest(url);
            clientRequest.HttpMethod = "get";
            userinfo = HttpHandler.GetResponseResult<UserInfo, ErrResult>(clientRequest, err);
            if (err.errcode > 0)
            {
                throw new PluginException("微信登录接口GetUserInfo出错: " + err.errmsg);
            }

            return userinfo;
        }
    }
}
