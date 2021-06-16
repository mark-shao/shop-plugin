using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Hishop.Plugins.Payment.WXQRCode
{
    [Plugin("微信扫码支付")]
    public class WXQRCodeRequest : PaymentRequest
    {
        private const string Gateway = "https://api.mch.weixin.qq.com/pay/unifiedorder";	//'支付接口
        string DeviceInfo { get; set; }
        string StrNonce { get; set; }
        string ProductInfo { get; set; }

        string OrderId { get; set; }

        decimal TotalFee { get; set; }

        string NotifyUrl { get; set; }

        string Subject { get; set; }

        /// <summary>
        /// 微信公众账号
        /// </summary>
        [ConfigElement("AppId", Nullable = false, Description = "由服务商代理申请的微信支付时，请填写服务商提供的AppId,否则填写商户自己的AppId")]
        public string AppId { get; set; }

        [ConfigElement("mch_id", Nullable = false, Description = "由服务商代理申请的微信支付时，请填写服务商提供的mch_id,否则填写商户自己的mch_id")]
        public string MCHID { get; set; }

        [ConfigElement("key", Nullable = false, HiddenPart = true, Description = "由服务商代理申请的微信支付时，请填写服务商提供的Key,否则填写商户自己设置的Key")]
        public string AppSecret { get; set; }
        [ConfigElement("子商户AppId", Nullable = true, Description = "由服务商代理申请的微信支付需要填写该项，填写商户自己的AppId,否则不需要填写")]
        public string Sub_AppId { get; set; }

        [ConfigElement("子商户号", Nullable = true, Description = "由服务商代理申请的微信支付需要填写该项,填写商户自己的Mch_Id,否则不需要填写")]
        public string Sub_mch_Id { get; set; }
        [ConfigElement("证书路径", Nullable = true, InputType = InputType.File, Description = "证书用于企业帐号支付以及退款原路返回，请使用扩展名为p12的证书文件")]
        /// <summary>
        /// 微信支付证书，用于退款
        /// </summary>
        public string CertPath { get; set; }
        //[ConfigElement("证书密码", Nullable = true, InputType = InputType.Password, Description = "证书密码用于企业帐号支付以及退款原路返回,与MCHID（商户号）相同")]
        public string CertPassword { get { return MCHID; } }
        public WXQRCodeRequest(string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            ProductInfo = body;
            OrderId = orderId;
            TotalFee = amount;
            NotifyUrl = notifyUrl;
            Subject = subject;
        }

        public WXQRCodeRequest() { }

        public override void SendRequest()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Sub_AppId) && !string.IsNullOrEmpty(Sub_mch_Id))
            {
                param.Add("sub_appid", Sub_AppId);
                param.Add("sub_mch_id", Sub_mch_Id);
            }
            param.Add("appid", AppId);
            param.Add("mch_id", MCHID);
            param.Add("device_info", string.Empty);
            param.Add("nonce_str", GetStrnonce());
            param.Add("body", OrderId);
            param.Add("attach", string.Empty);
            param.Add("out_trade_no", OrderId);
            param.Add("total_fee", ((int)(TotalFee * 100)).ToString());
            param.Add("spbill_create_ip", "127.0.0.1");
            param.Add("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            param.Add("time_expire", string.Empty);
            param.Add("goods_tag", string.Empty);
            param.Add("notify_url", NotifyUrl);
            param.Add("trade_type", "NATIVE");
            param.Add("openid", string.Empty);
            param.Add("product_id", OrderId);
            var sign = GetMD5String(CoverDictionaryToString(param) + "&key=" + AppSecret).ToUpper();
            param.Add("sign", sign);
            var codeUrl = GetCodrUrl(Gateway, GetXmlData(param));
            if (!string.IsNullOrEmpty(codeUrl) && !codeUrl.StartsWith("error"))
            {
                if (Subject.Trim().Equals("预付款充值"))
                {
                    HttpContext.Current.Response.Redirect("/pay/WXQRCode.aspx?code_url=" + HttpUtility.UrlDecode(codeUrl) + "&orderId=" + HttpUtility.UrlDecode(OrderId) + "&isrecharge=1");
                }
                else if (Subject.Trim().Equals("分销商充值"))
                {
                    HttpContext.Current.Response.Redirect("/pay/WXQRCode.aspx?code_url=" + HttpUtility.UrlDecode(codeUrl) + "&orderId=" + HttpUtility.UrlDecode(OrderId) + "&isrecharge=2");
                }
                else if (Subject.Trim().Equals("采购单支付"))
                {
                    HttpContext.Current.Response.Redirect("/pay/WXQRCode.aspx?code_url=" + HttpUtility.UrlDecode(codeUrl) + "&orderId=" + HttpUtility.UrlDecode(OrderId) + "&isrecharge=3");
                }
                else
                {
                    HttpContext.Current.Response.Redirect("/pay/WXQRCode.aspx?code_url=" + HttpUtility.UrlDecode(codeUrl) + "&orderId=" + HttpUtility.UrlDecode(OrderId));
                }
            }
            else
            {

                HttpContext.Current.Response.Redirect("/pay/WXQRCode.aspx?code_url=getcodeurl_error&orderId=" + HttpUtility.UrlDecode(OrderId) + "&msg=" + codeUrl);

            }
        }

        private string GetStrnonce()
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks;

            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)));
            for (int i = 0; i < 31; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }

            return str;
        }

        /// <summary>
        /// 获取xml格式的数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private string GetXmlData(Dictionary<string, string> param)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            foreach (string k in param.Keys)
            {
                string v = (string)param[k];
                if (v == null)
                    v = string.Empty;
                if (Regex.IsMatch(v, @"^[0-9.]$"))
                {

                    sb.Append("<" + k + ">" + v + "</" + k + ">");
                }
                else
                {
                    sb.Append("<" + k + "><![CDATA[" + v + "]]></" + k + ">");
                }
            }

            sb.Append("</xml>");
            return sb.ToString();
        }

        private string GetMD5String(string encypStr)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            //inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
            inputBye = Encoding.UTF8.GetBytes(encypStr);

            outputBye = m5.ComputeHash(inputBye);

            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   //直接确认，否则打不开
            return true;
        }
        /// <summary>
        /// 远程获取codeurl
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string GetCodrUrl(string url, string postData)
        {
            var codeUrl = string.Empty;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                Uri uri = new Uri(url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                Encoding encoding = Encoding.UTF8;
                byte[] bytes = encoding.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "text/xml";
                request.ContentLength = bytes.Length;

                using (Stream writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }

                #region 读取服务器返回信息

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        Encoding _encodingResponse = Encoding.UTF8;
                        Stream decompress = responseStream;
                        //decompress
                        if (response.ContentEncoding.ToLower() == "gzip")
                        {
                            decompress = new GZipStream(responseStream, CompressionMode.Decompress);
                        }
                        else if (response.ContentEncoding.ToLower() == "deflate")
                        {
                            decompress = new DeflateStream(responseStream, CompressionMode.Decompress);
                        }
                        using (StreamReader readStream = new StreamReader(decompress, _encodingResponse))
                        {
                            var result = readStream.ReadToEnd();
                            XmlDocument doc = new XmlDocument();
                            doc.XmlResolver = null;
                            try
                            {
                                doc.LoadXml(result);
                                if (doc != null)
                                {
                                    XmlNode e_returncode = doc.SelectSingleNode("xml/return_code");
                                    XmlNode e_resultmsg = doc.SelectSingleNode("xml/return_msg");
                                    if (e_returncode != null)
                                    {
                                        if (e_returncode.InnerText == "SUCCESS")
                                        {
                                            XmlNode e_resultcode = doc.SelectSingleNode("xml/result_code");
                                            XmlNode e_errdes = doc.SelectSingleNode("xml/err_code_des");
                                            if (e_resultcode.InnerText == "SUCCESS")
                                            {
                                                XmlNode e_codeUrl = doc.SelectSingleNode("xml/code_url");
                                                codeUrl = e_codeUrl.InnerText;
                                            }
                                            else
                                            {
                                                return "error" + e_errdes == null ? "" : e_errdes.InnerText;
                                            }
                                        }
                                        else
                                        {
                                            PayLog.AppendLog(null, postData, url, doc.OuterXml, LogType.WXQRCode);
                                            return "error" + result;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                PayLog.AppendLog(null, null, NotifyUrl, ex.StackTrace + ex.Message, LogType.WXQRCode);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PayLog.AppendLog(null, null, NotifyUrl, ex.StackTrace + ex.Message, LogType.WXQRCode);
            }
            return codeUrl;
            #endregion
        }

        /// 将Dictionary内容排序后输出为键值对字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string CoverDictionaryToString(Dictionary<string, string> data)
        {
            //如果不加stringComparer.Ordinal，排序方式和java treemap有差异
            SortedDictionary<string, string> treeMap = new SortedDictionary<string, string>(StringComparer.Ordinal);

            foreach (KeyValuePair<string, string> kvp in data)
            {
                if (string.IsNullOrEmpty(kvp.Value)) continue;
                treeMap.Add(kvp.Key, kvp.Value);
            }

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> element in treeMap)
            {
                builder.Append(element.Key + "=" + element.Value + "&");
            }

            return builder.ToString().Substring(0, builder.Length - 1);
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
