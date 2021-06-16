using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.user.account.search
    /// </summary>
    public class AlipayUserAccountSearchRequest : IAopRequest<AlipayUserAccountSearchResponse>
    {
        /// <summary>
        /// 查询的结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 需要过滤的字符
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// 查询的页数
        /// </summary>
        public string PageNo { get; set; }

        /// <summary>
        /// 每页的条数
        /// </summary>
        public string PageSize { get; set; }

        /// <summary>
        /// 查询的开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 查询账务的类型
        /// </summary>
        public string Type { get; set; }

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
            return "alipay.user.account.search";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("end_time", this.EndTime);
            parameters.Add("fields", this.Fields);
            parameters.Add("page_no", this.PageNo);
            parameters.Add("page_size", this.PageSize);
            parameters.Add("start_time", this.StartTime);
            parameters.Add("type", this.Type);
            return parameters;
        }

        #endregion
    }
}
