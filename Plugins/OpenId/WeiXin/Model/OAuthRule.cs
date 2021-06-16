using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hishop.Plugins.OpenId.WeiXin.Model
{
    public class OAuthRule
    {
        string codeUrlTemp = "https://open.weixin.qq.com/connect/qrconnect?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_login&state=hishop";
        string tokenUrlTemp ="https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
        string userInfoUrl = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}";

        public string GetCodeUrl 
        {
            get { return codeUrlTemp; }
            set { codeUrlTemp = value; }
        }
        public string GetTokenUrl 
        {
            get { return tokenUrlTemp; }
            set { tokenUrlTemp = value; }
        }
        public string GetUserInfoUrl
        {
            get { return userInfoUrl; }
            set { userInfoUrl = value; }
        }
    }
}
