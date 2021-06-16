using System;
using System.Collections.Generic;
using System.Web;

namespace Hishop.Weixin.Pay.Lib
{
    public class WxPayException : Exception 
    {
        public WxPayException(string msg) : base(msg) 
        {

        }
     }
}