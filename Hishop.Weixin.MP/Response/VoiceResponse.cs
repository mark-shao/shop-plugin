using System;
using System.Collections.Generic;
using Hishop.Weixin.MP.Domain;

namespace Hishop.Weixin.MP.Response
{
    /// <summary>
    /// œÏ”¶”Ô“Ùœ˚œ¢
    /// </summary>
    public class VoiceResponse : AbstractResponse
    {
        public Voice Voice { get; set; }

        public override ResponseMsgType MsgType
        {
            get { return ResponseMsgType.Voice; }
        }
    }
}
