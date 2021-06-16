using System.Web.Configuration;
using System.Configuration;
using System.IO;

namespace Hishop.Plugins.Payment.BankUnionGateWay.sdk
{

    public class SDKConfig
    {
        public static string signCertPath;
        public static string validateCertDir = System.Web.HttpContext.Current.Request.MapPath("~/config");
        public static string publicCertPath = System.Web.HttpContext.Current.Request.MapPath("~/config/publickey.cer");  //功能：加密公钥证书路径
        //public static string publicCertPath = System.Web.HttpContext.Current.Request.MapPath("~/config/verify_sign_acp.cer"); 
        public static string MerId = ""; //"777290058110605";
        public static string SignCertPwd = "";//功能：读取配置文件获取签名证书密码
    }
}