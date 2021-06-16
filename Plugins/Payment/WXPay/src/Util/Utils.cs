using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Hishop.Weixin.Pay.Notify;
using System.Data;
using System.Web;
using System.Reflection;

namespace Hishop.Weixin.Pay.Util
{
    internal class Utils
    {
        #region 微信时间转换

        //Unix起始时间
        private static readonly DateTime BaseTime = new DateTime(1970, 1, 1);

        public static DateTime ConvertSecondsToDateTime(long seconds)
        {
            return BaseTime.AddSeconds(seconds).AddHours(8);
        }

        public static long GetCurrentTimeSeconds()
        {
            return (long)(DateTime.UtcNow - BaseTime).TotalSeconds;
        }

        public static long GetTimeSeconds(DateTime dt)
        {
            return (long)(dt.ToUniversalTime() - BaseTime).TotalSeconds;
        }
        #endregion

        public static string CreateNoncestr()
        {
            return DateTime.Now.ToString("fffffff");
        }

        #region 解析通知（将xml解析成通知对象）

        private static Dictionary<string, XmlSerializer> parsers = new Dictionary<string, XmlSerializer>();
        public static T GetNotifyObject<T>(string xml) where T : NotifyObject
        {
            Type type = typeof(T);
            string key = type.FullName;
            //DataTable dt = new DataTable();
            //dt.Columns.Add(new DataColumn("key"));
            //dt.Columns.Add(new DataColumn("keyvalue"));
            //DataRow dr = dt.NewRow();


            XmlSerializer serializer = null;
            bool incl = parsers.TryGetValue(key, out serializer);

            if (!incl || serializer == null)
            {
                XmlAttributes rootAttrs = new XmlAttributes();
                rootAttrs.XmlRoot = new XmlRootAttribute("xml");

                XmlAttributeOverrides attrOvrs = new XmlAttributeOverrides();
                attrOvrs.Add(type, rootAttrs);

                serializer = new XmlSerializer(type, attrOvrs);
                parsers[key] = serializer;


            }

            object obj = null;

            try
            {
                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    obj = serializer.Deserialize(stream);
                    //dr["key"] = (obj == null ? "null" : obj.GetType().ToString());
                    //dr["keyvalue"] = xml;
                    //dt.Rows.Add(dr);
                    //dt.TableName = "NotifyObject";
                    //dt.WriteXml(HttpContext.Current.Request.MapPath("/NotifyObject.xml"));
                }
            }
            catch (Exception ex)
            {
                //dr["key"] = ex.Message;
                //dr["keyvalue"] = ex.StackTrace;
                //dt.Rows.Add(dr);
                //dt.TableName = "NotifyObject";
                //dt.WriteXml(HttpContext.Current.Request.MapPath("/NotifyObject.xml"));
                return null;
            }

            return obj as T;
        }
        #endregion

        public static string GetToken(string appid, string secret)
        {
            string url = String.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, secret);

            string val = new WebUtils().DoGet(url);

            if (String.IsNullOrEmpty(val))
                return String.Empty;

            Dictionary<string, string> dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(val);

            if (dict != null && dict.ContainsKey("access_token"))
                return dict["access_token"];

            return String.Empty;
        }

        /// <summary>
        /// 将实体类转为字典
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static PayDictionary GetPayDictionary(object obj)
        {
            Dictionary<String, Object> map = new Dictionary<string, object>();

            Type t = obj.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();

                if (mi != null && mi.IsPublic && p.CanRead)
                {
                    map.Add(p.Name, mi.Invoke(obj, new Object[] { }));
                }
            }
            PayDictionary stringMap = new PayDictionary();
            foreach (KeyValuePair<string, object> kv in map)
            {
                stringMap.Add(kv.Key, kv.Value);
            }
            return stringMap;
        }

    }
}
