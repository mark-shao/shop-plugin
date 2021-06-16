using System;
using System.Collections.Generic;
using System.Linq;

namespace Hishop.Weixin.MP.Domain.Menu
{
    /// <summary>
    /// 子菜单
    /// </summary>
    public class SubMenu : BaseButton
    {
        /// <summary>
        /// 子按钮数组，按钮个数应为2~5个
        /// </summary>
        public List<SingleButton> sub_button { get; set; }

        public SubMenu()
        {
            sub_button = new List<SingleButton>();
        }

        public SubMenu(string name)
            : this()
        {
            base.name = name;
        }
    }
}
