using System;
using System.Collections.Generic;
using System.Text;
using Hishop.Plugins;
using System.Collections.Specialized;
using System.Xml;
using System.Globalization;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;

namespace Hishop.Plugins.OpenId.AliPay
{
    public class AliPayNotify : OpenIdNotify
    {
        private readonly SortedDictionary<string, string> parameters;
        public AliPayNotify(NameValueCollection _parameters)
        {
            parameters = new SortedDictionary<string, string>();
            string[] requestItem = _parameters.AllKeys;

            for (int i = 0; i < requestItem.Length; i++)
            {
                parameters.Add(requestItem[i], _parameters[requestItem[i]]);
            }
            parameters.Remove("HIGW");
            parameters.Remove("HITO");
        }

        private string CreateUrl(XmlNode configNode)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "https://mapi.alipay.com/gateway.do?service=notify_verify&partner={0}&notify_id={1}",
                configNode.SelectSingleNode("Partner").InnerText, parameters["notify_id"]
                );
        }

        public override void Verify(int timeout, string configXml)
        {
            bool isValid;
            string msg = null;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);
            string appId = "", key = "", publicKey = "";
            if (doc != null)
            {
                var node = doc.SelectSingleNode("Partner");
                if (node != null)
                    appId = node.InnerText;
                node = doc.SelectSingleNode("Key");
                if (node != null)
                    key = node.InnerText;
                node = doc.SelectSingleNode("PublicKey");
                if (node != null)
                    publicKey = node.InnerText;
            }
            DefaultAopClient client = new DefaultAopClient("https://openapi.alipay.com/gateway.do", appId, key, "json", "1.0", "RSA2", publicKey, "UTF-8", false);
            AlipaySystemOauthTokenRequest request = new AlipaySystemOauthTokenRequest();
            request.set("2e4248c2f50b4653bf18ecee3466UC18");
            request.setGrantType("authorization_code");
            //try
            //{
            //    AlipaySystemOauthTokenResponse oauthTokenResponse = alipayClient.execute(request);
            //    System.out.println(oauthTokenResponse.getAccessToken());
            //}
            //catch (AlipayApiException e)
            //{
            //    //处理异常
            //    e.printStackTrace();
            //}
            // 通知验证
            try
            {
                isValid = bool.Parse(GetResponse(CreateUrl(doc.FirstChild), timeout));
            }
            catch
            {
                isValid = false;
                msg = "支付宝通知消息验证未通过";
            }

            if (isValid)
            {
                Dictionary<string, string> paras = Globals.Parameterfilter(parameters);
                string preSignStr = Globals.CreateLinkstring(paras);
                string mysign = Globals.BuildSign(paras, doc.FirstChild.SelectSingleNode("Key").InnerText, "MD5", "utf-8");
                isValid = (mysign == parameters["sign"]);

                if (!isValid)
                {
                    msg = "支付宝签名验证未通过";
                }
            }

            if (isValid)
                OnAuthenticated(parameters["user_id"]);
            else
                OnFailed(msg);
        }

    }
}