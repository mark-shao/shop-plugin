using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.trust.user.standard.verify.get
    /// </summary>
    public class AlipayTrustUserStandardVerifyGetRequest : IAopRequest<AlipayTrustUserStandardVerifyGetResponse>
    {
        /// <summary>
        /// mobile:手机号,addressProvince:省份,addressCity:城市,addressDistrict: 区,(可选)addressDetail:详细地址
        /// </summary>
        public string AliTrustUserInfo { get; set; }

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
            return "alipay.trust.user.standard.verify.get";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("ali_trust_user_info", this.AliTrustUserInfo);
            return parameters;
        }

        #endregion
    }
}
