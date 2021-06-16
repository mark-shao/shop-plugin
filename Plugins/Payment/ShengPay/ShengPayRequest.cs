using LitJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Hishop.Plugins.Payment.ShengPay
{
    [Plugin("盛付通即时交易")]
    public class ShengPayRequest : PaymentRequest
    {

        //private const string GatewayUrl = "http://mas.sdo.com/web-acquire-channel/cashier30.htm";
        private const string GatewayUrl = "https://mas.shengpay.com/web-acquire-channel/cashier.htm";
        //private const string GatewayUrl = "http://pre.netpay.sdo.com/paygate/default.aspx";// 测试环境
        private const string Version = "V4.1.1.1.1";
        private string SendTime = "";
        private readonly string Name = "B2CPayment";
        private const string Charset = "UTF-8";
        private const string CurrencyType = "CNY";
        private const string NotifyUrlType = "http";
        private const string SignType = "MD5";
        private const string DefaultChannel = "";
        private const string InstCode = "";
        private readonly string _payType = "PT001";
        private readonly string _payChannel = "";
        private readonly string _amount = "";
        private readonly string _orderNo = "";
        private readonly string _postBackUrl = "";
        private readonly string _notifyUrl = "";
        private readonly string _backUrl = "";
        private readonly string _merchantUserId = "";
        private readonly string _productNo = "";
        private readonly string _productDesc = "";
        private readonly string _orderTime = "";
        private readonly string _remark1 = "";
        private readonly string _remark2 = "";
        private readonly string _bankCode = "";
        private readonly string _productUrl = "";
        private string BuyerContact = "";
        private string BuyerIP = DataHelper.IPAddress;

        [ConfigElement("商户号", Nullable = false)]
        public string MerchantNo { get; set; }

        [ConfigElement("商户密钥", Nullable = false)]
        public string Key { get; set; }

        public ShengPayRequest(
             string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            _orderNo = orderId;
            _amount = amount.ToString("F2");
            _postBackUrl = HttpUtility.UrlDecode(returnUrl);
            _notifyUrl = HttpUtility.UrlDecode(notifyUrl);
            if (string.IsNullOrEmpty(showUrl))
                _backUrl = HttpUtility.UrlDecode(returnUrl);
            else
                _backUrl = HttpUtility.UrlDecode(showUrl);
            _postBackUrl = "";
            _productNo = subject;
            //_productDesc = body;
            _productUrl = showUrl;
            if (date != null)
                _orderTime = date.ToString("yyyyMMddHHmmss");
            else
                _orderTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            BuyerContact = buyerEmail;
            _remark1 = attach;
        }

        public ShengPayRequest()
        {
        }

        public override bool IsMedTrade
        {
            get { return false; }
        }

        public override void SendGoods(string tradeno, string logisticsName, string invoiceno, string transportType)
        {
        }

        public override void SendRequest()
        {
            SendTime = GetSendTimeSpan(MerchantNo);

            StringBuilder sbOriginStr = new StringBuilder();
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(Name) ? "" : Name+"|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(Version) ? "" : Version + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(Charset) ? "" : Charset + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(MerchantNo) ? "" : MerchantNo + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(SendTime) ? "" : SendTime + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(_orderNo) ? "" : _orderNo + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(_amount) ? "" : _amount + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(_orderTime) ? "" : _orderTime + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(CurrencyType) ? "" : CurrencyType + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(_payType) ? "" : _payType + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(_payChannel) ? "" : _payChannel + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(InstCode) ? "" : InstCode + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(_backUrl) ? "" : _backUrl + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(_notifyUrl) ? "" : _notifyUrl + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(_productNo) ? "" : _productNo + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(BuyerContact) ? "" : BuyerContact + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(BuyerIP) ? "" : BuyerIP + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(_remark1) ? "" : _remark1 + "|");
            sbOriginStr.AppendFormat(string.IsNullOrEmpty(SignType) ? "" : SignType + "|");

            string originStr = sbOriginStr.ToString();
            //string originStr = Name + Version + Charset + MerchantNo + SendTime + _orderNo + _amount + _orderTime + 
            //    CurrencyType + _payType + _payChannel + InstCode + _backUrl + _notifyUrl +
            //                   _productNo + BuyerContact + BuyerIP + _remark1 + SignType;
            ////PayLog.AppendLog(null, originStr, Key, "", LogType.ShengPay);
            string sign = FormsAuthentication.HashPasswordForStoringInConfigFile((originStr + Key), "MD5");

            StringBuilder sb = new StringBuilder();
            sb.Append(CreateField("Name", Name));
            sb.Append(CreateField("Version", Version));
            sb.Append(CreateField("Charset", Charset));
            sb.Append(CreateField("MsgSender", MerchantNo));
            sb.Append(CreateField("SendTime", SendTime));
            sb.Append(CreateField("OrderNo", _orderNo));
            sb.Append(CreateField("OrderAmount", _amount));
            sb.Append(CreateField("OrderTime", _orderTime));
            sb.Append(CreateField("Currency", CurrencyType));
            sb.Append(CreateField("PayType", _payType));
            sb.Append(CreateField("PayChannel", _payChannel));
            sb.Append(CreateField("InstCode", InstCode));
            sb.Append(CreateField("PageUrl", _backUrl));
           // sb.Append(CreateField("BackUrl", _postBackUrl));
            sb.Append(CreateField("NotifyUrl", _notifyUrl));
            sb.Append(CreateField("ProductName", _productNo));
            sb.Append(CreateField("BuyerContact", BuyerContact));
            sb.Append(CreateField("BuyerIp", BuyerIP));
            sb.Append(CreateField("Ext1", _remark1));
            sb.Append(CreateField("SignType", SignType));
            sb.Append(CreateField("SignMsg", sign));
            //PayLog.AppendLog(null, originStr, Key, CreateForm(sb.ToString(), GatewayUrl), LogType.ShengPay);

            IDictionary<string, string> para = new Dictionary<string, string>();
            para.Add("originStr", originStr);
            para.Add("Name", Name);
            para.Add("Version", Version);
            para.Add("Charset", Charset);
            para.Add("MsgSender", MerchantNo);
            para.Add("SendTime", SendTime);
            para.Add("OrderNo", _orderNo);
            para.Add("OrderAmount", _amount);
            para.Add("OrderTime", _orderTime);
            para.Add("Currency", CurrencyType);
            para.Add("PayType", _payType);
            para.Add("PayChannel", _payChannel);
            para.Add("InstCode", InstCode);
            para.Add("PageUrl", _backUrl);
            // para.Add("BackUrl", _postBackUrl));
            para.Add("NotifyUrl", _notifyUrl);
            para.Add("ProductName", _productNo);
            para.Add("BuyerContact", BuyerContact);
            para.Add("BuyerIp", BuyerIP);
            para.Add("Ext1", _remark1);
            para.Add("SignType", SignType);
            para.Add("SignMsg", sign);

          //  PayLog.AppendLog(para, sign, _notifyUrl, "支付日志", LogType.ShengPay);

            SubmitPaymentForm(CreateForm(sb.ToString(), GatewayUrl));
        }


        public string GetSendTimeSpan(string merchantNo)
        {
            String Url = string.Format("https://api.shengpay.com/mas/v1/timestamp?merchantNo={0}", merchantNo);
            try
            {

                string data = DataHelper.GetData(Url);
                //PayLog.AppendLog(null, "", Url, data, LogType.ShengPay);
                if (string.IsNullOrEmpty(data))
                    return "";
                else
                {
                    JsonData jd = JsonMapper.ToObject(data);
                    return (string)jd["timestamp"];
                }
            }
            catch (Exception ex)
            {
                //PayLog.AppendLog(null, "", Url, ex.Message, LogType.ShengPay);
                return "";
            }
        }
        public override string Description
        {
            get { return string.Empty; }
        }

        public override string Logo
        {
            get { return string.Empty; }
        }

        protected override bool NeedProtect
        {
            get { return true; }
        }

        public override string ShortDescription
        {
            get { return string.Empty; }
        }

    }
}