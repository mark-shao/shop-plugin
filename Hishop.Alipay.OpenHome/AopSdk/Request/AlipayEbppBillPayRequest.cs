using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.ebpp.bill.pay
    /// </summary>
    public class AlipayEbppBillPayRequest : IAopRequest<AlipayEbppBillPayResponse>
    {
        /// <summary>
        /// 支付宝的业务订单号，具有唯一性。
        /// </summary>
        public string AlipayOrderNo { get; set; }

        /// <summary>
        /// openapi的spanner上增加规则转发到pcimapi集群上
        /// </summary>
        public string DispatchClusterTarget { get; set; }

        /// <summary>
        /// 扩展字段
        /// </summary>
        public string Extend { get; set; }

        /// <summary>
        /// 输出机构的业务流水号，需要保证唯一性。
        /// </summary>
        public string MerchantOrderNo { get; set; }

        /// <summary>
        /// 支付宝订单类型。公共事业缴纳JF,信用卡还款HK
        /// </summary>
        public string OrderType { get; set; }

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
            return "alipay.ebpp.bill.pay";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("alipay_order_no", this.AlipayOrderNo);
            parameters.Add("dispatch_cluster_target", this.DispatchClusterTarget);
            parameters.Add("extend", this.Extend);
            parameters.Add("merchant_order_no", this.MerchantOrderNo);
            parameters.Add("order_type", this.OrderType);
            return parameters;
        }

        #endregion
    }
}
