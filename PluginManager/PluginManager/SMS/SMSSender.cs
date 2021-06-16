using System;
using System.Xml;

namespace Hishop.Plugins
{
    /// <summary>
    /// 手机短信发送接口
    /// </summary>
    public abstract class SMSSender : ConfigablePlugin, IPlugin
    {

        /// <summary>
        /// 短信接口-发送短信请求
        /// </summary>
        /// <param name="name">要创建的实例的完整类型名</param>
        /// <param name="configXml">用户设置的商家配置信息</param>
        /// <returns></returns>
        public static SMSSender CreateInstance(
            string name, string configXml)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            Type type = SMSPlugins.Instance().GetPlugin("SMSSender", name);
            if (type == null)
                return null;

            SMSSender instance = Activator.CreateInstance(type) as SMSSender;
            if (instance != null && !string.IsNullOrEmpty(configXml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(configXml);
                instance.InitConfig(doc.FirstChild);
            }

            return instance;
        }

        public static SMSSender CreateInstance(string name)
        {
            return CreateInstance(name, null);
        }

        /// <summary>
        /// 发送单条手机消息
        /// </summary>
        /// <param name="cellPhone">要发送的手机号码</param>
        /// <param name="message">信息内容</param>
        /// <param name="returnMsg">接口返回消息</param>
        /// <returns>是否发送成功</returns>
        public abstract bool Send(string cellPhone, string message, out string returnMsg, string speed = "0");

        /// <summary>
        /// 发送单条手机消息
        /// </summary>
        /// <param name="cellPhone">要发送的手机号码</param>
        /// <param name="message">信息内容</param>
        /// <param name="returnMsg">接口返回消息</param>
        /// <returns>是否发送成功</returns>
        public abstract bool Send(string cellPhone, string message, out string returnMsg);

        /// <summary>
        /// 群发手机消息
        /// </summary>
        /// <param name="phoneNumbers">要发送的手机号码列表</param>
        /// <param name="message">信息内容</param>
        /// <param name="returnMsg">接口返回消息</param>
        /// <returns>是否发送成功</returns>
        public abstract bool Send(string[] phoneNumbers, string message, out string returnMsg, string speed = "1");

        /// <summary>
        /// 群发手机消息
        /// </summary>
        /// <param name="phoneNumbers">要发送的手机号码列表</param>
        /// <param name="message">信息内容</param>
        /// <param name="returnMsg">接口返回消息</param>
        /// <returns>是否发送成功</returns>
        public abstract bool Send(string[] phoneNumbers, string message, out string returnMsg);
        /// <summary>
        /// 获取余额
        /// </summary>
        /// <returns></returns>
        public abstract int GetBalance(int spped);
        /// <summary>
        /// 获取余额
        /// </summary>
        /// <returns></returns>
        public abstract int GetBalance();
    }
}