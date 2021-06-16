using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.pass.tpl.content.update
    /// </summary>
    public class AlipayPassTplContentUpdateRequest : IAopRequest<AlipayPassTplContentUpdateResponse>
    {
        /// <summary>
        /// 支付宝pass唯一标识
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// 模版动态参数信息【支付宝pass模版参数键值对JSON字符串】
        /// </summary>
        public string TplParams { get; set; }

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
            return "alipay.pass.tpl.content.update";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("serial_number", this.SerialNumber);
            parameters.Add("tpl_params", this.TplParams);
            return parameters;
        }

        #endregion
    }
}
