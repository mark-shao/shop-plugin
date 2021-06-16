using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Globalization;
using System.Xml;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.TenpayAssure
{
    public class TenpayAssureNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        public TenpayAssureNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            string version = parameters["version"];
            // 任务代码
            string cmdno = parameters["cmdno"];
            // 0,成功，其它值，标识失败
            string retcode = parameters["retcode"];
            // 交易状态 1交易创建 2收获地址填写完毕 3买家付款成功 4卖家发货成功 5买家收货确认，交易成功 6交易关闭，未完成超时关闭 7修改交易价格成功 8买家发起退款 9退款成功 10退款关闭
            string status = parameters["status"];
            // 收款方财付通账号
            string seller_id = parameters["seller"];
            // 订单总价，单位为分
            string total_fee = parameters["total_fee"];
            // 商品总价格
            string trade_price = parameters["trade_price"];
            string transport_fee = parameters["transport_fee"];
            // 买家财付通帐号
            string buyer_id = parameters["buyer_id"];
            // 平台提供者的财付通账号
            string chnid = parameters["chnid"];
            // 财付通交易单号
            string cft_tid = parameters["cft_tid"];
            // 商家的定单号
            string mch_vno = parameters["mch_vno"];            
            string attach = parameters["attach"];
            string sign = parameters["sign"];

            // 交易状态
            if (!retcode.Equals("0"))
            {
                OnNotifyVerifyFaild();
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);

            // 根据返回参数生成验证
            StringBuilder buf = new StringBuilder();
            Globals.AddParameter(buf, "attach", attach);
            Globals.AddParameter(buf, "buyer_id", buyer_id);
            Globals.AddParameter(buf, "cft_tid", cft_tid);
            Globals.AddParameter(buf, "chnid", chnid);
            Globals.AddParameter(buf, "cmdno", cmdno);
            Globals.AddParameter(buf, "mch_vno", mch_vno);
            Globals.AddParameter(buf, "retcode", retcode);
            Globals.AddParameter(buf, "seller", seller_id);
            Globals.AddParameter(buf, "status", status);
            Globals.AddParameter(buf, "total_fee", total_fee);
            Globals.AddParameter(buf, "trade_price", trade_price);
            Globals.AddParameter(buf, "transport_fee", transport_fee);
            Globals.AddParameter(buf, "version", version);
            Globals.AddParameter(buf, "key", doc.FirstChild.SelectSingleNode("Key").InnerText);

            // 签名验证
            if (!sign.Equals(Globals.GetMD5(buf.ToString()))) 
            {
                OnNotifyVerifyFaild();
                return;
            }

            switch (status)
            {
                // 3买家付款成功
                case "3":
                    OnPayment();
                    break;

                // 5买家收货确认，交易成功
                case "5":
                    OnFinished(true);
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

        public override void WriteBack(HttpContext context, bool success)
        {
        }

        public override decimal GetOrderAmount()
        {
            return decimal.Parse(parameters["total_fee"], CultureInfo.InvariantCulture) / 100;
        }

        public override string GetOrderId()
        {
            return parameters["mch_vno"];
        }

        public override string GetGatewayOrderId()
        {
            return parameters["cft_tid"];
        }

    }
}