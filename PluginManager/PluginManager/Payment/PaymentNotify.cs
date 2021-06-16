using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Threading;

namespace Hishop.Plugins
{

    public class FinishedEventArgs : EventArgs
    {
        public FinishedEventArgs(bool isMedTrade)
        {
            IsMedTrade = isMedTrade;
        }

        public bool IsMedTrade { get; private set; }
    }

    public abstract class PaymentNotify : IPlugin
    {

        public static PaymentNotify CreateInstance(string name, NameValueCollection parameters)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            object[] paramArray = new object[1];
            paramArray[0] = parameters;
            PaymentPlugins container = PaymentPlugins.Instance();

            Type requestType = container.GetPlugin("PaymentRequest", name);
            if (requestType == null)
                return null;

            Type notifyType = container.GetPluginWithNamespace("PaymentNotify", requestType.Namespace);
            if (notifyType == null)
                return null;
            return Activator.CreateInstance(notifyType, paramArray) as PaymentNotify;
        }

        /// <summary>
        /// 通知验证失败
        /// </summary>
        public event EventHandler NotifyVerifyFaild;

        /// <summary>
        /// 买家已付款
        /// </summary>
        public event EventHandler Payment;

        /// <summary>
        /// 交易完成
        /// </summary>
        public event EventHandler<FinishedEventArgs> Finished;

        protected virtual void OnFinished(bool isMedTrade)
        {
            if (Finished != null)
                Finished(this, new FinishedEventArgs(isMedTrade));
        }

        /// <summary>
        /// 通知验证失败
        /// </summary>
        protected virtual void OnNotifyVerifyFaild()
        {
            if (NotifyVerifyFaild != null)
                NotifyVerifyFaild(this, null);
        }

        /// <summary>
        /// 买家已付款到中介公司
        /// </summary>
        protected virtual void OnPayment()
        {
            if (Payment != null)
                Payment(this, null);
        }

        /// <summary>
        /// 验证通知
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="configXml"></param>
        /// <returns></returns>
        public abstract void VerifyNotify(int timeout, string configXml);

        /// <summary>
        /// 获取当前订单编号
        /// </summary>
        /// <returns></returns>
        public abstract string GetOrderId();

        /// <summary>
        /// 获取第三方支付公司的交易编号
        /// </summary>
        /// <returns></returns>
        public abstract string GetGatewayOrderId();

        /// <summary>
        /// 获取当前的支付金额
        /// </summary>
        /// <returns></returns>
        public abstract decimal GetOrderAmount();

        protected virtual string GetResponse(string url, int timeout)
        {
            string strResult;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
                myReq.Timeout = Timeout.Infinite;
                HttpWebResponse response = (HttpWebResponse)myReq.GetResponse();

                using (Stream myStream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(myStream, Encoding.Default))
                    {
                        StringBuilder strBuilder = new StringBuilder();

                        while (-1 != sr.Peek())
                        {
                            strBuilder.Append(sr.ReadLine());
                        }

                        strResult = strBuilder.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                strResult = "Error:" + ex.Message;
            }

            return strResult;
        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }

        /// <summary>
        /// 向支付接口返回处理结果
        /// </summary>
        /// <param name="context">当前HTTP请求上下文</param>
        /// <param name="success">返回是否成功</param>
        public abstract void WriteBack(HttpContext context, bool success);

        /// <summary>
        /// 获取随支付请求发送的第一条备注
        /// </summary>
        /// <returns></returns>
        public virtual string GetRemark1()
        {
            return string.Empty;
        }

        /// <summary>
        /// 获取随支付请求发送的第二条备注
        /// </summary>
        /// <returns></returns>
        public virtual string GetRemark2()
        {
            return string.Empty;
        }

        public string ReturnUrl { get; set; }

    }
}