using System;
using System.Collections.Generic;
using System.Web;
using Hishop.Weixin.Pay.Domain;
using Hishop.Weixin.Pay.Lib;

namespace Hishop.Weixin.Pay
{
    /// <summary>
    /// 退款请求
    /// </summary>
    public class Refund
    {
        /// <summary>
        /// 申请退款完整业务流程逻辑
        /// transaction_id 微信订单号（优先使用）
        /// out_trade_no 商户订单号
        /// total_fee 订单总金额
        /// refund_fee 退款金额
        /// </summary>
        /// <param name="info">退款信息</param>
        /// <param name="config">帐号配置</param>
        /// <returns></returns>
        public static string SendRequest(RefundInfo info, PayConfig config)
        {

            WxPayData data = new WxPayData();
            if (!string.IsNullOrEmpty(info.transaction_id))//微信订单号存在的条件下，则已微信订单号为准
            {
                data.SetValue("transaction_id", info.transaction_id);
            }
            else//微信订单号不存在，才根据商户订单号去退款
            {
                data.SetValue("out_trade_no", info.out_trade_no);
            }

            data.SetValue("total_fee", (int)info.TotalFee);//订单总金额
            data.SetValue("refund_fee", (int)info.RefundFee);//退款金额
            data.SetValue("out_refund_no", info.out_refund_no);//随机生成商户退款单号
            data.SetValue("op_user_id", config.MchID);//操作员，默认为商户号
            data.SetValue("sub_appid", config.sub_appid);
            data.SetValue("sub_mch_id", config.sub_mch_id);
            data.SetValue("refund_account", "REFUND_SOURCE_RECHARGE_FUNDS");
            WxPayData result = WxPayApi.Refund(data, config);//提交退款申请给API，接收返回数据
            SortedDictionary<string, object> values = result.GetValues();
            WxPayLog.AppendLog(result.GetParam(), "", "", "", LogType.Refund);
            if (values["return_code"].ToString() == "SUCCESS" && values["result_code"].ToString() == "SUCCESS")
            {
                return "SUCCESS";
            }
            else
            {
                if (values["return_code"].ToString() == "SUCCESS")
                {
                    return values["err_code_des"].ToString();
                }
                else
                {
                    return values["return_msg"].ToString();
                }
            }
        }
    }
}