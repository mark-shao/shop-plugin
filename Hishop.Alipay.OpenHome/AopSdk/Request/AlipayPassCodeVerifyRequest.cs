using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.pass.code.verify
    /// </summary>
    public class AlipayPassCodeVerifyRequest : IAopRequest<AlipayPassCodeVerifyResponse>
    {
        /// <summary>
        /// 商户核销操作扩展信息
        /// </summary>
        public string ExtInfo { get; set; }

        /// <summary>
        /// 操作员id  如果operator_type为1，则此id代表核销人员id  如果operator_type为2，则此id代表核销机具id
        /// </summary>
        public string OperatorId { get; set; }

        /// <summary>
        /// 操作员类型  1 核销人员  2 核销机具
        /// </summary>
        public string OperatorType { get; set; }

        /// <summary>
        /// Alipass对应的核销码串
        /// </summary>
        public string VerifyCode { get; set; }

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
            return "alipay.pass.code.verify";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("ext_info", this.ExtInfo);
            parameters.Add("operator_id", this.OperatorId);
            parameters.Add("operator_type", this.OperatorType);
            parameters.Add("verify_code", this.VerifyCode);
            return parameters;
        }

        #endregion
    }
}
