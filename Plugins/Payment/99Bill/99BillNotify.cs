using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Xml;
using Hishop.Plugins;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace Hishop.Plugins.Payment.Bill99
{
    public class Bill99Notify : PaymentNotify
    {
        private readonly NameValueCollection parameters;

        public Bill99Notify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            string merchantAcctId = parameters["merchantAcctId"];
            string version = parameters["version"];
            string language = parameters["language"];
            string signType = parameters["signType"];
            string payType = parameters["payType"];
            string bankId = parameters["bankId"];
            string orderId = parameters["orderId"];
            string orderTime = parameters["orderTime"];
            string orderAmount = parameters["orderAmount"];  //获取原始订单金额
            string dealId = parameters["dealId"];  //获取快钱交易号,获取该交易在快钱的交易号
            string bankDealId = parameters["bankDealId"];  //获取银行交易号,如果使用银行卡支付时，在银行的交易号。如不是通过银行支付，则为空
            string dealTime = parameters["dealTime"];  //获取在快钱交易时间,14位数字。年[4位]月[2位]日[2位]时[2位]分[2位]秒[2位],如；20080101010101
            string payAmount = parameters["payAmount"]; //获取实际支付金额,单位为分,比方 2 ，代表0.02元
            string fee = parameters["fee"]; //获取交易手续费,单位为分,比方 2 ，代表0.02元
            string ext1 = parameters["ext1"];
            string ext2 = parameters["ext2"];

            //获取处理结果
            ///10代表 成功; 11代表 失败
            ///00代表 下订单成功（仅对电话银行支付订单返回）;01代表 下订单失败（仅对电话银行支付订单返回）
            string payResult = parameters["payResult"];
            ///获取错误代码
            ///详细见文档错误代码列表
            string errCode = parameters["errCode"];
            //获取加密签名串
            string signMsg = parameters["signMsg"];

            // 状态检查
            if (!payResult.Equals("10"))
            {
                OnNotifyVerifyFaild();
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);

            //生成加密串。必须保持如下顺序。
            String merchantSignMsgVal = "";
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "merchantAcctId", merchantAcctId);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "version", version);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "language", language);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "signType", signType);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "payType", payType);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "bankId", bankId);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "orderId", orderId);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "orderTime", orderTime);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "orderAmount", orderAmount);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "dealId", dealId);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "bankDealId", bankDealId);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "dealTime", dealTime);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "payAmount", payAmount);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "fee", fee);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "ext1", ext1);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "ext2", ext2);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "payResult", payResult);
            merchantSignMsgVal = AppendParam(merchantSignMsgVal, "errCode", errCode);
         //   merchantSignMsgVal = AppendParam(merchantSignMsgVal, "key", doc.FirstChild.SelectSingleNode("Key").InnerText);
         //   string mac = FormsAuthentication.HashPasswordForStoringInConfigFile(merchantSignMsgVal, "MD5").ToUpper();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(merchantSignMsgVal);
            byte[] SignatureByte = Convert.FromBase64String(signMsg);
            X509Certificate2 cert = new X509Certificate2(HttpContext.Current.Server.MapPath("~/plugins/payment/Cert/99bill.cer"), "");
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PublicKey.Key;
            rsapri.ImportCspBlob(rsapri.ExportCspBlob(false));
            RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsapri);
            byte[] result;
            f.SetHashAlgorithm("SHA1");
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            result = sha.ComputeHash(bytes);

            // 签名检查
            if (!f.VerifySignature(result, SignatureByte))
            {

                OnNotifyVerifyFaild();
                return;
            }

            OnFinished(false);
        }

        private static String AppendParam(String returnStr, String paramId, String paramValue)
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

        public override void WriteBack(HttpContext context, bool success)
        {
            if (context == null)
                return;

            int index = ReturnUrl.IndexOf("?");
            if (index > 0)
            {
                ReturnUrl = ReturnUrl.Substring(0, index);
            }

            context.Response.Clear();
            context.Response.Write(success
                                       ? string.Format("<result>1</result><redirecturl>{0}</redirecturl>", ReturnUrl)
                                       : string.Format("<result>0</result><redirecturl>{0}</redirecturl>", ReturnUrl));
            context.Response.End();
        }

        public override decimal GetOrderAmount()
        {
            return decimal.Parse(parameters["payAmount"]) / 100;
        }

        public override string GetOrderId()
        {
            return parameters["orderId"];
        }

        public override string GetGatewayOrderId()
        {
            return parameters["dealId"];
        }

    }
}