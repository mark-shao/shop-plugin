/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：AccessTokenResult.cs
    文件功能描述：access_token请求后的JSON返回格式
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using HiShop.API.Setting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HiShop.API.HiPOS.Entities
{
    /// <summary>
    /// access_token请求后的JSON返回格式
    /// </summary>
    public class AccessTokenResult : HiShopJsonResult
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
        /// <summary>
        /// 有效期秒
        /// </summary>
        public int expires_in { get; set; }

       
    }
}
