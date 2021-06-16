using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;

namespace Hishop.Plugins.Outpay.Alipay
{
    [Plugin("支付宝批量放款", Sequence = 1)]
    public class AlipayRequest : OutpayRequest
    {
        public AlipayRequest()
        {

        }
        public AlipayRequest(string[] outpayId, decimal[] amount,
            string[] userAccount, string[] realName, string[] openId, int[] userId, string[] desc, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            this.outpayId = outpayId;
            this.amount = amount;
            this.userAccount = userAccount;
            this.realName = realName;
            this.openId = openId;
            this.userId = userId;
            this.desc = desc;
            this.date = date;
            this.notifyUrl = notifyUrl;
            this.attach = attach;
            this.returnUrl = returnUrl;
        }
        [ConfigElement("partner", Nullable = false, Description = "合作者身份(PID)")]
        public string partner { get; set; } //卖家email

        [ConfigElement("key", Nullable = false, Description = "安全校验码(Key)")]
        public string key { get; set; }

        [ConfigElement("email", Nullable = false, Description = "付款帐号")]
        public string email { get; set; }

        [ConfigElement("account_name", Nullable = false, Description = "付款帐号名")]
        public string account_name { get; set; }

        public readonly string returnUrl;
        private readonly string attach;
        private readonly string notifyUrl;
        private readonly DateTime date;
        private readonly string[] outpayId;
        private readonly decimal[] amount;
        private readonly string[] userAccount;
        private readonly string[] realName;
        private readonly string[] openId;
        private readonly int[] userId;
        private readonly string[] desc;
        string input_charset = "utf-8";
        
        public override void SendRequest()
        {
            Core.setConfig(partner, "MD5", key, input_charset);

            ArrayList lenArray = new ArrayList();
            lenArray.Add(outpayId.Length);
            lenArray.Add(amount.Length);
            lenArray.Add(userAccount.Length);
            lenArray.Add(realName.Length);
            lenArray.Add(openId.Length);
            lenArray.Add(userId.Length);
            lenArray.Sort();
            int batchLen = (int)lenArray[0];
            if (batchLen == 0)
                return;
            decimal batch_fee = 0;
            string detail_data = "";
            for (int i = 0; i < batchLen; i++)
            {
                batch_fee += amount[i];
                if (i != 0)
                {
                    detail_data += "|";
                }
                detail_data += outpayId[i] + "^" + userAccount[i] + "^" + realName[i] + "^" + amount[i].ToString("f2") + "^" + (desc.Length > i ? desc[i] : "");
            }

            string pay_date = DateTime.Now.ToString("yyyyMMdd");//必填，格式：年[4位]月[2位]日[2位]，如：20100801
            string batch_no = DateTime.Now.ToString("yyyyMMddHHmmssff"); //批次号当天日期[8位]+序列号[3至16位]，如：201008010000001
            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();

            sParaTemp.Add("partner", partner);
            sParaTemp.Add("_input_charset", Core._input_charset);
            sParaTemp.Add("service", "batch_trans_notify");
            sParaTemp.Add("notify_url", notifyUrl);
            sParaTemp.Add("email", email);
            sParaTemp.Add("account_name", account_name);
            sParaTemp.Add("pay_date", pay_date);
            sParaTemp.Add("batch_no", batch_no);
            sParaTemp.Add("batch_fee", batch_fee.ToString());
            sParaTemp.Add("batch_num", batchLen.ToString());
            sParaTemp.Add("detail_data", detail_data);

            string sHtmlText = Core.BuildRequest(sParaTemp, "get", "确认");
            HttpContext.Current.Response.Write(sHtmlText);
        }

        public override IList<IDictionary<string, string>> SendRequestByResult()
        {
            return null;
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
