using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Plugins.OpenId.WeiXin.Model
{
    public class OAuthUserInfo
    {
        /// <summary>
        /// 从登录方返回的OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 是否为男性
        /// </summary>
        public bool? IsMale { get; set; }

    }
}
