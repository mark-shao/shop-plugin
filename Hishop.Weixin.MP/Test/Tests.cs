using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hishop.Weixin.MP.Domain.Menu;
using System.Web.Script.Serialization;
using Hishop.Weixin.MP.Domain;
using Hishop.Weixin.MP.Api;

namespace Hishop.Weixin.MP.Test
{
    public class Tests
    {
        const string AppID = "wxe7322013e6e964b8";
        const string AppSecret = "9e4e5617c1b543e3164befd1952716b0";

        public string GetToken()
        {
            string json = TokenApi.GetToken(AppID, AppSecret);

            return new JavaScriptSerializer().Deserialize<Token>(json).access_token;
        }

        public string CreateMenu()
        {
            string token = this.GetToken();
            string menuJson = this.GetMenuJson();

            return MenuApi.CreateMenus(token, menuJson);
        }

        public string GetMenu()
        {
            string token = this.GetToken();

            return MenuApi.GetMenus(token);
        }

        public string DeleteMenu()
        {
            string token = this.GetToken();

            return MenuApi.DeleteMenus(token);
        }

        public string GetMenuJson()
        {
            Menu root = new Menu();

            var btn1 = new SingleClickButton() { name = "热卖商品", key = "123" };

            var btn2 = new SingleClickButton() { name = "推荐商品", key = "SINGER" };

            var btn3 = new SingleViewButton() { name = "会员卡", url = "www.baidu.com" };

            var btn4 = new SingleViewButton() { name = "积分商城", url = "www.baidu.com" };

            var btn5 = new SubMenu() { name = "个人中心" };
            btn5.sub_button.Add(btn3);
            btn5.sub_button.Add(btn4);

            root.menu.button.Add(btn1);
            root.menu.button.Add(btn2);
            root.menu.button.Add(btn5);

            return new JavaScriptSerializer().Serialize(root.menu);
        }

    }
}
