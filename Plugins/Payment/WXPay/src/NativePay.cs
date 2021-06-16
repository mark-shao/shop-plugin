using System;
using System.Collections.Generic;
using System.Web;
using Hishop.Weixin.Pay.Lib;
using Hishop.Weixin.Pay.Domain;

namespace Hishop.Weixin.Pay
{
    public class NativePay
    {
        /**
        * 生成扫描支付模式一URL
        * @param productId 商品ID
        * @return 模式一URL
        */
        public string GetPrePayUrl(string productId, PayConfig config)
        {

            WxPayData data = new WxPayData();
            data.SetValue("appid", config.AppId);//公众帐号id
            data.SetValue("mch_id", config.MchID);//商户号
            data.SetValue("time_stamp", WxPayApi.GenerateTimeStamp());//时间戳
            data.SetValue("nonce_str", WxPayApi.GenerateNonceStr());//随机字符串
            data.SetValue("product_id", productId);//商品ID
            data.SetValue("sign", data.MakeSign(config.Key));//签名
            string str = ToUrlParams(data.GetValues());//转换为URL串
            string url = "weixin://wxpay/bizpayurl?" + str;

            return url;
        }

        /**
        * 生成直接支付url，支付url有效期为2小时,模式二
        * @param productId 商品ID
        * @return 模式二URL
        */
        public string GetPayUrl(PayInfo pay, PayConfig config)
        {

            WxPayData data = new WxPayData();
            data.SetValue("body", pay.OutTradeNo);//商品描述
            data.SetValue("attach", pay.Attach);//附加数据
            data.SetValue("out_trade_no", pay.OutTradeNo);//随机字符串
            data.SetValue("total_fee", pay.TotalFee);//总金额
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            data.SetValue("time_expire", pay.TimeEnd);//交易结束时间
            data.SetValue("goods_tag", pay.GoodsTag);//商品标记
            data.SetValue("trade_type", "NATIVE");//交易类型
            data.SetValue("product_id", pay.ProductId);//商品ID

            WxPayData result = WxPayApi.UnifiedOrder(data, config);//调用统一下单接口
            string url = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接
            return url;
        }

        /**
        * 参数数组转换为url格式
        * @param map 参数名与参数值的映射表
        * @return URL字符串
        */
        private string ToUrlParams(SortedDictionary<string, object> map)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in map)
            {
                buff += pair.Key + "=" + pair.Value + "&";
            }
            buff = buff.Trim('&');
            return buff;
        }
    }
}