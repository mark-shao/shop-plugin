using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.user.trade.search
    /// </summary>
    public class AlipayUserTradeSearchRequest : IAopRequest<AlipayUserTradeSearchResponse>
    {
        /// <summary>
        /// 支付宝订单号，为空查询所有记录
        /// </summary>
        public string AlipayOrderNo { get; set; }

        /// <summary>
        /// 结束时间。与开始时间间隔在七天之内
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 商户订单号，为空查询所有记录
        /// </summary>
        public string MerchantOrderNo { get; set; }

        /// <summary>
        /// 订单来源，为空查询所有来源。淘宝(TAOBAO)，支付宝(ALIPAY)，其它(OTHER)
        /// </summary>
        public string OrderFrom { get; set; }

        /// <summary>
        /// 订单状态，为空查询所有状态订单
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// 订单类型，为空查询所有类型订单。
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 页码。取值范围:大于零的整数; 默认值1
        /// </summary>
        public string PageNo { get; set; }

        /// <summary>
        /// 每页获取条数。最大值500。
        /// </summary>
        public string PageSize { get; set; }

        /// <summary>
        /// 开始时间，时间必须是今天范围之内。格式为yyyy-MM-dd HH:mm:ss，精确到秒(升级后的api 1.1版本)
        /// </summary>
        public string StartTime { get; set; }

        #region IAopRequest Members
		private string terminalType;
		private string terminalInfo;
        private string prodCode;

		public void SetTerminalType(String terminalType){
			this.terminalType=terminalType;
		}

    	public string GetTerminalType(){
    		return this.terminalType;
    	}

    	public void SetTerminalInfo(String terminalInfo){
    		this.terminalInfo=terminalInfo;
    	}

    	public string GetTerminalInfo(){
    		return this.terminalInfo;
    	}

        public void SetProdCode(String prodCode){
            this.prodCode=prodCode;
        }

        public string GetProdCode(){
            return this.prodCode;
        }

        public string GetApiName()
        {
            return "alipay.user.trade.search";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("alipay_order_no", this.AlipayOrderNo);
            parameters.Add("end_time", this.EndTime);
            parameters.Add("merchant_order_no", this.MerchantOrderNo);
            parameters.Add("order_from", this.OrderFrom);
            parameters.Add("order_status", this.OrderStatus);
            parameters.Add("order_type", this.OrderType);
            parameters.Add("page_no", this.PageNo);
            parameters.Add("page_size", this.PageSize);
            parameters.Add("start_time", this.StartTime);
            return parameters;
        }

        #endregion
    }
}
