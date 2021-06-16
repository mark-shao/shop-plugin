using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Net.Security;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace Hishop.Plugins
{
    public abstract class OpenIdNotify : IPlugin
    {

        public static OpenIdNotify CreateInstance(string name, NameValueCollection parameters)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            object[] paramArray = new object[1];
            paramArray[0] = parameters;
            OpenIdPlugins container = OpenIdPlugins.Instance();

            Type serviceType = container.GetPlugin("OpenIdService", name);
            if (serviceType == null)
                return null;

            Type notifyType = container.GetPluginWithNamespace("OpenIdNotify", serviceType.Namespace);
            if (notifyType == null)
                return null;

            return Activator.CreateInstance(notifyType, paramArray) as OpenIdNotify;
        }

        public abstract void Verify(int timeout, string configXml);

        public event EventHandler<AuthenticatedEventArgs> Authenticated;

        public event EventHandler<FailedEventArgs> Failed;

        protected virtual void OnAuthenticated(string openId)
        {
            if (Authenticated != null)
                Authenticated(this, new AuthenticatedEventArgs(openId));
        }

        protected virtual void OnFailed(string message)
        {
            if (Failed != null)
                Failed(this, new FailedEventArgs(message));
        }
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }
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

    }

    public class FailedEventArgs : EventArgs
    {
        public FailedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }

    public class AuthenticatedEventArgs : EventArgs
    {
        public AuthenticatedEventArgs(string openId)
        {
            OpenId = openId;
        }

        public string OpenId { get; private set; }
    }

}