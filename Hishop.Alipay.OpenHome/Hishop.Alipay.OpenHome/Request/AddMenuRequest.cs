
using Newtonsoft.Json;
namespace Hishop.Alipay.OpenHome.Request
{
    public class AddMenuRequest :  IRequest
    {

        private Model.Menu menu;

        public AddMenuRequest(Model.Menu menu)
        {
            this.menu = menu;
        }

        public string GetMethodName()
        {
            return "alipay.mobile.public.menu.add";
        }

        public string GetBizContent()
        {
            var setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            return Newtonsoft.Json.JsonConvert.SerializeObject(menu, setting);
        }
    }
}
