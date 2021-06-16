using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;
using Hishop.Plugins;

namespace Hishop.Plugins.Payment.Paypal
{
    public class PaypalNotify : PaymentNotify
    {
        private readonly NameValueCollection parameters;
        public PaypalNotify(NameValueCollection parameters)
        {
            this.parameters = parameters;
        }

        public override void VerifyNotify(int timeout, string configXml)
        {
            // IPN
            //创建回复的request

            HttpWebRequest req = (HttpWebRequest)
            WebRequest.Create("https://www.paypal.com/cgi-bin/webscr");

            //设置request的属性
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] param = HttpContext.Current.Request.BinaryRead(HttpContext.Current.Request.ContentLength);
            string strFormValues = Encoding.ASCII.GetString(param);
            
            //建议在此将接受到的信息记录到日志文件中以确认是否收到IPN信息
            string strNewValue = strFormValues + "&cmd=_notify-validate";
            req.ContentLength = strNewValue.Length;

            //发送request
            StreamWriter stOut = new StreamWriter(req.GetRequestStream(),
            System.Text.Encoding.ASCII);
            stOut.Write(strNewValue);
            stOut.Close();
            
            //回复IPN并接受反馈信息
            StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = stIn.ReadToEnd();
            stIn.Close();

            string payment_status = parameters["payment_status"];
            
            //确认IPN是否合法
            if (strResponse == "VERIFIED" && payment_status.Equals("Completed"))
            {
                OnFinished(false);
                return;
            }
            else
                OnNotifyVerifyFaild();

            // PDT
            //string txToken = this.parameters["tx"];

            //string str = "cmd=_notify-synch&tx=" + txToken + "&at=" + this.seller.PrimaryKey;
            //string result;

            //try
            //{
            //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.paypal.com/cgi-bin/webscr");
            //    req.Method = "POST";
            //    req.Timeout = timeout;

            //    byte[] buffer = Encoding.ASCII.GetBytes(str);
            //    req.ContentLength = buffer.Length;
            //    req.ContentType = "application/x-www-form-urlencoded";

            //    Stream stream = req.GetRequestStream();
            //    stream.Write(buffer, 0, buffer.Length);
            //    stream.Close();

            //    // 回发
            //    HttpWebResponse rep = (HttpWebResponse)req.GetResponse();
            //    StreamReader reader = new StreamReader(rep.GetResponseStream(), System.Text.Encoding.ASCII);

            //    // 获取返回数据
            //    result = reader.ReadToEnd();
            //    reader.Close();
            //}
            //catch {
            //    OnNotifyVerifyFaild(new NotifyEventArgs(this.parameters, "PayPalStandard", null, null, null));
            //    return;
            //}

            //string[] parameterList = result.Split(new char[] { '\n' });

            //string amount = string.Empty;
            //string orderId = string.Empty;
            //string status = string.Empty;

            //try
            //{
            //    for (int i = 0; i < parameterList.Length; i++)
            //    {
            //        string[] parameter = parameterList[i].Split(new char[] { '=' });

            //        if (parameter[0] == "mc_gross")
            //            amount = parameter[1];
            //        else if (parameter[0] == "invoice")
            //            orderId = parameter[1];
            //        else if (parameter[0] == "payment_status")
            //            status = parameter[1];

            //        if (!string.IsNullOrEmpty(amount) && !string.IsNullOrEmpty(orderId) && !string.IsNullOrEmpty(status))
            //            break;
            //    }
            //}
            //catch {
            //    OnNotifyVerifyFaild(new NotifyEventArgs(this.parameters, "PayPalStandard", null, null, null));
            //    return;
            //}

            //NotifyEventArgs args = new NotifyEventArgs(this.parameters, "PayPalStandard", amount, orderId, txToken);

            //if (
            //    !parameterList[0].Equals("SUCCESS") ||
            //    !status.Equals("Completed")
            //    )
            //{
            //    OnNotifyVerifyFaild(args);
            //    return;
            //}

            //OnPaidToMerchant(args);
        }

        public override void WriteBack(HttpContext context, bool success)
        {
        }

        public override decimal GetOrderAmount()
        {
            return decimal.Parse(parameters["mc_gross"]);
        }

        public override string GetOrderId()
        {
            return parameters["item_number"];
        }

        public override string GetGatewayOrderId()
        {
            return string.Empty;
        }

    }
}