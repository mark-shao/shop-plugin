using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace Hishop.Plugins
{
    public class PaymentPlugins : PluginContainer
    {

        private static readonly object LockHelper = new object();
        private static volatile PaymentPlugins instance = null;

        private PaymentPlugins()
            : base()
        {
        }

        public static PaymentPlugins Instance()
        {
            if (instance == null)
            {
                lock (LockHelper)
                {
                    if (instance == null)
                    {
                        instance = new PaymentPlugins();
                    }
                }
            }

            instance.VerifyIndex();
            return instance;
        }

        protected override string PluginLocalPath
        {

            get { return HttpContext.Current.Request.MapPath("~/plugins/payment"); }
        }

        protected override string PluginVirtualPath
        {
            get { return Utils.ApplicationPath + "/plugins/payment"; }
        }

        protected override string IndexCacheKey
        {
            get { return "plugin-payment-index"; }
        }

        protected override string TypeCacheKey
        {
            get { return "plugin-payment-type"; }
        }

        public override PluginItemCollection GetPlugins()
        {
            return GetPlugins("PaymentRequest");
        }

        public override PluginItem GetPluginItem(string fullName)
        {
            return GetPluginItem("PaymentRequest", fullName);
        }

    }
}