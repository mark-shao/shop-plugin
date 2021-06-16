using System;
using System.Collections.Generic;
using Hishop.Weixin.MP.Domain;

namespace Hishop.Weixin.MP.Response
{
    /// <summary>
    /// œÏ”¶“Ù¿÷œ˚œ¢
    /// </summary>
    public class MusicResponse : AbstractResponse
    {
        public Music Music { get; set; }

        public override ResponseMsgType MsgType
        {
            get { return ResponseMsgType.Music; }
        }
    }
}
