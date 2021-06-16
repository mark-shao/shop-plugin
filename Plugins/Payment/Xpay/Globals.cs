using System.Text;
using System.Security.Cryptography;
using System.Globalization;

namespace Hishop.Plugins.Payment.Xpay
{
    internal static class Globals
    {
        internal static string GetXpayMD5(string s)
        {
            using (MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider())
            {
                return System.BitConverter.ToString(hashmd5.ComputeHash(Encoding.Default.GetBytes(s))).Replace("-", "").ToLower(CultureInfo.InvariantCulture);
            }
        }

    }
}