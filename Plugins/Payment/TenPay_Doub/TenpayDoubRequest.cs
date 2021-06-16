using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Web;

namespace Hishop.Plugins.Payment.TenPay_Doub
{
    [Plugin("财付通双接口交易")]
   public class TenpayDoubRequest: PaymentRequest
    {
        /// <summary>
        /// 商家的商户号,有腾讯公司唯一分配
        /// </summary>
        [ConfigElement("商户号", Nullable = false)]
        public string Partner { get; set; }
        /// 商家的密钥
        [ConfigElement("商户密钥", Nullable = false)]
        public string Key { get; set; }

        public TenpayDoubRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            mch_price = Convert.ToInt32(amount * 100).ToString(CultureInfo.InvariantCulture);
            mch_vno = orderId;
            mch_returl = notifyUrl;
            show_url = returnUrl;
            mch_name = subject;
            mch_desc = body;
        }
        public TenpayDoubRequest()
        {
       }
        
        private const string Gatewayurl = "https://gw.tenpay.com/gateway/pay.htm";
        private const string Attach = "TenpayAssure";


        /// <summary>
        /// [7]商品总价，单位为分。而财付通界面不再允许选择数量
        /// </summary>
        private readonly string mch_price = "";

        /// <summary>
        /// [13]商户定单号,只能为数字
        /// </summary>
        private readonly string mch_vno = "";

        /// <summary>
        /// [14]回调通知URL,如果cmdno为12且此字段填写有效回调链接,财付通将把交易相关信息通知给此URL
        /// </summary>
        private readonly string mch_returl = "";

        /// <summary>
        /// [15]支付后的商户支付结果展示页面
        /// </summary>
        private readonly string show_url = "";

        /// <summary>
        /// 商品名称
        /// </summary>
        private readonly string mch_name = "";

        private readonly string mch_desc = "";



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


  
        public override void SendRequest()
        {
            // 生成签名        reqHandler.setParameter("", "GBK");
            StringBuilder buf = new StringBuilder();
            Globals.AddParameter(buf, "attach", Attach);
            Globals.AddParameter(buf, "bank_type", "DEFAULT");
            Globals.AddParameter(buf, "body", mch_desc);
            Globals.AddParameter(buf, "fee_type", "1");
            Globals.AddParameter(buf, "input_charset", "UTF-8");
            Globals.AddParameter(buf, "notify_url", mch_returl);
            Globals.AddParameter(buf, "out_trade_no", mch_vno);
            Globals.AddParameter(buf, "partner", Partner);
            Globals.AddParameter(buf, "return_url", show_url);
            Globals.AddParameter(buf, "spbill_create_ip", getRealIp());
            Globals.AddParameter(buf, "subject", mch_name);
            Globals.AddParameter(buf, "total_fee", mch_price);
            Globals.AddParameter(buf, "trade_mode", "3");
           Globals.AddParameter(buf, "trans_type", "1");      //虚拟还是实物交易  1 为实物 0 为虚拟 
            Globals.AddParameter(buf, "transport_fee", "0");
            Globals.AddParameter(buf, "key", Key);
            string sign = Globals.GetMD5(buf.ToString());
            string url = Gatewayurl + "?attach=" + Attach + "&bank_type=DEFAULT" + "&body=" + mch_desc + "&fee_type=1" + "&input_charset=UTF-8" + "&notify_url=" + mch_returl + "&out_trade_no=" + mch_vno + "&partner=" + Partner + "&return_url=" + show_url + "&spbill_create_ip=" + getRealIp() + "&subject=" + mch_name + "&total_fee=" + mch_price + "&trade_mode=3&trans_type=1&transport_fee=0" + "&sign=" + sign;
            RedirectToGateway(url);
        }

        public override bool IsMedTrade
        {
            get { return true; }
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

        protected override bool NeedProtect
        {
            get { return true; }
        }
    }
}
