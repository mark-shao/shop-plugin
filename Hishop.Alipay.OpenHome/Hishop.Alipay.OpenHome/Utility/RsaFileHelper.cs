using System;

namespace Hishop.Alipay.OpenHome.Utility
{
    class RsaFileHelper
    {

        /// <summary>
        /// 获取密钥内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isPubKey"></param>
        /// <returns></returns>
        public static string GetRSAKeyContent(string path, bool isPubKey)
        {
            string key = string.Empty;
            string type = isPubKey ? "PUBLIC KEY" : "RSA PRIVATE KEY";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
            {
                key = sr.ReadToEnd();
                sr.Close();
            }
            string header = String.Format("-----BEGIN {0}-----\\n", type);
            string footer = String.Format("-----END {0}-----", type);
            int start = key.IndexOf(header) + header.Length;
            int end = key.IndexOf(footer, start);
            key = key.Substring(start, (end - start));
            return key.Replace("\r", "").Replace("\n", "");
        }
    }
}
