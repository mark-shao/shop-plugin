using System;
using System.Collections.Generic;

namespace Hishop.Weixin.Pay
{
    /// <summary>
    /// 支付账户
    /// </summary>
    public class PayAccount
    {
        /// <summary>
        /// 默认购造函数
        /// </summary>
        public PayAccount()
        {

        }
        /// <summary>
        /// 带参数购造函数
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="partnerId"></param>
        /// <param name="partnerKey"></param>
        /// <param name="paySignKey"></param>
        /// <param name="sub_mch_id"></param>
        /// <param name="sub_AppId"></param>
        /// <param name="sub_OpenId"></param>
        public PayAccount(string appId, string appSecret, string partnerId, string partnerKey, string paySignKey, string sub_mch_id = "", string sub_AppId = "", string sub_OpenId = "")
        {
            AppId = appId;
            AppSecret = appSecret;
            PartnerId = partnerId;//对应新版的mchid 商户号
            PartnerKey = partnerKey;//对应新版的key
            PaySignKey = paySignKey;//此项在版中已无效
            this.sub_mch_id = sub_mch_id;
            Sub_AppId = sub_AppId;
            Sub_OpenId = sub_OpenId;
        }
        /// <summary>
        /// 子OpenId
        /// </summary>
        public string Sub_OpenId
        {
            get;
            set;
        }
        /// <summary>
        /// 子AppId
        /// </summary>
        public string Sub_AppId { get; set; }

        /// <summary>
        /// 公众号身份的唯一标识
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 公众平台接口API的权限获取所需密钥Key
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 财付通商户身份的标识
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// 财付通商户权限密钥Key
        /// </summary>
        public string PartnerKey { get; set; }

        /// <summary>
        /// 公众号支付请求中用于加密的密钥Key
        /// </summary>
        public string PaySignKey { get; set; }

        /// <summary>
        /// Sub_Mch_ID
        /// </summary>
        public string sub_mch_id { get; set; }
    }
}
