using Hishop.API.HiPOS;
using HiShop.API.HiPOS.AdvancedAPIs.Auth.AuthJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiShop.API.HiPOS.AdvancedAPIs.Auth
{
    public static class AuthApi
    {
        /// <summary>
        /// 获取商户API密钥
        /// </summary>
        /// <param name="hostname">应用程序主机名</param>
        /// <param name="notify_url">回调接口Token 通知地址</param>        
        /// <returns></returns>
        public static AuthResult GetAuth(string hostname, string notify_url)
        {            
            Dictionary<string, string> formData = new Dictionary<string, string>();
            formData.Add("hostname", hostname);
            formData.Add("notify_url", notify_url);
            AuthResult result = HiShop.API.Setting.HttpUtility.Post.PostGetJson<AuthResult>(HiPOSParameter.GETAUTH, null, formData, Encoding.UTF8);
            return result;
        }
    }
}
