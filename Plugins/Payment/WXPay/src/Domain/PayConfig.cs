using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Weixin.Pay.Domain
{
    /// <summary>
    /// 微信相关操作的基本参数
    /// </summary>
    public class PayConfig
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public PayConfig()
        {
            IPAddress = "127.0.0.1";
            SignType = "MD5";
            REPORT_LEVENL = 0;
            LOG_LEVENL = 0;
            SSLCERT_PATH = "cert/apiclient_cert.p12";
            SSLCERT_PASSWORD = "1233410002";
        }
        public  string AppId { get; set; }
        public  string AppSecret { get; set; }
        public  string MchID { get; set; }
        public  string Key { get; set; }
        public  string OpenId { get; set; }
        public string sub_appid { get; set; }
        public string sub_mch_id { get; set; }
        /// <summary>
        /// IP地址默认为127.0.0.1
        /// </summary>
        public  string IPAddress { get; set; }

        public  string SignType { get; private set; }


        //=======【证书路径设置】===================================== 
        /* 证书路径,注意应该填写绝对路径（仅退款、撤销订单时需要）
        */
        public  string SSLCERT_PATH { get; set; }
        public  string SSLCERT_PASSWORD { get; set; }



        //=======【支付结果通知url】===================================== 
        /* 支付结果通知回调url，用于商户接收支付结果
        */
        public  string NOTIFY_URL { get; set; }

        


        //=======【代理服务器设置】===================================
        /* 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
        */
        public  string PROXY_URL = "http://10.152.18.220:8080";

        //=======【上报信息配置】===================================
        /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        */
        public  int REPORT_LEVENL { get; set; }

        //=======【日志级别】===================================
        /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
        */
        public  int LOG_LEVENL { get; set; }
    }
}
