using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace Hishop.Plugins
{
    public class RefundPlugins : PluginContainer
    {

        private static readonly object LockHelper = new object();
        private static volatile RefundPlugins instance = null;

        private RefundPlugins()
            : base()
        {
        }

        public static RefundPlugins Instance()
        {
            if (instance == null)
            {
                lock (LockHelper)
                {
                    if (instance == null)
                    {
                        instance = new RefundPlugins();
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

                    return HttpContext.Current.Request.MapPath("~/plugins/refund");
                }
                else //非web程序引用
                {
                    string strPath = "plugins/refund";
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
            get { return Utils.ApplicationPath + "/plugins/refund"; }
        }

        protected override string IndexCacheKey
        {
            get { return "plugin-refund-index"; }
        }

        protected override string TypeCacheKey
        {
            get { return "plugin-refund-type"; }
        }

        public override PluginItemCollection GetPlugins()
        {
            return GetPlugins("RefundRequest");
        }

        public override PluginItem GetPluginItem(string fullName)
        {
            return GetPluginItem("RefundRequest", fullName);
        }

    }
}
