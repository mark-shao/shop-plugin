/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：Get.cs
    文件功能描述：Get
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using HiShop.API.Setting.Entities;
using HiShop.API.Setting.Exceptions;

namespace HiShop.API.Setting.HttpUtility
{
    public static class Get
    {
        public static T GetJson<T>(string url, Encoding encoding = null, string appId = "", string appSecret = "", string accessToken = "")
        {
            string returnText = HttpUtility.RequestUtility.HttpGet(url, null, appId, appSecret, accessToken);

            JavaScriptSerializer js = new JavaScriptSerializer();            

            T result = js.Deserialize<T>(returnText);

            return result;
        }
    }
}
