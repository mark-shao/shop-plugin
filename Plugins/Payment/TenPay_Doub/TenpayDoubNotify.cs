using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Web;

namespace Hishop.Plugins.Payment.TenPay_Doub
{
    public class TenpayDoubNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        private const string InputCharset = "utf-8";
        public TenpayDoubNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);
            string key = doc.FirstChild.SelectSingleNode("Key").InnerText;
            string partner = doc.FirstChild.SelectSingleNode("Partner").InnerText;
            ReturnHelper returnhelper = new ReturnHelper(parameters);
            returnhelper.Key = key;
            if (returnhelper.isTenpaySign())
            {
                string notify_id = returnhelper.getParameter("notify_id");
                VeriflyHelper verifly = new VeriflyHelper();
                verifly.Key = key;
                verifly.setParameter("partner", partner);
                verifly.setParameter("notify_id", notify_id);
                string verifynotifly = "https://gw.tenpay.com/gateway/simpleverifynotifyid.xml";
                string verifyresult = "";
                if (Globals.DoPost(verifynotifly + verifly.getRequestURL(), "post", out verifyresult))
                {
                 
                    ClientVerifly client = new ClientVerifly();
                    client.setContent(verifyresult);
                    client.key = key;
                
                    if (client.isTenpaySign() && client.getParameter("retcode") == "0")
                    {
                        string trade_mode = returnhelper.getParameter("trade_mode");
                        string trade_state = returnhelper.getParameter("trade_state");
                        switch (trade_mode)
                        {
                            //即时到帐
                            case "1":

                                if (trade_state == "0")
                                {

                                    OnFinished(false);
                                }

                                else
                                {
                                    OnNotifyVerifyFaild();
                                }
                                break;

                            //担保交易
                            case "2":
                                if (trade_state == "0")
                                {
                                    OnPayment();
                                    ////0付款成功1交易创建2收获地址填写完毕4卖家发货成功5买家收货确认，交易成功
                                    ///6交易关闭，未完成超时关闭7修改交易价格成功8买家发起退款9退款成功10退款关闭
                                }
                                if (trade_state == "5")
                                {
                                    OnFinished(true);
                                }
                                break;

                            //// 4	卖家发货成功
                            //case "4":
                            //    OnSendGoods();
                            //    break;

                            //// 6	交易关闭，未完成超时关闭
                            //case "6":
                            //    OnClosed();
                            //    break;
                        }
                    }
                }
                else
                {
                  
                    OnNotifyVerifyFaild();
                    return;
                }
             
            }
        }


        public override string GetGatewayOrderId()
        {
            return parameters["transaction_id"];
        }

        public override decimal GetOrderAmount()
        {
            if (!string.IsNullOrEmpty(parameters["total_fee"]))
            {
                return decimal.Parse(parameters["total_fee"], CultureInfo.InvariantCulture) / 100;
            }
            return 0.00M;
        }

        public override string GetOrderId()
        {
            return parameters["out_trade_no"];
        }

        public override void WriteBack(System.Web.HttpContext context, bool success)
        {
            if (context == null)
                return;
            context.Response.Clear();
            context.Response.Write(success ? "success" : "fail");
            context.Response.End();
        }
    }
}
