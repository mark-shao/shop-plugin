using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using System.Globalization;

namespace Hishop.Plugins
{
    public abstract class OpenIdService : ConfigablePlugin, IPlugin
    {

        public static OpenIdService CreateInstance(
            string name, string configXml, string returnUrl)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            object[] paramArray = new object[1];
            paramArray[0] = returnUrl;

            Type type =OpenIdPlugins.Instance().GetPlugin("OpenIdService", name);
            if (type == null)
                return null;

            OpenIdService instance = Activator.CreateInstance(type, paramArray) as OpenIdService;
            if (instance != null && !string.IsNullOrEmpty(configXml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(configXml);
                instance.InitConfig(doc.FirstChild);
            }

            return instance;
        }

        public static OpenIdService CreateInstance(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            Type type = OpenIdPlugins.Instance().GetPlugin("OpenIdService", name);
            if (type == null)
                return null;

            return Activator.CreateInstance(type) as OpenIdService;
        }

        public abstract void Post();

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// 转到信任登录页面(GET方式)
        /// </summary>
        /// <param name="url"></param>
        protected virtual void Redirect(string url)
        {
            HttpContext.Current.Response.Redirect(url, true);
        }

        private const string FormFormat = "<form id=\"openidform\" name=\"openidform\" action=\"{0}\" method=\"POST\">{1}</form>";
        private const string InputFormat = "<input type=\"hidden\" id=\"{0}\" name=\"{0}\" value=\"{1}\">";

        /// <summary>
        /// 创建字段
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="strValue">参数值</param>
        /// <returns></returns>
        protected virtual string CreateField(string name, string strValue)
        {
            return String.Format(CultureInfo.InvariantCulture, InputFormat, name, strValue);
        }

        /// <summary>
        /// 创建FORM
        /// </summary>
        /// <param name="content">字段集合</param>
        /// <param name="action">提交地址</param>
        /// <returns></returns>
        protected virtual string CreateForm(string content, string action)
        {
            content += "<input type=\"submit\" value=\"信任登录\" style=\"display:none;\">";
            return String.Format(CultureInfo.InvariantCulture, FormFormat, action, content);
        }

        /// <summary>
        /// 转到信任登录页面(POST方式)
        /// </summary>
        /// <param name="formContent"></param>
        protected virtual void Submit(string formContent)
        {
            string submitscr = formContent + "<script>document.forms['openidform'].submit();</script>";

            HttpContext.Current.Response.Write(submitscr);
            HttpContext.Current.Response.End();
        }

    }
}