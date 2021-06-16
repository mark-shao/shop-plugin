using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP.Domain
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 获取到的令牌
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 令牌有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }

        //public string AccessToken { set; get; }

        //public string ExpiresIn { set; get; }

        //public string RefreshToken { set; get; }

        //public string OpenId { set; get; }

        //public string Scope { set; get; }
    }
}
