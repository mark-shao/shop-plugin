using System;
using System.Collections.Generic;
using Hishop.Weixin.Pay.Domain;
using Hishop.Weixin.Pay.Util;
using System.Data;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Hishop.Weixin.Pay
{
    /// <summary>
    /// 支付客户端
    /// </summary>
    public class PayClient
    {
        /// <summary>
        /// 发货通知网关
        /// </summary>
        public static readonly string Deliver_Notify_Url = "https://api.weixin.qq.com/pay/delivernotify";
        public static readonly string prepay_id_Url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

        private PayAccount _payAccount;

        #region 构造器
        /// <summary>
        /// 支付请求
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="partnerId"></param>
        /// <param name="partnerKey"></param>
        /// <param name="paySignKey"></param>
        /// <param name="sub_mch_Id"></param>
        /// <param name="sub_appId"></param>
        /// <param name="sub_openId"></param>
        public PayClient(string appId, string appSecret, string partnerId, string partnerKey, string paySignKey = "", string sub_mch_Id = "", string sub_appId = "", string sub_openId = "")
        {
            _payAccount = new PayAccount()
            {
                AppId = appId,
                AppSecret = appSecret,
                PartnerId = partnerId,
                PartnerKey = partnerKey,
                PaySignKey = paySignKey,
                sub_mch_id = sub_mch_Id,
                Sub_AppId = sub_appId,
                Sub_OpenId = sub_openId
            };
        }
        /// <summary>
        /// 支付请求
        /// </summary>
        /// <param name="account"></param>
        public PayClient(PayAccount account)
            : this(account.AppId,
                account.AppSecret,
                account.PartnerId,
                account.PartnerKey,
                account.PaySignKey,
                account.sub_mch_id,
                account.Sub_AppId,
                account.Sub_OpenId
            )
        {
        }
        #endregion


        #region 内部方法

        /// <summary>
        /// 创建Package数据结构
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        internal string BuildPackage(PackageInfo package)
        {
            //公众账号ID appid 是  String(32) 微信分配的公众账号ID
            //商户号mch_id 是 String(32) 微信支付分配的商户号
            //设备号device_info 否 String(32) 微信支付分配的终端设备号
            //随机字符串nonce_str 是 String(32) 随机字符串，不长于32 位
            //签名sign 是 String(32) 签名,详细签名方法见3.2 节
            //商品描述body 是 String(127) 商品描述
            //附加数据attach 否 String(127) 附加数据，原样返回
            //商户订单号out_trade_no 是 String(32) 商户系统内部的订单号,32个字符内、可包含字母,确保在商户系统唯一,详细说明
            //总金额total_fee 是 Int 订单总金额，单位为分，不能带小数点
            //终端IP spbill_create_ip 是 String(16) 订单生成的机器IP
            //交易起始时间time_start 否S tring(14) 订单生成时间， 格式为yyyyMMddHHmmss，如2009 年12 月25 日9 点10 分10 秒表示为20091225091010。时区为GMT+8 beijing。该时间取自商户服务器
            //交易结束时间time_expire 否 String(14) 订单失效时间， 格式为yyyyMMddHHmmss，如2009 年12 月27 日9 点10 分10 秒表示为20091227091010。时区为GMT+8 beijing。该时间取自商户服务器
            //商品标记goods_tag 否 String(32) 商品标记，该字段不能随便填，不使用请填空
            //通知地址notify_url 是 String(256) 接收微信支付成功通知
            //交易类型trade_type 是 String(16) JSAPI、NATIVE、APP
            //用户标识openid 否 String(128) 用户在商户appid 下的唯一标识，trade_type 为JSAPI时，此参数必传
            //商品ID product_id 否 String(32)只在trade_type 为NATIVE时需要填写。此id 为二维码中包含的商品ID，
            PayDictionary dict = new PayDictionary();
            try
            {
                dict.Add("appid", _payAccount.AppId);
                dict.Add("mch_id", _payAccount.PartnerId);
                //如果包含了参数Sub_mch_id和sub_appid则为服务商模式
                if (!string.IsNullOrEmpty(_payAccount.sub_mch_id) && !string.IsNullOrEmpty(_payAccount.Sub_AppId))
                {
                    dict.Add("sub_mch_id", _payAccount.sub_mch_id);
                    dict.Add("sub_appid", _payAccount.Sub_AppId);
                    dict.Add("sub_openid", package.sub_openid);
                }
                else
                {
                    dict.Add("openid", package.OpenId);
                }
                dict.Add("device_info", "");
                dict.Add("nonce_str", Utils.CreateNoncestr());
                dict.Add("body", package.Body);
                dict.Add("attach", package.Attach);
                dict.Add("out_trade_no", package.OutTradeNo);
                dict.Add("total_fee", (int)package.TotalFee);
                dict.Add("spbill_create_ip", package.SpbillCreateIp);
                dict.Add("time_start", package.TimeExpire);
                dict.Add("time_expire", "");
                dict.Add("goods_tag", package.GoodsTag);
                dict.Add("notify_url", package.NotifyUrl);
                dict.Add("trade_type", "JSAPI");
                dict.Add("product_id", "");
                string sign = SignHelper.SignPackage(dict, _payAccount.PartnerKey);
                String Prepay_Id = GetPrepay_id(dict, sign);
                if (Prepay_Id.Length > 64) Prepay_Id = "";
                return string.Format("prepay_id=" + Prepay_Id);
            }
            catch (Exception ex)
            {
                WxPayLog.writeLog(dict, "", "", ex.Message, LogType.Error);
                return "";
            }
        }
        #endregion

        #region public PaymentInfo BuildPayRequest(PackageInfo package)

        /// <summary>
        /// 创建支付请求
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public PayRequestInfo BuildPayRequest(PackageInfo package)
        {
            PayRequestInfo payment = new PayRequestInfo();
            payment.appId = _payAccount.AppId;
            payment.package = BuildPackage(package);
            payment.prepayid = payment.package.Replace("prepay_id=", "");
            payment.timeStamp = Utils.GetCurrentTimeSeconds().ToString();
            payment.nonceStr = Utils.CreateNoncestr();
            PayDictionary dict = new PayDictionary();
            dict.Add("appId", _payAccount.AppId);
            //   dict.Add("appkey", _payAccount.PaySignKey);
            dict.Add("timeStamp", payment.timeStamp);
            dict.Add("package", payment.package);
            dict.Add("nonceStr", payment.nonceStr);

            dict.Add("signType", "MD5");
            payment.paySign = SignHelper.SignPay(dict, _payAccount.PartnerKey);
            return payment;
        }
        #endregion

        #region 非微信浏览器微信支付请求
        /// <summary>
        /// 创建Package数据结构
        /// </summary>
        /// <param name="package"></param>
        /// <param name="prepayId">预支付ID</param>
        /// <returns></returns>
        internal string BuildH5PayUrl(PackageInfo package, out string prepayId)
        {
            //公众账号ID appid 是  String(32) 微信分配的公众账号ID
            //商户号mch_id 是 String(32) 微信支付分配的商户号
            //设备号device_info 否 String(32) 微信支付分配的终端设备号
            //随机字符串nonce_str 是 String(32) 随机字符串，不长于32 位
            //签名sign 是 String(32) 签名,详细签名方法见3.2 节
            //商品描述body 是 String(127) 浏览器打开的移动网页的主页title名-商品概述
            //附加数据attach 否 String(127) 附加数据，原样返回
            //商户订单号out_trade_no 是 String(32) 商户系统内部的订单号,32个字符内、可包含字母,确保在商户系统唯一,详细说明
            //总金额total_fee 是 Int 订单总金额，单位为分，不能带小数点
            //终端IP spbill_create_ip 是 String(16) 订单生成的机器IP
            //交易起始时间time_start 否S tring(14) 订单生成时间， 格式为yyyyMMddHHmmss，如2009 年12 月25 日9 点10 分10 秒表示为20091225091010。时区为GMT+8 beijing。该时间取自商户服务器
            //交易结束时间time_expire 否 String(14) 订单失效时间， 格式为yyyyMMddHHmmss，如2009 年12 月27 日9 点10 分10 秒表示为20091227091010。时区为GMT+8 beijing。该时间取自商户服务器
            //商品标记goods_tag 否 String(32) 商品标记，该字段不能随便填，不使用请填空
            //通知地址notify_url 是 String(256) 接收微信支付成功通知
            //交易类型trade_type 是 String(16) JSAPI、NATIVE、APP
            //用户标识openid 否 String(128) 用户在商户appid 下的唯一标识，trade_type 为JSAPI时，此参数必传
            //商品ID product_id 否 String(32)只在trade_type 为NATIVE时需要填写。此id 为二维码中包含的商品ID，
            prepayId = "";
            PayDictionary dict = new PayDictionary();
            try
            {
                dict.Add("appid", _payAccount.AppId);
                dict.Add("mch_id", _payAccount.PartnerId);
                //如果包含了参数Sub_mch_id和sub_appid则为服务商模式
                if (!string.IsNullOrEmpty(_payAccount.sub_mch_id) && !string.IsNullOrEmpty(_payAccount.Sub_AppId))
                {
                    dict.Add("sub_mch_id", _payAccount.sub_mch_id);
                    dict.Add("sub_appid", _payAccount.Sub_AppId);
                    dict.Add("sub_openid", package.sub_openid);
                }
                else
                {
                    dict.Add("openid", package.OpenId);
                }
                dict.Add("device_info", "");
                dict.Add("nonce_str", Utils.CreateNoncestr());
                dict.Add("body", package.Body);
                dict.Add("attach", package.Attach);
                dict.Add("out_trade_no", package.OutTradeNo);
                dict.Add("total_fee", (int)package.TotalFee);
                dict.Add("spbill_create_ip", package.SpbillCreateIp);
                dict.Add("time_start", package.TimeExpire);
                dict.Add("time_expire", "");
                dict.Add("goods_tag", package.GoodsTag);
                dict.Add("notify_url", package.NotifyUrl);
                dict.Add("trade_type", "MWEB");
                dict.Add("product_id", "");
                string sign = SignHelper.SignPackage(dict, _payAccount.PartnerKey);
                String mweb_url = GetMWebUrl(dict, sign, out prepayId);
                return mweb_url;
            }
            catch (Exception ex)
            {
                WxPayLog.writeLog(dict, "", "", ex.Message, LogType.Error);
                return "";
            }
        }
        /// <summary>
        /// 创建支付请求
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public PayRequestInfo BuildH5PayRequest(PackageInfo package)
        {
            string prepayId = "";
            PayRequestInfo payment = new PayRequestInfo();
            payment.appId = _payAccount.AppId;
            payment.mweb_url = BuildH5PayUrl(package, out prepayId);
            payment.prepayid = prepayId;
            payment.package = "";
            payment.timeStamp = Utils.GetCurrentTimeSeconds().ToString();
            payment.nonceStr = Utils.CreateNoncestr();
            //PayDictionary dict = new PayDictionary();
            //dict.Add("appId", _payAccount.AppId);
            ////   dict.Add("appkey", _payAccount.PaySignKey);
            //dict.Add("timeStamp", payment.timeStamp);
            //dict.Add("package", payment.package);
            //dict.Add("nonceStr", payment.nonceStr);

            //dict.Add("signType", "MD5");
            //payment.paySign = SignHelper.SignPay(dict, _payAccount.PartnerKey);
            return payment;
        }
        /// <summary>
        /// 获取h5支付链接
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="sign"></param>
        /// <param name="prepayId"></param>
        /// <returns></returns>
        internal string GetMWebUrl(PayDictionary dict, string sign, out string prepayId)
        {
            prepayId = "";
            dict.Add("sign", sign);
            string query = SignHelper.BuildQuery(dict, false);
            String param = SignHelper.BuildXml(dict, false);
            string result = PostData(prepay_id_Url, param);
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            try
            {
                doc.LoadXml(result);
                // doc.Load(decompress);
            }
            catch (Exception ex)
            {
                WxPayLog.writeLog(dict, "加载xml文件错误：" + result + ",错误信息：" + ex.Message, query, param, LogType.GetPrepayID);
                return "";
            }
            try
            {
                if (doc == null) { WxPayLog.writeLog(dict, "加载xml文件错误：" + result, query, param, LogType.GetPrepayID); return ""; }
                XmlNode retrunnode = doc.SelectSingleNode("xml/return_code");
                XmlNode resultnode = doc.SelectSingleNode("xml/result_code");
                if (retrunnode == null || resultnode == null)
                {
                    WxPayLog.writeLog(dict, "retrunnode或者resultnode为空：" + result, query, param, LogType.GetPrepayID);
                    return "";
                }
                XmlNode prepayIdNode = doc.SelectSingleNode("xml/prepay_id");
                if (prepayIdNode != null)
                {
                    prepayId = prepayIdNode.InnerText;
                }
                if (retrunnode.InnerText == "SUCCESS" && resultnode.InnerText == "SUCCESS")
                {
                    XmlNode mwebUrlNode = doc.SelectSingleNode("xml/mweb_url");
                    if (mwebUrlNode != null)
                    {
                        return mwebUrlNode.InnerText;
                    }
                    else
                    {
                        WxPayLog.writeLog(dict, "获取mweb_url结节为空：" + result, query, param, LogType.GetPrepayID);
                        return "";
                    }
                }
                else
                {
                    WxPayLog.writeLog(dict, "返回状态为不成功：" + result, query, param, LogType.GetPrepayID);
                    return "";
                }
            }
            catch (Exception ex) { WxPayLog.writeLog(dict, "加载xml结点失败：" + result + "，错误信息：" + ex.Message, query, param, LogType.GetPrepayID); return ""; }
        }

        #endregion
        #region App端微信支付
        #region 创建APP微信支付请求
        /// <summary>
        /// 创建支付请求
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public PayRequestInfo BuildAppPayRequest(PackageInfo package)
        {
            PayRequestInfo payment = new PayRequestInfo();
            payment.appId = _payAccount.AppId;
            payment.package = "Sign=WXPay";
            payment.timeStamp = Utils.GetCurrentTimeSeconds().ToString();
            payment.nonceStr = Utils.CreateNoncestr();
            payment.prepayid = BuildAppPackage(package);
            PayDictionary dict = new PayDictionary();
            dict.Add("appid", _payAccount.AppId);
            dict.Add("partnerid", _payAccount.PartnerId);
            dict.Add("prepayid", payment.prepayid);
            dict.Add("package", payment.package);
            dict.Add("noncestr", payment.nonceStr);
            dict.Add("timestamp", payment.timeStamp);
            ////如果包含了参数Sub_mch_id和sub_appid则为服务商模式
            //if (!string.IsNullOrEmpty(_payAccount.sub_mch_id) && !string.IsNullOrEmpty(_payAccount.Sub_AppId))
            //{
            //    dict.Add("sub_mch_id", _payAccount.sub_mch_id);
            //    dict.Add("sub_appid", _payAccount.Sub_AppId);
            //    if (!string.IsNullOrEmpty(package.OpenId))
            //    {
            //        dict.Add("sub_openid", package.OpenId);
            //    }
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(package.OpenId))
            //    {
            //        dict.Add("openid", package.OpenId);
            //    }
            //}
            //dict.Add("signType", "MD5");
            payment.paySign = SignHelper.SignPay(dict, _payAccount.PartnerKey);
            return payment;
        }
        //        dict.Add("appid", _payAccount.AppId);
        //dict.Add("partnerId", _payAccount.PartnerId);
        //dict.Add("prepayid", payment.prepayid);
        //dict.Add("package", payment.package);
        //dict.Add("noncestr", payment.nonceStr);
        //dict.Add("timestamp", payment.timeStamp);
        //dict.Add("partner_key", _payAccount.PartnerKey);
        //dict.Add("signType", "MD5");
        #endregion

        #region 创建APP数据包
        internal string BuildAppPackage(PackageInfo package)
        {
            //公众账号ID appid 是  String(32) 微信分配的公众账号ID
            //商户号mch_id 是 String(32) 微信支付分配的商户号
            //设备号device_info 否 String(32) 微信支付分配的终端设备号
            //随机字符串nonce_str 是 String(32) 随机字符串，不长于32 位
            //签名sign 是 String(32) 签名,详细签名方法见3.2 节
            //商品描述body 是 String(127) 商品描述
            //附加数据attach 否 String(127) 附加数据，原样返回
            //商户订单号out_trade_no 是 String(32) 商户系统内部的订单号,32个字符内、可包含字母,确保在商户系统唯一,详细说明
            //总金额total_fee 是 Int 订单总金额，单位为分，不能带小数点
            //终端IP spbill_create_ip 是 String(16) 订单生成的机器IP
            //交易起始时间time_start 否S tring(14) 订单生成时间， 格式为yyyyMMddHHmmss，如2009 年12 月25 日9 点10 分10 秒表示为20091225091010。时区为GMT+8 beijing。该时间取自商户服务器
            //交易结束时间time_expire 否 String(14) 订单失效时间， 格式为yyyyMMddHHmmss，如2009 年12 月27 日9 点10 分10 秒表示为20091227091010。时区为GMT+8 beijing。该时间取自商户服务器
            //商品标记goods_tag 否 String(32) 商品标记，该字段不能随便填，不使用请填空
            //通知地址notify_url 是 String(256) 接收微信支付成功通知
            //交易类型trade_type 是 String(16) JSAPI、NATIVE、APP
            //用户标识openid 否 String(128) 用户在商户appid 下的唯一标识，trade_type 为JSAPI时，此参数必传
            //商品ID product_id 否 String(32)只在trade_type 为NATIVE时需要填写。此id 为二维码中包含的商品ID，
            PayDictionary dict = new PayDictionary();
            dict.Add("appid", _payAccount.AppId);
            dict.Add("mch_id", _payAccount.PartnerId);

            //如果包含了参数Sub_mch_id和sub_appid则为服务商模式
            if (!string.IsNullOrEmpty(_payAccount.sub_mch_id) && !string.IsNullOrEmpty(_payAccount.Sub_AppId))
            {
                dict.Add("sub_mch_id", _payAccount.sub_mch_id);
                dict.Add("sub_appid", _payAccount.Sub_AppId);
                if (!string.IsNullOrEmpty(package.OpenId))
                {
                    dict.Add("sub_openid", package.OpenId);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(package.OpenId))
                {
                    dict.Add("openid", package.OpenId);
                }
            }
            dict.Add("device_info", "");
            dict.Add("nonce_str", Utils.CreateNoncestr());
            dict.Add("body", package.Body);
            dict.Add("attach", package.Attach);
            dict.Add("out_trade_no", package.OutTradeNo);
            dict.Add("total_fee", (int)package.TotalFee);
            dict.Add("spbill_create_ip", package.SpbillCreateIp);
            dict.Add("time_start", package.TimeExpire);
            dict.Add("time_expire", "");
            dict.Add("goods_tag", package.GoodsTag);
            dict.Add("notify_url", package.NotifyUrl);
            dict.Add("trade_type", "APP");
            dict.Add("product_id", "");
            string sign = SignHelper.SignPackage(dict, _payAccount.PartnerKey);
            //WxPayLog.writeLog(dict, sign, "", "微信支付", LogType.GetPrepayID);
            String Prepay_Id = GetPrepay_id(dict, sign);
            //if (Prepay_Id.Length > 64) Prepay_Id = "";
            return Prepay_Id;
        }
        #endregion
        #endregion


        #region public bool DeliverNotify(DeliverInfo deliver, string token)

        public bool DeliverNotify(DeliverInfo deliver)
        {
            string token = Utils.GetToken(_payAccount.AppId, _payAccount.AppSecret);

            return DeliverNotify(deliver, token);
        }
        /// <summary>
        /// 发货通知
        /// </summary>
        /// <param name="deliver"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DeliverNotify(DeliverInfo deliver, string token)
        {
            PayDictionary dict = new PayDictionary();
            //  dict.Add("appkey", _payAccount.PaySignKey);
            dict.Add("appid", _payAccount.AppId);
            dict.Add("openid", deliver.OpenId);
            dict.Add("transid", deliver.TransId);
            dict.Add("out_trade_no", deliver.OutTradeNo);
            dict.Add("deliver_timestamp", Utils.GetTimeSeconds(deliver.TimeStamp));
            dict.Add("deliver_status", deliver.Status ? 1 : 0);
            dict.Add("deliver_msg", deliver.Message);
            //  dict.Remove("appkey");
            deliver.AppId = _payAccount.AppId;
            deliver.AppSignature = SignHelper.SignPay(dict);


            dict.Add("app_signature", deliver.AppSignature);
            dict.Add("sign_method", deliver.SignMethod);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dict);

            string url = String.Format("{0}?access_token={1}", Deliver_Notify_Url, token);

            string resp = new WebUtils().DoPost(url, json);

            if (String.IsNullOrEmpty(resp) || !resp.Contains("ok"))
                return false;

            return true;
        }
        #endregion
        /// <summary>
        /// 获取PrepayID
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        internal string GetPrepay_id(PayDictionary dict, string sign)
        {
            dict.Add("sign", sign);
            string query = SignHelper.BuildQuery(dict, false);
            String param = SignHelper.BuildXml(dict, false);
            string result = PostData(prepay_id_Url, param);
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            try
            {
                doc.LoadXml(result);
                // doc.Load(decompress);
            }
            catch (Exception ex)
            {
                WxPayLog.writeLog(dict, "加载xml文件错误：" + result + ",错误信息：" + ex.Message, query, param, LogType.GetPrepayID);
                return "";
            }
            try
            {
                if (doc == null) { WxPayLog.writeLog(dict, "加载xml文件错误：" + result, query, param, LogType.GetPrepayID); return ""; }
                XmlNode retrunnode = doc.SelectSingleNode("xml/return_code");
                XmlNode resultnode = doc.SelectSingleNode("xml/result_code");
                if (retrunnode == null || resultnode == null)
                {
                    WxPayLog.writeLog(dict, "retrunnode或者resultnode为空：" + result, query, param, LogType.GetPrepayID);
                    return "";
                }

                if (retrunnode.InnerText == "SUCCESS" && resultnode.InnerText == "SUCCESS")
                {
                    XmlNode prepayNode = doc.SelectSingleNode("xml/prepay_id");
                    if (prepayNode != null)
                    {
                        return prepayNode.InnerText;
                    }
                    else
                    {
                        WxPayLog.writeLog(dict, "获取Prepay_id结节为空：" + result, query, param, LogType.GetPrepayID);
                        return "";
                    }
                }
                else
                {
                    WxPayLog.writeLog(dict, "返回状态为不成功：" + result, query, param, LogType.GetPrepayID);
                    return "";
                }
            }
            catch (Exception ex) { WxPayLog.writeLog(dict, "加载xml结点失败：" + result + "，错误信息：" + ex.Message, query, param, LogType.GetPrepayID); return ""; }
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }
        /// <summary>
        /// 提交请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string PostData(string url, string postData)
        {
            string result = string.Empty;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
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
                        return result;

                        //}
                    }
                }
            }
            catch (Exception ex) { result = string.Format("ERROR获取信息错误post error：{0}", ex.Message) + result; }
            #endregion


            return result;
        }
    }
}

