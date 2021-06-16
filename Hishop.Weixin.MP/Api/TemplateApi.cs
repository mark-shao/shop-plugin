using Hishop.Weixin.MP.Domain;
using Hishop.Weixin.MP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Weixin.MP.Api
{
    public class TemplateApi
    {
        public static string SendMessage(string accessTocken, TemplateMessage templateMessage)
        {
            StringBuilder json = new StringBuilder("{");
            json.AppendFormat("\"touser\":\"{0}\",", templateMessage.Touser);
            json.AppendFormat("\"template_id\":\"{0}\",", templateMessage.TemplateId);
            json.AppendFormat("\"url\":\"{0}\",", templateMessage.Url);
            json.AppendFormat("\"topcolor\":\"{0}\",", templateMessage.Topcolor);
            json.Append("\"data\":{");
            foreach (var part in templateMessage.Data)
                json.AppendFormat("\"{0}\":{{\"value\":\"{1}\",\"color\":\"{2}\"}},", part.Name, part.Value, part.Color);
            json.Remove(json.Length - 1, 1);
            json.Append("}}");


            WebUtils webUtils = new WebUtils();
            string url = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + accessTocken;
            string response = webUtils.DoPost(url, json.ToString());
            return response;
        }

        public static string SendAppletMessage(string accessTocken, TemplateMessage templateMessage)
        {
            StringBuilder json = new StringBuilder("{");
            json.AppendFormat("\"touser\":\"{0}\",", templateMessage.Touser);
            json.AppendFormat("\"template_id\":\"{0}\",", templateMessage.TemplateId);
            json.AppendFormat("\"page\":\"{0}\",", templateMessage.Page);
            json.AppendFormat("\"form_id\":\"{0}\",", templateMessage.FormId);
            json.AppendFormat("\"color\":\"{0}\",", templateMessage.Topcolor);
            json.Append("\"data\":{");
            foreach (var part in templateMessage.Data)
                json.AppendFormat("\"{0}\":{{\"value\":\"{1}\",\"color\":\"{2}\"}},", part.Name, part.Value, part.Color);
            json.Remove(json.Length - 1, 1);
            json.Append("}");
            json.AppendFormat(",\"emphasis_keyword\":\"{0}\"", templateMessage.EmphasisKeyword);
            json.Append("}");

            WebUtils webUtils = new WebUtils();
            string url = "https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token=" + accessTocken;
            string response = webUtils.DoPost(url, json.ToString());
            return response;
        }
    }
}
