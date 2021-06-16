using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP
{
    /// <summary>
    /// 请求消息类型
    /// </summary>
    public enum RequestMsgType
    {
        /// <summary>
        /// 文本消息
        /// </summary>
        Text,
        /// <summary>
        /// 图片消息
        /// </summary>
        Image,
        /// <summary>
        /// 语音消息
        /// </summary>
        Voice,
        /// <summary>
        /// 视频消息
        /// </summary>
        Video,
        /// <summary>
        /// 地理位置消息
        /// </summary>
        Location,
        /// <summary>
        /// 链接消息
        /// </summary>
        Link,
        /// <summary>
        /// 事件消息
        /// </summary>
        Event,
        /// <summary>
        /// 多客服消息
        /// </summary>
        transfer_customer_service,

    }
}
