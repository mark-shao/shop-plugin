using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hishop.Weixin.MP.Request;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using Hishop.Weixin.MP.Util;

namespace Hishop.Weixin.MP.Test
{
    public class A : Handler.RequestHandler
    {
        /// <summary>
        /// Initializes a new instance of the A class.
        /// </summary>
        public A(string xml) : base(xml)
        {
            
        }

        public override AbstractResponse DefaultResponse(AbstractRequest requestMessage)
        {
            return null;
        }
    }

    internal class Utils
    {
        const string xml = @"<xml><ToUserName><![CDATA[gh_ef4e2090afe3]]></ToUserName><FromUserName><![CDATA[opUMDj9jbOmTtbZuE2hM6wnv27B0]]></FromUserName><CreateTime>1385887183</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[s]]></Content><MsgId>5952340126940580233</MsgId></xml>";

        public void Test04()
        {
            A a = new A(xml);
            int c = 0;
            object b = a.RequestDocument;
        }

        public string MethodName()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            //return doc.Attributes["ToUserName"].InnerText;
            return doc.SelectSingleNode("xml/ToUserName").InnerText;
        }

        public AbstractRequest ConvertRequest<T>(System.IO.Stream inputStream) where T : AbstractRequest
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(inputStream);

            string msgType = doc.SelectSingleNode("xml/MsgType").InnerText.ToLower();

            if (msgType != "text")
                return default(T);

            TextRequest request = new TextRequest();
            request.Content = doc.SelectSingleNode("xml/Content").InnerText;
            
            request.FromUserName = doc.SelectSingleNode("xml/FromUserName").InnerText;
            request.MsgId = Convert.ToInt32(doc.SelectSingleNode("xml/FromUserName").InnerText);

            return request;
        }



        public void Test02()
        {
            XDocument xdoc = XDocument.Parse(xml);

            TextRequest req = new TextRequest();

            EntityHelper.FillEntityWithXml(req, xdoc);
            
        }

        public string Test03()
        {
            Response.TextResponse res = new Response.TextResponse();
            res.Content = "hah";
            res.FromUserName = "123";
            res.ToUserName = "456";

            XDocument xdoc = EntityHelper.ConvertEntityToXml(res);

            return xdoc.ToString();
        }



    }
}
