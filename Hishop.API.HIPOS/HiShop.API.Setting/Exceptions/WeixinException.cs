/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：WeixinException.cs
    文件功能描述：自定义异常基类
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HiShop.API.Setting.Exceptions
{
    /// <summary>
    /// 自定义异常基类
    /// </summary>
    public class WeixinException : ApplicationException
    {
        public WeixinException(string message)
            : base(message, null)
        {
        }

        public WeixinException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
