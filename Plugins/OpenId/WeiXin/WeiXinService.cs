using Hishop.Plugins.OpenId.WeiXin.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Hishop.Plugins.OpenId.WeiXin
{
    [Plugin("微信扫码登录", Sequence = 5)]
    public class WeiXinService : OpenIdService
    {
        private const string ReUrl = "ReturnUrl";

        private readonly string returnUrl;

        [ConfigElement("AppID", Nullable = false)]
        public string Partner { get; set; }

        [ConfigElement("Appsecret", Nullable = false)]
        public string Key { get; set; }

        public WeiXinService() { }

        public WeiXinService(string returnUrl)
        {
            this.returnUrl = returnUrl;
        }

        protected override void Redirect(string url)
        {
            base.Redirect(url);
        }

        public override void Post()
        {
            HttpCookie cookieKey = HttpContext.Current.Request.Cookies[ReUrl];
            if (cookieKey == null)
                cookieKey = new HttpCookie(ReUrl);
            cookieKey.Value = returnUrl;
            cookieKey.Expires = DateTime.Now.AddHours(1);
            HttpContext.Current.Response.Cookies.Add(cookieKey);

            OAuthWXConfigInfo oAuthWXConfigInfo = new OAuthWXConfigInfo();
            oAuthWXConfigInfo.AppId = Partner;
            oAuthWXConfigInfo.AppSecret = Key;
            WeiXinTool weiXinTool = new WeiXinTool(oAuthWXConfigInfo);

            var authenticationUrl = weiXinTool.GetOpenLoginUrl(HttpUtility.UrlEncode(returnUrl));
            this.Redirect(authenticationUrl);
        }

        public override string Description
        {
            get { return string.Empty; }
        }

        public override string Logo
        {
            get { return "hishop.plugins.openid.weixin.weixinservice.gif"; }
        }

        public override string ShortDescription
        {
            get
            {
                return "<div class=\"loginTabao\"><ul><li class=\"taobaoTitle\">微信登录简介：</li>" +
                    "<li class=\"li1\">用户使微信扫描即可登录网站，扫描登录更可减少登录交互操作，大大降低网站注册门槛，提升购物体验，给网站带来海量新用户。</li>" +
                    "<li class=\"li2\"><a target=\"_blank\" href=\"https://open.weixin.qq.com/\" title=\"在线申请\"><img src=\"../images/qqlogin.gif\"/></a></li>" +
                    "</ul></div>";
            }
        }
    }
}
