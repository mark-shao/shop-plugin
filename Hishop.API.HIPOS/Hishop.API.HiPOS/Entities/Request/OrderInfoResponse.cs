using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.API.HiPOS.Entities.Request
{
    public class OrderInfoResponse
    {
        public string id { get; set; }
        public string detail { get; set; }
        public string amount { get; set; }
        public string paid { get; set; }
    }
    public class OrderInfoResult
    {
        public OrderInfoResponse order_info_response { get; set; }
    }




    public class ConfirmingResponse
    {
        public string result { get; set; }       
    }
    public class ConfirmingResult
    {
        public ConfirmingResponse confirming_response { get; set; }
    }
}
