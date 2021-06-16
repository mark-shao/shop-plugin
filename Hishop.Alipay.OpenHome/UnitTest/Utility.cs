using Hishop.Alipay.OpenHome;

namespace UnitTest
{
    class Utility
    {
        static string url = "https://openapi.alipay.com/gateway.do";
        static string applid = "2014053000006249";
        static string priKey = @"D:\Web\mec\config\rsa_private_key.pem";
        static string aliPubKey = @"D:\Web\mec\config\alipay_pubKey.pem";
        static string pubKey = @"D:\Web\mec\config\rsa_public_key.pem";

        public static AlipayOHClient GeDefaultAlipayOHClient()
        {
            return new AlipayOHClient(url, applid, aliPubKey, priKey, pubKey);
        }
    }
}
