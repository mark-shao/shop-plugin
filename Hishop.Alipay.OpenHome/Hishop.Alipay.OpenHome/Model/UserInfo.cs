using System;

namespace Hishop.Alipay.OpenHome.Model
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [Serializable]
    class UserInfo
    {
        /// <summary>
        /// 隐藏的支付宝账号，如：shu***@163.com
        /// </summary>
        public string logon_id { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string user_name { get; set; }

    }
}
