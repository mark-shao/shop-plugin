using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Hishop.Plugins.Outpay.Alipay
{
    public class AlipayNotify : OutpayNotify
    {
        private readonly NameValueCollection parameters;
        private const string InputCharset = "utf-8";
        IList<string> outPayIdList = null;
        IList<decimal> amountList = null;
        IList<String> gatewayOrderIdList = null;
        IList<DateTime> payTimeList = null;
        IList<Boolean> _Status = null;
        IList<String> _ErrMsg = null;
        public override bool VerifyNotify(int timeout, string configXml)
        {
            try
            {
                SortedDictionary<string, string> sPara = GetRequestPost(this.parameters);
                if (sPara == null || parameters == null)
                {
                    PayLog.writeLog_Collection(parameters, "", "", "参数为空", LogType.AlipayNotify);
                    return false;

                }
                PayLog.writeLog_Collection(parameters, "", "", "进入批量付款回调页面_plugins", LogType.AlipayNotify);
                Notify aliNotify = new Notify();
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.LoadXml(configXml);
                string sign_type = "MD5";
                if (sPara.ContainsKey("sign_type"))
                {
                    sign_type = sPara["sign_type"];
                }

                string key = doc.FirstChild.SelectSingleNode("key").InnerText;
                string partner = doc.FirstChild.SelectSingleNode("partner").InnerText;
                Core.setConfig(partner, sign_type, key, InputCharset);
                //Core.setConfig(doc.FirstChild.SelectSingleNode("key").InnerText,);
                bool verifyResult = aliNotify.Verify(sPara, sPara["notify_id"], sPara["sign"], key, sign_type);

                if (verifyResult)//验证成功
                {
                    string detail = "";
                    //批量付款数据中转账成功的详细信息
                    if (sPara.ContainsKey("success_details"))
                        detail = sPara["success_details"];
                    else
                        detail = sPara["fail_details"];
                    // 'SID号^帐号^姓名^金额^状态^备注^内部流水号^支付时间
                    if (!string.IsNullOrEmpty(detail))
                    {
                        outPayIdList = new List<string>();
                        amountList = new List<decimal>();
                        gatewayOrderIdList = new List<string>();
                        string[] ResultList = detail.Split('|');
                        _Status = new List<Boolean>();
                        _ErrMsg = new List<String>();
                        payTimeList = new List<DateTime>();
                        foreach (string ItemRs in ResultList)
                        {
                            if (string.IsNullOrEmpty(ItemRs))
                                continue;
                            string[] ItemDetail = ItemRs.Split('^');
                            if (ItemDetail.Length >= 8)
                            {
                                outPayIdList.Add(ItemDetail[0]);
                                _Status.Add(ItemDetail[4] == "F" ? false : true);
                                _ErrMsg.Add(ItemDetail[5]);
                                amountList.Add(Decimal.Parse(ItemDetail[3]));
                                gatewayOrderIdList.Add(ItemDetail[6]);
                                payTimeList.Add(ConvertToDateTime(ItemDetail[7]));
                            }
                        }
                    }
                    return true;
                }
                else//验证失败
                {
                    PayLog.writeLog_Collection(parameters, "", "", "签名验证失败_plugins", LogType.AlipayNotify);
                    return false;

                }
            }
            catch (Exception ex)
            {
                IDictionary<string, string> param = new Dictionary<string, string>();
                param.Add("Message", ex.Message);
                param.Add("Source", ex.Source);
                param.Add("StackTrace", ex.StackTrace);
                param.Add("TargetSite", ex.TargetSite.ToString());
                param.Add("ConfigXml", configXml);
                PayLog.writeLog(param, "", "", "", LogType.AlipayNotify);
                return false;
            }
        }


        private DateTime ConvertToDateTime(string timeStr)
        {
            string newTime = timeStr;
            if (timeStr.Length >= 14)
            {
                newTime = timeStr.Substring(0, 4) + "-" + timeStr.Substring(4, 2) + "-" + timeStr.Substring(6, 2) + " " + timeStr.Substring(8, 2) + ":" + timeStr.Substring(10, 2) + ":" + timeStr.Substring(12, 2);
            }
           // PayLog.AppendLog(null, "", newTime, "", LogType.AlipayNotify);
            DateTime time = DateTime.Now;

            DateTime.TryParse(newTime, out time);
            return time;
        }
        public AlipayNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
            //PayLog.AppendLog_Collection(parameters, "", "", "进入通知页面", LogType.AlipayNotify);
        }

        public SortedDictionary<string, string> GetRequestPost(NameValueCollection coll)
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            if (coll != null)
                foreach (string s in coll.AllKeys)
                {
                    sArray.Add(s, coll[s]);
                }
            return sArray;
        }
        public override void WriteBack(HttpContext context, bool success)
        {
            if (context == null)
                return;

            context.Response.Clear();
            context.Response.Write(success ? "success" : "fail");
            context.Response.End();
        }
        public override IList<decimal> GetOrderAmount()
        {
            return amountList;
        }

        public override IList<string> GetOutpayId()
        {
            return outPayIdList;
        }

        public override IList<string> GetGatewayOrderId()
        {
            return gatewayOrderIdList;
        }

        public override IList<DateTime> GetPayTime()
        {
            return payTimeList;
        }

        public override IList<bool> GetStatus()
        {
            return _Status;
        }

        public override IList<string> GetErrMsg()
        {
            return _ErrMsg;
        }
        public override string GetRemark1()
        {
            return base.GetRemark1();
        }

        public override string GetRemark2()
        {
            return base.GetRemark2();
        }

        protected override string GetResponse(string url, int timeout)
        {
            return base.GetResponse(url, timeout);
        }


        protected override void OnFinished()
        {
            base.OnFinished();
        }

        protected override void OnNotifyVerifyFaild()
        {
            base.OnNotifyVerifyFaild();
        }

        protected override void OnPayment()
        {
            base.OnPayment();
        }
    }
}
