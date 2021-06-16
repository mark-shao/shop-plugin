using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Hishop.Plugins.OpenId.Taobao
{
    [Plugin("淘宝信任登录", Sequence = 3)]
    public class TaoBaoService : OpenIdService
    {
        private const string ReUrl = "ReturnUrl";
        public TaoBaoService()
        { }
        public TaoBaoService(string returnUrl)
        {
            this.returnUrl = returnUrl;
        }
        [ConfigElement("App Key", Nullable = false)]
        public string AppKey { get; set; }
        [ConfigElement("AppSecret", Nullable = false)]
        public string AppSecret { get; set; }

        private readonly string returnUrl;

        public override void Post()
        {
            HttpCookie cookiereturnurl = HttpContext.Current.Request.Cookies[ReUrl];
            if (cookiereturnurl == null)
                cookiereturnurl = new HttpCookie(ReUrl);
            cookiereturnurl.Value = returnUrl;
            cookiereturnurl.Expires = DateTime.Now.AddHours(1);//过期时间是2小时
            HttpContext.Current.Response.Cookies.Add(cookiereturnurl);
            string view = "web";
            if (!string.IsNullOrEmpty(returnUrl) && (returnUrl.ToLower().IndexOf("/wapshop/") > -1 || returnUrl.ToLower().IndexOf("/alioh/") > -1))
            {
                view = "wap";
            }
            base.Redirect(string.Format("https://oauth.taobao.com/authorize?response_type=code&client_id=" + AppKey + "&redirect_uri=" + returnUrl + "&view={0}&state=hishop", view));
        }

        public override string Description
        {
            get { return string.Empty; }
        }

        public override string Logo
        {
            get { return "hishop.plugins.openid.taobao.taobaoservice.gif"; }
        }

        public override string ShortDescription
        {
            get
            {
                return "<div class=\"loginTabao\"><ul><li class=\"taobaoTitle\">淘宝信任登录简介：</li>" +
                    "<li class=\"li1\">海量淘宝用户只要登录淘宝账号，即可在您网站下单购物。快捷登录简化用户购物流程 ，提升网站下单率。</li>" +
                    "<li class=\"li2\"><a target=\"_blank\" href=\"http://open.taobao.com/index.htm\" title=\"在线申请\"><img src=\"../images/kuaijie.gif\"/></a></li>" +
                    "</ul></div>";
            }
        }
    }
}
