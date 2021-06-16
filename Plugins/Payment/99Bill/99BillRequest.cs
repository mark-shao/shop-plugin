using System;
using System.Text;
using System.Globalization;
using System.Web.Security;
using Hishop.Plugins;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Security.Cryptography;

namespace Hishop.Plugins.Payment.Bill99
{
    [Plugin("快钱", Sequence=6)]
    public class Bill99Request : PaymentRequest
    {

        public Bill99Request(
            string orderId, decimal amount,
            string subject, string body, string buyerEmail, DateTime date,
            string showUrl, string returnUrl, string notifyUrl, string attach)
        {
            productName = orderId;
            //bgUrl = returnUrl;
            bgUrl = notifyUrl;
            orderAmount = Convert.ToInt32(amount * 100).ToString(CultureInfo.InvariantCulture);
            this.orderId = orderId;
            orderTime = date.ToString("yyyyMMddhhmmss", CultureInfo.InvariantCulture);
        }

        public Bill99Request()
        {
        }

        #region 常量
        private const string ProductNum = "1";
        /// <summary>
        /// 网关地址
        /// </summary>
        private const string Gateway = "https://www.99bill.com/gateway/recvMerchantInfoAction.htm";

        /// <summary>
        /// 字符集
        /// 可为空，只能选择1、2、3.
        /// 1代表UTF-8; 2代表GBK; 3代表gb2312
        /// 默认值为1
        /// </summary>
        private const string InputCharset = "1";

        /// <summary>
        /// 网关版本.固定值
        /// 不可为空，快钱会根据版本号来调用对应的接口处理程序。
        /// 本代码版本号固定为v2.0
        /// </summary>
        private const string Version = "v2.0";

        /// <summary>
        /// 语言种类.固定选择值。
        /// 不可为空，只能选择1、2、3
        /// 1代表中文；2代表英文
        /// 默认值为1
        /// </summary>
        private const string Language = "1";

        /// <summary>
       ///  签名类型,该值为4，代表PKI加密方式,该参数必填。
        ///</summary>
        private const string SignType = "4";

        /// <summary>
        /// 扩展字段1，用于标识支付网关类型
        /// 可为空，在支付结束后原样返回给商户
        /// </summary>
        private const string Ext1 = "99Bill";

        /// <summary> 
        /// 支付方式.固定选择值
        /// 不可空，只能选择00、10、11、12、13、14
        /// 00：组合支付（网关支付页面显示快钱支持的各种支付方式，推荐使用）10：银行卡支付（网关支付页面只显示银行卡支付）.11：电话银行支付（网关支付页面只显示电话支付）.12：快钱账户支付（网关支付页面只显示快钱账户支付）.13：线下支付（网关支付页面只显示线下支付方式）.14：B2B支付（网关支付页面只显示B2B支付，但需要向快钱申请开通才能使用）
        /// </summary>
        private const string PayType = "00";

        /// <summary> 
        /// 同一订单禁止重复提交标志
        /// 不可为空，固定选择值： 1、0
        /// 1代表同一订单号只允许提交1次；0表示同一订单号在没有支付成功的前提下可重复提交多次。默认为0建议实物购物车结算类商户采用0；虚拟产品类商户采用1
        /// </summary>
        private const string RedoFlag = "0";
        #endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }

        /// <summary>
        /// 快钱将支付结果发送到bgUrl 对应的地址，并且获取
        /// 商户按照约定格式输出的地址，显示页面给用户
        /// </summary>
        private readonly string bgUrl;

        /// <summary>
        /// 商户编号
        /// 不可空，人民币网关账户号，本参数用来指定接收款项的人民币账号
        /// 请登录快钱系统获取用户编号，用户编号后加01即为人民币网关账户号。
        /// </summary>
        [ConfigElement("商户号", Nullable=false)]
        public string MerchantAcctId { get; set; }

        /// <summary>
        /// 商户订单号
        /// 不可空，由字母、数字、或[-][_]组成
        /// </summary>
        private readonly string orderId;

        /// <summary>
        /// 订单金额
        /// 不可空，以分为单位，必须是整型数字
        /// 比方2，代表0.02元
        /// </summary>
        private readonly string orderAmount;

        /// <summary>
        /// 订单提交时间
        /// 不可空，14位数字。年[4位]月[2位]日[2位]时[2位]分[2位]秒[2位]
        /// 如；20080101010101
        /// </summary>
        private readonly string orderTime;

        /// <summary>
        /// 商户密钥
        /// 不可为空，人民币网关密钥
        /// 区分大小写.请与快钱联系索取
        /// </summary>
    //    [ConfigElement("商户密钥", Nullable = false)]
    //    public string Key { get; set; }

        /// <summary>
        /// 快钱的合作伙伴的账户号
        /// 可为空，如未和快钱签订代理合作协议，不需要填写本参数
        /// </summary>
        [ConfigElement("合作伙伴账号")]
        public string Pid { get; set; }

        [ConfigElement("证书密码")]
        public string Certpwd { get; set; }


        private readonly string productName = "";
         
        public override void SendRequest()
        {
            // 生成签名
            //拼接字符串
            string signMsgVal = "";
            signMsgVal = appendParam(signMsgVal, "inputCharset", InputCharset);
            // signMsgVal = appendParam(signMsgVal, "pageUrl", pageUrl);
            signMsgVal = appendParam(signMsgVal, "bgUrl", bgUrl);
            signMsgVal = appendParam(signMsgVal, "version", Version);
            signMsgVal = appendParam(signMsgVal, "language", Language);
            signMsgVal = appendParam(signMsgVal, "signType", SignType);
            signMsgVal = appendParam(signMsgVal, "merchantAcctId", MerchantAcctId);
           // signMsgVal = appendParam(signMsgVal, "payerName", "");
          //  signMsgVal = appendParam(signMsgVal, "payerContactType", p);
           // signMsgVal = appendParam(signMsgVal, "payerContact", payerContact);
            signMsgVal = appendParam(signMsgVal, "orderId", orderId);
            signMsgVal = appendParam(signMsgVal, "orderAmount", orderAmount);
            signMsgVal = appendParam(signMsgVal, "orderTime", orderTime);
        //    signMsgVal = appendParam(signMsgVal, "productName", productName);
            signMsgVal = appendParam(signMsgVal, "productNum", ProductNum);
           // signMsgVal = appendParam(signMsgVal, "productId", productId);
            // signMsgVal = appendParam(signMsgVal, "productDesc", productDesc);
            signMsgVal = appendParam(signMsgVal, "ext1", Ext1);
            // signMsgVal = appendParam(signMsgVal, "ext2", ext2);
            signMsgVal = appendParam(signMsgVal, "payType", PayType);
            signMsgVal = appendParam(signMsgVal, "redoFlag", RedoFlag);
            if (!string.IsNullOrEmpty(Pid))
            {
                signMsgVal = appendParam(signMsgVal, "pid", Pid);
            }
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(signMsgVal);
            X509Certificate2 cert = new X509Certificate2(HttpContext.Current.Server.MapPath("~/plugins/payment/Cert/99bill-rsa.pfx"), Certpwd, X509KeyStorageFlags.MachineKeySet);
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PrivateKey;
            RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsapri);
            byte[] result;
            f.SetHashAlgorithm("SHA1");
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            result = sha.ComputeHash(bytes);
            string signmsg = System.Convert.ToBase64String(f.CreateSignature(result)).ToString();
           // 创建提交 form 
            StringBuilder sb = new StringBuilder();
            sb.Append(CreateField("inputCharset", InputCharset));
            sb.Append(CreateField("bgUrl", bgUrl));
            sb.Append(CreateField("version", Version));
            sb.Append(CreateField("language", Language));
            sb.Append(CreateField("signType", SignType));
            sb.Append(CreateField("signMsg", signmsg));
            sb.Append(CreateField("merchantAcctId", MerchantAcctId));
            sb.Append(CreateField("orderId", orderId));
            sb.Append(CreateField("orderAmount", orderAmount));
            sb.Append(CreateField("orderTime", orderTime));
          //  sb.Append(CreateField("productName", productName));
            sb.Append(CreateField("productNum", ProductNum));
            sb.Append(CreateField("ext1", Ext1));
            sb.Append(CreateField("payType", PayType));
            sb.Append(CreateField("redoFlag", RedoFlag));
            if (!string.IsNullOrEmpty(Pid))
            {
                sb.Append(CreateField("pid", Pid));
            }
           
            SubmitPaymentForm(CreateForm(sb.ToString(), Gateway));
        }

        //功能函数。将变量值不为空的参数组成字符串
        private string appendParam(string returnStr, string paramId, string paramValue)
        {
            if (returnStr != "")
            {
                if (paramValue != "")
                {
                    returnStr += "&" + paramId + "=" + paramValue;
                }
            }
            else
            {
                if (paramValue != "")
                {
                    returnStr = paramId + "=" + paramValue;
                }
            }
            return returnStr;
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