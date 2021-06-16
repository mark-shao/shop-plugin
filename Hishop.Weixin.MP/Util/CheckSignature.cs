using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace Hishop.Weixin.MP.Util
{
    public class CheckSignature
    {
        public static readonly string Token = "weixin_test";//必须和公众平台的token设置一致，或在方法中指定

        public static bool Check(string signature, string timestamp, string nonce, string token)
        {
            token = token ?? Token;

            string[] array = new string[] { timestamp, nonce, token };

            Array.Sort(array);

            string val = String.Join("", array);

            val = FormsAuthentication.HashPasswordForStoringInConfigFile(val, "SHA1");

            return signature == val.ToLower();
        }


        //public static bool Check(string signature, string timestamp, string nonce, string token = null)
        //{
        //    return signature == GetSignature(timestamp, nonce, token);
        //}

        //public static string GetSignature(string timestamp, string nonce, string token = null)
        //{
        //    token = token ?? Token;
        //    var arr = new[] { CheckSignature.Token, timestamp, nonce }.OrderBy(z => z).ToArray();
        //    var arrString = string.Join("", arr);
        //    //var enText = FormsAuthentication.HashPasswordForStoringInConfigFile(arrString, "SHA1");//使用System.Web.Security程序集
        //    var sha1 = System.Security.Cryptography.SHA1.Create();
        //    var sha1Arr = sha1.ComputeHash(Encoding.UTF8.GetBytes(arrString));
        //    StringBuilder enText = new StringBuilder();
        //    foreach (var b in sha1Arr)
        //    {
        //        enText.AppendFormat("{0:x2}", b);
        //    }

        //    return enText.ToString();
        //}
    }
}
