using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;

namespace Hishop.Plugins.SMS
{
    [Plugin("短信接口")]
    public class ymSMS : SMSSender
    {
        private const string Gateway = "http://sms.kuaidiantong.cn/SendMsg.aspx";
        [ConfigElement("Appkey", Nullable = false)]
        public string Appkey { get; set; }

        [ConfigElement("Appsecret", Nullable = false)]
        public string Appsecret { get; set; }

        public override bool Send(string cellPhone, string message, out string returnMsg)
        {

            return Send(cellPhone, message, out returnMsg, "0");
        }

        public override bool Send(string[] phoneNumbers, string message, out string returnMsg)
        {

            return Send(phoneNumbers, message, out returnMsg, "1");
        }

        /// <summary>
        /// 群发营销类短信
        /// </summary>
        /// <param name="phoneNumbers"></param>
        /// <param name="message"></param>
        /// <param name="returnMsg"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public override bool Send(string[] phoneNumbers, string message, out string returnMsg, string speed = "1")
        {
            if (phoneNumbers == null || phoneNumbers.Length == 0 || string.IsNullOrEmpty(message) || message.Trim().Length == 0)
            {
                returnMsg = "手机号码和消息内容不能为空";
                return false;
            }

            SortedDictionary<string, string> tmpParas = new SortedDictionary<string, string>();
            tmpParas.Add("mobiles", string.Join(",", phoneNumbers));
            tmpParas.Add("text", message);
            tmpParas.Add("appkey", Appkey);
            tmpParas.Add("sendtime", DateTime.Now.ToString());
            tmpParas.Add("speed", speed);
            Dictionary<string, string> paras = SMSAPiHelper.Parameterfilter(tmpParas);
            string sign = SMSAPiHelper.BuildSign(paras, Appsecret, "MD5", "utf-8");
            paras.Add("sign", sign);
            paras.Add("sign_type", "MD5");
            string postdata = SMSAPiHelper.CreateLinkstring(paras);
            try
            {
                string result = SMSAPiHelper.PostData(Gateway, postdata);
                if (result == "发送成功")
                {
                    returnMsg = "发送成功!";
                    return true;
                }
                else
                {
                    returnMsg = result;
                    return false;
                }
            }
            catch (Exception ex)
            {
                returnMsg = "未知错误";
                return false;
            }
        }

        public void writeError(string syssign, string param)
        {
            DataTable dt = new DataTable();
            dt.TableName = "SMSLog";
            dt.Columns.Add(new DataColumn("time"));
            dt.Columns.Add(new DataColumn("SysSign"));
            dt.Columns.Add(new DataColumn("Sign"));

            DataRow dr = dt.NewRow();
            dr["time"] = DateTime.Now;
            dr["SysSign"] = syssign;
            dr["sign"] = param;
            dt.Rows.Add(dr);
            dt.WriteXml(HttpContext.Current.Request.MapPath("/SMSLog.xml"));
        }

        /// <summary>
        /// 单条
        /// </summary>
        /// <param name="cellPhone"></param>
        /// <param name="message"></param>
        /// <param name="returnMsg"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public override bool Send(string cellPhone, string message, out string returnMsg, string speed = "0")
        {
            if (
                string.IsNullOrEmpty(cellPhone) || string.IsNullOrEmpty(message) ||
                cellPhone.Trim().Length == 0 || message.Trim().Length == 0)
            {
                returnMsg = "手机号码和消息内容不能为空";
                return false;
            }
            SortedDictionary<string, string> tmpParas = new SortedDictionary<string, string>();
            tmpParas.Add("mobiles", cellPhone);
            tmpParas.Add("text", message);
            tmpParas.Add("appkey", Appkey);
            tmpParas.Add("sendtime", DateTime.Now.ToString());
            tmpParas.Add("speed", speed);
            Dictionary<string, string> paras = SMSAPiHelper.Parameterfilter(tmpParas);
            string sign = SMSAPiHelper.BuildSign(paras, Appsecret, "MD5", "utf-8");
            paras.Add("sign", sign);
            paras.Add("sign_type", "MD5");
            // writeError(sign, cellPhone + "|" + message + "|" + Appkey + "|" + DateTime.Now.ToString() + "|" + speed);
            string postdata = SMSAPiHelper.CreateLinkstring(paras);
            try
            {
                string result = SMSAPiHelper.PostData(Gateway, postdata);
                if (result == "发送成功")
                {
                    returnMsg = "发送成功!";
                    return true;
                }
                else
                {
                    returnMsg = result;
                    SMSLog.writeLog(tmpParas, result, "", "", SMSLog.LogType.YMSMS);
                    return false;
                }
            }
            catch (Exception ex)
            {
                SMSLog.writeLog(tmpParas, ex.Message, ex.StackTrace, ex.TargetSite.ToString(), SMSLog.LogType.YMSMS);
                returnMsg = "未知错误";
                return false;
            }
        }

        public override int GetBalance(int spped)
        {
            throw new NotImplementedException();
        }

        public override int GetBalance()
        {
            throw new NotImplementedException();
        }

        protected override bool NeedProtect
        {
            get { return true; }
        }


        public override string Description
        {
            get { return string.Empty; }
        }

        public override string Logo
        {
            get { return string.Empty; }
        }

        public override string ShortDescription
        {
            get { return string.Empty; }
        }

    }
}
