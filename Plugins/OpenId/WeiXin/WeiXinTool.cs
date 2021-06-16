using Hishop.Plugins.OpenId.WeiXin.Assistant;
using Hishop.Plugins.OpenId.WeiXin.Exceptions;
using Hishop.Plugins.OpenId.WeiXin.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Hishop.Plugins.OpenId.WeiXin
{
    public class WeiXinTool
    {
        public static string WXWorkDirectory = string.Empty;
        static string ReturnUrl = string.Empty;

        OAuthWXConfigInfo oAuthWXConfigInfo=null;

        public OAuthWXConfigInfo OAuthWXConfigInfo
        {
            get { return oAuthWXConfigInfo; }
            set { oAuthWXConfigInfo = value; }
        }

        public WeiXinTool(OAuthWXConfigInfo oAuthWXConfigInfo)
        {
            this.oAuthWXConfigInfo = oAuthWXConfigInfo;
        }

        public FormData GetFormData()
        {
            OAuthWXConfigInfo config = this.oAuthWXConfigInfo;
            
            return new FormData()
            {
                Items = new FormData.FormItem[] { 
                   //AppId
                   new  FormData.FormItem(){
                     DisplayName = "AppId",
                     Name = "AppId",
                     IsRequired = true,
                       Type= FormData.FormItemType.text,
                       Value=config.AppId
                   },

                   //AppKey
                   new FormData.FormItem(){
                     DisplayName = "AppSecret",
                     Name = "AppSecret",
                     IsRequired = true,
                      Type= FormData.FormItemType.text,
                      Value=config.AppSecret
                   }
                }
            };

        }

        public string GetOpenLoginUrl(string returnUrl)
        {
            ReturnUrl = returnUrl;
            OAuthWXConfigInfo config = this.oAuthWXConfigInfo;
            OAuthRule rule = new OAuthRule();

            if (string.IsNullOrEmpty(returnUrl))
                throw new PluginException("未传入回调地址！");
            if (string.IsNullOrEmpty(config.AppId))
                throw new PluginException("未设置AppId！");
            if (string.IsNullOrEmpty(config.AppSecret))
                throw new PluginException("未设置AppSecret！");
            if (string.IsNullOrEmpty(rule.GetCodeUrl))
                throw new PluginException("未设置微信接口地址！");

            string strGetCodeUrl = string.Format(rule.GetCodeUrl, config.AppId, ReturnUrl);

            return strGetCodeUrl;
        }

        public OAuthUserInfo GetUserInfo(NameValueCollection queryString)
        {
            OAuthUserInfo oAuthUser = new OAuthUserInfo();
            try
            {
                
                string strCode = string.Empty;
                string strState = string.Empty;

                OAuthWXConfigInfo config = this.oAuthWXConfigInfo;
                if (queryString["code"] != null && queryString["state"] != null)
                {
                    strCode = queryString["code"];
                    strState = queryString["state"];
                    if (string.IsNullOrEmpty(config.AppSecret))
                        throw new System.MissingFieldException("未设置AppSecret！");

                    UserInfo userinfo = WeiXinApi.GetUserInfo(strCode, config.AppId, config.AppSecret);
                    oAuthUser.OpenId = string.IsNullOrWhiteSpace(userinfo.unionid) ? userinfo.openid : userinfo.unionid;
                    oAuthUser.NickName = userinfo.nickname;
                    oAuthUser.IsMale = userinfo.sex == 0 ? false : true;
                }
            }catch(Exception ex)
            {
                OpenIdLog.writeLog_Collection(queryString, ex.Message, ex.TargetSite.ToString(), ex.StackTrace, LogType.Weixin);
            }
            return oAuthUser;
        }

        public string GetValidateContent()
        {
            return string.Empty;
        }


        public void SetFormValues(IEnumerable<KeyValuePair<string, string>> values)
        {
            //KeyValuePair<string, string> appid = values.FirstOrDefault(item => item.Key == "AppId");
            //if (string.IsNullOrWhiteSpace(appid.Value))
            //{
            //    throw new PluginException("Appid不能为空");
            //}
            //KeyValuePair<string, string> appsecret = values.FirstOrDefault(item => item.Key == "AppSecret");
            //if (string.IsNullOrWhiteSpace(appsecret.Value))
            //{
            //    throw new PluginException("AppSecret不能为空");
            //}

            //OAuthWXConfigInfo config = new OAuthWXConfigInfo();
            //config.AppId = appid.Value;
            //config.AppSecret = appsecret.Value;
            //ConfigService<OAuthWXConfigInfo>.SaveConfig(config, WXWorkDirectory + "\\Config\\OAuthWXConfig.config");
            //ConfigService<OAuthWXConfigInfo>.SaveConfig()
        }

        public string ShortName
        {
            get { return "微信"; }
        }

        public void CheckCanEnable()
        {
            OAuthWXConfigInfo config = this.oAuthWXConfigInfo;
            OAuthRule rule = new OAuthRule();

            if (string.IsNullOrEmpty(config.AppId))
                throw new PluginException("未设置AppId！");
            if (string.IsNullOrEmpty(config.AppSecret))
                throw new PluginException("未设置AppSecret！");
            if (string.IsNullOrEmpty(rule.GetCodeUrl))
                throw new PluginException("未设置微信接口地址！");
        }

        public string WorkDirectory
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("WorkDirectory不能为空");
                WeiXinTool.WXWorkDirectory = value;
            }
        }


        public string Icon_Default
        {
            get
            {
                if (string.IsNullOrWhiteSpace(WXWorkDirectory))
                {
                    throw new System.MissingFieldException("未设置工作目录！");
                }
                return WXWorkDirectory + "/Resource/weixin1.png";
            }
        }

        public string Icon_Hover
        {
            get
            {
                if (string.IsNullOrWhiteSpace(WXWorkDirectory))
                {
                    throw new System.MissingFieldException("未设置工作目录！");
                }
                return WXWorkDirectory + "/Resource/weixin2.png";
            }
        }
    }
}
