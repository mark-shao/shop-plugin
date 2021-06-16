using System;
using System.Collections.Generic;
using Hishop.Weixin.MP.Domain;

namespace Hishop.Weixin.MP.Response
{
    /// <summary>
    /// 响应视频消息
    /// </summary>
    public class VideoResponse : AbstractResponse
    {
        public Video Video { get; set; }

        public override ResponseMsgType MsgType
        {
            get { return ResponseMsgType.Video; }
        }
    }
}
