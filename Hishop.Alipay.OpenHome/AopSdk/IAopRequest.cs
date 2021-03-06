using System.Collections.Generic;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP请求接口。
    /// </summary>
    public interface IAopRequest<T> where T : AopResponse
    {
        /// <summary>
        /// 获取AOP的API名称。
        /// </summary>
        /// <returns>API名称</returns>
        string GetApiName();

        /// <summary>
        /// 获取终端类型。
        /// </summary>
        /// <returns>终端类型</returns>
		string GetTerminalType();

        /// <summary>
        /// 设置终端类型。
        /// </summary>
        /// <returns>终端类型</returns>
		void SetTerminalType(string terminalType);

        /// <summary>
        /// 获取终端信息。
        /// </summary>
        /// <returns>终端信息</returns>
		string GetTerminalInfo();

        /// <summary>
        /// 设置终端信息。
        /// </summary>
        /// <returns>终端信息</returns>
		void SetTerminalInfo(string terminalInfo);

        /// <summary>
        /// 获取产品码。
        /// </summary>
        /// <returns>产品码</returns>
        string GetProdCode();

        /// <summary>
        /// 设置产品码。
        /// </summary>
        /// <returns>产品码</returns>
        void SetProdCode(string prodCode);

        /// <summary>
        /// 获取所有的Key-Value形式的文本请求参数字典。其中：
        /// Key: 请求参数名
        /// Value: 请求参数文本值
        /// </summary>
        /// <returns>文本请求参数字典</returns>
        IDictionary<string, string> GetParameters();
    }
}
