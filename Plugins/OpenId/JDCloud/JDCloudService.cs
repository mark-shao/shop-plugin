using JDCloudSDK.Ias.Apis;
using JDCloudSDK.Ias.Model;
using System;
using System.Web;

namespace Hishop.Plugins.OpenId.JDCloud
{
    [Plugin("京东云共享登录", Sequence = 6)]
    public class JDCloudService : OpenIdService
    {
        private const string ReUrl = "ReturnUrl";

        public JDCloudService() { }

        public JDCloudService(string returnUrl)
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

        [ConfigElement("是否删除旧京东云ClientId(2:删除新创建)", Nullable = false)]
        public string DelOldClientId { get; set; }

        public override void Post()
        {
            HttpCookie cookieKey = HttpContext.Current.Request.Cookies[ReUrl];
            if (cookieKey == null)
                cookieKey = new HttpCookie(ReUrl);
            cookieKey.Value = returnUrl;
            cookieKey.Expires = DateTime.Now.AddHours(1);//过期时间是2小时
            HttpContext.Current.Response.Cookies.Add(cookieKey);

            JDCloudService cloudService = new JDCloudService();
            cloudService.Partner = Partner;
            cloudService.Key = Key;
            cloudService.DelOldClientId = DelOldClientId;

            WebUtils util = new WebUtils();
            ApplicationRes res = util.GetAppDefault(cloudService, returnUrl);//获取已创建的clientid应用

            string strclient_id = string.Empty;
            if (res != null)
            {
                strclient_id = res.ClientId;
            }
            else
            {
                CreateAppResponse createApp = util.GetCreateApp(cloudService, returnUrl);

                if (createApp != null && createApp.Result != null)
                    strclient_id = createApp.Result.ClientId;
            }

            string returnUrlstr = returnUrl;
            if (returnUrlstr.IndexOf("?") == -1)
                returnUrlstr += "?";
           else
                returnUrlstr += "&";
            returnUrlstr += "client_id=" + strclient_id;
            string authenticationUrl = string.Format("https://oauth2.jdcloud.com/authorize?client_id={0}&redirect_uri={1}&response_type=code&state=12341234&code_challenge_method=S256&code_challenge=Vuu-tYpwl_4xB8miLyRO2p__zQoADgG1A40LoYCYsgU"
                , strclient_id, HttpUtility.UrlEncode(returnUrlstr));

            //OpenIdLog.AppendLog(null, null, "", authenticationUrl, LogType.JDCloud);
            this.Redirect(authenticationUrl);

        }

        public override string Description
        {
            get { return string.Empty; }
        }

        public override string Logo
        {
            get { return "hishop.plugins.openid.JDCloud.JDCloudservice.gif"; }
        }

        public override string ShortDescription
        {
            get
            {
                return "<div class=\"loginTabao\"><ul><li class=\"taobaoTitle\">京东云登录简介：</li>" +
                    "<li class=\"li1\">用户只要登录京东云账号，即可在您网站下单购物。快捷登录简化用户购物流程 ，提升网站下单率。</li>" +
                    "<li class=\"li2\"><a target=\"_blank\" href=\"https://www.jdcloud.com/\" title=\"在线申请\"><img src=\"../images/JDCloudlogin.gif\"/></a></li>" +
                    "</ul></div>";
            }
        }
    }
}
