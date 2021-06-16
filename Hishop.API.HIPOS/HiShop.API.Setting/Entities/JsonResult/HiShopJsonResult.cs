
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HiShop.API.Setting.Entities
{
    public class Error
    {
        public string message { get; set; }
        public int code { get; set; }
    }
    public interface IHiShopJsonResult
    {
        Error error { get; set; }
    }

    public class HiShopJsonResult : IHiShopJsonResult
    {
        public Error error { get; set; }
    }



    public interface IPaging
    {
        int page { get; set; }
        int page_size { get; set; }        
    }
}
