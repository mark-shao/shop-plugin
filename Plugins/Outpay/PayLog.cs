using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Web;

namespace Hishop.Plugins.Outpay
{
    public enum LogType
    {
        Outpay = 0,
        Weixin = 1,
        Alipay = 2,
        WeixinNotify = 3,
        AlipayNotify = 4
    }
    public class PayLog
    {

        public static void writeLog_Collection(NameValueCollection param, string sign, string url, string msg, LogType logtype)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.TableName = "log";
                dt.Columns.Add(new DataColumn("HishopOperTime"));
                if (param != null)
                {
                    foreach (string s in param.AllKeys)
                    {
                        dt.Columns.Add(new DataColumn(s));
                    }
                }
                dt.Columns.Add(new DataColumn("HishopMsg"));
                dt.Columns.Add(new DataColumn("HishopSign"));
                dt.Columns.Add(new DataColumn("HishopUrl"));
                DataRow dr = dt.NewRow();
                dr["HishopOperTime"] = DateTime.Now;
                if (param != null)
                {
                    foreach (string s in param.AllKeys)
                    {
                        dr[s] = param[s];
                    }
                }
                dr["HishopMsg"] = msg;
                dr["HishopSign"] = sign;
                dr["HishopUrl"] = url;
                dt.Rows.Add(dr);

                dt.WriteXml(GetLogPath + "outpay_" + logtype.ToString("G") + ".xml");
            }
            catch (Exception ex)
            {
              //  AppendLog(null, "", "", ex.Message, LogType.Outpay);
            };
        }
        public static void writeLog(IDictionary<string, string> param, string sign, string url, string msg, LogType logtype)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.TableName = "log";
                dt.Columns.Add(new DataColumn("HishopOperTime"));
                if (param != null)
                {
                    foreach (KeyValuePair<string, string> key in param)
                    {
                        dt.Columns.Add(new DataColumn(key.Key));
                    }
                }
                dt.Columns.Add(new DataColumn("HishopMsg"));
                dt.Columns.Add(new DataColumn("HishopSign"));
                dt.Columns.Add(new DataColumn("HishopUrl"));
                DataRow dr = dt.NewRow();
                dr["HishopOperTime"] = DateTime.Now;
                if (param != null)
                {
                    foreach (KeyValuePair<string, string> key in param)
                    {
                        dr[key.Key] = key.Value;
                    }
                }
                dr["HishopMsg"] = msg;
                dr["HishopSign"] = sign;
                dr["HishopUrl"] = url;
                dt.Rows.Add(dr);

                dt.WriteXml(GetLogPath + "outpay_" + logtype.ToString("G") + ".xml");
            }
            catch (Exception ex)
            {
              //  System.IO.File.WriteAllText(@"D:\error.txt", ex.Message + "---" + ex.Source + "---" + ex.StackTrace);
            };
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
        /// 检查文件大小，如果文件大小超过500KB则删除日志 
        /// </summary>
        /// <param name="?"></param>
        public static void CheckFileSize(LogType logtype)
        {
            string fileUrl = GetLogPath + "outpay_" + logtype.ToString() + ".txt";
            if (File.Exists(fileUrl))
            {
                FileStream fs = File.Open(fileUrl, FileMode.Open);
                if (fs.Length > 10 * 1024)
                {

                    File.Delete(fileUrl);
                }
                fs.Close();
            }
        }
        /// <summary>
        /// 追加日志
        /// </summary>
        /// <param name="param"></param>
        /// <param name="sign"></param>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="logtype"></param>
        public static void AppendLog(IDictionary<string, string> param, string sign, string url, string msg, LogType logtype)
        {

            CheckFileSize(logtype);
            using (StreamWriter fs = File.AppendText(GetLogPath + "outpay_" + logtype.ToString() + ".txt"))
            {
                fs.WriteLine("时间：" + DateTime.Now.ToString());
                if (param != null && param.Count > 0)
                {
                    foreach (KeyValuePair<string, string> key in param)
                    {
                        fs.WriteLine(key.Key + ":" + key.Value);
                    }
                }
                fs.WriteLine("HishopUrl:" + url);
                fs.WriteLine("Hishopmsg:" + msg);
                fs.WriteLine("Hishopsign:" + sign);

                fs.WriteLine("");
                fs.WriteLine("");
                fs.WriteLine("");
            }
        }

        /// <summary>
        /// 追加日志
        /// </summary>
        /// <param name="param"></param>
        /// <param name="sign"></param>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="logtype"></param>
        public static void AppendLog_Collection(NameValueCollection param, string sign, string url, string msg, LogType logtype)
        {

            CheckFileSize(logtype);
            using (StreamWriter fs = File.AppendText(GetLogPath + "outpay_" + logtype.ToString() + ".txt"))
            {
                fs.WriteLine("时间：" + DateTime.Now.ToString());
                if (param != null)
                {
                    foreach (string s in param.AllKeys)
                    {
                        fs.WriteLine(s + ":" + param[s]);
                    }
                }
                fs.WriteLine("HishopUrl:" + url);
                fs.WriteLine("Hishopmsg:" + msg);
                fs.WriteLine("Hishopsign:", sign);

                fs.WriteLine("");
                fs.WriteLine("");
                fs.WriteLine("");
            }
        }
    }
}
