using System;
using System.Text;
using System.Globalization;
using System.Web.Security;
using Hishop.Plugins;
using Com.HisunCmpay;
using System.Collections;
using System.Web;

namespace Hishop.Plugins.Payment.CMPay_D
{
    [Plugin("手机支付即时到帐(双向确认)")]
    public class CMPayDRequest : PaymentRequest
    {

        public CMPayDRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            this.productName = orderId;
            this.callbackUrl = returnUrl;
            this.notifyUrl = notifyUrl;
            this.amount = Convert.ToInt32(amount * 100).ToString(CultureInfo.InvariantCulture);
            this.orderId = orderId;
            this.orderDate = this.merAcDate = date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public CMPayDRequest()
        {
        }

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// 商户编号
        /// 不可空，人民币网关账户号，本参数用来指定接收款项的人民币账号
        /// 请登录快钱系统获取用户编号，用户编号后加01即为人民币网关账户号。
        /// </summary>
        [ConfigElement("商户号", Nullable=false)]
        public string MerchantAcctId { get; set; }

        /// <summary>
        /// 商户密钥
        /// 不可为空，人民币网关密钥
        /// 区分大小写.请与快钱联系索取
        /// </summary>
        [ConfigElement("商户密钥", Nullable = false)]
        public string Key { get; set; }

        //===================================================

        private const string Gateway = "https://ipos.10086.cn/ips/cmpayService";

        // 协议参数
        //
        String ipAddress = IPosMUtil.getIpAddress();                    // IP地址
        String characterSet = "02";                                     //  字符集
        String callbackUrl = string.Empty;                              //  页面返回URL
        String notifyUrl = string.Empty;                                //  后台通知URL
        //String merchantId = GlobalParam.getInstance().merchantId;     //  商户编号
        String requestId = IPosMUtil.getTicks();                        //  商户请求号(订单编号)
        String signType = "MD5";                                        //  签名方式(只能是MD5，RSA)
        String type = "DirectPayConfirm";                               //  接口类型(DirectPayConfirm)
        String version = "2.0.0";                                       //  版本号 2.0.0
        String merchantCert = string.Empty;                             //  不参与签名；如果signType=RSA，此项必输
        String hmac = string.Empty;                                     //  签名数据,获得hmac的方法见签名算法,参数顺序按照表格中从上到下的顺序,但丌包括本参数.         

        // 业务参数
        //
        String amount = "0";                                            // 订单金额(以分为单位)
        String bankAbbr = string.Empty;                                       // 银行代码
        String currency = "00";                                         // 币种(00)
        String orderDate = string.Empty;                                // 订单提交日期(年年年年月月日日)
        String merAcDate = string.Empty;                                // 商户发起请求的会计日期
        String orderId = string.Empty;                                  // 商户系统订单号
        String period = "3";                                            // 有效期数量
        String periodUnit = "03";                                       // 有效期单位
        String merchantAbbr = string.Empty;                             // 商户展示名称
        String productDesc = string.Empty;                              // 商品描述
        String productId = string.Empty;                                // productId
        String productName = string.Empty;                              // productName
        String productNum = string.Empty;                               // productNum
        String reserved1 = string.Empty;                                // 保留字段
        String reserved2 = string.Empty;                                // 保留字段
        String userToken = string.Empty;                                // 待支付的手机号或者手机支付账户昵称

        String showUrl = string.Empty;                                  // 商品展示的url
        String couponsFlag = "00";                                      // 营销工具使用控制

        public override void SendRequest()
        {
            // 生成签名
            String signData = characterSet + callbackUrl + notifyUrl
                        + ipAddress + MerchantAcctId + requestId + signType + type
                        + version + amount + bankAbbr + currency
                        + orderDate + orderId + merAcDate + period + periodUnit + merchantAbbr
                        + productDesc + productId + productName + productNum
                        + reserved1 + reserved2 + userToken
                        + showUrl + couponsFlag;

            String reqHmac1 = SignUtil.HmacSign(signData);
            String reqHmac = SignUtil.HmacSign(reqHmac1, Key);

            ///组织支付请求原始报文
            String reqData = "characterSet=" + characterSet + "&callbackUrl="
                        + callbackUrl + "&notifyUrl=" + notifyUrl
                        + "&ipAddress=" + ipAddress + "&merchantId="
                        + MerchantAcctId + "&requestId=" + requestId + "&signType="
                        + signType + "&type=" + type + "&version=" + version
                        + "&amount=" + amount + "&bankAbbr=" + bankAbbr
                        + "&currency=" + currency + "&orderDate=" + orderDate
                        + "&orderId=" + orderId + "&merAcDate=" + merAcDate + "&period=" + period
                        + "&periodUnit=" + periodUnit + "&merchantAbbr=" + merchantAbbr
                        + "&productDesc=" + productDesc + "&productId=" + productId
                        + "&productName=" + productName + "&productNum="
                        + productNum + "&reserved1=" + reserved1
                        + "&reserved2=" + reserved2 + "&userToken=" + userToken
                        + "&showUrl=" + showUrl + "&couponsFlag=" + couponsFlag + "&hmac=" + reqHmac;

            ///发送支付请求，并接收手机支付平台返回的支付地址
            String recData = IPosMUtil.httpRequest(Gateway, reqData);

            Hashtable ht = IPosMUtil.parseStringToMap(recData);

            String recHmac = (String)ht["hmac"];
            String recReturnCode = (String)ht["returnCode"];
            String message = (String)ht["message"];

            if ("000000".Equals(recReturnCode))
            {
                ///组织接收报文的签名原文
                String verData = (String)ht["merchantId"] +
                    (String)ht["requestId"] +
                    (String)ht["signType"] +
                    (String)ht["type"] +
                    (String)ht["version"] +
                    recReturnCode +
                    message +
                    (String)ht["payUrl"];

                ///验签
                Boolean flag = SignUtil.verifySign(verData, Key, recHmac);

                if (flag)
                {
                    String recPayUrl = (String)ht["payUrl"];
                    HttpContext.Current.Response.Redirect(IPosMUtil.getRedirectUrl(recPayUrl));
                }
                else
                {
                    return;
                }
            }
            else
                return;
        }

        //功能函数。将变量值不为空的参数组成字符串
        private static string AppendParam(String returnStr, String paramId, String paramValue)
        {
            if (returnStr != "")
            {
                if (paramValue != "")
                {

                    returnStr += "&" + paramId + "=" + paramValue;
                }
            }
            else
            {
                if (paramValue != "")
                {
                    returnStr = paramId + "=" + paramValue;
                }
            }

            return returnStr;
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

    }
}