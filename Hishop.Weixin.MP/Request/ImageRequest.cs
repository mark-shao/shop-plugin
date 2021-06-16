using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP.Request
{
    /// <summary>
    /// 图片消息
    /// </summary>
    public class ImageRequest : AbstractRequest
    {
        /// <summary>
        /// 图片链接
        /// </summary>
        public string PicUrl { get; set; }

        /// <summary>
        /// 图片消息媒体id
        /// </summary>
        public int MediaId { get; set; }
    }
}