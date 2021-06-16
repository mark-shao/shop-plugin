using System;
using System.Collections.Generic;
using System.Text;

namespace Com.HisunCmpay
{
    /// <summary>
    /// 安全组件
    /// </summary>
    public class SignUtil
    {
        //private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(SignUtil));
        public static string toHex(byte[] input)
        {
            if (input == null)
            {
                return null;
            }
            StringBuilder output = new StringBuilder(input.Length * 2);
            for (int i = 0; i < input.Length; i++)
            {
                int current = input[i] & 0xff;
                if (current < 0x10)
                {
                    output.Append("0");
                }
                output.Append(current.ToString("x"));
            }
            return output.ToString();
        }
        
        /// <summary>
        /// 生成mac值
        /// 手机支付平台的mac值需要先调用HmacSign（String source），再调用HmacSign（string aValue, string aKey）
        /// </summary>
        /// <param name="aValue">生成mac值的原文</param>
        /// <param name="aKey">MD5算法的key值</param>
        /// <returns></returns>
       public static string HmacSign(string aValue, string aKey)
        {
            byte[] k_ipad = new byte[0x40];
            byte[] k_opad = new byte[0x40];
            byte[] keyb = Encoding.UTF8.GetBytes(aKey);
            byte[] Value = Encoding.UTF8.GetBytes(aValue);
            for (int i = keyb.Length; i < 0x40; i++)
            {
                k_ipad[i] = 0x36;
            }
            for (int i = keyb.Length; i < 0x40; i++)
            {
                k_opad[i] = 0x5c;
            }
            for (int i = 0; i < keyb.Length; i++)
            {
                k_ipad[i] = (byte)(keyb[i] ^ 0x36);
                k_opad[i] = (byte)(keyb[i] ^ 0x5c);
            }
            HmacMD5 md = new HmacMD5();
            md.update(k_ipad, (uint)k_ipad.Length);
            md.update(Value, (uint)Value.Length);
            byte[] dg = md.finalize();
            md.init();
            md.update(k_opad, (uint)k_opad.Length);
            md.update(dg, 0x10);
            String result = toHex(md.finalize());
            //log.Info("HmacSign-RSP:Hmac="+result);
            return result;
        }

        /// <summary>
        /// 生成mac值，先执行HmacSign（String aValue），可进一步防止mac值被破解
        /// </summary>
        /// <param name="aValue">生成mac值的原文</param>
        /// <returns></returns>
        public static string HmacSign(string aValue)
        {
            String key = "";
            String result = HmacSign(aValue, key);
            return result;
        }

        /// <summary>
        /// 验签MD5
        /// </summary>
        /// <param name="source">进行验签的原文</param>
        /// <param name="key">MD5算法的key值</param>
        /// <param name="hmac">mac值</param>
        /// <returns></returns>
        public static Boolean verifySign(String source, String key, String hmac)
        {
            String res = HmacSign(source);
            String newHmac = HmacSign(res, key);
            if (!String.IsNullOrEmpty(newHmac) && newHmac.Equals(hmac))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
