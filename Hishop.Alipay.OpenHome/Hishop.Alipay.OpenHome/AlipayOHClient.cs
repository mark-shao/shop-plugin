using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using Hishop.Alipay.OpenHome.AlipayOHException;
using Hishop.Alipay.OpenHome.Handle;
using Hishop.Alipay.OpenHome.Model;
using Hishop.Alipay.OpenHome.Request;
using Hishop.Alipay.OpenHome.Response;
using Hishop.Alipay.OpenHome.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Hishop.Alipay.OpenHome
{
    public delegate string OnUserFollow(object sender, EventArgs e); 
    public class AlipayOHClient
    {
        const string APP_ID = "app_id";
        const string FORMAT = "format";
        const string METHOD = "method";
        const string TIMESTAMP = "timestamp";
        const string VERSION = "version";
        const string SIGN_TYPE = "sign_type";
        const string ACCESS_TOKEN = "auth_token";
        const string SIGN = "sign";
        const string TERMINAL_TYPE = "terminal_type";
        const string TERMINAL_INFO = "terminal_info";
        const string PROD_CODE = "prod_code";
        const string BIZ_CONTENT = "biz_content";


        private string version;
        private string format;
        private string signType = "RSA";
        string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        string serverUrl;
        string appId;
        string privateKey;
        string pubKey;
        string aliPubKey;
        string charset;
        public Model.AliRequest request;
        WebUtils webUtils = new WebUtils();

        HttpContext context;

        const string SING = "sign", CONTENT = "biz_content", SING_TYPE = "sign_type", SERVICE = "service", CHARSET = "charset";

        /// <summary>
        /// 事件类型
        /// </summary>
        class EventType
        {
            /// <summary>
            /// 验证网关
            /// </summary>
            public const string Verifygw = "verifygw"; 

            /// <summary>
            /// 关注
            /// </summary>
            public const string Follow= "follow";
        }


        public AlipayOHClient(string url, string appId, string aliPubKey, string priKey, string pubKey, string charset = "UTF-8")
        {
            serverUrl = url;
            this.appId = appId;
            privateKey = priKey;
            this.charset = charset;
            this.pubKey = pubKey;
            this.aliPubKey = aliPubKey;
        }

        public AlipayOHClient( string aliPubKey, string priKey, string pubKey, string charset = "UTF-8")
        {
            privateKey = priKey;
            this.charset = charset;
            this.pubKey = pubKey;
            this.aliPubKey = aliPubKey;
        }

        public event OnUserFollow OnUserFollow;//声明用户关注事件

        internal string FireUserFollowEvent()
        {
            return OnUserFollow(this, new EventArgs());
        }

        public void HandleAliOHResponse(HttpContext context)
        {
            this.context = context;
            string sign = context.Request[SING];
            string content = context.Request[CONTENT];
            string singType = context.Request[SING_TYPE];
            string service = context.Request[SERVICE];
             request = Utility.XmlSerialiseHelper.Deserialize<Model.AliRequest>(content);

            IHandle handle = null;
            switch (request.EventType)
            {
                case EventType.Verifygw://网关校验请求
                    handle = new VerifyGateWayHandle();
                    break;
                case EventType.Follow:
                    handle = new UserFollowHandle();
                    break;
            }


            if (handle != null)
            {
                handle.client = this;
                handle.LocalRsaPriKey = privateKey;
                handle.LocalRsaPubKey = pubKey;
                handle.AliRsaPubKey = aliPubKey;
                string response = handle.Handle(content);//处理

                //返回响应
                context.Response.AddHeader("Content-Type", "application/xml");
                context.Response.Write(response);
            }
        }


        public T Execute<T>(IRequest request) where T : AliResponse, IAliResponseStatus
        {
            const string SUCCESS_CODE = "200";

            // 添加协议级请求参数
            Dictionary<string, string> txtParams = new Dictionary<string, string>();
            txtParams.Add(METHOD, request.GetMethodName());
            txtParams.Add(APP_ID, appId);
            txtParams.Add(TIMESTAMP, DateTime.Now.ToString(dateTimeFormat));
            txtParams.Add(SIGN_TYPE, signType);
            txtParams.Add(CHARSET, charset);
            txtParams.Add(BIZ_CONTENT, request.GetBizContent());

            // 添加签名参数
            txtParams.Add(SIGN, AlipaySignature.RSASign(txtParams, privateKey, charset));

            string body = webUtils.DoPost(serverUrl, txtParams, charset);
            T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(body);
            if (t.error_response != null && t.error_response.IsError)
                throw new AliResponseException(t.error_response.code, t.error_response.sub_msg);
            if (t.Code != SUCCESS_CODE)
                throw new AliResponseException(t.Code, t.Message);
            return t;
        }


        /// <summary>
        /// 通过auth_code换取网页，授权access_token
        /// </summary>
        /// <param name="authCode">授权码</param>
        /// <returns></returns>
        public AlipaySystemOauthTokenResponse OauthTokenRequest(string authCode)
        {
            // 封装好的接口请求对象，接口不同请求对象也不同
            AlipaySystemOauthTokenRequest oauthTokenRequest = new AlipaySystemOauthTokenRequest();
            oauthTokenRequest.GrantType = AlipaySystemOauthTokenRequest.AllGrantType.authorization_code;

            // 输入获取到的auth_code
            oauthTokenRequest.Code = authCode;
            AlipaySystemOauthTokenResponse oauthTokenResponse = null;
            try
            {
                IAopClient alipayClient = new DefaultAopClient(serverUrl, appId, privateKey);
                // 执行业务请求
                oauthTokenResponse = alipayClient.Execute(oauthTokenRequest);
            }
            catch (AopException e)
            {

                // 业务调用处理异常
            }
            return oauthTokenResponse;
        }



        public AlipayUserUserinfoShareResponse GetAliUserInfo(string accessToken)
        {
            AlipayUserUserinfoShareRequest userInfoRequest = new AlipayUserUserinfoShareRequest();
            userInfoRequest.AuthToken = accessToken;
            IAopClient alipayClient = new DefaultAopClient(serverUrl, appId, privateKey);
            AlipayUserUserinfoShareResponse response = alipayClient.Execute(userInfoRequest);
            return response;
        }


    }
}
