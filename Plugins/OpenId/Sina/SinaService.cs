using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using NetDimension.Weibo;
using System.Configuration;


namespace Hishop.Plugins.OpenId.Sina
{
    [Plugin("新浪微博登录", Sequence =4)]
    public class SinaService : OpenIdService
    {
        private const string ReUrl = "ReturnUrl";
        [ConfigElement("App Key", Nullable = false)]
        public string AppKey { get; set; }
        [ConfigElement("AppSecret", Nullable = false)]
        public string AppSecret { get; set; }

        private readonly string returnUrl;

        public SinaService(string returnUrl)
        {
            this.returnUrl = returnUrl;
        }

        public SinaService()
        {
 
        }

        protected override void Redirect(string url)
        {
            base.Redirect(url);
        }

        public override void Post()
        {

            HttpCookie cookiereturnurl = HttpContext.Current.Request.Cookies[ReUrl];
            if (cookiereturnurl == null)
                cookiereturnurl = new HttpCookie(ReUrl);
            cookiereturnurl.Value =returnUrl;
            cookiereturnurl.Expires = DateTime.Now.AddHours(1);//过期时间是2小时
            HttpContext.Current.Response.Cookies.Add(cookiereturnurl);

            OAuth oauth = new OAuth(AppKey, AppSecret,returnUrl); 
            string url = oauth.GetAuthorizeURL(ResponseType.Code, null, DisplayType.Default);
            Redirect(url);

        }

        public override string Description
        {
            get {  return string.Empty;  }
        }

        public override string ShortDescription
        {
            get
            {
                return "<div class=\"loginTabao\"><ul><li class=\"taobaoTitle\">新浪微博登录简介：</li>" +
                    "<li class=\"li1\">用户只要使用微博账号进行登录即可注册成为网站会员，帮助您提升网站的用户注册量和提升网站访问数据。</li>" +
                    "<li class=\"li1\">申请接口输入的回调地址为：http://域名/openid/OpenIdEntry/hishop_plugins_openid_sina_sinaservice</li>" +
                    "<li class=\"li2\"><a target=\"_blank\" href=\"http://open.weibo.com\" title=\"在线申请\"><img src=\"../images/sinalogo.gif\"/></a></li>" +
                    "</ul></div>";
            }
        }

        public override string Logo
        {
            get { return "hishop.plugins.openid.sina.sinaservice.gif"; }
        }


    }
}
