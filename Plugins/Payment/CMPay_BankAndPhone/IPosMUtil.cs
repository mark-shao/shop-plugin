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
    /// ���湤�߰�
    /// </summary>
    public class IPosMUtil
    {
        //private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(IPosMUtil));
        
        /// <summary>
        /// ��ȡ�ͻ���IP��ַ
        /// </summary>
        /// <returns>�ͻ���IP��ַ</returns>

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
        /// ��ȡϵͳʱ��
        /// </summary>
        /// <returns>ϵͳʱ��</returns>

        public  static String getTicks()
        {
            string ret = string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now);
            return ret;
        }


        /// <summary>
        /// ��ƽ̨����֧�������ԭʼ����
        /// </summary>
        /// <param name="url">�̻������ֻ�֧��ƽ̨��url</param>
        /// <param name="data">���͵��ֻ�֧��ƽ̨��ԭʼ����</param>
        /// <returns>�ֻ�֧��ƽ̨���صı���</returns>
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
        /// ��ƽ̨���ص����ݣ�&���ӵ��ַ�����ת��ΪHashtable����
        /// </summary>
        /// <param name="source">ƽ̨���ص�����</param>
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
        /// ����������ת�����ַ��������˵�ϵͳ����
        /// </summary>
        /// <param name="param">request�Ĳ���ֵ����</param>
        /// <returns>�������</returns>
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
        /// URL�ض���
        /// </summary>
        /// <param name="payUrl">�ֻ�֧��ƽ̨���ص�url��ַ</param>
        /// <returns>������װ���url��ַ</returns>
        public static String getRedirectUrl(String payUrl)
        {
            //log.Info("getRedirectUrl-REQ:payUrl=" + payUrl);
            Hashtable rdUrl = new Hashtable();
            String str1 = payUrl.Replace("<hi:$$>", "&");
            String str2 = str1.Replace("<hi:=>", "*");
            //log.Info("getRedirectUrl-REQ:payUrl=" + str2);
            if (payUrl != null)
            {
                // ������<hi:$$>�ָ�


                char[] temp1 = "&".ToCharArray();
                String[] items = str2.Split(temp1);

                if (items != null)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        String item = (String)items[i];
                        if (item != null)
                        {
                            // ÿ�����������ֺ�ֵ��<hi:=>�ָ�
                            char[] temp2 = "*".ToCharArray();
                            String[] element = item.Split(temp2);
                            if (element != null && element.Length == 2)
                            {
                                // ��ֵ����MAP��
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
