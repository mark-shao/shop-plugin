using Hishop.Alipay.OpenHome.Model;
using System;

namespace Hishop.Alipay.OpenHome.Response
{
    /// <summary>
    /// 菜单添加响应
    /// </summary>
    [Serializable]
    public class MenuAddResponse : AliResponse, IAliResponseStatus
    {
        public AliResponseMessage alipay_mobile_public_menu_add_response { get; set; }

        public string Code
        {
            get { return alipay_mobile_public_menu_add_response.code; }
        }

        public string Message
        {
            get { return alipay_mobile_public_menu_add_response.msg; }
        }


    }
}
