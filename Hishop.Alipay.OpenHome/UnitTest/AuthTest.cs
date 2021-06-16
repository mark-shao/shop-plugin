using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hishop.Alipay.OpenHome;
using Aop.Api.Response;

namespace UnitTest
{
    [TestClass]
    public class AuthTest
    {
        [TestMethod]
        public void OauthTokenRequestTest()
        {
            AlipayOHClient client = Utility.GeDefaultAlipayOHClient();
            string authCode = "6df16146aba6401282492ebcc0263b50";
            AlipaySystemOauthTokenResponse response = client.OauthTokenRequest(authCode);
            Assert.IsTrue(!response.IsError);
        }


        [TestMethod]
        public void AliUserInfoShareRequestTest()
        {
            AlipayOHClient client = Utility.GeDefaultAlipayOHClient();
            string token = "publicpB2338bbf2b58a4646847f1b901a51499a";
            AlipayUserUserinfoShareResponse response = client.GetAliUserInfo(token);
            Assert.IsTrue(!response.IsError);
        }



    }
}
