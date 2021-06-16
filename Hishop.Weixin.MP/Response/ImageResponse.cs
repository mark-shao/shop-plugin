using System;
using System.Collections.Generic;
using Hishop.Weixin.MP.Domain;

namespace Hishop.Weixin.MP.Response
{
    /// <summary>
    /// 响应图文消息
    /// </summary>
    public class ImageResponse : AbstractResponse
    {
        public Image Image { get; set; }

        public override ResponseMsgType MsgType
        {
            get { return ResponseMsgType.Image; }
        }
    }
}
