using System;
using System.IO;
using System.Web;
using System.Collections.Specialized;

//using log4net;

/// <summary>
/// 后台通知 的摘要说明
/// </summary>
/// 
namespace Com.HisunCmpay
{
    public class NotifyHttpHandler : IHttpHandler
    {
        //public static log4net.ILog log = log4net.LogManager.GetLogger(typeof(NotifyHttpHandler));
        public NotifyHttpHandler()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region IHttpHandler 成员

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 接收后台支付结果通知的处理
        /// </summary>
        /// <param name="context">HttpContext对象</param>
        public void ProcessRequest(HttpContext context)
        {
            //后台接收支付结果通知
            NameValueCollection param = context.Request.Form;
            try
            {
                HiOrderNotifyRes res = new HiOrderNotifyRes(param);
                if ("SUCCESS".Equals(res.Status))
                {
                    ///商户改自行处理代码
                    context.Response.Write("SUCCESS");
                    
                }
            }
            catch (Exception e1)
            {
                //log.Error(e1.Message);
                return;
            }
        }

        #endregion
    }
}
