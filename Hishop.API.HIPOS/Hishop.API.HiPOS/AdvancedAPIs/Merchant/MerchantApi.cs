using Hishop.API.HiPOS.CommonAPIs;
using HiShop.API.HiPOS.AdvancedAPIs.Auth.AuthJson;
using HiShop.API.HiPOS.AdvancedAPIs.Merchant.MerchantJson;
using HiShop.API.Setting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hishop.API.HiPOS.AdvancedAPIs.Merchant
{
    public class MerchantApi
    {
        /// <summary>
        /// 更新商户资料
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="merchantId"></param>
        /// <param name="name">商户名称</param>
        /// <param name="contact">联系人</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static MerchantResult UpdateMerchant(string accessTokenOrAppId, string merchantId, string name, string contact, string mobile, int timeOut = Config.TIME_OUT)
        {
            return ApiHandlerWapper.TryCommonApi(accessToken =>
             {
                 var url = string.Format(HiPOSParameter.UPDATEMERCHANTS, merchantId);
                 Dictionary<string, string> postDataDictionary = new Dictionary<string, string>();
                 postDataDictionary.Add("name", name);
                 postDataDictionary.Add("contact", contact);
                 postDataDictionary.Add("mobile", mobile);
                 return HiShop.API.Setting.HttpUtility.Post.PostFileGetJson<MerchantResult>(url, null, null, postDataDictionary, null, timeOut, null, null, accessToken, Hishop.API.HiPOS.HiPOSParameter.HttpMethod.PUT);
             }, accessTokenOrAppId);
        }

        /// <summary>
        /// 获取支付宝开发者公钥
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="merchantId"></param>        
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static AlipayKeyResult GetAlipayKey(string accessTokenOrAppId, string merchantId, int timeOut = Config.TIME_OUT)
        {
            return ApiHandlerWapper.TryCommonApi(accessToken =>
            {
                var url = string.Format(HiPOSParameter.ALIPAYKEY, merchantId);
                return HiShop.API.Setting.HttpUtility.Get.GetJson<AlipayKeyResult>(url, null, string.Empty, string.Empty, accessToken);
            }, accessTokenOrAppId);
        }

        /// <summary>
        /// 更新 HiShopO2O 功能接口设置
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="merchantId">商户号</param>
        /// <param name="status">订单状态接口</param>
        /// <param name="confirm">订单确认接口</param>
        /// <param name="authqr">设备授权回调</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static HishopO2OResult SetHishopO2O(string accessTokenOrAppId, string merchantId, string status, string confirm, string authqr, int timeOut = Config.TIME_OUT)
        {
            return ApiHandlerWapper.TryCommonApi(accessToken =>
            {
                var url = string.Format(HiPOSParameter.HISHOPO2O, merchantId);
                Dictionary<string, string> postDataDictionary = new Dictionary<string, string>();
                postDataDictionary.Add("status", status);
                postDataDictionary.Add("confirm", confirm);
                postDataDictionary.Add("authqr", authqr);
                return HiShop.API.Setting.HttpUtility.Post.PostFileGetJson<HishopO2OResult>(url, null, null, postDataDictionary, null, timeOut, null, null, accessToken, Hishop.API.HiPOS.HiPOSParameter.HttpMethod.PUT);
            }, accessTokenOrAppId);
        }


        /// <summary>
        /// 更新支付方式
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="merchantId"></param>
        /// <param name="aliAppId">支付宝AppId</param>
        /// <param name="ali_pri_key">支付宝私钥</param>
        /// <param name="wxAppId">公众号AppId</param>
        /// <param name="wxMchId">微信支付商户号</param>
        /// <param name="wxPaySecret">微信支付API密钥</param>
        /// <param name="wxPayCert">微信支付证书(B64)</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static PaymentsResult SetPayments(string accessTokenOrAppId, string merchantId, string aliAppId, string alPriKey,string wxAppId, string wxMchId, string wxPaySecret, string wxPayCert, int timeOut = Config.TIME_OUT)
        {
            return ApiHandlerWapper.TryCommonApi(accessToken =>
            {
                var url = string.Format(HiPOSParameter.PAYMENTS, merchantId);
                Dictionary<string, string> postDataDictionary = new Dictionary<string, string>();
                postDataDictionary.Add("ali_app_id", aliAppId);
                postDataDictionary.Add("ali_pri_key", alPriKey);
                postDataDictionary.Add("wx_app_id", wxAppId);
                postDataDictionary.Add("wx_mch_id", wxMchId);
                postDataDictionary.Add("wx_pay_secret", wxPaySecret);
                postDataDictionary.Add("wx_pay_cert", wxPayCert);
                return HiShop.API.Setting.HttpUtility.Post.PostFileGetJson<PaymentsResult>(url, null, null, postDataDictionary, null, timeOut, null, null, accessToken, Hishop.API.HiPOS.HiPOSParameter.HttpMethod.PUT);
            }, accessTokenOrAppId);
        }

        /// <summary>
        /// 请求生成设备授权码
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="merchantId">商户号</param>
        /// <param name="storeName">门店名称</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static AuthCodeResult GetAuthCode(string accessTokenOrAppId, string merchantId, string storeName, int timeOut = Config.TIME_OUT)
        {
            return ApiHandlerWapper.TryCommonApi(accessToken =>
            {
                var url = string.Format(HiPOSParameter.AUTHCODE, merchantId);
                Dictionary<string, string> postDataDictionary = new Dictionary<string, string>();
                postDataDictionary.Add("store_name", storeName);
                return HiShop.API.Setting.HttpUtility.Post.PostFileGetJson<AuthCodeResult>(url, null, null, postDataDictionary, null, timeOut, null, null, accessToken, Hishop.API.HiPOS.HiPOSParameter.HttpMethod.POST);
            }, accessTokenOrAppId);
        }


        /// <summary>
        /// 查询交易统计
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="merchantId"></param>
        /// <param name="storeName">门店名称</param>
        /// <param name="from">开始日期</param>
        /// <param name="to">截止日期</param>
        /// <param name="page">当前页码</param>
        /// <param name="page_size">每页返回数量</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static TradesResult GetHishopTrades(string accessTokenOrAppId, string merchantId, string storeName, string from, string to, int page = 1, int page_size = 10, int timeOut = Config.TIME_OUT)
        {
            return ApiHandlerWapper.TryCommonApi(accessToken =>
            {
                var sbUrl = new StringBuilder();
                sbUrl.AppendFormat(HiPOSParameter.HISHOPTRADES, merchantId);
                sbUrl.AppendFormat("?store_name={0}&from={1}&to={2}&page={3}&page_size={4}", storeName, from, to, page, page_size);
                return HiShop.API.Setting.HttpUtility.Get.GetJson<TradesResult>(sbUrl.ToString(), null, string.Empty, string.Empty, accessToken);
            }, accessTokenOrAppId);
        }


        /// <summary>
        /// 查询交易详情
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="merchantId">商户号</param>
        /// <param name="storeId">门店编号</param>
        /// <param name="device_id">HiPOS编号</param>
        /// <param name="hishop_only">仅限商城订单</param>
        /// <param name="from">开始日期</param>
        /// <param name="to">截止日期</param>
        /// <param name="page">当前页码</param>
        /// <param name="page_size">每页返回数量</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static DetailResult GetHishopTradesDetail(string accessTokenOrAppId, string merchantId, string storeId, string device_id,string code, bool hishop_only, string from, string to, int page = 1, int page_size = 10, int timeOut = Config.TIME_OUT)
        {
            return ApiHandlerWapper.TryCommonApi(accessToken =>
            {
                var sbUrl = new StringBuilder();
                sbUrl.AppendFormat(HiPOSParameter.STOREDETAIL, merchantId,storeId);
                sbUrl.AppendFormat("?device_id={0}&hishop_only={1}&from={2}&to={3}&page={4}&page_size={5}&code={6}", device_id, hishop_only.ToString().ToLower(), from, to, page, page_size,code);
                return HiShop.API.Setting.HttpUtility.Get.GetJson<DetailResult>(sbUrl.ToString(), null, string.Empty, string.Empty, accessToken);
            }, accessTokenOrAppId);
        }
    }
}
