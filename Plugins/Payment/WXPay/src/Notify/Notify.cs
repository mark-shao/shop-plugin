using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Hishop.Weixin.Pay.Domain;
using Hishop.Weixin.Pay.Lib;
namespace Hishop.Weixin.Pay.Notify
{
    /// <summary>
    /// 回调处理基类
    /// 主要负责接收微信支付后台发送过来的数据，对数据进行签名验证
    /// 子类在此类基础上进行派生并重写自己的回调处理过程
    /// </summary>
    public class Notify
    {
        /// <summary>
        /// 页面
        /// </summary>
        public Page page {get;set;}
        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="page"></param>
        public Notify(Page page,PayConfig config)
        {
            this.page = page;
            this.config = config;
        }

        public PayConfig config { get; set; }
        /// <summary>
        /// 接收从微信支付后台发送过来的数据并验证签名
        /// </summary>
        /// <returns>微信支付后台返回的数据</returns>
        public WxPayData GetNotifyData()
        {
            IDictionary<string, string> logDict = new Dictionary<string, string>();
            logDict.Add("AppID", config.AppId);
            logDict.Add("AppSecret", config.AppSecret);
            logDict.Add("Key", config.Key);
            logDict.Add("MchID", config.MchID);
            //接收从微信后台POST过来的数据
            System.IO.Stream s = page.Request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();

           

            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            try
            {
                data.FromXml(builder.ToString(),config.Key);
            }
            catch(WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                logDict.Add("return_code", "FAIL");
                logDict.Add("result", res.ToXml());
                WxPayLog.writeLog(logDict, "", HttpContext.Current.Request.Url.ToString(), ex.Message, LogType.Error);
                page.Response.Write(res.ToXml());
                page.Response.End();
            }
            return data;
        }

        //派生类需要重写这个方法，进行不同的回调处理
        public virtual void ProcessNotify()
        {

        }
    }
}