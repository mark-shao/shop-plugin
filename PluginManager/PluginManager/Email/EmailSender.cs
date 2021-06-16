using System;
using System.Net.Mail;
using System.Text;
using System.Xml;

namespace Hishop.Plugins
{
    public abstract class EmailSender : ConfigablePlugin, IPlugin
    {

        /// <summary>
        /// 短信接口-发送短信请求
        /// </summary>
        /// <param name="name">要创建的实例的完整类型名</param>
        /// <param name="configXml">用户设置的商家配置信息</param>
        /// <returns></returns>
        public static EmailSender CreateInstance(
            string name, string configXml)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            Type type = EmailPlugins.Instance().GetPlugin("EmailSender", name);
            if (type == null)
                return null;

            EmailSender instance = Activator.CreateInstance(type) as EmailSender;
            if (instance != null && !string.IsNullOrEmpty(configXml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(configXml);
                instance.InitConfig(doc.FirstChild);
            }

            return instance;
        }

        public static EmailSender CreateInstance(string name)
        {
            return CreateInstance(name, null);
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        public abstract bool Send(MailMessage mail, Encoding emailEncoding);

    }
}