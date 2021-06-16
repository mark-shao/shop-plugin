using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.user.userinfo.share
    /// </summary>
    public class AlipayUserUserinfoShareRequest : IAopRequest<AlipayUserUserinfoShareResponse>
    {
        #region IAopRequest Members
		private string terminalType;
		private string terminalInfo;
        private string prodCode;

        public string AuthToken { get; set; }

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
            return "alipay.user.userinfo.share";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("auth_token", this.AuthToken);
            return parameters;
        }

        #endregion
    }
}
