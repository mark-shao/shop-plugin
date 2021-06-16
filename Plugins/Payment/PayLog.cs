using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Web;

namespace Hishop.Plugins.Payment
{
    public enum LogType
    {
        KuaiQian = 1,
        Alipay_Assure = 2,
        Alipay_bank = 3,
        Alipay_Direct = 4,
        Alipay_QR_Code = 5,
        AllBuy = 6,
        BankUnion = 7,
        BankUnion_GateWay = 8,
        ChinaBank = 9,
        CMPay_BankAndPhone = 10,
        CnCard = 11,
        GoPay = 12,
        IPay = 13,
        IPS = 14,
        IPS_Express = 15,
        Paypal = 16,
        ShengPay = 17,
        ShengpayMobile = 18,
        Tenpay = 19,
        WS_WapPay = 20,
        XPay = 21,
        YeePay = 22,
        WXQRCode = 23,
        ChangJiePay = 24,
        ChangJieWapPay = 25,
        Alipay_Forex = 26,
        WxAliPay = 27,
        WS_WapPay_Notify = 28,
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

                dt.WriteXml(GetLogPath + "pay_" + logtype.ToString("G") + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "-" + ".xml");
            }
            catch (Exception ex)
            {
                AppendLog(null, "", "", ex.Message, LogType.Alipay_Direct);
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

                dt.WriteXml(GetLogPath + "pay_" + logtype.ToString("G") + ".xml");
            }
            catch (Exception ex)
            {

            };
        }

        private static string GetLogPath
        {
            get
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
        }
        /// <summary>
        /// 检查文件大小，如果文件大小超过500KB则删除日志 
        /// </summary>
        /// <param name="?"></param>
        public static void CheckFileSize(LogType logtype)
        {
            string fileUrl = GetLogPath + "pay_" + logtype.ToString() + ".txt";
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
            using (StreamWriter fs = File.AppendText(GetLogPath + "pay_" + logtype.ToString() + "_" + DateTime.Now.ToString("yyMMddHHmm") + ".txt"))
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
            using (StreamWriter fs = File.AppendText(GetLogPath + "pay_" + logtype.ToString() + ".txt"))
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

        public static void WriteExpectionLog(Exception ex, IDictionary<string, string> iParam, LogType logtype)
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
            AppendLog(iParam, "", "", url, logtype);
        }

        public static void WriteExpectionLog_Page(Exception ex, NameValueCollection param, LogType logtype)
        {
            IDictionary<string, string> iParam = NameValueCollectionToDictionary(param);
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
            AppendLog(iParam, "", "", url, logtype);
        }

        /// <summary>
        /// 对NameValueCollection转换为Dictionary
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IDictionary<string, string> NameValueCollectionToDictionary(NameValueCollection collection)
        {

            IDictionary<string, string> result = new Dictionary<string, string>();
            if (collection == null)
            {
                return result;
            }
            foreach (string s in collection.AllKeys)
            {
                result.Add(new KeyValuePair<string, string>(s, collection[s]));
            }
            return result;
        }
    }
}
