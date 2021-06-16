using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.Plugins.Payment.BankUnion
{
    public class QuickPayConf
    {
        //版本号
        public static string version = "1.0.0";

        //编码方式
        public static string charset = "UTF-8";

        //支付网址
        public static string gateWay = "http://172.17.136.37:8080/UpopWeb/api/Pay.action";

        //后续交易网址
        public static string backStagegateWay = "http://172.17.136.37:8080/UpopWeb/api/BSPay.action";

        //查询网址
        public static string queryUrl = "http://172.17.136.37:8080/UpopWeb/api/Query.action";

        //商户代码
        public static string merCode = "105550149170027";

        //商户名称
        public static string merName = "银联商城";

        //加密方式
        public static string signType = "MD5";

        //商城密匙，需要和银联商户网站上配置的一样
        public static string securityKey = "88888888";

        //签名
        public static string signature = "signature";
        public static string signMethod = "signMethod";

        //组装消费请求包
        public static string[] reqVo = new string[]{
				"version",
				"charset",
				"transType",
				"origQid",
				"merId",
				"merAbbr",
				"acqCode",
				"merCode",
				"commodityUrl",
				"commodityName",
				"commodityUnitPrice",
				"commodityQuantity",
				"commodityDiscount",
				"transferFee",
				"orderNumber",
				"orderAmount",
				"orderCurrency",
				"orderTime",
				"customerIp",
				"customerName",
				"defaultPayType",
				"defaultBankNumber",
				"transTimeout",
				"frontEndUrl",
				"backEndUrl",
				"merReserved"
		};

        public static string[] notifyVo = new string[]{
				"charset",
				"cupReserved",
				"exchangeDate",
				"exchangeRate",
				"merAbbr",
				"merId",
				"orderAmount",
				"orderCurrency",
				"orderNumber",
				"qid",
				"respCode",
				"respMsg",
				"respTime",
				"settleAmount",
				"settleCurrency",
				"settleDate",
				"traceNumber",
				"traceTime",
				"transType",
				"version"
		};

        public static string[] queryVo = new string[]{
			"version",
			"charset",
			"transType",
			"merId",
			"orderNumber",
			"orderTime",
			"merReserved"
		};
    }
}
