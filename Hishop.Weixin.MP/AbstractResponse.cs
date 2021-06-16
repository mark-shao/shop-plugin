using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP
{
    public class AbstractResponse
    {
        public AbstractResponse()
        {
            CreateTime = DateTime.Now;
        }

        public string ToUserName { get; set; }

        public string FromUserName { get; set; }

        public DateTime CreateTime { get; set; }

        public bool FuncFlag { get; set; }

        //public virtual ResponseMsgType MsgType 
        //{
        //    get { return ResponseMsgType.Text; }
        //    set { }
        //}

        public virtual ResponseMsgType MsgType
        { get; set; }

    }
}