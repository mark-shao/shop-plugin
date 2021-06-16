using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Hishop.Plugins.Outpay.Weixin
{
    [Plugin("微信批量放款", Sequence = 1)]
    public class WeixinRequest : OutpayRequest
    {
        public WeixinRequest(string[] outpayId, decimal[] amount,
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
        }

        public WeixinRequest()
        {

        }
        public override void SendRequest()
        {

        }

        public override IList<IDictionary<string, string>> SendRequestByResult()
        {
            IList<IDictionary<string, string>> result = new List<IDictionary<string, string>>();
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
                return null;
            for (int i = 0; i < batchLen; i++)
            {
                result.Add(WeiXinPayOut(outpayId[i], amount[i], userAccount[i], realName[i], openId[i], userId[i], desc.Length > i ? desc[i] : ""));
            }

            return result;

        }

        const string serverIP = "127.0.0.1";
        private const string Gateway = "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers";	//'支付接口
        public IDictionary<string, string> WeiXinPayOut(string outpayId, decimal amount, string userAccount, string realName, string openId, int userId, string desc)
        {
            
            IDictionary<string, string> result = new Dictionary<string, string>();
            SortedDictionary<string, string> tparam = new SortedDictionary<string, string>();
            tparam.Add("mch_appid", mch_appid); //微信公司APPID
            tparam.Add("mchid", mchid); //商户号
            tparam.Add("nonce_str", GetRandomString(20));
            tparam.Add("partner_trade_no", outpayId);
            tparam.Add("openid", openId);
            tparam.Add("check_name", check_name);
            tparam.Add("amount", amount.ToString("f0"));
            tparam.Add("desc", desc);
            tparam.Add("spbill_create_ip", serverIP);
            tparam.Add("re_user_name", realName);
            tparam.Add("device_info", "");
            string pkey = Key; //"CCD6101C1D1237CBEB45592DBDB285BB";

            string sign = "";
            StringBuilder PostXml = new StringBuilder();
            PostXml.AppendLine("<xml>");
            foreach (string tkey in tparam.Keys)
            {
                if (tparam[tkey] != "")
                {
                    sign += @"&" + tkey + "=" + tparam[tkey];
                    PostXml.AppendLine(@"<" + tkey + ">" + tparam[tkey] + @"</" + tkey + ">");
                }
            }

            sign = sign.Remove(0, 1);
            sign = sign + "&key=" + pkey;
            string presign = sign;
            sign = GetMD5(sign);
            sign = sign.ToUpper();

            PostXml.AppendLine("<sign>" + sign + "</sign>");
            PostXml.AppendLine("</xml>");
           // Outpay.PayLog.AppendLog(tparam, PostXml.ToString(), sign, presign, LogType.Weixin);
            //    System.IO.File.WriteAllText(@"D:\k.txt",PostXml.ToString());

            string xmlreturn = new HttpHelp().DoPost(Gateway, PostXml.ToString(), certPassword, certPath);
            result.Add("return_code", "FAIL");
            result.Add("return_msg", "访问服务器出错了！");
            result.Add("err_code", "SERVERERR");
            result.Add("userId", userId.ToString());
            result.Add("amount", amount.ToString("f2"));
            result.Add("partner_trade_no", outpayId);
            result.Add("result_code", "SUCCESS");
            try
            {
                
                //                System.IO.File.WriteAllText(@"D:\k.txt", xmlreturn);
                XmlDocument xmld = new XmlDocument();
                xmld.XmlResolver = null;
                xmld.LoadXml(xmlreturn);
                //  PayLog.AppendLog(tparam, sign, pkey, PostXml.ToString(), LogType.Weixin);
                result["return_code"] = xmld.SelectSingleNode(@"/xml/return_code").InnerText;
                result["return_msg"] = xmld.SelectSingleNode(@"/xml/return_msg").InnerText;

                if (xmld.SelectSingleNode(@"/xml/result_code") != null)
                {
                    result["result_code"] = xmld.SelectSingleNode(@"/xml/result_code").InnerText;
                }
                //如果成功，继续读取数据
                if (result["return_code"].ToUpper() == "SUCCESS" && result["result_code"] == "SUCCESS")
                {
                    //result["mch_appid"] = xmld.SelectSingleNode(@"/xml/mch_appid").InnerText;
                    //result["mchid"] = xmld.SelectSingleNode(@"/xml/mchid").InnerText;
                    //result["device_info"] = xmld.SelectSingleNode(@"/xml/device_info").InnerText;
                    result["nonce_str"] = xmld.SelectSingleNode(@"/xml/nonce_str").InnerText;
                    result["result_code"] = xmld.SelectSingleNode(@"/xml/result_code").InnerText;
                    result["partner_trade_no"] = xmld.SelectSingleNode(@"/xml/partner_trade_no").InnerText;
                    result["payment_no"] = xmld.SelectSingleNode(@"/xml/payment_no").InnerText;
                    result["payment_time"] = xmld.SelectSingleNode(@"/xml/payment_time").InnerText;
                }
                else
                {
                    Outpay.PayLog.AppendLog(result, sign, xmlreturn, "付款失败", LogType.WeixinNotify);
                    if (result["return_code"].ToUpper() == "SUCCESS")
                    {
                        result["return_code"] = "FAIL";
                        result["err_code"] = xmld.SelectSingleNode(@"/xml/err_code_des").InnerText;
                    }
                    else
                    {
                        result["err_code"] = xmld.SelectSingleNode(@"/xml/return_msg").InnerText;
                    }
                    //NAME_MISMATCH OPENID_ERROR
                }
                //System.IO.File.WriteAllText(@"D:\k.txt", result.err_code);

            }
            catch (Exception ex)
            {
                tparam.Add("xmlreturn", xmlreturn);
                tparam.Add("certPath", HttpHelp.GetphysicsPath(certPath));
                tparam.Add("certPassword", certPassword);
                tparam.Add("ErrorMessage", ex.Message);
                tparam.Add("PostXml", PostXml.ToString());
                tparam.Add("StackTrace", ex.StackTrace);
                if (ex.InnerException != null)
                {
                    tparam.Add("InnerException", ex.InnerException.ToString());
                }
                if (ex.GetBaseException() != null)
                    tparam.Add("BaseException", ex.GetBaseException().Message);
                if (ex.TargetSite != null)
                    tparam.Add("TargetSite", ex.TargetSite.ToString());
                tparam.Add("ExSource", ex.Source);

                PayLog.AppendLog(tparam, "", "", ex.Message, LogType.Weixin);
                result["return_code"] = "FAIL";
                result["return_msg"] = ex.Message.ToString() + "---1";

            }
            //  PayLog.writeLog(result, "", "", xmlreturn, LogType.Outpay);

            return result;
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="myString"></param>
        /// <returns></returns>
        public static string GetMD5(string myString, string _input_charset = "utf-8")
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.GetEncoding(_input_charset).GetBytes(myString);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;
            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x").PadLeft(2, '0');
            }
            return byte2String;
        }


        public static char[] Chars = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'R', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        //产生随机数
        public static string GetRandomString(int length)
        {
            StringBuilder sb = new StringBuilder();
            Random rd = new Random();
            string yymmdd = DateTime.Now.ToString("yyyyMMdd");

            sb.Append(yymmdd);
            for (int j = 0; j < length; j++)
            {
                sb.Append(Chars[rd.Next(0, Chars.Length)]);
            }

            return sb.ToString();
        }


        [ConfigElement("mch_appid", Nullable = false, Description = "微信AppId")]
        public string mch_appid { get; set; } //卖家email

        [ConfigElement("mchid", Nullable = false, Description = "商户号")]
        public string mchid { get; set; }

        [ConfigElement("check_name", Nullable = false, Description = "校验用户姓名选项,不校验真实姓名(NO_CHECK)、强制校验(FORCE_CHECK)、通过实名验证则校验(OPTION_CHECK)")]
        public string check_name { get; set; }
        [ConfigElement("证书路径", Nullable = false, Description = "证书用于企业帐号支付以及退款原路返回，请使用扩展名为p12的证书文件")]

        public string certPath { get; set; }
        [ConfigElement("证书密码", Nullable = false, InputType = InputType.Password, Description = "证书密码用于企业帐号支付以及退款原路返回")]
        public string certPassword { get; set; }

        [ConfigElement("商户密钥", Nullable = false, InputType = InputType.TextBox, HiddenPart = true, Description = "KEY由商户在商户中心自己设置的32位字符串")]
        public string Key { get; set; }

        private readonly string[] outpayId;	//subject		商品名称
        private readonly decimal[] amount;		//body			商品描述
        private readonly string[] userAccount;                      //总金额					0.01～50000.00
        private readonly string[] realName;
        private readonly string[] openId;
        private readonly int[] userId;
        private readonly string[] desc;




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
