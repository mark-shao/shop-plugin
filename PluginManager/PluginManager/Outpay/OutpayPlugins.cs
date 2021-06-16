using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Hishop.Plugins
{
    public class OutpayPlugins : PluginContainer
    {

        private static readonly object LockHelper = new object();
        private static volatile OutpayPlugins instance = null;

        private OutpayPlugins()
            : base()
        {
        }

        public static OutpayPlugins Instance()
        {
            if (instance == null)
            {
                lock (LockHelper)
                {
                    if (instance == null)
                    {
                        instance = new OutpayPlugins();
                    }
                }
            }

            instance.VerifyIndex();
            return instance;
        }

        protected override string PluginLocalPath
        {
            get
            {
                if (HttpContext.Current != null)
                {

                    return HttpContext.Current.Request.MapPath("~/plugins/outpay");
                }
                else //非web程序引用
                {
                    string strPath = "plugins/outpay";
                    strPath = strPath.Replace("/", "\\");
                    if (strPath.StartsWith("\\"))
                    {
                        strPath = strPath.TrimStart('\\');
                    }
                    return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
                }

            }
        }

        protected override string PluginVirtualPath
        {
            get { return Utils.ApplicationPath + "/plugins/outpay"; }
        }

        protected override string IndexCacheKey
        {
            get { return "plugin-outpay-index"; }
        }

        protected override string TypeCacheKey
        {
            get { return "plugin-outpay-type"; }
        }

        public override PluginItemCollection GetPlugins()
        {
            return GetPlugins("OutpayRequest");
        }

        public override PluginItem GetPluginItem(string fullName)
        {
            return GetPluginItem("OutpayRequest", fullName);
        }

    }
}