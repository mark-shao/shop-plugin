using System;
using Hishop.Plugins;
using System.Globalization;
using System.IO;
using System.Web;
using SDK.yop.client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using SDK.yop.utils;

namespace Hishop.Plugins.Payment.YeePay
{
    /// <summary>
    /// 易宝支付
    /// </summary>
    [Plugin("易宝支付")]
    public class YeepayRequest : PaymentRequest
    {

        public YeepayRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            this.amount = amount.ToString("F", CultureInfo.InvariantCulture);
            this.orderId = orderId;
            this.subject = subject;
            this.body = body;
            this.buyerEmail = buyerEmail;
            this.returnUrl = returnUrl;
            this.notifyUrl = notifyUrl;
        }

        public YeepayRequest()
        {
        }

        #region 常量
        private const string Cur = "CNY";
        private const string SMctProperties = "YeePay";

        /// <summary>
        /// 需要填写送货信息 0：不需要  1:需要
        /// </summary>
        private const string AddressFlag = "0";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// 商家ID（商户号）
        /// </summary>
        [ConfigElement("商户编号(merchantNo)", Nullable = false)]
        public string MerchantId { get; set; }

        /// <summary>
        /// 商家密钥
        /// </summary>
        [ConfigElement("商家私钥", Nullable = false)]
        public string KeyValue { get; set; }

        /// <summary>
        /// 商家的交易定单号
        /// </summary>
        private readonly string orderId="";

        /// <summary>
        /// 订阅金额
        /// </summary>
        private readonly string amount = "";

        /// <summary>
        /// 标题
        /// </summary>
        private readonly string subject = "";

        /// <summary>
        /// 内容
        /// </summary>
        private readonly string body = "";

        /// <summary>
        /// 用户邮箱(可传值用户Id)
        /// </summary>
        private readonly string buyerEmail = "";

        /// <summary>
        /// 同步结果通知地址
        /// </summary>
        private readonly string returnUrl = "";

        /// <summary>
        /// 异步通知地址
        /// </summary>
        private readonly string notifyUrl = "";

        /// <summary>
        /// 合作伙伴商户号(合作伙伴账号（parentMerchantNo：主商户编号）)
        /// </summary>
        [ConfigElement("主商户编号(parentMerchantNo)")]
        public string Pid { get; set; }

        /// <summary>
        /// 类型(1基础版,其他标准版)
        /// </summary>
        [ConfigElement("类型(1基础版,其他标准版)")]
        public string YeePayType { get; set; }


        public override void SendRequest()
        {
            //string str = Buy.CreateUrl(MerchantId, KeyValue, orderId, amount, Cur, "", merchantCallbackURL,
            //                                AddressFlag, SMctProperties, "");
            //Buy.AppendLog(str);
            //RedirectToGateway(str);

            string token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                //Buy.AppendLog("获取token失败");
                return;
            }
            
            Buy.SetPrivateKeyTxt(KeyValue);//私钥重新设置一下保存在记事本里

            #region //拼接收银台

            string privatekey = KeyValue;
            string merchantNo = MerchantId;
            string timesTampstr = GetTimeStamp();
            Dictionary<string, string> cashter = new Dictionary<string, string>();

            cashter.Add("merchantNo", merchantNo);//收款商户编号
            cashter.Add("token", token);//订单token
            cashter.Add("timestamp", timesTampstr);//以请求的发起时间转换为时间戳，用来控制一次收银台操作的最大时间, 精确到秒
            cashter.Add("directPayType", "");//设置该参数后，直接调用支付工具，不显示易宝移动收银台页面。枚举值：WECHAT： WX支付ALIPAY： ZFB 支付YJZF： 易宝一键支付CFL: 分期支付DBFQ: 担保分期网银直联编码请参考 5.1、5.2部分值可不传，但参数参与排序生成 sign
            cashter.Add("cardType", "");//卡种(限制交易的卡类型，当前只能限制一键支付卡类型，网银请联系运营配置，枚举值：DEBIT：借记卡CREDIT：信用卡该项目不传时，不限制支付卡种值可不传，但参数参与排序生成 sign)
            cashter.Add("userNo", buyerEmail);//用户标识，必填，用户在商户的唯一标识。
            cashter.Add("userType", "");//用户标识类型(IMEI ：IMEI MAC：MAC 地USER_ID ：用户 IDEMAIL ：用户 EmailPHONE：用户手机号ID_CARD ：用户身份证号)
            cashter.Add("ext", "");//扩展字段

            string getUrl = GetUrl(cashter, privatekey);

            //Buy.AppendLog("getUrl:"+ getUrl);
            #endregion

            string url = "https://cash.yeepay.com/cashier/std?" + getUrl;
            RedirectToGateway(url);
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        private string GetToken()
        {
            //Buy.AppendLog("222");
            string privatekey = KeyValue;//私钥
            string yopPublicKey = Buy.YopPublicKey;//公钥
            string merchantNo = MerchantId;//收单商户商编
            string parentMerchantNo = Pid;//系统商或者平台商商编，如果是单商户，和收单商户商编保持一致
            if (string.IsNullOrEmpty(parentMerchantNo))
                parentMerchantNo = merchantNo;
            string appKey = "OPR:" + MerchantId;
            //Buy.AppendLog(string.Format("MerchantId:{0}--parentMerchantNo:{1}--appKey:{2}", merchantNo, parentMerchantNo, appKey));

            string token = string.Empty;
            YopRequest request = new YopRequest(appKey, privatekey,
                   "https://open.yeepay.com/yop-center", yopPublicKey);
            //Buy.AppendLog("privatekey:" + privatekey);

            request.addParam("parentMerchantNo", parentMerchantNo);
            request.addParam("merchantNo", merchantNo);
            request.addParam("orderId", orderId);
            request.addParam("orderAmount", amount);
            request.addParam("timeoutExpress", "");//订单有效期,单位：分钟，默认 24小时，最小1分钟，1 最大180天
            request.addParam("requestDate", "");//请求时间,用于计算订单有效期，格式 yyyy-MM - dd HH: mm: ss，不传默认为易宝接收到请求的时间
            request.addParam("redirectUrl", returnUrl);
            request.addParam("notifyUrl", notifyUrl);
            request.addParam("goodsParamExt", "{\"goodsName\":\""+ subject + "\",\"goodsDesc\":\""+ body + "\"}");
            request.addParam("paymentParamExt", "{\"goods_id\":\"" + orderId + "\"}");//支付扩展参数Json
            request.addParam("industryParamExt", "");//行业扩展参数Json
            request.addParam("memo", "");//定义对账备注，商户可以自定义自身业务需要使用的字段：如对账时定义该订单应属的会计科目。
            request.addParam("riskParamExt", "");
            request.addParam("csUrl", "");//清算成功服务器回调地址

            //System.Diagnostics.Debug.WriteLine(request.toQueryString());

            //Buy.AppendLog("开始:---");
            ////YopResponse response = YopClient3.postRsa("/rest/v1.0/std/trade/order", request);//基础版本
            //YopResponse response = YopClient3.postRsa("/rest/v1.0/sys/trade/order", request);//标准版本
            YopResponse response = new YopResponse();
            if (YeePayType=="1")
                response = YopClient3.postRsa("/rest/v1.0/std/trade/order", request);//基础版本
            else
                response = YopClient3.postRsa("/rest/v1.0/sys/trade/order", request);//标准版本
            //Buy.AppendLog("validSign:" + response.validSign);

            if (response.validSign)
            {

                JObject obj = (JObject)JsonConvert.DeserializeObject(response.stringResult);  //序列化
                token = Convert.ToString(obj["token"]);
                Buy.AppendLog("订单号:" + orderId + "返回结果签名验证成功:token： " + token + " | YopRequestId: " + YopConfig.getYopRequestId() + " |result:token：" + response.stringResult);
            }
            else
            {
                string errorText = JsonConvert.SerializeObject(response.error);
                Buy.AppendLog("订单号:" + orderId + "请求失败，返回结果: " + errorText + " < br > ");
            }
            return token;
        }


        public override bool IsMedTrade
        {
            get { return false; }
        }

        public override void SendGoods(string tradeno, string logisticsName, string invoiceno, string transportType)
        {

        }

        public override string Description
        {
            get { return string.Empty; }
        }

        public override string Logo
        {
            get { return string.Empty; }
        }

        public override string ShortDescription
        {
            get { return string.Empty; }
        }
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        public static string GetUrl(Dictionary<string, string> cashter, string privateKey)
        {
            string getSign = "";

            foreach (KeyValuePair<string, string> kvp in cashter)
            {

                getSign += kvp.Key + "=" + kvp.Value + "&";
            }

            getSign = getSign.Substring(0, getSign.Length - 1);
            string content = getSign;

            string sign = SHA1withRSA.signRsa(content, privateKey);
            sign = sign.Replace("&timestamp", "&amp;timestamp");
            string url = content + "&sign=" + sign;
            return url;
        }

    }
}