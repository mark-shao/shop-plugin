using Hishop.Alipay.OpenHome.Model;

namespace Hishop.Alipay.OpenHome.Response
{
    public class OauthTokenResponse : AliResponse, IAliResponseStatus
    {

        public AliUserTokenInfo alipay_system_oauth_token_response { get; set; }

        public string Code
        {
            get { return alipay_system_oauth_token_response != null ? "200" : "0"; }
        }

        public string Message
        {
            get { return Code != "200" ? "未知错误" : ""; }
        }
    }
}
