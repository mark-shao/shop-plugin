using System;
using Hishop.Plugins;
using System.Globalization;
using System.Web;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using System.Collections.Generic;

namespace Hishop.Plugins.Refund.AlipayDirect
{
    [Plugin("支付宝即时到帐退款", Sequence = 1)]
    public class DirectRequest : Hishop.Plugins.RefundRequest
    {
        private string out_trade_no = "";
        private string trade_no = "", refund_reason, out_request_no;
        private decimal refund_amount;
        public DirectRequest(
            string[] orderId, string refundOrderId, decimal[] amount, decimal[] refundAmount, string[] body, string sellerEmail, DateTime date, string returnUrl, string notifyUrl, string attach)
        {
            this.body = body;

            // outTradeNo = orderId[0];
            totalFee = amount;
            this.refund_amount = refundAmount[0];
            this.trade_no = orderId[0];
            this.refund_reason = body[0];
            this.out_request_no = refundOrderId;
            this.returnUrl = returnUrl;
            this.notifyUrl = notifyUrl;
            //this.refundFee = refundAmount;
            //this.refundOrderId = refundOrderId;
            //this.body = body;
            //// this.SellerEmail = sellerEmail;
            //refund_date = date.ToString("yyyy-MM-dd hh:mm:ss");
            //this.refundOrderId = refundOrderId;
            //batch_no = refundOrderId;
            //batch_num = "1";
            //int x = orderId.Length, y = refundAmount.Length, z = body.Length;
            ////取三个数组中长度最小的数组长度
            //if (x != y && y != z)
            //{
            //    x = x < y ? x : y;
            //    x = x < z ? x : z;
            //}
            //batch_num = x.ToString();
            //for (int i = 0; i < x; i++)
            //{
            //    detail_data += (i == 0 ? "" : "#");
            //    if (!string.IsNullOrEmpty(body[i]))
            //        detail_data = orderId[i] + "^" + refundFee[i].ToString("f2") + "^" + body[i];
            //    else
            //        detail_data = orderId[i] + "^" + refundFee[i].ToString("f2") + "^" + "用户退款";
            //}
        }

        public DirectRequest()
        {
        }

        //#region 常量
        //private const string Gateway = "https://mapi.alipay.com/gateway.do?";	//'退款接口
        //private const string Service = "refund_fastpay_by_platform_pwd";
        //private const string SignType = "MD5";
        //private const string InputCharset = "utf-8";
        //private readonly string batch_no;
        //#endregion

        protected override bool NeedProtect
        {
            get { return true; }
        }
        [ConfigElement("商户公钥", Nullable = false)]
        public string PublicKey { get; set; } //卖家email

        [ConfigElement("商户号/AppId", Nullable = false)]
        public string Partner { get; set; }

        private readonly string[] body;		//body			退款说明
        private readonly decimal[] totalFee;                      //总金额					0.01～50000.00
        //private readonly decimal[] refundFee;
        //private readonly string refund_date;
        //private readonly string batch_num = "1";
        //private readonly string detail_data = "";

        [ConfigElement("商户私钥", Nullable = false)]
        public string Key { get; set; }              //账户的支付宝安全校验码

        private readonly string returnUrl;
        private readonly string notifyUrl;
        private readonly string outTradeNo;
        private readonly string refundOrderId;
        public override void SendRequest()
        {
            //HttpContext.Current.Response.Write(
            //    Globals.CreatRefundUrl(Gateway, Service, Partner, SignType, SellerEmail, Key, returnUrl,
            //                           InputCharset, notifyUrl, refund_date, batch_no, batch_num, detail_data
            //        ));
            //HttpContext.Current.Response.End();
        }

        public override ResponseResult SendRequest_Ret()
        {
            //IDictionary<string, string> param = new Dictionary<string, string>();
            //param.Add("Req_url", config.gatewayUrl);
            //param.Add("Partner", Partner);
            //param.Add("Key", Key);
            //param.Add("Format", "json");
            //param.Add("V", "1.0");
            //param.Add("SignType", config.sign_type);
            //param.Add("PublicKey", PublicKey);
            //param.Add("Out_trade_no", outTradeNo);
            //param.Add("TradeNo", trade_no);
            //param.Add("RefundAmount", refund_amount.ToString("f2"));
            //param.Add("RefundReason", refund_reason);
            //param.Add("OutRequestNo", out_request_no);

            //RefundLog.writeLog(param, "", "", "", LogType.WS_WapPay);
            ResponseResult result = new ResponseResult();
            DefaultAopClient client = new DefaultAopClient(config.gatewayUrl, Partner, Key, "json", "1.0", config.sign_type, PublicKey, config.charset, false);

            //// 商户订单号，和交易号不能同时为空
            //string out_trade_no = WIDout_trade_no.Text.Trim();

            //// 支付宝交易号，和商户订单号不能同时为空
            //string trade_no = WIDtrade_no.Text.Trim();

            //// 退款金额，不能大于订单总金额
            //string refund_amount = WIDrefund_amount.Text.Trim();

            //// 退款原因
            //string refund_reason = WIDrefund_reason.Text.Trim();

            //// 退款单号，同一笔多次退款需要保证唯一，部分退款该参数必填。
            //string out_request_no = WIDout_request_no.Text.Trim();

            AlipayTradeRefundModel model = new AlipayTradeRefundModel();
            model.OutTradeNo = outTradeNo;
            model.TradeNo = trade_no;
            model.RefundAmount = refund_amount.ToString("f2");
            model.RefundReason = refund_reason;
            model.OutRequestNo = out_request_no;
            AlipayTradeRefundRequest request = new AlipayTradeRefundRequest();
            request.SetBizModel(model);

            AlipayTradeRefundResponse response = null;
            try
            {
                response = client.Execute(request);
                result.Status = response.Code == "10000" ? ResponseStatus.Success : ResponseStatus.Failed;
                result.Code = response.Code;
                result.Msg = response.Msg;
                result.SubCode = response.SubCode;
                result.SubMsg = response.SubMsg;
                result.TradeNo = response.TradeNo;
                decimal RefundCharge = 0;
                decimal.TryParse(response.FundChange, out RefundCharge);
                result.RefundCharge = RefundCharge;
                result.OutTradeNo = response.OutTradeNo;
                result.OriginalResult = JsonHelper.GetJson<AlipayTradeRefundResponse>(response);
                // WIDresule.Text = response.Body;

            }
            catch (Exception exp)
            {
                result.Status = ResponseStatus.Error;
                result.Msg = exp.Message;
                result.Code = exp.StackTrace;
                result.SubCode = exp.Source;
                result.SubMsg = exp.InnerException == null ? "" : exp.InnerException.ToString();
            }
            return result;
        }

        public override bool IsMedTrade
        {
            get { return false; }
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
