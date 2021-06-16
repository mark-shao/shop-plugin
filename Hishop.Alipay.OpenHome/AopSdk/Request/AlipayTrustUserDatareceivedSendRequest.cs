using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.trust.user.datareceived.send
    /// </summary>
    public class AlipayTrustUserDatareceivedSendRequest : IAopRequest<AlipayTrustUserDatareceivedSendResponse>
    {
        /// <summary>
        /// Json格式，具体内容根据不同的type_id而不同。详见芝麻信用的数据类型文档（线下提供）。
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 用以标识用户身份的字段，JSON格式，共包括5个属性。其中至少用包含name在内的两个字段来刻画该用户，并尽可能填写完整。
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// 数据类型ID，由芝麻信用针对不同商户而分配
        /// </summary>
        public string TypeId { get; set; }

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
            return "alipay.trust.user.datareceived.send";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("data", this.Data);
            parameters.Add("identity", this.Identity);
            parameters.Add("type_id", this.TypeId);
            return parameters;
        }

        #endregion
    }
}
