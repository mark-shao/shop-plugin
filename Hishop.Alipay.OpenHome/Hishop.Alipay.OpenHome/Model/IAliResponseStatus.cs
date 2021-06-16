
namespace Hishop.Alipay.OpenHome.Model
{
    public interface IAliResponseStatus
    {
        /// <summary>
        /// 响应结果码
        /// </summary>
        string Code { get; }

        /// <summary>
        /// 结果信息
        /// </summary>
        string Message { get; }


    }
}
