using Aop.Api.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Alipay.OpenHome.Handle
{
    class UserFollowHandle:IHandle
    {


        public string LocalRsaPriKey
        {
            get;
            set;
        }

        public string LocalRsaPubKey
        {
            get;
            set;
        }

        public string AliRsaPubKey
        {
            get;
            set;
        }


        public AlipayOHClient client
        {
            get;
            set;
        }


        public string Handle(string requestContent)
        {

            string content = client.FireUserFollowEvent();
            string responseContent = AlipaySignature.encryptAndSign(content, this.AliRsaPubKey, this.LocalRsaPriKey, "UTF-8", false, true);
            return responseContent;
        }
    }
}
