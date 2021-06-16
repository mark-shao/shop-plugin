using System;
using System.IO;
using System.Web;

namespace Hishop.Plugins.Payment.YeePay
{
	/// <summary>
	/// Buy 的摘要说明。
	/// </summary>
	internal static class Buy
	{
        //private static string nodeAuthorizationURL = @"https://www.yeepay.com/app-merchant-proxy/node";

        //易宝公钥，请勿修改
        public static string YopPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA6p0XWjscY+gsyqKRhw9MeLsEmhFdBRhT2emOck/F1Omw38ZWhJxh9kDfs5HzFJMrVozgU+SJFDONxs8UB0wMILKRmqfLcfClG9MyCNuJkkfm0HFQv1hRGdOvZPXj3Bckuwa7FrEXBRYUhK7vJ40afumspthmse6bs6mZxNn/mALZ2X07uznOrrc2rk41Y2HftduxZw6T4EmtWuN2x4CZ8gwSyPAW5ZzZJLQ6tZDojBK4GZTAGhnn3bg5bBsBlw2+FLkCQBuDsJVsFPiGh/b6K/+zGTvWyUcu+LUj2MejYQELDO3i2vQXVDk7lVi2/TcUYefvIcssnzsfCfjaorxsuwIDAQAB";

  //      // 创建在线支付URL
  //      internal static string  CreateUrl(	
		//									string merchantId,
		//									string keyValue,//私钥
		//									string orderId,
		//									string amount,
		//									string cur,

		//									string productId,
		//									string merchantCallbackURL,
		//									string addressFlag,

		//									string sMctProperties,
		//									string frpId)
		//{
		//	string messageType	= "Buy";
		//	string needResponse = "1";
		//	Digest digest		= new Digest();

		//	string sbOld="";
		//	sbOld = sbOld + messageType;
		//	sbOld = sbOld + merchantId;
		//	sbOld = sbOld + orderId;
		//	sbOld = sbOld + amount;
	
		//	sbOld = sbOld + cur;		
		//	sbOld = sbOld + productId;
		//	sbOld = sbOld + merchantCallbackURL;
			
		//	sbOld = sbOld + addressFlag;
		//	sbOld = sbOld + sMctProperties;
		//	sbOld = sbOld + frpId;
		//	sbOld = sbOld + needResponse;

		//	string sNewString=digest.HmacSign(sbOld,keyValue);

		//	string html = "";

		//	html += nodeAuthorizationURL;
		//	html += "?p0_Cmd=" + messageType;
		//	html += "&p1_MerId=" + merchantId;
		//	html += "&p2_Order=" + orderId;
		//	html += "&p3_Amt=" + amount;

		//	html += "&p4_Cur=" + cur;
		//	html += "&p5_Pid=" + productId;
		//	html += "&p8_Url=" + System.Web.HttpUtility.UrlEncode(merchantCallbackURL,System.Text.Encoding.GetEncoding("gb2312"));

		//	html += "&p9_SAF=" + addressFlag;
		//	html += "&pa_MP=" + sMctProperties;
		//	html += "&pd_FrpId=" + frpId;
		//	html += "&pr_NeedResponse=" + needResponse;

		//	html += "&hmac=" + sNewString;
			
		//	return html;
		//}

		// 返回url检查md5
		internal static bool VerifyCallback(	string merchantId,
											string keyValue,
											string sCmd,
											string sErrorCode,
											string sTrxId,

											string amount,
											string cur,
											string productId,
											string orderId,
											string userId,

											string mp,
											string bType,
											string hmac
											)
		{
			Digest digest = new Digest();

			string sbOld="";

			sbOld = sbOld + merchantId;
			sbOld = sbOld + sCmd;
			sbOld = sbOld + sErrorCode;
			sbOld = sbOld + sTrxId;
			sbOld = sbOld + amount;
	
			sbOld = sbOld + cur;		
			sbOld = sbOld + productId;
			sbOld = sbOld + orderId;
			sbOld = sbOld + userId;
			sbOld = sbOld + mp;

			sbOld = sbOld + bType;

			string sNewString=digest.HmacSign(sbOld,keyValue);

			if(hmac==sNewString)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        internal static string GetQueryString(string strArgName, string strUrl)
        {
            //处理Request，解决乱码！！！
            strUrl = strUrl.Replace("?", "&");
            string strArgValue = "";
            string[] strList = strUrl.Split('&');
            int intCount = strList.Length;
            for (int i = 0; i < intCount; i++)
            {
                int intPos = strList[i].ToString().IndexOf("=");
                if (intPos == -1) continue;
                string strListArgName = strList[i].ToString().Substring(0, intPos);
                if (strListArgName == strArgName)
                {
                    strArgValue = strList[i].ToString().Substring(intPos + 1);
                }
            }
            strArgValue = System.Web.HttpUtility.UrlDecode(strArgValue, System.Text.Encoding.GetEncoding("gb2312"));
            return strArgValue;
        }



        #region 写日志处理
        /// <summary>
        /// 追加日志
        /// </summary>
        /// <param name="msg"></param>
        public static void AppendLog(string msg)
        {
            using (StreamWriter fs = File.AppendText(GetLogPath + "yee" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt"))
            {
                fs.WriteLine("时间：" + DateTime.Now.ToString());
                fs.WriteLine("Hishopmsg:" + msg);
                fs.WriteLine("");
            }
        }
        private static string GetLogPath
        {
            get
            {
                try
                {
                    string path = "";
                    if (HttpContext.Current != null)
                    {
                        path = HttpContext.Current.Server.MapPath("/log/");
                    }
                    else
                    {
                        string strPath = "/log/";
                        strPath = strPath.Replace("/", "\\");
                        if (strPath.StartsWith("\\"))
                        {
                            strPath = strPath.TrimStart('\\');
                        }
                        path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
                    }
                    if (!Directory.Exists(path))//如果日志目录不存在就创建
                    {
                        Directory.CreateDirectory(path);
                    }
                    return path;
                }
                catch (Exception ex)
                {
                    // System.IO.File.WriteAllText(@"D:\error1.txt", ex.Message + "---" + ex.Source + "---" + ex.StackTrace);
                    return "";
                }

            }
        }

        /// <summary>
        /// 写私钥
        /// </summary>
        /// <param name="msg"></param>
        public static void SetPrivateKeyTxt(string privateKey)
        {
            using (StreamWriter fs = new StreamWriter(GetPrivateKeyPath + "yeeprivateKey.txt"))
            {
                fs.Write(privateKey);
                fs.Close();
            }
        }
        /// <summary>
        /// 写私钥
        /// </summary>
        /// <param name="msg"></param>
        public static string GetPrivateKeyTxt()
        {
            string privateKey = string.Empty;
            using (StreamReader fs = new StreamReader(GetPrivateKeyPath + "yeeprivateKey.txt"))
            {
                privateKey = fs.ReadToEnd();
                fs.Close();
            }
            return privateKey;
        }
        /// <summary>
        /// 私钥存放位置
        /// </summary>
        private static string GetPrivateKeyPath
        {
            get
            {
                try
                {
                    string path = "";
                    if (HttpContext.Current != null)
                    {
                        path = HttpContext.Current.Server.MapPath("/plugins/payment/");
                    }
                    else
                    {
                        string strPath = "/plugins/payment/";
                        strPath = strPath.Replace("/", "\\");
                        if (strPath.StartsWith("\\"))
                        {
                            strPath = strPath.TrimStart('\\');
                        }
                        path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
                    }
                    if (!Directory.Exists(path))//如果日志目录不存在就创建
                    {
                        Directory.CreateDirectory(path);
                    }
                    return path;
                }
                catch (Exception ex)
                {
                    return "";
                }

            }
        }
        #endregion
    }
}
