using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP.Request
{
    /// <summary>
    /// 视频消息
    /// </summary>
    public class VideoRequest : AbstractRequest
    {
        /// <summary>
        /// 视频消息媒体id
        /// </summary>
        public int MediaId { get; set; }

        /// <summary>
        /// 视频消息缩略图的媒体id
        /// </summary>
        public int ThumbMediaId { get; set; }
    }
}
