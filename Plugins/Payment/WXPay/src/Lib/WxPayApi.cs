using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using Hishop.Weixin.Pay.Domain;

namespace Hishop.Weixin.Pay.Lib
{
    /// <summary>
    /// 微信支付API
    /// </summary>
    public class WxPayApi
    {
        /**
        * 提交被扫支付API
        * 收银员使用扫码设备读取微信用户刷卡授权码以后，二维码或条码信息传送至商户收银台，
        * 由商户收银台或者商户后台调用该接口发起支付。
        * @param WxPayData inputObj 提交给被扫支付API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回调用结果，其他抛异常
        */
        public static WxPayData Micropay(WxPayData inputObj, PayConfig config, int timeOut = 10)
        {
            string url = "https://api.mch.weixin.qq.com/pay/micropay";
            //检测必填参数
            if (!inputObj.IsSet("body"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "缺少必填参数body", LogType.MicroPay);
            }
            else if (!inputObj.IsSet("out_trade_no"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "缺少必填参数out_trade_no", LogType.MicroPay);
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "缺少必填参数total_fee", LogType.MicroPay);
            }
            else if (!inputObj.IsSet("auth_code"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "缺少必填参数auth_code", LogType.MicroPay);
            }

            inputObj.SetValue("spbill_create_ip", config.IPAddress);//终端ip
            inputObj.SetValue("appid", config.AppId);//公众账号ID
            inputObj.SetValue("mch_id", config.MchID);//商户号
            inputObj.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(config.Key));//签名
            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            string response = HttpService.Post(xml, url, false, config, timeOut);//调用HTTP通信接口以提交数据到API

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的结果转换为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response, config.Key);

            ReportCostTime(url, timeCost, result, config);//测速上报

            return result;
        }


        /**
        *    
        * 查询订单
        * @param WxPayData inputObj 提交给查询订单API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回订单查询结果，其他抛异常
        */
        public static WxPayData OrderQuery(WxPayData inputObj, PayConfig config, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/orderquery";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "缺少必填参数out_trade_no 或者 transaction_id", LogType.OrderQuery);
            }

            inputObj.SetValue("appid", config.AppId);//公众账号ID
            inputObj.SetValue("mch_id", config.MchID);//商户号
            inputObj.SetValue("nonce_str", WxPayApi.GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(config.Key));//签名

            string xml = inputObj.ToXml();

            var start = DateTime.Now;

            string response = HttpService.Post(xml, url, false, config, timeOut);//调用HTTP通信接口提交数据

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的数据转化为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response, config.Key);
            ReportCostTime(url, timeCost, result, config);//测速上报
            return result;
        }


        /**
        * 
        * 撤销订单API接口
        * @param WxPayData inputObj 提交给撤销订单API接口的参数，out_trade_no和transaction_id必填一个
        * @param int timeOut 接口超时时间
        * @throws WxPayException
        * @return 成功时返回API调用结果，其他抛异常
        */
        public static WxPayData Reverse(WxPayData inputObj, PayConfig config, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/secapi/pay/reverse";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "撤销订单API接口中缺少必填参数out_trade_no 或者 transaction_id", LogType.CloseOrder);
            }

            inputObj.SetValue("appid", config.AppId);//公众账号ID
            inputObj.SetValue("mch_id", config.MchID);//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(config.Key));//签名
            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            string response = HttpService.Post(xml, url, true, config, timeOut);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response, config.Key);

            ReportCostTime(url, timeCost, result, config);//测速上报

            return result;
        }


        /**
        * 
        * 申请退款
        * @param WxPayData inputObj 提交给申请退款API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回接口调用结果，其他抛异常
        */
        public static WxPayData Refund(WxPayData inputObj, PayConfig config, int timeOut = 60)
        {
            string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "退款订单API接口中缺少必填参数out_trade_no 或者 transaction_id", LogType.Refund);
            }
            else if (!inputObj.IsSet("out_refund_no"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "退款订单API接口中缺少必填参数out_refund_no", LogType.Refund);
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "退款订单API接口中缺少必填参数total_fee", LogType.Refund);
            }
            else if (!inputObj.IsSet("refund_fee"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "退款订单API接口中缺少必填参数refund_fee", LogType.Refund);
            }
            else if (!inputObj.IsSet("op_user_id"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "退款订单API接口中缺少必填参数op_user_id", LogType.Refund);
            }

            inputObj.SetValue("appid", config.AppId);//公众账号ID
            inputObj.SetValue("mch_id", config.MchID);//商户号
            inputObj.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(config.Key));//签名
            
            string xml = inputObj.ToXml();
            //WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), xml, LogType.Refund);
            var start = DateTime.Now;

            string response = HttpService.Post(xml, url, true, config, timeOut);//调用HTTP通信接口提交数据到API

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的结果转换为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response, config.Key);

            ReportCostTime(url, timeCost, result, config);//测速上报

            return result;
        }


        /**
	    * 
	    * 查询退款
	    * 提交退款申请后，通过该接口查询退款状态。退款有一定延时，
	    * 用零钱支付的退款20分钟内到账，银行卡支付的退款3个工作日后重新查询退款状态。
	    * out_refund_no、out_trade_no、transaction_id、refund_id四个参数必填一个
	    * @param WxPayData inputObj 提交给查询退款API的参数
	    * @param int timeOut 接口超时时间
	    * @throws WxPayException
	    * @return 成功时返回，其他抛异常
	    */
        public static WxPayData RefundQuery(WxPayData inputObj, PayConfig config, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/refundquery";
            //检测必填参数
            if (!inputObj.IsSet("out_refund_no") && !inputObj.IsSet("out_trade_no") &&
                !inputObj.IsSet("transaction_id") && !inputObj.IsSet("refund_id"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "退款查询接口中，out_refund_no、out_trade_no、transaction_id、refund_id四个参数必填一个！", LogType.RefundQuery);
            }

            inputObj.SetValue("appid", config.AppId);//公众账号ID
            inputObj.SetValue("mch_id", config.MchID);//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(config.Key));//签名

            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            string response = HttpService.Post(xml, url, false, config, timeOut);//调用HTTP通信接口以提交数据到API

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的结果转换为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response, config.Key);

            ReportCostTime(url, timeCost, result, config);//测速上报

            return result;
        }


        /**
        * 下载对账单
        * @param WxPayData inputObj 提交给下载对账单API的参数
        * @param int timeOut 接口超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
        public static WxPayData DownloadBill(WxPayData inputObj, PayConfig config, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/downloadbill";
            //检测必填参数
            if (!inputObj.IsSet("bill_date"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "对账单接口中，缺少必填参数bill_date", LogType.DownLoadBill);
            }

            inputObj.SetValue("appid", config.AppId);//公众账号ID
            inputObj.SetValue("mch_id", config.MchID);//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(config.Key));//签名

            string xml = inputObj.ToXml();

            string response = HttpService.Post(xml, url, false, config, timeOut);//调用HTTP通信接口以提交数据到API

            WxPayData result = new WxPayData();
            //若接口调用失败会返回xml格式的结果
            if (response.Substring(0, 5) == "<xml>")
            {
                result.FromXml(response, config.Key);
            }
            //接口调用成功则返回非xml格式的数据
            else
                result.SetValue("result", response);

            return result;
        }


        /**
	    * 
	    * 转换短链接
	    * 该接口主要用于扫码原生支付模式一中的二维码链接转成短链接(weixin://wxpay/s/XXXXXX)，
	    * 减小二维码数据量，提升扫描速度和精确度。
	    * @param WxPayData inputObj 提交给转换短连接API的参数
	    * @param int timeOut 接口超时时间
	    * @throws WxPayException
	    * @return 成功时返回，其他抛异常
	    */
        public static WxPayData ShortUrl(WxPayData inputObj, PayConfig config, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/tools/shorturl";
            //检测必填参数
            if (!inputObj.IsSet("long_url"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "需要转换的URL，签名用原串，传输需URL encode！", LogType.ShortUrl);
            }

            inputObj.SetValue("appid", config.AppId);//公众账号ID
            inputObj.SetValue("mch_id", config.MchID);//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串	
            inputObj.SetValue("sign", inputObj.MakeSign(config.Key));//签名
            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            string response = HttpService.Post(xml, url, false, config, timeOut);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response, config.Key);
            ReportCostTime(url, timeCost, result, config);//测速上报

            return result;
        }


        /**
        * 
        * 统一下单
        * @param WxPaydata inputObj 提交给统一下单API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
        public static WxPayData UnifiedOrder(WxPayData inputObj, PayConfig config, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "缺少统一支付接口必填参数out_trade_no！", LogType.UnifiedOrder);
            }
            else if (!inputObj.IsSet("body"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "缺少统一支付接口必填参数body！", LogType.UnifiedOrder);
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "缺少统一支付接口必填参数total_fee！", LogType.UnifiedOrder);
            }
            else if (!inputObj.IsSet("trade_type"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "缺少统一支付接口必填参数trade_type!", LogType.UnifiedOrder);
            }

            //关联参数
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！", LogType.UnifiedOrder);
            }
            if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！", LogType.UnifiedOrder);
            }
            if (!inputObj.IsSet("NOTIFY_URL"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "统一支付接口中，缺少必填参数NOTIFY_URL！", LogType.UnifiedOrder);
            }
            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("NOTIFY_URL"))
            {
                inputObj.SetValue("NOTIFY_URL", config.NOTIFY_URL);//异步通知url
            }

            inputObj.SetValue("appid", config.AppId);//公众账号ID
            inputObj.SetValue("mch_id", config.MchID);//商户号
            inputObj.SetValue("spbill_create_ip", config.IPAddress);//终端ip	  	    
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串

            //签名
            inputObj.SetValue("sign", inputObj.MakeSign(config.Key));
            string xml = inputObj.ToXml();

            var start = DateTime.Now;

            string response = HttpService.Post(xml, url, false, config, timeOut);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response, config.Key);

            ReportCostTime(url, timeCost, result, config);//测速上报

            return result;
        }


        /**
        * 
        * 关闭订单
        * @param WxPayData inputObj 提交给关闭订单API的参数
        * @param int timeOut 接口超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
        public static WxPayData CloseOrder(WxPayData inputObj, PayConfig config, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/closeorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "关闭订单接口中，out_trade_no必填！", LogType.CloseOrder);
            }

            inputObj.SetValue("appid", config.AppId);//公众账号ID
            inputObj.SetValue("mch_id", config.MchID);//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串		
            inputObj.SetValue("sign", inputObj.MakeSign(config.Key));//签名
            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            string response = HttpService.Post(xml, url, false, config, timeOut);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response, config.Key);

            ReportCostTime(url, timeCost, result, config);//测速上报

            return result;
        }


        /**
	    * 
	    * 测速上报
	    * @param string interface_url 接口URL
	    * @param int timeCost 接口耗时
	    * @param WxPayData inputObj参数数组
	    */
        private static void ReportCostTime(string interface_url, int timeCost, WxPayData inputObj, PayConfig config)
        {
            //如果不需要进行上报
            if (config.REPORT_LEVENL == 0)
            {
                return;
            }

            //如果仅失败上报
            if (config.REPORT_LEVENL == 1 && inputObj.IsSet("return_code") && inputObj.GetValue("return_code").ToString() == "SUCCESS" &&
             inputObj.IsSet("result_code") && inputObj.GetValue("result_code").ToString() == "SUCCESS")
            {
                return;
            }

            //上报逻辑
            WxPayData data = new WxPayData();
            data.SetValue("interface_url", interface_url);
            data.SetValue("execute_time_", timeCost);
            //返回状态码
            if (inputObj.IsSet("return_code"))
            {
                data.SetValue("return_code", inputObj.GetValue("return_code"));
            }
            //返回信息
            if (inputObj.IsSet("return_msg"))
            {
                data.SetValue("return_msg", inputObj.GetValue("return_msg"));
            }
            //业务结果
            if (inputObj.IsSet("result_code"))
            {
                data.SetValue("result_code", inputObj.GetValue("result_code"));
            }
            //错误代码
            if (inputObj.IsSet("err_code"))
            {
                data.SetValue("err_code", inputObj.GetValue("err_code"));
            }
            //错误代码描述
            if (inputObj.IsSet("err_code_des"))
            {
                data.SetValue("err_code_des", inputObj.GetValue("err_code_des"));
            }
            //商户订单号
            if (inputObj.IsSet("out_trade_no"))
            {
                data.SetValue("out_trade_no", inputObj.GetValue("out_trade_no"));
            }
            //设备号
            if (inputObj.IsSet("device_info"))
            {
                data.SetValue("device_info", inputObj.GetValue("device_info"));
            }

            try
            {
                Report(data, config);
            }
            catch (WxPayException ex)
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "关闭订单接口中，out_trade_no必填！", LogType.CloseOrder);
                //不做任何处理
            }
        }


        /**
	    * 
	    * 测速上报接口实现
	    * @param WxPayData inputObj 提交给测速上报接口的参数
	    * @param int timeOut 测速上报接口超时时间
	    * @throws WxPayException
	    * @return 成功时返回测速上报接口返回的结果，其他抛异常
	    */
        public static WxPayData Report(WxPayData inputObj, PayConfig config, int timeOut = 1)
        {
            string url = "https://api.mch.weixin.qq.com/payitil/report";
            //检测必填参数
            if (!inputObj.IsSet("interface_url"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "接口URL，缺少必填参数interface_url！", LogType.Report);
            }
            if (!inputObj.IsSet("return_code"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "返回状态码，缺少必填参数return_code！", LogType.Report);
            }
            if (!inputObj.IsSet("result_code"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "业务结果，缺少必填参数result_code！", LogType.Report);
            }
            if (!inputObj.IsSet("user_ip"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "访问接口IP，缺少必填参数user_ip！", LogType.Report);
            }
            if (!inputObj.IsSet("execute_time_"))
            {
                WxPayLog.AppendLog(inputObj.GetParam(), "", HttpContext.Current.Request.Url.ToString(), "接口耗时，缺少必填参数execute_time_！", LogType.Report);
            }

            inputObj.SetValue("appid", config.AppId);//公众账号ID
            inputObj.SetValue("mch_id", config.MchID);//商户号
            inputObj.SetValue("user_ip", config.IPAddress);//终端ip
            inputObj.SetValue("time", DateTime.Now.ToString("yyyyMMddHHmmss"));//商户上报时间	 
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(config.Key));//签名
            string xml = inputObj.ToXml();

            string response = HttpService.Post(xml, url, false, config, timeOut);

            WxPayData result = new WxPayData();
            result.FromXml(response, config.Key);
            return result;
        }

        /**
        * 根据当前系统时间加随机序列来生成订单号
         * @return 订单号
        */
        public static string GenerateOutTradeNo(PayConfig config)
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", config.MchID, DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }

        /**
        * 生成时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数
         * @return 时间戳
        */
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /**
        * 生成随机串，随机串包含字母或数字
        * @return 随机串
        */
        public static string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}