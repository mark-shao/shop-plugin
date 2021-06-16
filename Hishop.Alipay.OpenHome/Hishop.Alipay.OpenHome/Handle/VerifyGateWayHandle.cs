using Aop.Api.Util;
using Hishop.Alipay.OpenHome.Model;

namespace Hishop.Alipay.OpenHome.Handle
{
    class VerifyGateWayHandle : IHandle
    {
        public string Handle(string requestContent)
        {
            string content = string.Format("<success>true</success><biz_content>{0}</biz_content>", Utility.RsaFileHelper.GetRSAKeyContent(this.LocalRsaPubKey, true));
            string responseContent = AlipaySignature.encryptAndSign(content, this.AliRsaPubKey, this.LocalRsaPriKey, "UTF-8", false, true);
            return responseContent;
        }

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
    }
}
