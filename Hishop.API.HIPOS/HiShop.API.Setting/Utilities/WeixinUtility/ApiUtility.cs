/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：StreamUtility.cs
    文件功能描述：对象公共类
    
    
    创建标识：Senparc - 20150703
    
----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HiShop.API.Setting.Utilities.WeixinUtility
{
    public static class ApiUtility
    {
        /// <summary>
        /// 判断accessTokenOrAppId参数是否是AppId
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        /// <returns></returns>
        public static bool IsAppId(string accessTokenOrAppId)
        {
            return accessTokenOrAppId != null && accessTokenOrAppId.Length <= 18/*wxc3c90837b0e76080*/;
        }
    }
}
