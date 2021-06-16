using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP.Request
{
    /// <summary>
    /// 事件消息基类
    /// </summary>
    public abstract class EventRequest : AbstractRequest
    {
        public virtual RequestEventType Event { get; set; }
        public string EventKey
        {
            get;
            set;
        }
    }
}
