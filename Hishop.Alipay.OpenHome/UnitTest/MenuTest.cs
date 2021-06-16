using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Response;
using Hishop.Alipay.OpenHome;
using Hishop.Alipay.OpenHome.Model;
using Hishop.Alipay.OpenHome.Request;
using Hishop.Alipay.OpenHome.Response;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    /// <summary>
    /// MenuTest 的摘要说明
    /// </summary>
    [TestClass]
    public class MenuTest
    {
        public MenuTest()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void MenuAddTest()
        {
            Menu menu = new Menu();
            menu.button = new Button[] { 
                new Button() { actionParam = "ZFB_HFCZ1", actionType = "link", name = "话费充值" } ,
                new Button() { actionParam = "ZFB_HFCZ2", actionType ="link", name = "查询",
                    subButton=new Button[]{
                new Button() { actionParam = "ZFB_HFCZ4", actionType = "link", name = "余额查询" } ,
                new Button() { actionParam = "ZFB_HFCZ5", actionType = "link", name = "流量查询" } ,
                    }
                } ,
                new Button() { actionParam = "ZFB_HFCZ3", actionType ="link", name = "最新优惠" } 
            };
            AddMenuRequest request = new AddMenuRequest(menu);

            AlipayOHClient client = Utility.GeDefaultAlipayOHClient();
            MenuAddResponse response = client.Execute<MenuAddResponse>(request);
            Assert.IsTrue(response.alipay_mobile_public_menu_add_response.code == "200");

        }


        [TestMethod]
        public void MenuUpdateTest()
        {
            Menu menu = new Menu();
            menu.button = new Button[] { 
                new Button() { actionParam = "ZFB_HFCZ1", actionType = "link", name = "话费充值" } ,
                new Button() { name = "查询",
                    subButton=new Button[]{
                new Button() { actionParam = "ZFB_HFCZ4", actionType = "link", name = "余额查询" } ,
                new Button() { actionParam = "ZFB_HFCZ5", actionType = "link", name = "流量查询" } ,
                    }
                } ,
                new Button() { actionParam = "ZFB_HFCZ3", actionType = "link", name = "最新优惠" } 
            };
            UpdateMenuRequest request = new UpdateMenuRequest(menu);


            AlipayOHClient client = Utility.GeDefaultAlipayOHClient();
            MenuUpdateResponse response = client.Execute<MenuUpdateResponse>(request);
            Assert.IsTrue(response.alipay_mobile_public_menu_update_response.code == "200");
        }

        [TestMethod]
        public  void MenuGet()
        {

            //支付宝网关地址
            // -----沙箱地址-----
            string serverUrl = "https://openapi.alipay.com/gateway.do";

            //应用ID
            string appId = "2014061100006416";
            //商户私钥
            string priKey = @"D:\Web\mec\config\rsa_private_key.pem";

            IAopClient client = new DefaultAopClient(serverUrl, appId, priKey);


            AlipayMobilePublicMenuGetRequest req = new AlipayMobilePublicMenuGetRequest();
            AlipayMobilePublicMenuGetResponse res = client.Execute(req);
            System.Console.Write("-------------公众号菜单查询-------------" + "\n\r");
            System.Console.Write("Body:" + res.Body + "\n\r");
        }
    }
}
