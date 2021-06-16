using System;
using System.Globalization;
using System.Web;
using System.Xml;

namespace Hishop.Plugins
{
    public abstract class PaymentRequest : ConfigablePlugin, IPlugin
    {
        /// <summary>
        /// 支付接口-发送支付请求
        /// </summary>
        /// <param name="name">要创建的实例的完整类型名</param>
        /// <param name="configXml">用户设置的商家配置信息</param>
        /// <param name="orderId">订单号</param>
        /// <param name="amount">支付金额</param>
        /// <param name="subject">支付显示标题</param>
        /// <param name="body">支付显示内容</param>
        /// <param name="buyerEmail">买家电子邮件</param>
        /// <param name="date">订单日期</param>
        /// <param name="showUrl">商家展示地址</param>
        /// <param name="returnUrl">支付成功返回地址</param>
        /// <param name="notifyUrl">支付成功后台通知接收地址</param>
        /// <param name="attach">附加参数（自定义其用途）</param>
        /// <returns></returns>
        public static PaymentRequest CreateInstance(
            string name, string configXml, string orderId, decimal amount, 
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            object[] paramArray = new object[10];

            paramArray[0] = orderId;
            paramArray[1] = amount;
            paramArray[2] = subject;
            paramArray[3] = body;
            paramArray[4] = buyerEmail;
            paramArray[5] = date;
            paramArray[6] = showUrl;
            paramArray[7] = returnUrl;
            paramArray[8] = notifyUrl;
            paramArray[9] = attach;

            Type type = PaymentPlugins.Instance().GetPlugin("PaymentRequest", name);
            if (type == null)
                return null;

            PaymentRequest instance = Activator.CreateInstance(type, paramArray) as PaymentRequest;
            if (instance != null && !string.IsNullOrEmpty(configXml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(configXml);
                instance.InitConfig(doc.FirstChild);
            }

            return instance;
        }

        public static PaymentRequest CreateInstance(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            Type type = PaymentPlugins.Instance().GetPlugin("PaymentRequest", name);
            if (type == null)
                return null;

            return Activator.CreateInstance(type) as PaymentRequest;
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

        public virtual object SendRequest_Ret()
        {
            return "";
        }
        /// <summary>
        /// 发送支付请求
        /// </summary>
        public abstract void SendRequest();

        /// <summary>
        /// 获取支付接口是否为中介担保类接口
        /// </summary>
        public abstract bool IsMedTrade { get; }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="tradeno">支付公司方对应交易的编号</param>
        /// <param name="logisticsName">物流公司名称</param>
        /// <param name="invoiceno">发货单号</param>
        /// <param name="transportType">运输类型</param>
        public abstract void SendGoods(string tradeno, string logisticsName, string invoiceno, string transportType);

    }
}