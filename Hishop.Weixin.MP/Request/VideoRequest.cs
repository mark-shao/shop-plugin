using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP.Request
{
    /// <summary>
    /// ��Ƶ��Ϣ
    /// </summary>
    public class VideoRequest : AbstractRequest
    {
        /// <summary>
        /// ��Ƶ��Ϣý��id
        /// </summary>
        public int MediaId { get; set; }

        /// <summary>
        /// ��Ƶ��Ϣ����ͼ��ý��id
        /// </summary>
        public int ThumbMediaId { get; set; }
    }
}
