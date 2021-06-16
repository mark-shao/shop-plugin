using System;
using System.Globalization;
using System.Web;
using System.Xml;
using System.Collections.Generic;

namespace Hishop.Plugins
{
    public abstract class OutpayRequest : ConfigablePlugin, IPlugin
    {
        /// <summary>
        /// 放款接口-发送请求
        /// </summary>
        /// <param name="name">要创建的实例的完整类型名</param>
        /// <param name="configXml">用户设置的商家配置信息</param>
        /// <param name="outpayId">付款流水号</param>
        /// <param name="amount">付款金额</param>
        /// <param name="userAccount">付款帐号</param>
        /// <param name="realName">真实姓名</param>
        /// <param name="checkName">验证名称</param>
        /// <param name="openId">OpenId</param>
        /// <param name="userId">用户ID</param>
        /// <param name="date">日期</param>
        /// <param name="showUrl">付款成功后台显示地址</param>
        /// <param name="returnUrl">支付成功后台通知返回地址</param>
        /// <param name="notifyUrl">支付成功后台通知接收地址</param>
        /// <param name="attach">附加参数（自定义其用途）</param>
        /// <returns></returns>
        public static OutpayRequest CreateInstance(
            string name, string configXml, string[] outpayId, decimal[] amount,
            string[] userAccount, string[] realName, string[] openId, int[] userId,string[] desc, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            object[] paramArray = new object[12];

            paramArray[0] = outpayId;
            paramArray[1] = amount;
            paramArray[2] = userAccount;
            paramArray[3] = realName;
            paramArray[4] = openId;
            paramArray[5] = userId;
            paramArray[6] = desc;
            paramArray[7] = date;
            paramArray[8] = showUrl;
            paramArray[9] = returnUrl;
            paramArray[10] = notifyUrl;
            paramArray[11] = attach;

            Type type = OutpayPlugins.Instance().GetPlugin("OutpayRequest", name);
            if (type == null)
                return null;

            OutpayRequest instance = Activator.CreateInstance(type, paramArray) as OutpayRequest;
            if (instance != null && !string.IsNullOrEmpty(configXml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(configXml);
                instance.InitConfig(doc.FirstChild);
            }

            return instance;
        }

        public static OutpayRequest CreateInstance(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            Type type = PaymentPlugins.Instance().GetPlugin("OutpayRequest", name);
            if (type == null)
                return null;

            return Activator.CreateInstance(type) as OutpayRequest;
        }

        /// <summary>
        /// 支付请求(GET方式)
        /// </summary>
        /// <param name="url"></param>
        protected virtual void RedirectToGateway(string url)
        {
            HttpContext.Current.Response.Redirect(url, true);
        }

        private const string FormFormat = "<form id=\"payform\" name=\"payform\" action=\"{0}\" method=\"POST\">{1}</form>";
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
            content += "<input type=\"submit\" value=\"在线支付\" style=\"display:none;\">";
            return String.Format(CultureInfo.InvariantCulture, FormFormat, action, content);
        }

        /// <summary>
        /// 支付请求(POST方式)
        /// </summary>
        /// <param name="formContent"></param>
        protected virtual void SubmitPaymentForm(string formContent)
        {
            string submitscr = formContent + "<script>document.forms['payform'].submit();</script>";

            HttpContext.Current.Response.Write(submitscr);
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 发送支付请求
        /// </summary>
        public abstract void SendRequest();

        public abstract IList<IDictionary<string, string>> SendRequestByResult();
    }
}