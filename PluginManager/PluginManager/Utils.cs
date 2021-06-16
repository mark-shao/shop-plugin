using System;
using System.IO;
using System.Reflection;
using System.Web;

namespace Hishop.Plugins
{
    public static class Utils
    {

        public static string GetResourceContent(string sFileName)
        {
            try
            {
                string fileContent = "";
                using (Stream oStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(sFileName))
                {
                    using (StreamReader oReader = new StreamReader(oStream))
                    {
                        fileContent = oReader.ReadToEnd();
                    }
                }

                return fileContent;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not read resource \"" + sFileName + "\": " + ex);
            }
        }

        public static string ApplicationPath
        {
            get
            {
                string applicationPath = "/";

                if (HttpContext.Current != null)
                    applicationPath = HttpContext.Current.Request.ApplicationPath;

                if (applicationPath == "/")
                {
                    return string.Empty;
                }
                else
                {
                    return applicationPath;
                }
            }
        }

    }
}