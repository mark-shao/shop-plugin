using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.Plugins.Payment.BankUnionGateWay
{
    /// <summary>
    /// 银联支付报文类
    /// </summary>
    public class BankUnionGateWayConfig
    {
        private string version = "5.0.0";  //版本号

        public string Version
        {
            get { return version; }
            set { version = value; }
        }
        private string encoding = "UTF-8"; //编码方式

        public string Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }
        private string certId;   //证书ID

        public string CertId
        {
            get { return certId; }
            set { certId = value; }
        }
        private string signature;//签名

        public string Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        private string signMethod = "01"; //签名方法

        public string SignMethod
        {
            get { return signMethod; }
            set { signMethod = value; }
        }
        private string txnType = "01"; //交易类型

        public string TxnType
        {
            get { return txnType; }
            set { txnType = value; }
        }
        private string txnSubType = "01";  //交易子类

        public string TxnSubType
        {
            get { return txnSubType; }
            set { txnSubType = value; }
        }
        private string bizType = "000201"; //产品类型

        public string BizType
        {
            get { return bizType; }
            set { bizType = value; }
        }
        private string channelType = "08"; //渠道类型

        public string ChannelType
        {
            get { return channelType; }
            set { channelType = value; }
        }
        private string frontUrl; //前台通知地址

        public string FrontUrl
        {
            get { return frontUrl; }
            set { frontUrl = value; }
        }
        private string backUrl; //后台通知地址

        public string BackUrl
        {
            get { return backUrl; }
            set { backUrl = value; }
        }
        private string accessType = "0";//接入类型

        public string AccessType
        {
            get { return accessType; }
            set { accessType = value; }
        }
        private string merId; //商户代码

        public string MerId
        {
            get { return merId; }
            set { merId = value; }
        }
        private string orderId; //商户订单号

        public string OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }
        private string txnTime; //订单发送时间

        public string TxnTime
        {
            get { return txnTime; }
            set { txnTime = value; }
        }
        private string txnAmt;//交易金额

        public string TxnAmt
        {
            get { return txnAmt; }
            set { txnAmt = value; }
        }
        private string currencyCode = "156";//交易币种

        public string CurrencyCode
        {
            get { return currencyCode; }
            set { currencyCode = value; }
        }
    }
}
