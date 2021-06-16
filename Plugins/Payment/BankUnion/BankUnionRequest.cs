using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Globalization;
using System.Web;
using System.Data;

namespace Hishop.Plugins.Payment.BankUnion
{
    [Plugin("银联在线")]
    public class BankUnionRequest : PaymentRequest
    {
        public BankUnionRequest(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            v_oid = orderId;
            v_amount = (Math.Round(amount * 100, 0).ToString());
            v_returnUrl = returnUrl;
            v_date = date.ToString("yyyyMMddHHmmss");
            v_notifyUrl = notifyUrl;
        }

        public BankUnionRequest()
        {
        }

        #region 常量
        //测试环境http://58.246.226.99/UpopWeb/api/Pay.action
        //正式环境https://unionpaysecure.com/api/Pay.action
        private const string Gateway = "https://unionpaysecure.com/api/Pay.action";
        private const string v_moneytype = "CNY";
        private const string Remark1 = "Bankunion";
        #endregion

        /// <summary>
        /// 商户编号
        /// </summary>
        [ConfigElement("商户号", Nullable = false)]
        public string Vmid { get; set; }


        /// <summary>
        /// 商户名称
        /// </summary>
        [ConfigElement("商户名称", Nullable = false)]
        public string VmName { get; set; }

        /// <summary>
        /// 商户密钥
        /// </summary>
        [ConfigElement("商户密钥", Nullable = false)]
        public string Key { get; set; }              //账户的支付宝安全校验码

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        private readonly string v_oid = "";

        /// <summary>
        /// 交易金额 单位：分
        /// </summary>
        private readonly string v_amount = "";

        /// <summary>
        /// 交易时间
        /// </summary>
        private readonly string v_date = "";

        /// <summary>
        /// 前台回调商户URL
        /// </summary>
        private readonly string v_returnUrl = "";

        /// <summary>
        /// 后台回调商户URL
        /// </summary>
        private readonly string v_notifyUrl = "";

        private string[] ValueVo
        {
            get;
            set;
        }
        public override void SendRequest()
        {
            // 生成签名
            QuickPayConf.merCode = Vmid;
            QuickPayConf.merName = VmName;
            QuickPayConf.securityKey = Key;
            //商户需要组装如下对象的数据
            string[] valueVo = new string[]{
				QuickPayConf.version,//协议版本
				QuickPayConf.charset,//字符编码
	            "01",//交易类型
	            "",//原始交易流水号
	            QuickPayConf.merCode,//商户代码
	            QuickPayConf.merName,//商户简称
	            "",//收单机构代码（仅收单机构接入需要填写）
	            "",//商户类别（收单机构接入需要填写）
	            "",//商品URL
	            "",//商品名称
	            v_amount,//商品单价 单位：分
	            "1",//商品数量
	            "0",//折扣 单位：分
	            "0",//运费 单位：分
				v_oid,//订单号（需要商户自己生成）
	            v_amount,//交易金额 单位：分
	            "156",//交易币种
				v_date,//交易时间
	            GetUserIP(),// "127.0.0.1",//用户IP
	            "",//用户真实姓名
	            "",//默认支付方式
	            "",//默认银行编号
	            "",//交易超时时间
	            v_returnUrl,//前台回调商户URL
	            v_notifyUrl,//后台回调商户URL
	            ""//商户保留域
			};
            ValueVo = valueVo;
            QuickPayConf.gateWay = Gateway;
            SubmitPaymentForm("");
        }
        protected override void SubmitPaymentForm(string formContent)
        {
            string html = new QuickPayUtils().createPayHtml(ValueVo);
            HttpContext.Current.Response.ContentType = "text/html;charset=" + QuickPayConf.charset;
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding(QuickPayConf.charset);
            try
            {
                SortedDictionary<string, string> map = new SortedDictionary<string, string>(StringComparer.Ordinal);
                for (int i = 0; i < QuickPayConf.reqVo.Length; i++)
                {
                    map.Add(QuickPayConf.reqVo[i], ValueVo[i]);
                }
                //WriteError(joinMapValue(map, '&'),html);
                HttpContext.Current.Response.Write(html);
            }
            catch (Exception)
            {
            }
        }

        private string joinMapValue(SortedDictionary<string, string> map, char connector)
        {
            StringBuilder b = new StringBuilder();
            foreach (KeyValuePair<string, string> entry in map)
            {
                b.Append(entry.Key);
                b.Append('=');
                if (entry.Value != null)
                {
                    b.Append(entry.Value);
                }
                b.Append(connector);
            }
            return b.ToString();
        }
        public void WriteError(string data,string error)
        {
            DataTable dt = new DataTable();
            dt.TableName = "BankUnion";
            dt.Columns.Add(new DataColumn("OperTime"));
            dt.Columns.Add(new DataColumn("OperIP"));
            dt.Columns.Add(new DataColumn("OperData"));
            dt.Columns.Add(new DataColumn("Html"));
            DataRow dr = dt.NewRow();
            dr["OperTime"] = DateTime.Now;
            dr["OperIP"] = GetUserIP();
            dr["OperData"] = data;
            dr["html"] = error;
            dt.Rows.Add(dr);
            dt.WriteXml(HttpContext.Current.Server.MapPath("/BankUnionErr.xml"));
                 
        }

        /// <summary>
        ///获取用户的IP地址获取客户IP地址
        /// </summary>
        /// <returns></returns>
        public static String GetUserIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            if (result.Length >= 20 || result == null || string.IsNullOrEmpty(result)) { result = "127.0.0.1"; }
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
