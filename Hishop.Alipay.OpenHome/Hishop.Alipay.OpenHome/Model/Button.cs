using System.Collections.Generic;

namespace Hishop.Alipay.OpenHome.Model
{
    /// <summary>
    /// 按钮
    /// </summary>
    public class Button
    {
        /// <summary>
        /// 按钮名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 按钮标识
        /// </summary>
        public string actionParam { get; set; }

        /// <summary>
        /// 动作类型 link,out
        /// </summary>
        public string actionType { get; set; }

        public string authType { get { return "loginAuth"; } }

        /// <summary>
        /// 子按钮
        /// </summary>
        public IEnumerable<Button> subButton { get; set; }

    }
}
