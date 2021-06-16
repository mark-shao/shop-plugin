using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Hishop.Plugins;

namespace Hishop.Plugins.SMS
{
    [Plugin("默认短信接口")]
    public class Zhidian : SMSSender
    {

        private const string Gateway = "http://agentin.zhidian3g.cn/MSMSEND.ewing?";

        [ConfigElement("企业代码", Nullable = false)]
        public string ECode { get; set; }
        [ConfigElement("用户名", Nullable = false)]
        public string Username { get; set; }

        [ConfigElement("用户密码", Nullable = false, InputType = InputType.Password)]
        public string Password { get; set; }

        public override bool Send(string cellPhone, string message, out string returnMsg)
        {

            return Send(cellPhone, message, out returnMsg, "0");
        }

        public override bool Send(string[] phoneNumbers, string message, out string returnMsg)
        {

            return Send(phoneNumbers, message, out returnMsg, "1");
        }

        public override bool Send(string[] phoneNumbers, string message, out string returnMsg, string speed = "1")
        {
            if (
                phoneNumbers == null || phoneNumbers.Length == 0 ||
                string.IsNullOrEmpty(message) || message.Trim().Length == 0)
            {
                returnMsg = "手机号码和消息内容不能为空";
                return false;
            }

            StringBuilder builder = new StringBuilder();

            builder.Append(Gateway);
            builder.Append("ECODE=").Append(ECode);
            builder.Append("&USERNAME=").Append(Username);
            builder.Append("&PASSWORD=").Append(Password);
            builder.Append("&MOBILE=").Append(string.Join(",", phoneNumbers));
            builder.Append("&CONTENT=");
            builder.Append(Uri.EscapeDataString(message));
            return Send(builder.ToString(), out returnMsg);
        }

        public override bool Send(string cellPhone, string message, out string returnMsg, string speed = "0")
        {
            if (
                string.IsNullOrEmpty(cellPhone) || string.IsNullOrEmpty(message) ||
                cellPhone.Trim().Length == 0 || message.Trim().Length == 0)
            {
                returnMsg = "手机号码和消息内容不能为空";
                return false;
            }

            StringBuilder builder = new StringBuilder();

            builder.Append(Gateway);
            builder.Append("ECODE=").Append(ECode);
            builder.Append("&USERNAME=").Append(Username);
            builder.Append("&PASSWORD=").Append(Password);
            builder.Append("&MOBILE=").Append(cellPhone);
            builder.Append("&CONTENT=");
            builder.Append(Uri.EscapeDataString(message));

            return Send(builder.ToString(), out returnMsg);
        }

        protected override bool NeedProtect
        {
            get { return true; }
        }

        private static bool Send(string url, out string returnMsg)
        {
            string strResult = "";

            try
            {
                HttpWebRequest smsRequest = (HttpWebRequest)WebRequest.Create(url);
                smsRequest.Timeout = 5000;
                HttpWebResponse response = (HttpWebResponse)smsRequest.GetResponse();

                using (Stream myStream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(myStream, Encoding.Default))
                    {
                        StringBuilder strBuilder = new StringBuilder();

                        while (-1 != sr.Peek())
                        {
                            strBuilder.Append(sr.ReadLine());
                        }

                        strResult = strBuilder.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                returnMsg = e.Message;
                return false;
            }

            bool success = false;

            switch (strResult)
            {
                case "1":
                    success = true;
                    returnMsg = "提交短信成功";
                    break;

                case "-1":
                    returnMsg = "不能初始化SO";
                    break;

                case "-2":
                    returnMsg = "网络不通";
                    break;

                case "-3":
                    returnMsg = "一次发送的手机号码过多";
                    break;

                case "-4":
                    returnMsg = "内容包含不合法文字";
                    break;

                case "-5":
                    returnMsg = "登录账户错误";
                    break;

                case "-6":
                    returnMsg = "通信数据传送";
                    break;

                case "-7":
                    returnMsg = "没有进行参数初始化";
                    break;

                case "-8":
                    returnMsg = "扩展号码长度不对";
                    break;

                case "-9":
                    returnMsg = "手机号码不合";
                    break;

                case "-10":
                    returnMsg = "号码太长";
                    break;

                case "-11":
                    returnMsg = "内容太长";
                    break;

                case "-12":
                    returnMsg = "内部错误";
                    break;

                case "-13":
                    returnMsg = "余额不足";
                    break;

                case "-14":
                    returnMsg = "扩展号不正确";
                    break;

                case "-17":
                    returnMsg = "发送内容为空";
                    break;

                case "-19":
                    returnMsg = "没有找到该动作（不存在的url地址）";
                    break;

                case "-20":
                    returnMsg = "手机号格式不正确";
                    break;

                case "-50":
                    returnMsg = "配置参数错误";
                    break;

                default:
                    returnMsg = "未知错误";
                    break;
            }

            return success;
        }

        public override int GetBalance(int spped)
        {
            throw new NotImplementedException();
        }

        public override int GetBalance()
        {
            throw new NotImplementedException();
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