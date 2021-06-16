using System;
using System.Text;
using System.Globalization;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.TenpayAssure
{
    [Plugin("财付通担保交易")]
    public class TenpayAssureRequest : PaymentRequest
    {

        public TenpayAssureRequest(
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

        public TenpayAssureRequest()
        {
        }

        #region 常量
        private const string Gatewayurl = "https://www.tenpay.com/cgi-bin/med/show_opentrans.cgi";
        private const string Version = "2";
        private const string Cmdno = "12";
        private const string EncodeType = "2";
        private const string NeedBuyerinfo = "2";
        private const string MchType = "1";
        private const string Attach = "TenpayAssure";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// [4]平台提供者的财付通账号
        /// </summary>
        //private readonly string chnid = "";

        /// <summary>
        /// [5]收款方财付通账号
        /// </summary>
        [ConfigElement("商户号", Nullable = false)]
        public string Seller { get; set; }

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

        /// <summary>
        /// 密钥
        /// </summary>
        [ConfigElement("商户密钥", Nullable = false)]
        public string Key { get; set; }

        public override void SendRequest()
        {
            // 生成签名
            StringBuilder buf = new StringBuilder();
            Globals.AddParameter(buf, "attach", Attach);
            Globals.AddParameter(buf, "chnid", Seller);
            Globals.AddParameter(buf, "cmdno", Cmdno);
            Globals.AddParameter(buf, "encode_type", EncodeType);
            Globals.AddParameter(buf, "mch_desc", mch_desc);
            Globals.AddParameter(buf, "mch_name", mch_name);
            Globals.AddParameter(buf, "mch_price", mch_price);
            Globals.AddParameter(buf, "mch_returl", mch_returl);
            Globals.AddParameter(buf, "mch_type", MchType);
            Globals.AddParameter(buf, "mch_vno", mch_vno);
            Globals.AddParameter(buf, "need_buyerinfo", NeedBuyerinfo);
            Globals.AddParameter(buf, "seller", Seller);
            Globals.AddParameter(buf, "show_url", show_url);
            Globals.AddParameter(buf, "transport_desc", "");
            Globals.AddParameter(buf, "transport_fee", "0");
            Globals.AddParameter(buf, "version", Version);
            Globals.AddParameter(buf, "key", Key);
            
            string sign = Globals.GetMD5(buf.ToString());

            string url = Gatewayurl + "?attach=" + Attach + "&chnid=" + Seller + "&cmdno=" + Cmdno + "&encode_type=" + EncodeType +
                "&mch_desc=" + mch_desc + "&mch_name=" + mch_name + "&mch_price=" + mch_price + "&mch_returl=" + mch_returl
                + "&mch_type=" + MchType + "&mch_vno=" + mch_vno + "&need_buyerinfo=" + NeedBuyerinfo + "&seller=" + Seller
                + "&show_url=" + show_url + "&transport_desc=" + "&transport_fee=0" + "&version=" + Version + "&sign=" + sign;

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

    }
}