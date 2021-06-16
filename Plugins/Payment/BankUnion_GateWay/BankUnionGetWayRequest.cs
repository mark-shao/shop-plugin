using Hishop.Plugins.Payment.BankUnionGateWay.sdk;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hishop.Plugins.Payment.BankUnionGateWay
{
    [Plugin("银联全渠道支付")]
    public class BankUnionGetWayRequest : PaymentRequest
    {
        public static BankUnionGateWayConfig BUConfig;

        #region 常量
        //测试环境aa
        //private const string Gateway = "https://101.231.204.80:5000/gateway/api/frontTransReq.do ";
        private const string Gateway = "https://gateway.95516.com/gateway/api/frontTransReq.do";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// 商户编号
        /// </summary>
        [ConfigElement("商户号", Nullable = false)]
        public string Vmid { get; set; }

        [ConfigElement("证书文件名", Nullable = true)]
        public string SignCertFileName { get; set; }

        /// <summary>
        /// 证书密码
        /// </summary>
        [ConfigElement("证书密码", Nullable = false)]
        public string Key { get; set; }

        public BankUnionGetWayRequest() { }

        public BankUnionGetWayRequest( string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            if (BUConfig == null)
            {
                BUConfig = new BankUnionGateWayConfig();
            }
                                                                                     
            BUConfig.OrderId = orderId;
            BUConfig.TxnAmt = (Math.Round(amount * 100, 0).ToString());
            BUConfig.TxnTime = date.ToString("yyyyMMddHHmmss");
            BUConfig.FrontUrl = returnUrl;
            BUConfig.BackUrl = notifyUrl;
        }

        /// <summary>
        /// 定向到支付页面
        /// </summary>
        public override void SendRequest()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
           
            BUConfig.MerId = SDKConfig.MerId = Vmid;
            SDKConfig.SignCertPwd = Key;
            SDKConfig.signCertPath = Path.Combine(SDKConfig.validateCertDir, SignCertFileName);
            BUConfig.CertId = CertUtil.GetSignCertId();
            param.Add("version", BUConfig.Version);
            param.Add("encoding", BUConfig.Encoding);
            param.Add("certId", BUConfig.CertId);
            param.Add("signMethod", BUConfig.SignMethod);
            param.Add("txnType", BUConfig.TxnType);
            param.Add("txnSubType", BUConfig.TxnSubType);
            param.Add("bizType", BUConfig.BizType);
            param.Add("channelType", BUConfig.ChannelType);
            param.Add("frontUrl", BUConfig.FrontUrl);
            param.Add("backUrl", BUConfig.BackUrl);
            param.Add("accessType", BUConfig.AccessType);
            param.Add("merId", BUConfig.MerId);
            param.Add("orderId", BUConfig.OrderId);
            param.Add("txnTime", BUConfig.TxnTime);
            param.Add("txnAmt", BUConfig.TxnAmt);
            param.Add("currencyCode", BUConfig.CurrencyCode);
            param.Add("userMac", "userMac");
            SDKUtil.Sign(param, Encoding.UTF8);
            // 将SDKUtil产生的Html文档写入页面，从而引导用户浏览器重定向
            string html = SDKUtil.CreateAutoSubmitForm(Gateway, param, Encoding.UTF8);
            PayLog.writeLog(param, "", "", html, LogType.BankUnion_GateWay);
            SubmitPaymentForm(html);
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
        public override bool IsMedTrade
        {
            get { return false; }
        }
        public override string ShortDescription
        {
            get { return string.Empty; }
        }
    }
}
