using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hishop.Alipay.OpenHome.Model;
using Hishop.Alipay.OpenHome.Response;
using Hishop.Alipay.OpenHome.Request;
using Hishop.Alipay.OpenHome;

namespace UnitTest
{
    [TestClass]
    public class MessageTest
    {
        [TestMethod]
        public void CreateMessageTest()
        {

            Articles article = new Articles()
            {
                Item =
                  new Item()
                  {
                      Description = "这是说明",
                      Title = "这是标题",
                  }

            };

            string appid = "2014053000006249";
            string toUserId = "Ezyz2KVVH9nDMq6sGbcn7XetTiEuN4c5FHa1xAIZOtQRD3k4dA27E0+R2LADp-Tr01";

            IRequest request = new MessagePushRequest(appid, toUserId, article, 1);

            AlipayOHClient client = Utility.GeDefaultAlipayOHClient();
            MessagePushResponse messageSendResponse = client.Execute<MessagePushResponse>(request);
            Assert.IsTrue(messageSendResponse.alipay_mobile_public_message_push_response.code == "200");

        }
    }
}
