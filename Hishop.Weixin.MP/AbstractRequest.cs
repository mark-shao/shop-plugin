using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP
{
    public class AbstractRequest
    {
        public long MsgId { get; set; }

        public string ToUserName { get; set; }

        public string FromUserName { get; set; }

        public DateTime CreateTime { get; set; }

        public RequestMsgType MsgType { get; set; }
    }
}
