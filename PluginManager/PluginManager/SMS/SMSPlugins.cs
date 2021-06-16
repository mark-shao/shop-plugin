using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace Hishop.Plugins
{
    public class SMSPlugins : PluginContainer
    {

        private static readonly object LockHelper = new object();
        private static volatile SMSPlugins instance = null;

        private SMSPlugins()
            : base()
        {
        }

        public static SMSPlugins Instance()
        {
            if (instance == null)
            {
                lock (LockHelper)
                {
                    if (instance == null)
                    {
                        instance = new SMSPlugins();
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

                    return HttpContext.Current.Request.MapPath("~/plugins/sms");
                }
                else //非web程序引用
                {
                    string strPath = "plugins/sms";
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
            get { return Utils.ApplicationPath + "/plugins/sms"; }
        }

        protected override string IndexCacheKey
        {
            get { return "plugin-sms-index"; }
        }

        protected override string TypeCacheKey
        {
            get { return "plugin-sms-type"; }
        }

        public override PluginItemCollection GetPlugins()
        {
            return GetPlugins("SMSSender");
        }

        public override PluginItem GetPluginItem(string fullName)
        {
            return GetPluginItem("SMSSender", fullName);
        }

    }
}