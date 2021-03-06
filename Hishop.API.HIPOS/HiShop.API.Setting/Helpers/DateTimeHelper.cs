/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：DateTimeHelper.cs
    文件功能描述：时间处理
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HiShop.API.Setting.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime BaseTime = new DateTime(1970, 1, 1);//Unix起始时间

        /// <summary>
        /// 转换DateTime时间到C#时间
        /// </summary>
        /// <param name="dateTimeFromXml">DateTime</param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromXml(long dateTimeFromXml)
        {
            return BaseTime.AddTicks((dateTimeFromXml + 8 * 60 * 60) * 10000000);
        }
        /// <summary>
        /// 转换DateTime时间到C#时间
        /// </summary>
        /// <param name="dateTimeFromXml">DateTime</param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromXml(string dateTimeFromXml)
        {
            return GetDateTimeFromXml(long.Parse(dateTimeFromXml));
        }

        /// <summary>
        /// 获取DateTime（UNIX时间戳）
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <returns></returns>
        public static long GetWeixinDateTime(DateTime dateTime)
        {
            return (dateTime.Ticks - BaseTime.Ticks) / 10000000 - 8 * 60 * 60;
        }
    }
}
