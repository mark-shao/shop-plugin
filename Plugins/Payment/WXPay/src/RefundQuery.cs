using Hishop.Weixin.Pay.Domain;
using Hishop.Weixin.Pay.Lib;
using System;
using System.Collections.Generic;
using System.Web;

namespace Hishop.Weixin.Pay
{
    public class RefundQuery
    {
        /***
        * 退款查询完整业务流程逻辑
        * @param refund_id 微信退款单号（优先使用）
        * @param out_refund_no 商户退款单号
        * @param transaction_id 微信订单号
        * @param out_trade_no 商户订单号
        * @return 退款查询结果（xml格式）
        */
        public static string SendRequest(RefundInfo info,PayConfig config)
        {

            WxPayData data = new WxPayData();
            if (!string.IsNullOrEmpty(info.RefundID))
            {
                data.SetValue("refund_id", info.RefundID);//微信退款单号，优先级最高
            }
            else if (!string.IsNullOrEmpty(info.out_refund_no))
            {
                data.SetValue("out_refund_no", info.out_refund_no);//商户退款单号，优先级第二
            }
            else if (!string.IsNullOrEmpty(info.transaction_id))
            {
                data.SetValue("transaction_id", info.transaction_id);//微信订单号，优先级第三
            }
            else
            {
                data.SetValue("out_trade_no", info.out_trade_no);//商户订单号，优先级最低
            }

            WxPayData result = WxPayApi.RefundQuery(data, config);//提交退款查询给API，接收返回数据

            return result.ToPrintStr();
        }
    }
}