using System.Web;
using Hishop.Plugins;
using System;

namespace Hishop.Plugins
{
    public class ConfigHandler : IHttpHandler
    {

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                switch (context.Request["type"])
                {
                    case "PaymentRequest":
                        ProcessPaymentRequest(context);
                        break;
                    case "OpenIdService":
                        ProcessOpenId(context);
                        break;
                    case "EmailSender":
                        ProcessEmailSender(context);
                        break;
                    case "SMSSender":
                        ProcessSMSSender(context);
                        break;
                    case "Logistics":
                        break;
                }
            }
            catch
            {
            }
        }

        private void ProcessOpenId(HttpContext context)
        {
            if (context.Request["action"] == "getlist")
            {
                OpenIdPlugins openId = OpenIdPlugins.Instance();
                context.Response.ContentType = "application/json";
                context.Response.Write(openId.GetPlugins().ToJsonString());

                return;
            }

            if (context.Request["action"] == "getmetadata")
            {
                context.Response.ContentType = "text/xml";
                OpenIdService instance = OpenIdService.CreateInstance(context.Request["name"]);
                if (instance == null)
                    context.Response.Write("<xml></xml>");
                else
                    context.Response.Write(instance.GetMetaData().OuterXml);

                return;
            }
        }

        private static void ProcessPaymentRequest(HttpContext context)
        {
            if (context.Request["action"] == "getlist")
            {
                PaymentPlugins payments = PaymentPlugins.Instance();
                context.Response.ContentType = "application/json";
                context.Response.Write(payments.GetPlugins().ToJsonString());

                return;
            }

            if (context.Request["action"] == "getmetadata")
            {
                context.Response.ContentType = "text/xml";
                PaymentRequest instance = PaymentRequest.CreateInstance(context.Request["name"]);
                if (instance == null)
                    context.Response.Write("<xml></xml>");
                else
                    context.Response.Write(instance.GetMetaData().OuterXml);

                return;
            }
        }

        private static void ProcessSMSSender(HttpContext context)
        {
            if (context.Request["action"] == "getlist")
            {
                SMSPlugins sms = SMSPlugins.Instance();
                context.Response.ContentType = "application/json";
                context.Response.Write(sms.GetPlugins().ToJsonString());

                return;
            }

            if (context.Request["action"] == "getmetadata")
            {
                context.Response.ContentType = "text/xml";
                SMSSender instance = SMSSender.CreateInstance(context.Request["name"]);
                if (instance == null)
                    context.Response.Write("<xml></xml>");
                else
                    context.Response.Write(instance.GetMetaData().OuterXml);

                return;
            }
        }

        private static void ProcessEmailSender(HttpContext context)
        {
            if (context.Request["action"] == "getlist")
            {
                EmailPlugins emails = EmailPlugins.Instance();
                context.Response.ContentType = "application/json";
                context.Response.Write(emails.GetPlugins().ToJsonString());

                return;
            }

            if (context.Request["action"] == "getmetadata")
            {
                context.Response.ContentType = "text/xml";
                EmailSender instance = EmailSender.CreateInstance(context.Request["name"]);
                if (instance == null)
                    context.Response.Write("<xml></xml>");
                else
                    context.Response.Write(instance.GetMetaData().OuterXml);

                return;
            }
        }

    }
}