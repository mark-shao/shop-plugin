using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Alipay.OpenHome.Utility
{
    class TimeHelper
    {
        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double TransferToMilStartWith1970(DateTime dateTime)
        {
            double intResult = 0;
            System.DateTime startTime = new System.DateTime(1970, 1, 1);
            intResult = (dateTime - startTime).TotalMilliseconds;
            return intResult;        
        }

    }
}
