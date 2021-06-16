using HiShop.API.Setting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiShop.API.HiPOS.AdvancedAPIs.Auth.AuthJson
{
    public class AuthResult : HiShopJsonResult
    {        
        public HishopAuthResponse hishop_auth_response { get; set; }
    }

    public class HishopAuthResponse
    {
        public string message { get; set; }
        public string notify_url { get; set; }
    }
}
