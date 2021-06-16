using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Hishop.Plugins;
using System.Net.Mail;

namespace Hishop.Plugins.Email
{
    [Plugin("ASP.NET邮件发送组件")]
    public class ASPNETMail : EmailSender
    {

        private SmtpClient smtp;

        protected override void InitConfig(XmlNode configXml)
        {
            base.InitConfig(configXml);
            smtp = new SmtpClient(SmtpServer, SmtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Username, Password),
                EnableSsl = EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
        }

        [ConfigElement("SMTP服务器", Nullable = false)]
        public string SmtpServer { get; set; }

        [ConfigElement("SMTP服务器端口", Nullable = false)]
        public int SmtpPort { get; set; }

        [ConfigElement("安全连接(SSL)", InputType = InputType.CheckBox, Nullable = false)]
        public bool EnableSsl { get; set; }

        [ConfigElement("SMTP用户名", Nullable = false)]
        public string Username { get; set; }

        [ConfigElement("SMTP用户密码", InputType = InputType.Password, Nullable = false)]
        public string Password { get; set; }

        [ConfigElement("SMTP邮箱", Nullable = false)]
        public string ReplyAddress { get; set; }

        [ConfigElement("显示名称", Nullable = false)]
        public string DisplayName { get; set; }

        public override bool Send(MailMessage mail, Encoding emailEncoding)
        {
            if (mail == null)
            {
                throw new ArgumentNullException("mail");
            }

            if (mail.From == null)
            {
                mail.From = new MailAddress(ReplyAddress, DisplayName, emailEncoding);
            }
            
            if (mail.IsBodyHtml)
            {
                // mail.Body = FormatPlainTextAsHtml(mail.Body);
                mail.Body = mail.Body;
            }

            mail.BodyEncoding = emailEncoding;
            smtp.Send(mail);

            return true;
        }

        //private static string FormatPlainTextAsHtml(string stringToFormat)
        //{
        //    if (string.IsNullOrEmpty(stringToFormat)) return "";

        //    // 将\n字符替换成html的换行
        //    stringToFormat = Regex.Replace(stringToFormat, "\r\n", "<br />", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //    stringToFormat = Regex.Replace(stringToFormat, "\n", "<br />", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        //    return stringToFormat;
        //}

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