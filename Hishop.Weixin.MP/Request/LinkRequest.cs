using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP.Request
{
    /// <summary>
    /// 链接消息
    /// </summary>
    public class LinkRequest : AbstractRequest
    {
        /// <summary>
        /// 消息标题
        /// </summary>
        public int Title { get; set; }

        /// <summary>
        /// 消息描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 消息链接
        /// </summary>
        public string Url { get; set; }
    }
}
