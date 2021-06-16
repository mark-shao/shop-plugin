using System;
using System.Collections.Generic;
using System.Linq;

namespace Hishop.Weixin.MP.Domain.Menu
{
    public class Menu
    {
        public ButtonGroup menu { get; set; }

        public Menu()
        {
            menu = new ButtonGroup();
        }
    }


    //public class MenuJsonRoot
    //{
    //    public MenuJson menu { get; set; }
    //}

    //public class MenuJson
    //{
    //    public List<ButtonJson> button { get; set; }
    //}

    //public class ButtonJson
    //{
    //    public string type { get; set; }
    //    public string key { get; set; }
    //    public string name { get; set; }
    //    public string url { get; set; }
    //    public List<ButtonJson> sub_button { get; set; }
    //}
}
