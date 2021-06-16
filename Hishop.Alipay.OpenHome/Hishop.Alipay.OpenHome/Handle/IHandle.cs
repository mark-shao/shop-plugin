using Hishop.Alipay.OpenHome.Model;

namespace Hishop.Alipay.OpenHome.Handle
{
    interface IHandle
    {
        string LocalRsaPriKey{get;set;}

        string LocalRsaPubKey { get; set; }

        string AliRsaPubKey { get; set; }

        string Handle(string requestContent);

        AlipayOHClient client { get; set; }
    }
}
