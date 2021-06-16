using System;
using System.Collections.Generic;
using System.Text;
using Hishop.Plugins;
using Aop.Api;
using Aop.Api.Domain;
using System.Web;

namespace Hishop.Plugins.OpenId.AliPay
{
    [Plugin("支付宝快捷登录", Sequence = 1)]
    public class AliPayService : OpenIdService
    {

        public AliPayService(string returnUrl)
        {
            this.returnUrl = returnUrl;
        }
        #region 常量
        private const string Gateway = "https://openapi.alipay.com/gateway.do";	//'支付接口
        private const string V = "1.0";//版本
        private const string SignType = "RSA2";//签名类型
        private const string Format = "json";//请求参数格式
        private const string Input_charset_UTF8 = "UTF-8";//编码格式UTF-8
        #endregion
        public AliPayService()
        {
        }
        /// <summary>
        /// 商户号ID
        /// </summary>
        [ConfigElement("商户号/AppId", Nullable = false)]
        public string Partner { set; get; }

        /// <summary>
        /// 商户私钥,读取rsa_private_key.pem的文件内容
        /// </summary>
        [ConfigElement("商户私钥", Nullable = false)]
        public string Key { set; get; }
        /// <summary>
        /// 商户私钥，获取开放平台密钥中的支付宝公钥(SHA256withRsa)
        /// </summary>
        [ConfigElement("商户公钥", Nullable = true)]
        public string PublicKey { set; get; }

        private readonly string returnUrl;

        public override void Post()
        {
            // DefaultAopClient client = new DefaultAopClient(Gateway, Partner, Key, Format, V, SignType, PublicKey, Input_charset_UTF8, false);
            // 组装业务参数model
            try
            {

                IList<string> scopes = new List<string>();
                scopes.Add("auth_user");
                scopes.Add("auth_base");
                string postUrl = "https://openauth.alipay.com/oauth2/publicAppAuthorize.htm?app_id=" + Partner + "&scope=" + string.Join(",", scopes) + "state=hishop&redirect_uri=" + returnUrl;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Response.Redirect(postUrl);
                }
                else
                {
                    Submit(CreateForm("", postUrl));
                }
                
            
            }
            catch (Exception ex)
            {
                OpenIdLog.writeLog(null, "", "", ex.Message, LogType.AliPay);
            }
            //// 设置同步回调地址
            //request.SetReturnUrl(returnUrl);
            //// 设置异步通知接收地址
            //request.SetNotifyUrl(notifyUrl);
            //// 将业务model载入到request
            //request.SetBizModel(model);
            //SortedDictionary<string, string> tmpParas = new SortedDictionary<string, string>();

            //tmpParas.Add("service", "alipay.auth.authorize");
            //tmpParas.Add("target_service", "user.auth.quick.login");
            //tmpParas.Add("partner", Partner);
            //tmpParas.Add("_input_charset", "utf-8");
            //tmpParas.Add("return_url", returnUrl);

            //Dictionary<string, string> paras = Globals.Parameterfilter(tmpParas);
            //string sign = Globals.BuildSign(paras, Key, "MD5", "utf-8");
            //paras.Add("sign", sign);
            //paras.Add("sign_type", "MD5");

            //StringBuilder sb = new StringBuilder();

            //foreach (KeyValuePair<string, string> temp in paras)
            //{
            //    sb.Append(CreateField(temp.Key, temp.Value));

            //}

            //tmpParas.Clear();
            //paras.Clear();

            //Submit(CreateForm(sb.ToString(), "https://mapi.alipay.com/gateway.do?_input_charset=utf-8"));
        }

        public override string Description
        {
            get { return string.Empty; }
        }

        public override string Logo
        {
            get { return "Hishop.Plugins.OpenId.AliPay.alipayservice.gif"; }
        }

        public override string ShortDescription
        {
            get
            {
                return "<div class=\"loginTabao\"><ul><li class=\"taobaoTitle\">支付宝快捷登录简介：</li>" +
                    "<li class=\"li1\">海量支付宝用户只要登录支付宝账号，即可在您网站下单购物。快捷登录简化用户购物流程 ，提升网站下单率。</li>" +
                    "<li class=\"li2\"><a target=\"_blank\" href=\"http://act.life.alipay.com/systembiz/hishop/?src=hishop\" title=\"在线申请\"><img src=\"../images/kuaijie.gif\"/></a></li>" +
                    "</ul><ul class=\"ul2\"><li class=\"taobaoTitle\">签约提示：</li>" +
                    "<li>目前快捷登录必须与支付宝即时交易、担保交易、双功能接口搭配使用。您必须全新签约快登+接口套餐</li>" +
                    "</ul></div>";
            }
        }

    }
}