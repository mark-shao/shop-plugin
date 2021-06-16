using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Hishop.Weixin.MP.Domain;
using Hishop.Weixin.MP.Request;
using Hishop.Weixin.MP.Request.Event;

namespace Hishop.Weixin.MP.Util
{
    public static class MsgTypeHelper
    {
        public static RequestMsgType GetMsgType(XDocument doc)
        {
            return GetMsgType(doc.Root.Element("MsgType").Value);
        }

        public static RequestMsgType GetMsgType(string str)
        {
            return (RequestMsgType)Enum.Parse(typeof(RequestMsgType), str, true);
        }
    }

    public static class EventTypeHelper
    {

        public static RequestEventType GetEventType(XDocument doc)
        {
            return GetEventType(doc.Root.Element("Event").Value);
        }

        public static RequestEventType GetEventType(string str)
        {
            return (RequestEventType)Enum.Parse(typeof(RequestEventType), str, true);
        }
    }

    public static class EntityHelper
    {
        public static void FillEntityWithXml<T>(T entity, XDocument doc) where T : AbstractRequest, new()
        {
            entity = entity ?? new T();
            var root = doc.Root;

            var props = entity.GetType().GetProperties();
            foreach (var prop in props)
            {
                var propName = prop.Name;
                if (root.Element(propName) != null)
                {
                    switch (prop.PropertyType.Name)
                    {
                        //case "String":
                        //    goto default;
                        case "DateTime":
                            prop.SetValue(entity, new DateTime(long.Parse(root.Element(propName).Value)), null);
                            break;
                        case "Boolean":
                            if (propName == "FuncFlag")
                            {
                                prop.SetValue(entity, root.Element(propName).Value == "1", null);
                            }
                            else
                            {
                                goto default;
                            }
                            break;
                        case "Int64":
                            prop.SetValue(entity, long.Parse(root.Element(propName).Value), null);
                            break;
                        case "RequestEventType":
                            prop.SetValue(entity, EventTypeHelper.GetEventType(root.Element(propName).Value), null);
                            break;
                        case "RequestMsgType":
                            prop.SetValue(entity, MsgTypeHelper.GetMsgType(root.Element(propName).Value), null);
                            break;
                        default:
                            prop.SetValue(entity, root.Element(propName).Value, null);
                            break;
                    }
                }
            }
        }

        public static XDocument ConvertEntityToXml<T>(T entity) where T : class , new()
        {
            entity = entity ?? new T();
            var doc = new XDocument();
            doc.Add(new XElement("xml"));
            var root = doc.Root;

            //经过测试，微信对字段排序有严格要求，这里对排序进行强制约束
            var propNameOrder = new[] { "ToUserName", "FromUserName", "CreateTime", "MsgType", "Content", "ArticleCount", "Articles", "FuncFlag",/*以下是Atricle属性*/"Title ", "Description ", "PicUrl", "Url" }.ToList();
            Func<string, int> orderByPropName = propNameOrder.IndexOf;

            var props = entity.GetType().GetProperties().OrderBy(p => orderByPropName(p.Name)).ToList();
            foreach (var prop in props)
            {
                var propName = prop.Name;
                if (propName == "Articles")
                {
                    var atriclesElement = new XElement("Articles");
                    var articales = prop.GetValue(entity, null) as List<Article>;
                    foreach (var articale in articales)
                    {
                        var subNodes = ConvertEntityToXml(articale).Root.Elements();
                        atriclesElement.Add(new XElement("item", subNodes));
                    }
                    root.Add(atriclesElement);
                }
                else
                {
                    switch (prop.PropertyType.Name)
                    {
                        case "String":
                            root.Add(new XElement(propName,
                                                  new XCData(prop.GetValue(entity, null) as string ?? "")));
                            break;
                        case "DateTime":
                            root.Add(new XElement(propName, ((DateTime)prop.GetValue(entity, null)).Ticks));
                            break;
                        case "Boolean":
                            if (propName == "FuncFlag")
                            {
                                root.Add(new XElement(propName, (bool)prop.GetValue(entity, null) ? "1" : "0"));
                            }
                            else
                            {
                                goto default;
                            }
                            break;
                        case "ResponseMsgType":
                            root.Add(new XElement(propName, prop.GetValue(entity, null).ToString().ToLower()));
                            break;
                        case "Article":
                            root.Add(new XElement(propName, prop.GetValue(entity, null).ToString().ToLower()));
                            break;
                        default:
                            root.Add(new XElement(propName, prop.GetValue(entity, null)));
                            break;
                    }
                }
            }
            return doc;
        }
    }

    public static class RequestMessageFactory
    {
        public static AbstractRequest GetRequestEntity(XDocument doc)
        {
            var msgType = MsgTypeHelper.GetMsgType(doc);
            AbstractRequest requestMessage = null;
            switch (msgType)
            {
                case RequestMsgType.Text:
                    requestMessage = new TextRequest();
                    break;
                case RequestMsgType.Video:
                    requestMessage = new VideoRequest();
                    break;
                case RequestMsgType.Voice:
                    requestMessage = new VoiceRequest();
                    break;
                case RequestMsgType.Location:
                    requestMessage = new LocationRequest();
                    break;
                case RequestMsgType.Image:
                    requestMessage = new ImageRequest();
                    break;
                case RequestMsgType.Link:
                    requestMessage = new LinkRequest();
                    break;
                case RequestMsgType.Event:
                    var eventType = EventTypeHelper.GetEventType(doc);
                    switch (eventType)
                    {
                        case RequestEventType.Subscribe:
                            requestMessage = new SubscribeEventRequest();
                            break;
                        case RequestEventType.UnSubscribe:
                            requestMessage = new UnSubscribeEventRequest();
                            break;
                        case RequestEventType.Scan:
                            requestMessage = new ScanEventRequest();
                            break;
                        case RequestEventType.Location:
                            requestMessage = new LocationEventRequest();
                            break;
                        case RequestEventType.Click:
                            requestMessage = new ClickEventRequest();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            EntityHelper.FillEntityWithXml(requestMessage, doc);
            return requestMessage;
        }
    }
}
