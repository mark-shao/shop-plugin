using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Net;
using System.IO;
using HiShop.API.HiPOS.AdvancedAPIs.Auth;
using Hishop.API.HiPOS.Entities.Request;
using System.Web.Script.Serialization;

namespace Hishop.API.HiPOS.Test
{
    [TestClass]
    public class ModelTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            var s=js.Serialize(new OrderInfoResult { order_info_response = new OrderInfoResponse { amount = "amount", paid = "paid", id = "id", detail = "detail" } });
        }
    }
}
