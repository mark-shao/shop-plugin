using System;

namespace Hishop.Alipay.OpenHome.Model
{
    /// <summary>
    /// 阿里响应
    /// </summary>
    [Serializable]
    public abstract class AliResponse
    {
        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }

        public ErrorResponse error_response { get; set; }
    }
}
