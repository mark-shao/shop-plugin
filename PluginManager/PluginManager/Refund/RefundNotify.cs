using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Hishop.Plugins
{
    public abstract class RefundNotify : IPlugin
    {

        public static RefundNotify CreateInstance(string name, NameValueCollection parameters)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            object[] paramArray = new object[1];
            paramArray[0] = parameters;
            RefundPlugins container = RefundPlugins.Instance();

            Type requestType = container.GetPlugin("RefundRequest", name);
            if (requestType == null)
                return null;

            Type notifyType = container.GetPluginWithNamespace("RefundNotify", requestType.Namespace);
            if (notifyType == null)
                return null;

            return Activator.CreateInstance(notifyType, paramArray) as RefundNotify;
        }

        /// <summary>
        /// 通知验证失败
        /// </summary>
        public event EventHandler NotifyVerifyFaild;

        /// <summary>
        /// 买退款成功
        /// </summary>
        public event EventHandler Refund;

        /// <summary>
        /// 通知验证失败
        /// </summary>
        protected virtual void OnNotifyVerifyFaild()
        {
            if (NotifyVerifyFaild != null)
                NotifyVerifyFaild(this, null);
        }

        /// <summary>
        /// 买退款成功
        /// </summary>
        protected virtual void OnRefund()
        {
            if (Refund != null)
                Refund(this, null);
        }

        /// <summary>
        /// 验证通知
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="configXml"></param>
        /// <returns></returns>
        public abstract bool VerifyNotify(int timeout, string configXml);

        /// <summary>
        /// 获取当前订单编号
        /// </summary>
        /// <returns></returns>
        public abstract string GetOrderId();

      
        protected virtual string GetResponse(string url, int timeout)
        {
            string strResult;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
                myReq.Timeout = timeout;
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

        public string ReturnUrl { get; set; }

    }
}
