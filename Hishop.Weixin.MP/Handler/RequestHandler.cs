using System;
using System.Collections.Generic;
using System.IO;
using Hishop.Weixin.MP.Request;
using Hishop.Weixin.MP.Request.Event;
using System.Xml;
using System.Xml.Linq;
using Hishop.Weixin.MP.Util;

namespace Hishop.Weixin.MP.Handler
{
    public abstract class RequestHandler
    {
        public XDocument RequestDocument { get; set; }

        public string ResponseDocument {
            get
            {
                if (ResponseMessage == null)
                {
                    return String.Empty;
                }
                return EntityHelper.ConvertEntityToXml(ResponseMessage as AbstractResponse).ToString();
            } 
        }

        public AbstractRequest RequestMessage { get; set; }

        public AbstractResponse ResponseMessage { get; set; }

        public RequestHandler(Stream inputStream)
        {
            using (XmlReader xr = XmlReader.Create(inputStream))
            {
                RequestDocument = XDocument.Load(xr);
                Init(RequestDocument);
            }
        }

        public RequestHandler(string xml)
        {
            using (XmlReader xr = XmlReader.Create(new System.IO.StringReader(xml)))
            {
                RequestDocument = XDocument.Load(xr);
                Init(RequestDocument);
            }
        }

        private void Init(XDocument requestDocument)
        {
            RequestDocument = requestDocument;
            RequestMessage = RequestMessageFactory.GetRequestEntity(RequestDocument);
        }



        /// <summary>
        /// 执行微信请求
        /// </summary>
        public void Execute()
        {
            if (RequestMessage == null)
                return;

            switch (RequestMessage.MsgType)
            {
                case RequestMsgType.Text:
                    ResponseMessage = OnTextRequest(RequestMessage as TextRequest);
                    break;
                case RequestMsgType.Image:
                    ResponseMessage = OnImageRequest(RequestMessage as ImageRequest);
                    break;
                case RequestMsgType.Voice:
                    ResponseMessage = OnVoiceRequest(RequestMessage as VoiceRequest);
                    break;
                case RequestMsgType.Video:
                    ResponseMessage = OnVideoRequest(RequestMessage as VideoRequest);
                    break;
                case RequestMsgType.Location:
                    ResponseMessage = OnLocationRequest(RequestMessage as LocationRequest);
                    break;
                case RequestMsgType.Link:
                    ResponseMessage = OnLinkRequest(RequestMessage as LinkRequest);
                    break;
                case RequestMsgType.Event:
                    ResponseMessage = OnEventRequest(RequestMessage as EventRequest);
                    break;
                default:
                    throw new WeixinException("未知的MsgType请求类型");
            }
        }

        /// <summary>
        /// 默认返回消息
        /// </summary>
        public abstract AbstractResponse DefaultResponse(AbstractRequest requestMessage);

        #region 消息请求

        /// <summary>
        /// 文字类型请求
        /// </summary>
        public virtual AbstractResponse OnTextRequest(TextRequest textRequest)
        {
            return DefaultResponse(textRequest);
        }

        /// <summary>
        /// 图片类型请求
        /// </summary>
        public virtual AbstractResponse OnImageRequest(ImageRequest imageRequest)
        {
            return DefaultResponse(imageRequest);
        }

        /// <summary>
        /// 语音类型请求
        /// </summary>
        public virtual AbstractResponse OnVoiceRequest(VoiceRequest voiceRequest)
        {
            return DefaultResponse(voiceRequest);
        }

        /// <summary>
        /// 视频类型请求
        /// </summary>
        public virtual AbstractResponse OnVideoRequest(VideoRequest videoRequest)
        {
            return DefaultResponse(videoRequest);
        }

        /// <summary>
        /// 位置类型请求
        /// </summary>
        public virtual AbstractResponse OnLocationRequest(LocationRequest locationRequest)
        {
            return DefaultResponse(locationRequest);
        }

        /// <summary>
        /// 链接消息类型请求
        /// </summary>
        public virtual AbstractResponse OnLinkRequest(LinkRequest linkRequest)
        {
            return DefaultResponse(linkRequest);
        }
        #endregion

        /// <summary>
        /// Event事件类型请求
        /// </summary>
        public AbstractResponse OnEventRequest(EventRequest eventRequest)
        {
            AbstractResponse responseMessage = null;

            switch (eventRequest.Event)
            {
                case RequestEventType.Subscribe:
                    responseMessage = OnEvent_SubscribeRequest(eventRequest as SubscribeEventRequest);
                    break;
                case RequestEventType.UnSubscribe:
                    responseMessage = OnEvent_UnSubscribeRequest(eventRequest as UnSubscribeEventRequest);
                    break;
                case RequestEventType.Scan:
                    ResponseMessage = OnEvent_ScanRequest(eventRequest as ScanEventRequest);
                    break;
                case RequestEventType.Location:
                    responseMessage = OnEvent_LocationRequest(eventRequest as LocationEventRequest);
                    break;
                case RequestEventType.Click:
                    responseMessage = OnEvent_ClickRequest(eventRequest as ClickEventRequest);
                    break;
                default:
                    throw new WeixinException("未知的Event下属请求信息");
            }

            return responseMessage;
        }

        #region Event事件请求

        /// <summary>
        /// 菜单点击事件请求
        /// </summary>
        public virtual AbstractResponse OnEvent_ClickRequest(ClickEventRequest clickEventRequest)
        {
            return DefaultResponse(clickEventRequest);
        }

        /// <summary>
        /// 定位事件请求
        /// </summary>
        public virtual AbstractResponse OnEvent_LocationRequest(LocationEventRequest locationEventRequest)
        {
            return DefaultResponse(locationEventRequest);
        }

        /// <summary>
        /// 二维码扫描事件请求
        /// </summary>
        public virtual AbstractResponse OnEvent_ScanRequest(ScanEventRequest scanEventRequest)
        {
            return DefaultResponse(scanEventRequest);
        }

        /// <summary>
        /// 取消关注事件请求
        /// </summary>
        public virtual AbstractResponse OnEvent_UnSubscribeRequest(UnSubscribeEventRequest unSubscribeEventRequest)
        {
            return DefaultResponse(unSubscribeEventRequest);
        }

        /// <summary>
        /// 关注事件请求
        /// </summary>
        public virtual AbstractResponse OnEvent_SubscribeRequest(SubscribeEventRequest subscribeEventRequest)
        {
            return DefaultResponse(subscribeEventRequest);
        }
        #endregion

    }
}
