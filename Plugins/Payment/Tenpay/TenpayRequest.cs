using System;
using System.Globalization;
using Hishop.Plugins;
using System.Web;

namespace Hishop.Plugins.Payment.Tenpay
{
    [Plugin("财付通即时交易")]
    public class TenpayRequest : PaymentRequest
    {

        public TenpayRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            this.date = date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            desc = UrlEncode(subject);
            sp_billno = orderId;
            return_url = returnUrl;
            total_fee = Convert.ToInt32(amount * 100).ToString(CultureInfo.InvariantCulture);
        }

        public TenpayRequest()
        {
        }

        #region 常量
        private const string GatewayUrl = "https://www.tenpay.com/cgi-bin/v1.0/pay_gate.cgi";
       // private const string GatewayUrl = "http://service.tenpay.com/cgi-bin/v3.0/payservice.cgi";
        private const string Cmdno = "1";
        private const string FeeType = "1";
        private const string Attach = "Tenpay";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// 商户日期：如20051212
        /// </summary>
        private readonly string date = "";
 
        /// <summary>
        /// 交易的商品名称
        /// </summary>
        private readonly string desc = "";

        /// <summary>
        /// 商家的商户号,有腾讯公司唯一分配
        /// </summary>
        [ConfigElement("商户号", Nullable = false)]
        public string BargainorId { get; set; }

        [ConfigElement("商户密钥", Nullable = false)]
        public string Key { get; set; }

        /// <summary>
        /// 交易号(订单号)
        /// </summary>
        private string transaction_id = "";

        /// <summary>
        /// 商户系统内部的定单号，此参数仅在对账时提供
        /// </summary>
        private readonly string sp_billno = "";

        /// <summary>
        /// 总金额，以分为单位
        /// </summary>
        private readonly string total_fee = "";
        
        /// <summary>
        /// 接收财付通返回结果的URL
        /// </summary>
        private readonly string return_url = "";

        public override void SendRequest()
        {
            string spbill_create_ip = getRealIp();
            transaction_id = BargainorId + date + UnixStamp();
            // 生成签名
            string s = "cmdno=" + Cmdno + "&date=" + date + "&bargainor_id=" + BargainorId
                + "&transaction_id=" + transaction_id + "&sp_billno=" + sp_billno + "&total_fee="
                + total_fee + "&fee_type=" + FeeType + "&return_url=" + return_url + "&attach=" + Attach + 
                "&spbill_create_ip=" + spbill_create_ip + "&key=" + Key;

            string sign = Globals.GetMD5(s);

            string url = GatewayUrl + "?cmdno=" + Cmdno + "&date=" + date + "&bank_type=0&desc=" + desc + "&purchaser_id=&bargainor_id="
                    + BargainorId + "&transaction_id=" + transaction_id + "&sp_billno=" + sp_billno + "&total_fee=" + total_fee
                    + "&fee_type=" + FeeType + "&return_url=" + return_url + "&attach=" + Attach + "&spbill_create_ip=" + spbill_create_ip + "&cs=utf-8" + "&sign=" + sign;

            RedirectToGateway(url);
        }

        private static string getRealIp()
        {
            string UserIP;
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null) //得到穿过代理服务器的ip地址
            {
                UserIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                UserIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            return UserIP;
        }

        /// <summary>
        /// 取时间戳生成随即数,替换交易单号中的后10位流水号
        /// 财付通的交易单号中不允许出现非数字的字符
        /// </summary>
        /// <returns></returns>
        private static UInt32 UnixStamp()
        {
            TimeSpan ts = DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return Convert.ToUInt32(ts.TotalSeconds);
        }

        private static string UrlEncode(string instr)
        {
            if (instr == null || instr.Trim() == "")
                return "";

            return instr.Replace("%", "%25").Replace("=", "%3d").Replace("&", "%26").
                Replace("\"", "%22").Replace("?", "%3f").Replace("'", "%27").Replace(" ", "%20");
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