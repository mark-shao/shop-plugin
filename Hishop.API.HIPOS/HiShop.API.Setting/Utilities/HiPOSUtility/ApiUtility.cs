using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HiShop.API.Setting.Utilities.HiPOSUtility
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
            return accessTokenOrAppId != null && accessTokenOrAppId.Length == 32;   
        }
    }
}
