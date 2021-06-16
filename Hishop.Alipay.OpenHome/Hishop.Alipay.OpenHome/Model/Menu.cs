using System.Collections.Generic;

namespace Hishop.Alipay.OpenHome.Model
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class Menu:ModelBase
    {
        /// <summary>
        /// 按钮
        /// </summary>
        public IEnumerable<Button> button { get; set; }

    }


}
