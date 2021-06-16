using System;
using System.Text;
using System.Security.Cryptography;

namespace Hishop.Plugins.Payment.TenpayAssure
{
    internal static class Globals
    {
        /// <summary>
        /// 获取大写的MD5签名结果
        /// </summary>
        /// <param name="encypStr"></param>
        /// <returns></returns>
        internal static string GetMD5(string encypStr)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            // inputBye = Encoding.GetEncoding("gb2312").GetBytes(encypStr);
            inputBye = Encoding.UTF8.GetBytes(encypStr);

            outputBye = m5.ComputeHash(inputBye);

            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }

        /// <summary>
        /// 添加参数,惹参数值不为空串,则添加。反之,不添加。
        /// </summary>
        internal static StringBuilder AddParameter(StringBuilder buf, String parameterName, String parameterValue)
        {
            if (null == parameterValue || "".Equals(parameterValue))
            {
                return buf;
            }

            if ("".Equals(buf.ToString()))
            {
                buf.Append(parameterName);
                buf.Append("=");
                buf.Append(parameterValue);
            }
            else
            {
                buf.Append("&");
                buf.Append(parameterName);
                buf.Append("=");
                buf.Append(parameterValue);
            }
            return buf;
        }

    }
}