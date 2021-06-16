using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Alipay.OpenHome.Request
{
    public class UpdateMenuRequest : IRequest
    {
        private Model.Menu menu;

        public UpdateMenuRequest(Model.Menu menu)
        {
            this.menu = menu;
        }

        public string GetMethodName()
        {
            return "alipay.mobile.public.menu.update";
        }

        public string GetBizContent()
        {
            var setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            return Newtonsoft.Json.JsonConvert.SerializeObject(menu, setting);
        }
    }
}
