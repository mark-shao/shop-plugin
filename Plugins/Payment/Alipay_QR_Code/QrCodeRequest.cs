using System;
using Hishop.Plugins;
using System.Globalization;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;
using System.Data;
using System.Collections.Generic;

namespace Hishop.Plugins.Payment.AlipayQrCode
{
    [Plugin("支付宝扫码支付", Sequence = 4)]

    public class QrCodeRequest : PaymentRequest
    {
        static IDictionary<string, string> param = new Dictionary<string, string>();
        public QrCodeRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            this.body = body;
            outTradeNo = orderId;
            this.subject = subject;
            this.timestamp = date.ToString("yyyy-MM-dd hh:mm:ss");
            totalFee = amount.ToString("F", CultureInfo.InvariantCulture);
            this.returnUrl = returnUrl;
            this.notifyUrl = notifyUrl;
            this.showUrl = showUrl;
            this.trade_type = "1";
            this.need_address = "F";
            this.memo = attach;
            this.id = orderId;
            this.name = subject;
            this.price = amount.ToString("f2");
            this.inventory = "";
            this.sku_title = "";
            this.sku = "";
            this.expiry_date = "";
            this.desc = body;
            param.Clear();
            param.Add("body", body);
            param.Add("outTradeNo", orderId);
            param.Add("subject", subject);
            param.Add("timestamp", timestamp);
            param.Add("totalFee", totalFee);
            param.Add("returnUrl", returnUrl);
            param.Add("notifyUrl", notifyUrl);
            param.Add("showUrl", showUrl);
            param.Add("trade_type", trade_type);
            param.Add("need_address", need_address);
            param.Add("memo", memo);
            param.Add("id", id);
            param.Add("name", name);
            param.Add("price", price);
            param.Add("inventory", inventory);
            param.Add("sku_title", sku_title);
            param.Add("sku", sku);
            param.Add("expiry_date", expiry_date);
            param.Add("desc", desc);
        }


        public QrCodeRequest(
           string orderId, decimal amount,
           string subject, string body, string buyerEmail, DateTime date,
           string showUrl, string returnUrl, string notifyUrl, string attach, string tradeType, string QRCode, Boolean NeedAddress, string productId)
        {
            this.body = body;
            outTradeNo = orderId;
            this.subject = subject;
            this.timestamp = date.ToString("yyyy-MM-dd hh:mm:ss");
            totalFee = amount.ToString("F", CultureInfo.InvariantCulture);
            this.returnUrl = returnUrl;
            this.notifyUrl = notifyUrl;
            this.showUrl = showUrl;
            this.trade_type = tradeType;
            this.qrCode = (QRCode == "" ? "" : "https://qr.alipay.com/" + QRCode);
            this.need_address = NeedAddress ? "T" : "F";
            this.memo = attach;
            this.id = productId == "" ? orderId : productId;
            this.name = subject;
            this.price = amount.ToString("f2");
            this.inventory = "";
            this.sku_title = "";
            this.sku = "";
            this.expiry_date = "";
            this.desc = body;
            param.Clear();
            param.Add("body", body);
            param.Add("outTradeNo", orderId);
            param.Add("subject", subject);
            param.Add("timestamp", timestamp);
            param.Add("totalFee", totalFee);
            param.Add("returnUrl", returnUrl);
            param.Add("notifyUrl", notifyUrl);
            param.Add("qrCode", qrCode);
            param.Add("need_address", need_address);
            param.Add("memo", memo);
            param.Add("id", id);
            param.Add("name", name);
            param.Add("price", price);
            param.Add("inventory", inventory);
            param.Add("sku_title", sku_title);
            param.Add("sku", sku);
            param.Add("expiry_date", expiry_date);
            param.Add("desc", desc);

        }
        public QrCodeRequest()
        {

        }



        #region 常量
        private const string Gateway = "https://mapi.alipay.com/gateway.do?";	//'支付接口
        private const string Service = "alipay.mobile.qrcode.manage";  //接口名称
        private const string SignType = "MD5";
        private const string PaymentType = "1";                  //支付类型	
        private const string InputCharset = "UTF-8";    //参数编码
        private const string Agent = "C4335302345904805116";
        private const string extend_param = "isv^yf31";
        private const string method = "add";
        private const string biztype = "10";

        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }
        [ConfigElement("收款支付宝账号", Nullable = false)]
        public string SellerEmail { get; set; } //卖家email

        [ConfigElement("合作者身份(PID)", Nullable = false)]
        public string Partner { get; set; }

        private readonly string subject;	//subject		商品名称
        private readonly string body;		//body			商品描述
        private readonly string totalFee;                      //总金额					0.01～50000.00
        private readonly string showUrl;
        //bizdata数据
        private string trade_type { get; set; }
        private string qrCode { get; set; }

        private string need_address { get; set; }
        private string timestamp { get; set; }
        private string memo { get; set; }
        //商品数据
        private string id { get; set; }
        private string name { get; set; }
        private string price { get; set; }
        private string inventory { get; set; }
        private string sku_title { get; set; }
        private string sku { get; set; }
        private string expiry_date { get; set; }
        private string desc { get; set; }


        [ConfigElement("安全校验码(Key)", Nullable = false)]
        public string Key { get; set; }              //账户的支付宝安全校验码

        private readonly string returnUrl;
        private readonly string notifyUrl;
        private readonly string outTradeNo;
        private readonly string query_url = "";
        private readonly string ext_info = "";

        public string buildGoodsInfo()
        {
            StringBuilder goodsinfo = new StringBuilder("{");
            goodsinfo.AppendFormat("\"id\":\"{0}\",", id);
            goodsinfo.AppendFormat("\"name\":\"{0}\",", name);
            goodsinfo.AppendFormat("\"price\":\"{0}\",", price);
            if (!string.IsNullOrEmpty(inventory))
                goodsinfo.AppendFormat("\"inventory\":\"{0}\",", inventory);
            if (!string.IsNullOrEmpty(sku_title))
                goodsinfo.AppendFormat("\"sku_title\":\"{0}\",", sku_title);
            if (!string.IsNullOrEmpty(sku))
                goodsinfo.AppendFormat("\"sku\":{0},", sku);
            if (!string.IsNullOrEmpty(expiry_date))
                goodsinfo.AppendFormat("\"expiry_date\":\"{0}\",", expiry_date);
            if (!string.IsNullOrEmpty(desc))
                goodsinfo.AppendFormat("\"desc\":\"{0}\",", desc);
            param.Add("goodsinfo", goodsinfo.ToString().Substring(0, goodsinfo.ToString().Length - 1) + "}");
            return goodsinfo.ToString().Substring(0, goodsinfo.ToString().Length - 1) + "}";
        }

        public string buildBizdata()
        {
            StringBuilder bizdata = new StringBuilder("{");
            bizdata.AppendFormat("\"trade_type\":\"{0}\",", trade_type);
            bizdata.AppendFormat("\"need_address\":\"{0}\",", need_address);
            bizdata.AppendFormat("\"goods_info\":{0},", buildGoodsInfo());

            if (!string.IsNullOrEmpty(returnUrl))
                bizdata.AppendFormat("\"return_url\":\"{0}\",", returnUrl);
            if (!string.IsNullOrEmpty(notifyUrl))
                bizdata.AppendFormat("\"notify_url\":\"{0}\",", notifyUrl);
            if (!string.IsNullOrEmpty(query_url))
                bizdata.AppendFormat("\"query_url\":\"{0}\",", query_url);
            if (!string.IsNullOrEmpty(ext_info))
                bizdata.AppendFormat("\"ext_info\":{0},", "{}");
            if (!string.IsNullOrEmpty(memo))
                bizdata.AppendFormat("\"memo\":\"{0}\",", memo);
            if (!string.IsNullOrEmpty(showUrl))
                bizdata.AppendFormat("\"url\":\"{0}\",", HttpUtility.UrlDecode(showUrl));
            param.Add("bizdata", bizdata.ToString().Substring(0, bizdata.ToString().Length - 1) + "}");
            return bizdata.ToString().Substring(0, bizdata.ToString().Length - 1) + "}";
        }
        public override void SendRequest()
        {
            //RedirectToGateway(
            //    Globals.CreatDirectUrl(Gateway, Service, Partner, InputCharset, SignType, method, timestamp, qrCode, biztype, buildBizdata(), Key
            //        ));
            string requestUrl = Globals.CreatDirectUrl(Gateway, Service, Partner, InputCharset, SignType, method, timestamp, qrCode, biztype, buildBizdata(), Key);
            string QRCodeUrl = "";
            bool status = false;
            string QRCodeImg = PostData(requestUrl, "", out QRCodeUrl, out status);

            try
            {

                if (status)
                {
                    if (subject.Trim().IndexOf("预付款充值") > -1)
                    {
                        HttpContext.Current.Response.Redirect("/pay/QrCode.aspx?status=1&QRCodeImg=" + HttpUtility.UrlDecode(QRCodeImg) + "&QrCodeUrl=" + HttpUtility.UrlDecode(QRCodeUrl) + "&OrderId=" + outTradeNo + "&isrecharge=1");
                    }
                    else
                    {
                        HttpContext.Current.Response.Redirect("/pay/QrCode.aspx?status=1&QRCodeImg=" + HttpUtility.UrlDecode(QRCodeImg) + "&QrCodeUrl=" + HttpUtility.UrlDecode(QRCodeUrl) + "&OrderId=" + outTradeNo);
                    }
                }
                else
                {
                    if (subject.Trim().IndexOf("预付款充值") > -1)
                    {
                        HttpContext.Current.Response.Redirect("/pay/QrCode.aspx?QRCodeImg=F&status=0" + "&OrderId=" + outTradeNo + "&isrecharge=1" + "&msg=" + QRCodeUrl);
                    }
                    else
                    {
                        HttpContext.Current.Response.Redirect("/pay/QrCode.aspx?QRCodeImg=F&status=0" + "&OrderId=" + outTradeNo);
                    }
                    PayLog.AppendLog(param, requestUrl, QRCodeUrl, QRCodeImg, LogType.Alipay_QR_Code);
                }
            }
            catch (Exception ex)
            {
                PayLog.AppendLog(param, ex.Message, QRCodeUrl, QRCodeImg, LogType.Alipay_QR_Code);
                if (subject.Trim().IndexOf("预付款充值") > -1)
                {
                    HttpContext.Current.Response.Write("/pay/QrCode.aspx?status=1&QRCodeImg=" + HttpUtility.UrlDecode(QRCodeImg) + "&QrCodeUrl=" + HttpUtility.UrlDecode(QRCodeUrl) + "&OrderId=" + outTradeNo + "&isrecharge=1" + " & msg=" + ex.Message);
                }
                else
                {
                    HttpContext.Current.Response.Write("/pay/QrCode.aspx?status=1&QRCodeImg=" + HttpUtility.UrlDecode(QRCodeImg) + "&QrCodeUrl=" + HttpUtility.UrlDecode(QRCodeUrl) + "&OrderId=" + outTradeNo + "&isrecharge=1" + "&msg=" + ex.Message);
                }
            }

        }

        /// <summary>
        /// 远程获取PrepayId
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string PostData(string url, string postData, out string QRCode, out bool Success)
        {
            QRCode = "";
            Success = false;
            string result = string.Empty;
            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                Encoding encoding = Encoding.UTF8;
                byte[] bytes = encoding.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "text/xml";
                request.ContentLength = postData.Length;

                using (StreamWriter writeStream = new StreamWriter(request.GetRequestStream()))
                {
                    writeStream.Write(postData);
                }

                #region 读取服务器返回信息

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        Encoding _encodingResponse = Encoding.UTF8;
                        //if(response)
                        StreamReader decompress = new StreamReader(responseStream, _encodingResponse);
                        //decompress
                        //if (response.ContentEncoding.ToLower() == "gzip")
                        //{
                        //    decompress = new GZipStream(responseStream, CompressionMode.Decompress);
                        //}
                        //else if (response.ContentEncoding.ToLower() == "deflate")
                        //{
                        //    decompress = new DeflateStream(responseStream, CompressionMode.Decompress);
                        //}
                        //using (StreamReader readStream = new StreamReader(decompress, _encodingResponse))
                        //{

                        //使用一个XmlDocument对象rssDoc来存储流中的XML内容。XmlDocument对象用来调入XML的内容              
                        result = decompress.ReadToEnd();
                        XmlDocument doc = new XmlDocument();
                        try
                        {
                            doc.LoadXml(result);
                            // doc.Load(decompress);
                        }
                        catch (Exception ex)
                        {
                            result = string.Format("获取信息错误doc.load：{0}", ex.Message) + result;
                        }
                        //  Globals.writeLog(param, "", "", doc.OuterXml);
                        try
                        {
                            if (doc == null) { return result; }
                            XmlNode resultnode = doc.SelectSingleNode("alipay/is_success");
                            if (resultnode == null)
                            {
                                return result;
                            }

                            if (resultnode.InnerText == "T")
                            {
                                Success = true;
                                XmlNode codeNode = doc.SelectSingleNode("alipay/response/alipay/qrcode");
                                XmlNode QRImageNode = doc.SelectSingleNode("alipay/response/alipay/qrcode_img_url");
                                if (codeNode == null && QRImageNode == null)
                                {
                                    Success = false;
                                    XmlNode errorNode = doc.SelectSingleNode("alipay/response/alipay/error_message");
                                    XmlNode errorcodeNode = doc.SelectSingleNode("alipay/response/alipay/result_code");
                                    return errorNode.InnerText + "---" + errorcodeNode.InnerText;

                                }
                                if (codeNode != null)
                                {
                                    QRCode = codeNode.InnerText;
                                }

                                if (QRImageNode != null)
                                {
                                    return QRImageNode.InnerText;
                                }
                            }
                            else
                            {
                                return doc.OuterXml;
                            }
                        }
                        catch (Exception ex) { result = string.Format("获取信息错误node.load：{0}", ex.Message) + result; }
                        //}
                    }
                }
            }
            catch (Exception ex) { result = string.Format("获取信息错误post error：{0}", ex.Message) + result; }
            #endregion


            return result;
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