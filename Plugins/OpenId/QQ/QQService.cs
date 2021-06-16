using System;
using System.Web;

namespace Hishop.Plugins.OpenId.QQ
{
    [Plugin("QQ共享登录", Sequence = 2)]
    public class QQService : OpenIdService
    {
        private const string ReUrl = "ReturnUrl";

        public QQService() { }

        public QQService(string returnUrl)
        {
            this.returnUrl = returnUrl;
        }

        protected override void Redirect(string url)
        {
            base.Redirect(url);
        }

        private readonly string returnUrl;

        [ConfigElement("APP ID", Nullable = false)]
        public string Partner { get; set; }

        [ConfigElement("App Key", Nullable = false)]
        public string Key { get; set; }

        public override void Post()
        {
            //var context=  new QzoneContext();
           
            ////Get a Request Token
            //var requestToken = context.GetRequestToken(returnUrl);

            HttpCookie cookieKey = HttpContext.Current.Request.Cookies[ReUrl];
            if (cookieKey == null)
                cookieKey = new HttpCookie(ReUrl);
            cookieKey.Value = returnUrl;
            cookieKey.Expires = DateTime.Now.AddHours(1);//过期时间是2小时
            HttpContext.Current.Response.Cookies.Add(cookieKey);
            //cookieKey.Expires = DateTime.Now.AddHours(1);//过期时间是2小时
            //HttpContext.Current.Response.Cookies.Add(cookieKey);
            //HttpCookie cookieSecret = HttpContext.Current.Request.Cookies[TokenSecret];
            //  cookieSecret = new HttpCookie(TokenSecret);
            //cookieSecret.Value = requestToken.TokenSecret;
            //cookieSecret.Expires = DateTime.Now.AddHours(2);//过期时间是2小时
            //HttpContext.Current.Response.Cookies.Add(cookieSecret);

            ////Get the Qzone authentication page for the user to go to in order to authorize the Request Token.
            //var authenticationUrl = context.GetAuthorizationUrl(requestToken, returnUrl);
            var authenticationUrl = "https://graph.qq.com/oauth2.0/authorize?response_type=code&client_id=" + Partner + "&redirect_uri=" +HttpUtility.UrlEncode(returnUrl)+ "&state=hishop&scope=get_user_info";
            this.Redirect(authenticationUrl);

        }

        public override string Description
        {
            get { return string.Empty; }
        }

        public override string Logo
        {
            get { return "hishop.plugins.openid.qq.qqservice.gif"; }
        }

        public override string ShortDescription
        {
            get
            {
                return "<div class=\"loginTabao\"><ul><li class=\"taobaoTitle\">QQ登录简介：</li>" +
                    "<li class=\"li1\">用户使用已有的QQ号码即可登录网站，QQ一键登录更可减少登录交互操作，大大降低网站注册门槛，提升购物体验，给网站带来海量新用户。</li>" +
                    "<li class=\"li1\">同时打通QQ空间、朋友网、腾讯微博三大平台，一站互联全线打通。</li>" +
                    "<li class=\"li2\"><a target=\"_blank\" href=\"http://connect.qq.com/\" title=\"在线申请\"><img src=\"../images/qqlogin.gif\"/></a></li>" +
                    "</ul></div>";
            }
        }
    }
}
