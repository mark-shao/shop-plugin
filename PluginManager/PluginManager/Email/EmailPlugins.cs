using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace Hishop.Plugins
{
    public class EmailPlugins : PluginContainer
    {

        private static readonly object LockHelper = new object();
        private static volatile EmailPlugins instance = null;

        private EmailPlugins()
            : base()
        {
        }

        public static EmailPlugins Instance()
        {
            if (instance == null)
            {
                lock (LockHelper)
                {
                    if (instance == null)
                    {
                        instance = new EmailPlugins();
                    }
                }
            }

            instance.VerifyIndex();
            return instance;
        }

        protected override string PluginLocalPath
        {
            get {
                
                if (HttpContext.Current != null)
                {

                    return HttpContext.Current.Request.MapPath("~/plugins/email");
                }
                else //非web程序引用
                {
                    string strPath = "plugins/email";
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
            get { return Utils.ApplicationPath + "/plugins/email"; }
        }

        protected override string IndexCacheKey
        {
            get { return "plugin-email-index"; }
        }

        protected override string TypeCacheKey
        {
            get { return "plugin-email-type"; }
        }

        public override PluginItemCollection GetPlugins()
        {
            return GetPlugins("EmailSender");
        }

        public override PluginItem GetPluginItem(string fullName)
        {
            return GetPluginItem("EmailSender", fullName);
        }

    }
}