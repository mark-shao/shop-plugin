using Hishop.API.HiPOS.CommonAPIs;
using HiShop.API.Setting.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hishop.API.HiPOS.Test
{
    [TestClass]
    public class TokenTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string appId = "";
            string appSecret = "";
            AccessTokenContainer.Register(appId, appSecret);
            var accessTokenOrAppId = AccessTokenContainer.GetToken(appId);           
        }

    }
}
