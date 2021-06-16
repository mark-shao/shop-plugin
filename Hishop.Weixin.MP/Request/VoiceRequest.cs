using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP.Request
{
    /// <summary>
    /// 语音消息
    /// </summary>
    public class VoiceRequest : AbstractRequest
    {
        /// <summary>
        /// 语音消息媒体id
        /// </summary>
        public int MediaId { get; set; }

        /// <summary>
        /// 语音格式
        /// </summary>
        public string Format { get; set; }
    }
}
