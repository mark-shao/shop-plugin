using Hishop.API.HiPOS.AdvancedAPIs.Merchant;
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
    public class MerchantTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string appId = "";
            string appSecret = "";
            AccessTokenContainer.Register(appId, appSecret);
            //var accessTokenOrAppId = AccessTokenContainer.GetToken(appId);
            var accessTokenOrAppId = "";
            MerchantApi.UpdateMerchant(accessTokenOrAppId, "1", "sdfasdf", "asdf", "14789925201");
        }

        [TestMethod]
        public void TestMethod2()
        {
            string appId = "";
            string appSecret = "";
            AccessTokenContainer.Register(appId, appSecret);
            var accessTokenOrAppId = AccessTokenContainer.GetToken(appId);
            var result = MerchantApi.GetAlipayKey(accessTokenOrAppId, "13");
        }

        [TestMethod]
        public void TestMethod3()
        {
            string appId = "";
            string appSecret = "";
            AccessTokenContainer.Register(appId, appSecret);
            var accessTokenOrAppId = AccessTokenContainer.GetToken(appId);

            var result = MerchantApi.SetHishopO2O(accessTokenOrAppId, "1", "http://ysc.hishop.com.cn/API/HiPOSAPI.ashx?action=auth", "http://ysc.hishop.com.cn/API/HiPOSAPI.ashx?action=auth", "http://ysc.hishop.com.cn/API/HiPOSAPI.ashx?action=auth");
        }

        [TestMethod]
        public void TestMethod4()
        {
            string appId = "";
            string appSecret = "";
            AccessTokenContainer.Register(appId, appSecret);
            var accessTokenOrAppId = AccessTokenContainer.GetToken(appId);
            var s = @"";

            var result = MerchantApi.SetPayments(accessTokenOrAppId, "5551", "2088021647869650",s, "", "", "", "");
        }

        [TestMethod]
        public void TestMethod5()
        {
            string appId = "";
            string appSecret = "";
            AccessTokenContainer.Register(appId, appSecret);
            var accessTokenOrAppId = AccessTokenContainer.GetToken(appId);

            var result = MerchantApi.GetAuthCode(accessTokenOrAppId, "1", "asdf213lsd095kv");
        }

        [TestMethod]
        public void TestMethod6()
        {
            string appId = "";
            string appSecret = "";
            AccessTokenContainer.Register(appId, appSecret);
            var accessTokenOrAppId = AccessTokenContainer.GetToken(appId);

            var result = MerchantApi.GetHishopTrades(accessTokenOrAppId, "13", "大剧院店", "", "");
        }

        [TestMethod]
        public void TestMethod7()
        {
            string appId = "";
            string appSecret = "";
            AccessTokenContainer.Register(appId, appSecret);
            var accessTokenOrAppId = AccessTokenContainer.GetToken(appId);

            var result = MerchantApi.GetHishopTradesDetail(accessTokenOrAppId, "13", "56937a63d5c8c66e0948f58f", "","", false, "", "");
        }
    }
}
