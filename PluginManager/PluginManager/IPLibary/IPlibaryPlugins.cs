using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Hishop.Plugins
{
    public class IPlibaryPlugins : PluginContainer
    {
        private static readonly object LockHelper = new object();
        private static volatile IPlibaryPlugins instance = null;

        private IPlibaryPlugins()
            : base()
        {
        }

        public static IPlibaryPlugins Instance()
        {
            if (instance == null)
            {
                lock (LockHelper)
                {
                    if (instance == null)
                    {
                        instance = new IPlibaryPlugins();
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

                    return HttpContext.Current.Request.MapPath("~/plugins/iplibary");
                }
                else //非web程序引用
                {
                    string strPath = "plugins/iplibary";
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
            get { return Utils.ApplicationPath + "/plugins/iplibary"; }
        }

        protected override string IndexCacheKey
        {
            get { return "plugin-iplibary-index"; }
        }

        protected override string TypeCacheKey
        {
            get { return "plugin-iplibary-type"; }
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