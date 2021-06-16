using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP
{
    /// <summary>
    /// 响应消息类型
    /// </summary>
    public enum ResponseMsgType
    {
        /// <summary>
        /// 文本消息
        /// </summary>
        Text,
        /// <summary>
        /// 图文消息
        /// </summary>
        News,
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
        /// 音乐消息
        /// </summary>
        Music,
        /// <summary>
        /// 多客服消息
        /// </summary>
        transfer_customer_service
    }
}
