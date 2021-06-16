using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace Hishop.Plugins
{
    public class OpenIdPlugins : PluginContainer
    {

        private static readonly object LockHelper = new object();
        private static volatile OpenIdPlugins instance = null;

        private OpenIdPlugins()
            : base()
        {
        }

        public static OpenIdPlugins Instance()
        {
            if (instance == null)
            {
                lock (LockHelper)
                {
                    if (instance == null)
                    {
                        instance = new OpenIdPlugins();
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

                    return HttpContext.Current.Request.MapPath("~/plugins/openid");
                }
                else //非web程序引用
                {
                    string strPath = "plugins/openid";
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
            get { return Utils.ApplicationPath + "/plugins/openid"; }
        }

        protected override string IndexCacheKey
        {
            get { return "plugin-openid-index"; }
        }

        protected override string TypeCacheKey
        {
            get { return "plugin-openid-type"; }
        }

        public override PluginItemCollection GetPlugins()
        {
            return GetPlugins("OpenIdService");
        }

        public override PluginItem GetPluginItem(string fullName)
        {
            return GetPluginItem("OpenIdService", fullName);
        }

    }
}