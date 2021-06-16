using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Hishop.Plugins
{
    public abstract class IPlibaryRequest : IPlugin
    {
        /// <summary>
        /// 支付接口-发送支付请求
        /// </summary>
        /// <param name="name">要创建的实例的完整类型名</param>
        /// <param name="IPAddress">IP地址</param>
        /// <returns></returns>
        public static IPlibaryRequest CreateInstance(
            string name, string IPAddress,string DataUrl)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            object[] paramArray = new object[2];

            paramArray[0] = IPAddress;
            paramArray[1] = DataUrl;

            Type type = IPlibaryPlugins.Instance().GetPlugin("IPlibaryRequest", name);
            if (type == null)
                return null;

            IPlibaryRequest instance = Activator.CreateInstance(type, paramArray) as IPlibaryRequest;
           

            return instance;
        }

        public static IPlibaryRequest CreateInstance(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            Type type = IPlibaryPlugins.Instance().GetPlugin("IPlibaryRequest", name);
            if (type == null)
                return null;

            return Activator.CreateInstance(type) as IPlibaryRequest;
        }

         public abstract IPData IPLocation();
    }
}
