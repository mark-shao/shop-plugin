using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;
using Hishop.Plugins.Refund;

namespace Hishop.Plugins
{
    public abstract class RefundRequest : ConfigablePlugin, IPlugin
    {
        /// <summary>
        /// 支付接口-发送支付请求
        /// </summary>
        /// <param name="name">要创建的实例的完整类型名</param>
        /// <param name="configXml">用户设置的商家配置信息</param>
        /// <param name="orderId">订单号</param>
        /// <param name="refundOrderId">退款订单号</param>
        /// <param name="amount">支付金额</param>
        /// <param name="refundaAmount">退款金额</param>
        /// <param name="body">退款说明</param>
        /// <param name="date">退款日期</param>
        /// <param name="returnUrl">退款成功返回地址</param>
        /// <param name="notifyUrl">退款成功后台通知接收地址</param>
        /// <param name="attach">附加参数（自定义其用途）</param>
        /// <returns></returns>
        public static RefundRequest CreateInstance(
            string name, string configXml, string[] orderId, string refundOrderId, decimal[] amount,
            decimal[] refundaAmount, string[] body, string buyerEmail, DateTime date, string returnUrl, string notifyUrl, string attach)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            object[] paramArray = new object[10];

            paramArray[0] = orderId;
            paramArray[1] = refundOrderId;
            paramArray[2] = amount;
            paramArray[3] = refundaAmount;
            paramArray[4] = body;
            paramArray[5] = buyerEmail;
            paramArray[6] = date;
            paramArray[7] = returnUrl;
            paramArray[8] = notifyUrl;
            paramArray[9] = attach;

            Type type = RefundPlugins.Instance().GetPlugin("RefundRequest", name);
            if (type == null)
                return null;

            RefundRequest instance = Activator.CreateInstance(type, paramArray) as RefundRequest;
            if (instance != null && !string.IsNullOrEmpty(configXml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(configXml);
                instance.InitConfig(doc.FirstChild);
            }

            return instance;
        }

        public static RefundRequest CreateInstance(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            Type type = RefundPlugins.Instance().GetPlugin("RefundRequest", name);
            if (type == null)
                return null;

            return Activator.CreateInstance(type) as RefundRequest;
        }

        /// <summary>
        /// 支付请求(GET方式)
        /// </summary>
        /// <param name="url"></param>
        protected virtual void RedirectToGateway(string url)
        {
            HttpContext.Current.Response.Redirect(url, true);
        }

        private const string FormFormat = "<form id=\"refundform\" name=\"refundform\" action=\"{0}\" method=\"POST\">{1}</form>";
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
            content += "<input type=\"submit\" value=\"退款请求\" style=\"display:none;\">";
            return String.Format(CultureInfo.InvariantCulture, FormFormat, action, content);
        }

        /// <summary>
        /// 支付请求(POST方式)
        /// </summary>
        /// <param name="formContent"></param>
        protected virtual void SubmitRefundForm(string formContent)
        {
            string submitscr = formContent + "<script>document.forms['refundform'].submit();</script>";

            HttpContext.Current.Response.Write(submitscr);
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 发送支付请求
        /// </summary>
        public abstract void SendRequest();

        public virtual ResponseResult SendRequest_Ret() { return new ResponseResult(); }
        /// <summary>
        /// 获取支付接口是否为中介担保类接口
        /// </summary>
        public abstract bool IsMedTrade { get; }



    }
}
