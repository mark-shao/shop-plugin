using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP.Response
{
    /// <summary>
    /// 响应文本消息
    /// </summary>
    public class TextResponse : AbstractResponse
    {
        public string Content { get; set; }

        public override ResponseMsgType MsgType
        {
            get { return ResponseMsgType.Text; }
        }
    }
}
