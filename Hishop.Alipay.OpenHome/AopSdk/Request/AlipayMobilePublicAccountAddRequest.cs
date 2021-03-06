using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.mobile.public.account.add
    /// </summary>
    public class AlipayMobilePublicAccountAddRequest : IAopRequest<AlipayMobilePublicAccountAddResponse>
    {
        /// <summary>
        /// 协议号
        /// </summary>
        public string AgreementId { get; set; }

        /// <summary>
        /// 绑定账户
        /// </summary>
        public string BindAccountNo { get; set; }

        /// <summary>
        /// json
        /// </summary>
        public string BizContent { get; set; }

        /// <summary>
        /// 绑定账户的名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 关注者标识
        /// </summary>
        public string FromUserId { get; set; }

        /// <summary>
        /// 绑定账户的用户名
        /// </summary>
        public string RealName { get; set; }

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
            return "alipay.mobile.public.account.add";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("agreement_id", this.AgreementId);
            parameters.Add("bind_account_no", this.BindAccountNo);
            parameters.Add("biz_content", this.BizContent);
            parameters.Add("display_name", this.DisplayName);
            parameters.Add("from_user_id", this.FromUserId);
            parameters.Add("real_name", this.RealName);
            return parameters;
        }

        #endregion
    }
}
