/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：BroswerUtility.cs
    文件功能描述：浏览器公共类
    
    
    创建标识：Senparc - 20150419
    
----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HiShop.API.Setting.BrowserUtility
{
    public static class BroswerUtility
    {
        /// <summary>
        /// 判断是否在内置浏览器中
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static bool SideInWeixinBroswer(HttpContextBase httpContext)
        {
            var userAgent = httpContext.Request.UserAgent;
            if (string.IsNullOrEmpty(userAgent) || (!userAgent.Contains("MicroMessenger") && !userAgent.Contains("Windows Phone")))
            {
                //在外部
                return false;
            }
            //在内部
            return true;
        }
    }
}
