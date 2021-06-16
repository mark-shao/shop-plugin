using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Hishop.Weixin.Pay.Domain
{

    public class WxPayLog
    {

        public static void writeLog(IDictionary<string, string> param, string sign, string url, string msg, LogType logtype)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.TableName = "log";
                dt.Columns.Add(new DataColumn("HishopOperTime"));
                foreach (KeyValuePair<string, string> key in param)
                {
                    dt.Columns.Add(new DataColumn(key.Key));
                }

                dt.Columns.Add(new DataColumn("HishopMsg"));
                dt.Columns.Add(new DataColumn("HishopSign"));
                dt.Columns.Add(new DataColumn("HishopUrl"));
                DataRow dr = dt.NewRow();
                dr["HishopOperTime"] = DateTime.Now;
                foreach (KeyValuePair<string, string> key in param)
                {
                    dr[key.Key] = key.Value;
                }
                dr["HishopMsg"] = msg;
                dr["HishopSign"] = sign;
                dr["HishopUrl"] = url;
                dt.Rows.Add(dr);

                dt.WriteXml(GetLogPath + "wx" + logtype.ToString("G") + ".xml");
            }
            catch (Exception ex)
            {

            };
        }

        private static string GetLogPath
        {
            get
            {
                string path = HttpContext.Current.Server.MapPath("/log/");
                if (!Directory.Exists(path))//如果日志目录不存在就创建
                {
                    Directory.CreateDirectory(path);
                }
                return path;
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
            using (StreamWriter fs = File.AppendText(GetLogPath + "wx" + logtype.ToString() + ".txt"))
            {
                fs.WriteLine("时间：" + DateTime.Now.ToString());
                if (param != null && param.Count > 0)
                {
                    foreach (KeyValuePair<string, string> key in param)
                    {
                        fs.WriteLine(key.Key + ":" + key.Value);
                    }
                }
                fs.WriteLine("Url:" + url);
                fs.WriteLine("msg:" + msg);
                fs.WriteLine("sign:" + sign);
                fs.WriteLine("");
                fs.WriteLine("");
                fs.WriteLine("");
            }
        }


        /// <summary>
        /// 写异常日志，输出指定的参数
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="param">页面参数</param>
        public static void WriteExceptionLog(Exception ex, IDictionary<string, string> iParam, LogType logType)
        {
            if (iParam == null)
            {
                iParam = new Dictionary<string, string>();
            }
            if (ex is System.Threading.ThreadAbortException)
            {
                return;
            }
            iParam.Add("ErrorMessage", ex.Message);
            iParam.Add("StackTrace", ex.StackTrace);
            if (ex.InnerException != null)
            {
                iParam.Add("InnerException", ex.InnerException.ToString());
            }
            if (ex.GetBaseException() != null)
                iParam.Add("BaseException", ex.GetBaseException().Message);
            if (ex.TargetSite != null)
                iParam.Add("TargetSite", ex.TargetSite.ToString());
            iParam.Add("ExSource", ex.Source);
            string url = "";
            if (HttpContext.Current != null)
            {
                url = HttpContext.Current.Request.Url.ToString();
            }
            AppendLog(iParam, "", "", url, logType);
        }
    }
}
