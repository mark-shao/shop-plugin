using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Alipay.OpenHome.Request
{
    public interface IRequest
    {
        /// <summary>
        /// 获取请求方法名
        /// </summary>
        /// <returns></returns>
        string GetMethodName();


        /// <summary>
        /// 获取业务内容
        /// </summary>
        /// <returns></returns>
        string GetBizContent();
    }
}
