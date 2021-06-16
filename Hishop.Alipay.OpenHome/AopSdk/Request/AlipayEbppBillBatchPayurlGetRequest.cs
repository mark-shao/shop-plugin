using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.ebpp.bill.batch.payurl.get
    /// </summary>
    public class AlipayEbppBillBatchPayurlGetRequest : IAopRequest<AlipayEbppBillBatchPayurlGetResponse>
    {
        /// <summary>
        /// 回调系统url
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// alipayOrderNo-merchantOrderNo即业务流水号和业务订单号
        /// </summary>
        public List<String> PayBillList { get; set; }

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
            return "alipay.ebpp.bill.batch.payurl.get";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("callback_url", this.CallbackUrl);
            parameters.Add("order_type", this.OrderType);
            parameters.Add("pay_bill_list", this.PayBillList);
            return parameters;
        }

        #endregion
    }
}
