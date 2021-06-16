using System;
using System.Collections.Generic;
using System.IO;
using Hishop.Weixin.Pay.Notify;
using Hishop.Weixin.Pay.Util;
using System.Data;
using System.Web;
using System.Xml.Serialization;
using Hishop.Weixin.Pay.Domain;

namespace Hishop.Weixin.Pay
{
    /// <summary>
    /// 通知客户端
    /// </summary>
    public class NotifyClient
    {
        /// <summary>
        /// 标记客户的投诉处理状态网关
        /// </summary>
        public static readonly string Update_Feedback_Url = "https://api.weixin.qq.com/payfeedback/update";

        private PayAccount _payAccount;

        #region 构造器

        public NotifyClient(string appId, string appSecret, string partnerId, string partnerKey, string paySignKey = "", string sub_appid = "", string sub_mchid = "")
        {
            _payAccount = new PayAccount()
            {
                AppId = appId,
                AppSecret = appSecret,
                PartnerId = partnerId,
                PartnerKey = partnerKey,
                PaySignKey = paySignKey,
                Sub_AppId = sub_appid,
                sub_mch_id = sub_mchid
            };
        }

        public NotifyClient(PayAccount account)
            : this(account.AppId,
                account.AppSecret,
                account.PartnerId,
                account.PartnerKey,
                account.PaySignKey,
                account.Sub_AppId,
                account.sub_mch_id)
        {
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取POST数据
        /// </summary>
        /// <param name="inStream"></param>
        /// <returns></returns>
        private string ReadString(Stream inStream)
        {
            if (inStream == null)
                return null;

            byte[] byts = new byte[inStream.Length];

            inStream.Read(byts, 0, byts.Length);

            return System.Text.Encoding.UTF8.GetString(byts);
        }

        /// <summary>
        /// 验证支付通知签名
        /// </summary>
        /// <param name="notify"></param>
        /// <returns></returns>
        private bool ValidPaySign(PayNotify notify, out string servicesign)
        {

            PayDictionary dict = new PayDictionary();
            //dict.Add("appid", _payAccount.AppId);
            //// dict.Add("appkey", _payAccount.PaySignKey);
            //dict.Add("timestamp", notify.TimeStamp);
            //dict.Add("noncestr", notify.NonceStr);
            //dict.Add("openid", notify.OpenId);
            //dict.Add("issubscribe", notify.IsSubscribe ? 1 : 0);
            //notify.PayInfo.is
            //dict.Add("appId", _payAccount.AppId);
            ////   dict.Add("appkey", _payAccount.PaySignKey);
            //dict.Add("timeStamp", notify.TimeStamp);
            //dict.Add("package", payment.package);
            //dict.Add("nonceStr", notify.NonceStr);
            //dict.Add("appid", notify.appid);
            //dict.Add("bank_type", notify.bank_type);
            //dict.Add("cash_fee", notify.cash_fee);
            //dict.Add("fee_type", notify.fee_type);
            //dict.Add("is_subscribe", notify.is_subscribe);
            //dict.Add("mch_id", notify.mch_id);
            //dict.Add("nonce_str", notify.nonce_str);
            //dict.Add("openid", notify.openid);
            //dict.Add("out_trade_no", notify.out_trade_no);
            //dict.Add("result_code", notify.result_code);
            //dict.Add("return_code", notify.return_code);
            //dict.Add("time_end", notify.time_end);
            //dict.Add("total_fee", notify.total_fee);
            //dict.Add("trade_type", notify.trade_type);
            //dict.Add("transaction_id", notify.transaction_id);
            //dict.Add("attach", notify.attach);
            //dict.Add("sub_mch_id", notify.sub_mch_id);
            //dict.Add("sub_openid", notify.sub_openid);
            //dict.Add("sub_appid", notify.sub_appid);
            //dict.Add("sub_is_subscribe", notify.sub_is_subscribe);
            //dict.Add("prepay_id", notify.prepay_id);
            dict = Utils.GetPayDictionary(notify);
            servicesign = SignHelper.SignPay(dict, _payAccount.PartnerKey);
            
            bool ret = (notify.sign == servicesign);
            if (!ret)
            {
                WxPayLog.writeLog(dict, servicesign, "", "签名验证失败", LogType.PayNotify);
            }
            servicesign = servicesign + "-" + SignHelper.BuildQuery(dict, false);
            return ret;
        }

        /// <summary>
        /// 验证告警通知签名
        /// </summary>
        /// <param name="notify"></param>
        /// <returns></returns>
        private bool ValidAlarmSign(AlarmNotify notify)
        {
            //官方文档居然没写哪些参数参与验证,所以直接返回True
            return true;
        }

        /// <summary>
        /// 验证反馈通知签名
        /// </summary>
        /// <param name="notify"></param>
        /// <returns></returns>
        private bool ValidFeedbackSign(FeedBackNotify notify)
        {
            PayDictionary dict = new PayDictionary();
            dict.Add("appid", _payAccount.AppId);
            // dict.Add("appkey", _payAccount.PaySignKey);
            dict.Add("timestamp", notify.TimeStamp);
            dict.Add("openid", notify.OpenId);

            return notify.AppSignature == SignHelper.SignPay(dict);
        }
        #endregion

        #region public PayNotify GetPayNotify(Stream inStream)

        public PayNotify GetPayNotify(Stream inStream)
        {
            string xml = ReadString(inStream);
            
            return GetPayNotify(xml);
        }
        /// <summary>
        /// 创建错误表格
        /// </summary>
        /// <param name="tabName"></param>
        /// <returns></returns>
        public DataTable ErrorTable(String tabName = "Notify")
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("OperTime"));
            dt.Columns.Add(new DataColumn("Error"));
            dt.Columns.Add(new DataColumn("Param"));
            dt.Columns.Add(new DataColumn("PayInfo"));
            dt.TableName = tabName;
            return dt;

        }

        /// <summary>
        /// 获取支付通知
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public PayNotify GetPayNotify(string xml)
        {
            DataTable Notifydt = ErrorTable();
            DataRow dr = Notifydt.NewRow();

            dr["OperTime"] = DateTime.Now;
            try
            {
                if (String.IsNullOrEmpty(xml))
                {
                    //dr["Error"] = "InStream Null";
                    //dr["Param"] = "null";
                    //Notifydt.Rows.Add(dr);
                    //WriteLog(Notifydt);
                    return null;
                }
                xml = xml.Trim().Replace(" ", "");//去掉空格
                PayNotify notify = Utils.GetNotifyObject<PayNotify>(xml);
                string servicesign = "";
                if (notify == null || !ValidPaySign(notify, out servicesign))
                {

                    Notifydt.Rows.Add(dr);
                    IDictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("ErrorMsg", (notify == null ? "Notify Null" : "Valid pay Sign Error"));
                    param.Add("result", xml);
                    WxPayLog.AppendLog(param, servicesign, "", "签名验证失败", LogType.PayNotify);
                    return null;
                }
                #region 支付信息是通过URL参数传递

                notify.PayInfo = new Domain.PayInfo()
                {
                    SignType = "MD5",
                    Sign = notify.sign,
                    TradeMode = 0,
                    BankType = notify.bank_type,
                    BankBillNo = "",
                    TotalFee = ((decimal)notify.total_fee) / 100,
                    FeeType = notify.fee_type == "CNY" ? 1 : 0,
                    NotifyId = "",
                    TransactionId = notify.transaction_id,
                    OutTradeNo = notify.out_trade_no,
                    //TimeEnd = Convert.ToDateTime(UrlHelper.GetStringUrlParam("time_end")),
                    TransportFee = 0,
                    ProductFee = 0,
                    Discount = 1,
                    BuyerAlias = "",
                    Attach = notify.attach
                };
                #endregion
                //string xmlRootName = "xml";
                //Type type = notify.PayInfo.GetType();
                //XmlSerializer serializer = new XmlSerializer(type);
                //using (StreamWriter writer = new StreamWriter(HttpContext.Current.Request.MapPath("/Notify.PayInfo.xml")))
                //{
                //    System.Xml.Serialization.XmlSerializer xmlSerializer = string.IsNullOrWhiteSpace(xmlRootName) ?
                //        new System.Xml.Serialization.XmlSerializer(type) :
                //        new System.Xml.Serialization.XmlSerializer(type, new XmlRootAttribute(xmlRootName));
                //    xmlSerializer.Serialize(writer, notify.PayInfo);
                //}

                return notify;
            }
            catch (Exception ex)
            {

                IDictionary<string, string> param = new Dictionary<string, string>();
                param.Add("result", xml);
                WxPayLog.WriteExceptionLog(ex, param, LogType.Error);

                return null;
            }



        }
        #endregion

        #region public AlarmNotify GetAlarmNotify(Stream inStream)

        /// <summary>
        /// 获取告警通知实体
        /// </summary>
        /// <param name="inStream"></param>
        /// <returns></returns>
        public AlarmNotify GetAlarmNotify(Stream inStream)
        {
            string xml = ReadString(inStream);

            return GetAlarmNotify(xml);
        }

        public AlarmNotify GetAlarmNotify(string xml)
        {
            if (String.IsNullOrEmpty(xml))
                return null;

            AlarmNotify notify = Utils.GetNotifyObject<AlarmNotify>(xml);

            if (notify == null || !ValidAlarmSign(notify))
                return null;

            return notify;
        }
        #endregion

        #region public FeedBackNotify GetFeedBackNotify(Stream inStream)

        public FeedBackNotify GetFeedBackNotify(Stream inStream)
        {
            string xml = ReadString(inStream);

            return GetFeedBackNotify(xml);
        }
        /// <summary>
        /// 获取反馈通知实体
        /// </summary>
        /// <param name="inStream"></param>
        /// <returns></returns>
        public FeedBackNotify GetFeedBackNotify(string xml)
        {
            if (String.IsNullOrEmpty(xml))
                return null;

            FeedBackNotify notify = Utils.GetNotifyObject<FeedBackNotify>(xml);

            if (notify == null || !ValidFeedbackSign(notify))
                return null;

            return notify;
        }
        #endregion

        #region public bool UpdateFeedback(string feedbackid, string openid, string token)

        public bool UpdateFeedback(string feedbackid, string openid)
        {
            string token = Utils.GetToken(_payAccount.AppId, _payAccount.AppSecret);

            return UpdateFeedback(feedbackid, openid, token);
        }

        /// <summary>
        /// 标记客户的投诉处理状态
        /// </summary>
        /// <param name="feedbackid"></param>
        /// <param name="openid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool UpdateFeedback(string feedbackid, string openid, string token)
        {
            string url = String.Format("{0}?access_token={1}&openid={2}&feedbackid={3}",
                Update_Feedback_Url, token, openid, feedbackid);

            string resp = new WebUtils().DoGet(url);

            if (String.IsNullOrEmpty(resp) || !resp.Contains("ok"))
                return false;

            return true;
        }
        #endregion
    }
}
