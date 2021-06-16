using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP.Request
{
    /// <summary>
    /// 地理位置消息
    /// </summary>
    public class LocationRequest : AbstractRequest
    {
        /// <summary>
        /// 地理位置维度
        /// </summary>
        public float Location_X { get; set; }

        /// <summary>
        /// 地理位置精度
        /// </summary>
        public float Location_Y { get; set; }

        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label { get; set; }
    }
}
