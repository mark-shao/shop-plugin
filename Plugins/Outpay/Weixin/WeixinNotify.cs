using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Hishop.Plugins.Outpay.Weixin
{
    public class WeixinNotify : OutpayNotify
    {
        public override bool VerifyNotify(int timeout, string configXml)
        {
            throw new NotImplementedException();
        }

        public override void WriteBack(System.Web.HttpContext context, bool success)
        {
            throw new NotImplementedException();
        }
        public override IList<decimal> GetOrderAmount()
        {
            return null;
        }

        public override IList<string> GetOutpayId()
        {
            return null;
        }

        public override IList<string> GetGatewayOrderId()
        {
            return null;
        }


        public override IList<DateTime> GetPayTime()
        {
            return null;
        }
        public override string GetRemark1()
        {
            return base.GetRemark1();
        }

        public override string GetRemark2()
        {
            return base.GetRemark2();
        }

        protected override string GetResponse(string url, int timeout)
        {
            return base.GetResponse(url, timeout);
        }

        public override IList<string> GetErrMsg()
        {
            throw new NotImplementedException();
        }

        public override IList<bool> GetStatus()
        {
            throw new NotImplementedException();
        }
        protected override void OnFinished()
        {
            base.OnFinished();
        }

        protected override void OnNotifyVerifyFaild()
        {
            base.OnNotifyVerifyFaild();
        }

        protected override void OnPayment()
        {
            base.OnPayment();
        }
    }
}
