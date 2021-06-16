using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.Net;
using System.Web.Security;
using Newtonsoft.Json;
using LitJson;
using System.Collections.Specialized;
using Hishop.Weixin.Pay.Domain;
using System.Collections;
namespace Hishop.Weixin.Pay.Lib
{
    public class JsApiPay
    {
        /// <summary>
        /// 保存页面对象，因为要在类的方法中使用Page的Request对象
        /// </summary>
        private static Page page { get; set; }
        /// <summary>
        /// AppSecret
        /// </summary>
        private string appSecret;
        /// <summary>
        /// appId
        /// </summary>
        private string appId;
        /// <summary>
        /// openId
        /// </summary>
        private string openId;
        /// <summary>
        /// access_token
        /// </summary>
        private string access_token;
        /// <summary>
        /// 密钥
        /// </summary>
        private string Key;
        /// <summary>
        /// 代理URL
        /// </summary>
        private string PROXY_URL;
        /// <summary>
        /// 统一下单接口返回结果
        /// </summary>
        public static WxPayData unifiedOrderResult { get; set; }

        public JsApiPay()
        {
        }

        public static IDictionary<string, string> InitParamDict(NameValueCollection nv)
        {
            IDictionary<string, string> param = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> item in nv)
            {
                param.Add(item.Key, item.Value);
            }
            return param;

        }
        /// <summary>
        /// 获取code
        /// </summary>
        /// <param name="page"></param>
        /// <param name="appId"></param>
        public static void GetCode(Page page, string appId, bool appendLog = false)
        {
            //构造网页授权获取code的URL
            string redirect_uri = HttpUtility.UrlEncode(page.Request.Url.ToString());
            WxPayData data = new WxPayData();
            data.SetValue("appid", appId);
            data.SetValue("redirect_uri", redirect_uri);
            data.SetValue("response_type", "code");
            data.SetValue("scope", "snsapi_base");
            data.SetValue("state", "STATE" + "#wechat_redirect");
            string url = "https://open.weixin.qq.com/connect/oauth2/authorize?" + data.ToUrl();
            if (appendLog)
                WxPayLog.AppendLog(paramDict, url, redirect_uri, "跳转去获取code页面", LogType.GetTokenOrOpenID);

            //触发微信返回code码         
            page.Response.Redirect(url);//Redirect函数会抛出ThreadAbortException异常，不用处理这个异常
        }

        private static IDictionary<string, string> paramDict = new Dictionary<string, string>();
        /// <summary>
        /// 网页授权获取用户基本信息的全部过程
        /// 详情请参看网页授权获取用户基本信息：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
        /// 第一步：利用url跳转获取code
        /// 第二步：利用code去获取openid和access_token
        /// </summary>
        public static NameValueCollection GetOpenidAndAccessToken(Page page, string appId, string appSecret, bool appendLog = false)
        {
            if (!string.IsNullOrEmpty(page.Request.QueryString["code"]))
            {
                //获取code码，以获取openid和access_token
                string code = page.Request.QueryString["code"];
                return GetOpenidAndAccessTokenFromCode(code, page, appId, appSecret);
            }
            else
            {
                //构造网页授权获取code的URL
                string host = page.Request.Url.Host;
                string path = page.Request.Path;
                string redirect_uri = HttpUtility.UrlEncode(page.Request.Url.ToString());
                WxPayData data = new WxPayData();
                data.SetValue("appid", appId);
                data.SetValue("redirect_uri", redirect_uri);
                data.SetValue("response_type", "code");
                data.SetValue("scope", "snsapi_base");
                data.SetValue("state", "STATE" + "#wechat_redirect");
                string url = "https://open.weixin.qq.com/connect/oauth2/authorize?" + data.ToUrl();
                if (appendLog)
                    WxPayLog.AppendLog(paramDict, url, page.Request.Url.ToString(), "未获取到code,跳转去获取code页面", LogType.GetTokenOrOpenID);
                try
                {
                    //触发微信返回code码         
                    page.Response.Redirect(url);//Redirect函数会抛出ThreadAbortException异常，不用处理这个异常
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                }
            }
            return null;
        }

        /// <summary>
        ///通过code换取网页授权access_token和openid的返回数据，正确时返回的JSON数据包如下：
        ///{
        ///"access_token":"ACCESS_TOKEN",
        ///"expires_in":7200,
        ///"refresh_token":"REFRESH_TOKEN",
        ///"openid":"OPENID",
        ///"scope":"SCOPE",
        ///"unionid": "o6_bmasdasdsad6_2sgVt7hMZOPfL"
        ///}
        ///其中access_token可用于获取共享收货地址
        ///openid是微信支付jsapi支付接口统一下单时必须的参数
        ///更详细的说明请参考网页授权获取用户基本信息：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
        ///@失败时抛异常WxPayException
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static NameValueCollection GetOpenidAndAccessTokenFromCode(string code, Page page, string appId, string appSecret, bool appendLog = false)
        {
            NameValueCollection nv = new NameValueCollection();
            nv.Add("OpenID", "");
            nv.Add("Access_Token", "");
            string openId = "", access_token = "";
            try
            {
                //构造获取openid及access_token的url
                WxPayData data = new WxPayData();
                data.SetValue("appid", appId);
                data.SetValue("secret", appSecret);
                data.SetValue("code", code);
                data.SetValue("grant_type", "authorization_code");
                string url = "https://api.weixin.qq.com/sns/oauth2/access_token?" + data.ToUrl();

                //请求url以获取数据
                string result = HttpService.Get(url, "");

                //保存access_token，用于收货地址获取
                JsonData jd = JsonMapper.ToObject(result);
                access_token = (string)jd["access_token"];

                //获取用户openid
                openId = (string)jd["openid"];
                nv.Set("OpenID", openId);
                nv.Set("Access_Token", access_token);

                if (!paramDict.ContainsKey("appid")) paramDict.Add("appid", appId);
                if (!paramDict.ContainsKey("secret")) paramDict.Add("secret", appSecret);
                if (!paramDict.ContainsKey("code")) paramDict.Add("code", code);
                if (!paramDict.ContainsKey("grant_type")) paramDict.Add("grant_type", "authorization_code");
                if (!paramDict.ContainsKey("RequestUrl")) paramDict.Add("RequestUrl", url);
                if (!paramDict.ContainsKey("OpenID")) paramDict.Add("OpenID", openId);
                if (!paramDict.ContainsKey("Access_Token")) paramDict.Add("Access_Token", access_token);
                if (appendLog)
                    WxPayLog.AppendLog(paramDict, "", page.Request.Url.ToString(), "根据code获取OpenId和AccessToken", LogType.GetTokenOrOpenID);
                return nv;

            }
            catch (Exception ex)
            {
                if (!paramDict.ContainsKey("appid")) paramDict.Add("appid", appId);
                if (!paramDict.ContainsKey("secret")) paramDict.Add("secret", appSecret);
                if (!paramDict.ContainsKey("code")) paramDict.Add("code", code);
                if (!paramDict.ContainsKey("grant_type")) paramDict.Add("grant_type", "authorization_code");
                if (!paramDict.ContainsKey("OpenID")) paramDict.Add("OpenID", openId);
                if (!paramDict.ContainsKey("Access_Token")) paramDict.Add("Access_Token", access_token);
                WxPayLog.AppendLog(paramDict, "", page.Request.Url.ToString(), "根据code获取OpenId和AccessToken" + ex.Message, LogType.GetTokenOrOpenID);
                throw new WxPayException(ex.ToString());
            }
        }



        /// <summary>
        ///从统一下单成功返回的数据中获取微信浏览器调起jsapi支付所需的参数，
        /// 微信浏览器调起JSAPI时的输入参数格式如下：
        /// {
        /// "appId" : "wx2421b1c4370ec43b",     //公众号名称，由商户传入     
        /// "timeStamp":" 1395712654",         //时间戳，自1970年以来的秒数     
        /// "nonceStr" : "e61463f8efa94090b1f366cccfbbb444", //随机串     
        /// "package" : "prepay_id=u802345jgfjsdfgsdg888",     
        /// "signType" : "MD5",         //微信签名方式:    
        /// "paySign" : "70EA570631E4BB79628FBCA90534C63FF7FADD89" //微信签名 
        ///}
        ///@return string 微信浏览器调起JSAPI时的输入参数，json格式可以直接做参数用
        ///更详细的说明请参考网页端调起支付API：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_7
        /// </summary>
        /// <returns></returns>

        public static string GetJsApiParameters(PayConfig config)
        {
            WxPayData jsApiParam = new WxPayData();
            jsApiParam.SetValue("appId", unifiedOrderResult.GetValue("appid"));
            jsApiParam.SetValue("timeStamp", WxPayApi.GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", WxPayApi.GenerateNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + unifiedOrderResult.GetValue("prepay_id"));
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign(config.Key));

            string parameters = jsApiParam.ToJson();


            return parameters;
        }


        /// <summary>
        /// 获取收货地址js函数入口参数,详情请参考收货地址共享接口：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_9
        /// </summary>
        /// <returns>共享收货地址js函数需要的参数，json格式可以直接做参数使用</returns>
        public static string GetEditAddressParameters(string appId, string access_token, bool appendLog = false)
        {
            string parameter = "";
            try
            {
                string host = page.Request.Url.Host;
                string path = page.Request.Path;
                string queryString = page.Request.Url.Query;
                //这个地方要注意，参与签名的是网页授权获取用户信息时微信后台回传的完整url
                string url = "http://" + host + path + queryString;

                //构造需要用SHA1算法加密的数据
                WxPayData signData = new WxPayData();
                signData.SetValue("appid", appId);
                signData.SetValue("url", url);
                signData.SetValue("timestamp", WxPayApi.GenerateTimeStamp());
                signData.SetValue("noncestr", WxPayApi.GenerateNonceStr());
                signData.SetValue("accesstoken", access_token);
                string param = signData.ToUrl();

                //SHA1加密
                string addrSign = FormsAuthentication.HashPasswordForStoringInConfigFile(param, "SHA1");

                //获取收货地址js函数入口参数
                WxPayData afterData = new WxPayData();
                afterData.SetValue("appId", appId);
                afterData.SetValue("scope", "jsapi_address");
                afterData.SetValue("signType", "sha1");
                afterData.SetValue("addrSign", addrSign);
                afterData.SetValue("timeStamp", signData.GetValue("timestamp"));
                afterData.SetValue("nonceStr", signData.GetValue("noncestr"));

                //转为json格式
                parameter = afterData.ToJson();
                paramDict.Add("paramJson", parameter);
                if (appendLog)
                    WxPayLog.AppendLog(paramDict, addrSign, page.Request.Url.ToString(), "获取收货地址js函数入口参数", LogType.GetOrEditAddress);
            }
            catch (Exception ex)
            {
                WxPayLog.AppendLog(paramDict, "", page.Request.Url.ToString(), "获取收货地址js函数入口参数:" + ex.Message, LogType.GetOrEditAddress);
                throw new WxPayException(ex.ToString());
            }

            return parameter;
        }
    }
}