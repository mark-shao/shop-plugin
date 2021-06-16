using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
//using log4net;
using System.Web;

namespace Com.HisunCmpay
{
    /// <summary>
    /// 常规工具包
    /// </summary>
    public class IPosMUtil
    {
        //private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(IPosMUtil));
        
        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns>客户端IP地址</returns>

        public static String getIpAddress()
        {
            string ip;
            try
            {
                HttpRequest request = HttpContext.Current.Request;

                if (request.ServerVariables["HTTP_VIA"] != null)   //using proxy
                {
                    ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
                }
                else
                {
                    ip = request.UserHostAddress;
                }
                if (String.IsNullOrEmpty(ip))
                {
                    ip = "unknown";
                }
                return ip;
            }
            catch (Exception e)
            {
                //log.Error("GetIpAddress failed: " + e.Message);
                return "unknown";
            }

        }


        /// <summary>
        /// 获取系统时间
        /// </summary>
        /// <returns>系统时间</returns>

        public  static String getTicks()
        {
            string ret = string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now);
            return ret;
        }


        /// <summary>
        /// 向平台发送支付请求的原始报文
        /// </summary>
        /// <param name="url">商户请求到手机支付平台的url</param>
        /// <param name="data">发送到手机支付平台的原始报文</param>
        /// <returns>手机支付平台返回的报文</returns>
        public static String httpRequest(String url, String data)
        {
            //log.Info("reqData: [" + data + "]");
            String recv = "";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
          
            try
            {
                using (StreamWriter myWriter = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.Default))
                {
                    myWriter.Write(data);
                }
                
                using (WebResponse wr = req.GetResponse())
                {
                    Stream ReceiveStream = wr.GetResponseStream();
                    using (StreamReader myStreamReader = new StreamReader(ReceiveStream, System.Text.Encoding.Default))
                    {
                        recv = (String)myStreamReader.ReadToEnd();
                    }

                    //log.Info("rspData: [" + recv + "]");
                    return recv;

                }
            }
            catch (Exception e)
            {
               //log.Debug(e.Message);
                return e.Message;
            }
        }

        /// <summary>
        /// 将平台返回的数据（&连接的字符串）转换为Hashtable类型
        /// </summary>
        /// <param name="source">平台返回的数据</param>
        /// <returns></returns>
        public static Hashtable parseStringToMap(String source)
        {
            //log.Debug("parseStringToMap-REQ: ^START^");
            Hashtable hashtable = new Hashtable();
            String[] temps = source.Split('&');
            String stemp;
            String[] stemps = new String[2];
            String valuetmp = "";
            try
            {
                for (int i = 0; i < temps.Length; i++)
                {
                    stemp = temps[i];
                    stemps = stemp.Split('=');

                    if (("amtItem".Equals(stemps[0])) || ("payUrl".Equals(stemps[0])))
                    {
                        valuetmp = "";
                        for (int j = 1; j < stemps.Length; j++)
                        {
                            if (j < stemps.Length - 1)
                                valuetmp = valuetmp + stemps[j] + "=";
                            else
                                valuetmp = valuetmp + stemps[j];
                        }
                    }
                    else
                    {
                        valuetmp = stemps[1];
                    }
                    hashtable.Add(stemps[0], valuetmp);
                }
            }
            catch (Exception el)
            {
                //log.Error("parseStringToMap-RSP:errMsg = " + el.Message);
                hashtable.Add("message", source);
                return hashtable;
            }

            return hashtable;
        }

        /// <summary>
        /// 将参数数组转换成字符串，过滤掉系统参数
        /// </summary>
        /// <param name="param">request的参数值对象</param>
        /// <returns>结果报文</returns>
        public static String keyValueToString(NameValueCollection param)
        {
            String sret = "";
            string key, value;
            for (int i = 0; i < param.Count; i++)
            {
                key = param.Keys[i];
                value = param[key];
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    if (sret.Length <= 0)
                    {
                        sret += key + "=" + value;
                    }
                    else
                    {
                        sret += "&" + key + "=" + value;
                    }
                    
                }

            }
            return sret;
        }
       

        /// <summary>
        /// URL重定向
        /// </summary>
        /// <param name="payUrl">手机支付平台返回的url地址</param>
        /// <returns>重新组装后的url地址</returns>
        public static String getRedirectUrl(String payUrl)
        {
            //log.Info("getRedirectUrl-REQ:payUrl=" + payUrl);
            Hashtable rdUrl = new Hashtable();
            String str1 = payUrl.Replace("<hi:$$>", "&");
            String str2 = str1.Replace("<hi:=>", "*");
            //log.Info("getRedirectUrl-REQ:payUrl=" + str2);
            if (payUrl != null)
            {
                // 首先用<hi:$$>分割


                char[] temp1 = "&".ToCharArray();
                String[] items = str2.Split(temp1);

                if (items != null)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        String item = (String)items[i];
                        if (item != null)
                        {
                            // 每个参数的名字和值用<hi:=>分割
                            char[] temp2 = "*".ToCharArray();
                            String[] element = item.Split(temp2);
                            if (element != null && element.Length == 2)
                            {
                                // 把值放在MAP中
                                rdUrl.Add(element[0], element[1]);
                            }
                        }
                    }
                }
            }
            return (String)rdUrl["url"] + "?" + "sessionId=" + (String)rdUrl["sessionId"];
        }

    }
}
