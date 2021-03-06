using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hishop.Plugins.Outpay.Weixin
{
    public class WeixinPayResult
    {
        public int Amount { get; set; } //支付金额分
        public int UserId { get; set; }
        public string return_code { get; set; }

        public string return_msg { get; set; }

        public string mch_appid { get; set; }

        public string mchid { get; set; }

        public string device_info { get; set; }

        public string nonce_str { get; set; }

        public string result_code { get; set; }

        public string partner_trade_no { get; set; }

        //交易流水

        public string payment_no { get; set; }


        public string payment_time { get; set; }

        public string err_code { get; set; }
    }
}
