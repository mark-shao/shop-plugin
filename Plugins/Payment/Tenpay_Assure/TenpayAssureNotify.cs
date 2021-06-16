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
            // �������
            string cmdno = parameters["cmdno"];
            // 0,�ɹ�������ֵ����ʶʧ��
            string retcode = parameters["retcode"];
            // ����״̬ 1���״��� 2�ջ��ַ��д��� 3��Ҹ���ɹ� 4���ҷ����ɹ� 5����ջ�ȷ�ϣ����׳ɹ� 6���׹رգ�δ��ɳ�ʱ�ر� 7�޸Ľ��׼۸�ɹ� 8��ҷ����˿� 9�˿�ɹ� 10�˿�ر�
            string status = parameters["status"];
            // �տ�Ƹ�ͨ�˺�
            string seller_id = parameters["seller"];
            // �����ܼۣ���λΪ��
            string total_fee = parameters["total_fee"];
            // ��Ʒ�ܼ۸�
            string trade_price = parameters["trade_price"];
            string transport_fee = parameters["transport_fee"];
            // ��ҲƸ�ͨ�ʺ�
            string buyer_id = parameters["buyer_id"];
            // ƽ̨�ṩ�ߵĲƸ�ͨ�˺�
            string chnid = parameters["chnid"];
            // �Ƹ�ͨ���׵���
            string cft_tid = parameters["cft_tid"];
            // �̼ҵĶ�����
            string mch_vno = parameters["mch_vno"];            
            string attach = parameters["attach"];
            string sign = parameters["sign"];

            // ����״̬
            if (!retcode.Equals("0"))
            {
                OnNotifyVerifyFaild();
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);

            // ���ݷ��ز���������֤
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

            // ǩ����֤
            if (!sign.Equals(Globals.GetMD5(buf.ToString()))) 
            {
                OnNotifyVerifyFaild();
                return;
            }

            switch (status)
            {
                // 3��Ҹ���ɹ�
                case "3":
                    OnPayment();
                    break;

                // 5����ջ�ȷ�ϣ����׳ɹ�
                case "5":
                    OnFinished(true);
                    break;

                //// 4	���ҷ����ɹ�
                //case "4":
                //    OnSendGoods();
                //    break;

                //// 6	���׹رգ�δ��ɳ�ʱ�ر�
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