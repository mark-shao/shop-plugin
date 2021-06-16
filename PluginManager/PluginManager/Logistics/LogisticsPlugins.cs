using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace Hishop.Plugins
{
    public class LogisticsPlugins : PluginContainer
    {

        private static readonly object LockHelper = new object();
        private static volatile LogisticsPlugins instance = null;

        private LogisticsPlugins()
            : base()
        {
        }

        public static LogisticsPlugins Instance()
        {
            if (instance == null)
            {
                lock (LockHelper)
                {
                    if (instance == null)
                    {
                        instance = new LogisticsPlugins();
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

                    return HttpContext.Current.Request.MapPath("~/plugins/logistics");
                }
                else //非web程序引用
                {
                    string strPath = "plugins/logistics";
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
            get { return Utils.ApplicationPath + "/plugins/logistics"; }
        }

        protected override string IndexCacheKey
        {
            get { return "plugin-logistics-index"; }
        }

        protected override string TypeCacheKey
        {
            get { return "plugin-logistics-type"; }
        }

        public override PluginItemCollection GetPlugins()
        {
            throw new NotImplementedException();
        }

        public override PluginItem GetPluginItem(string fullName)
        {
            return null;
        }

    }
}