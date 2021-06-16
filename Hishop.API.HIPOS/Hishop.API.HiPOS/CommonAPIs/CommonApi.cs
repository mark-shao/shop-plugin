using HiShop.API.HiPOS.Entities;
using HiShop.API.Setting.HttpUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.API.HiPOS.CommonAPIs
{
    public static class CommonApi
    {
        /// <summary>
        /// 获取凭证接口
        /// </summary>
        /// <param name="grant_type">获取access_token填写client_credential</param>
        /// <param name="appid">第三方用户唯一凭证</param>
        /// <param name="secret">第三方用户唯一凭证密钥，既appsecret</param>
        /// <returns></returns>
        public static AccessTokenResult GetToken(string appid, string appSecret, string grant_type = "client_credentials")
        {            
            Dictionary<string, string> formData = new Dictionary<string, string>();
            formData.Add("grant_type",grant_type);
            AccessTokenResult result = Post.PostGetJson<AccessTokenResult>(HiPOSParameter.GETTOKEN, null, formData, null, appid, appSecret);
            return result;
        }
       
    }
}
