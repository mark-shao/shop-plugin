

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using HiShop.API.Setting.Entities;
using HiShop.API.Setting.Exceptions;

namespace HiShop.API.Setting.HttpUtility
{
    public static class Post
    {
        #region 同步方法

        /// <summary>
        /// 获取Post结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="returnText"></param>
        /// <returns></returns>
        public static T GetResult<T>(string returnText)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            //var errorJson=js.Deserialize<HiShopJsonResult>(returnText);
            //if (errorJson != null && !string.IsNullOrEmpty(errorJson.error.message))
            //{

            //}
            //if (returnText.Contains("errcode"))
            //{
            //    //可能发生错误
            //    WxJsonResult errorResult = js.Deserialize<WxJsonResult>(returnText);
            //    if (errorResult.errcode != ReturnCode.请求成功)
            //    {
            //        //发生错误
            //        throw new ErrorJsonResultException(
            //            string.Format("Post请求发生错误！错误代码：{0}，说明：{1}",
            //                          (int)errorResult.errcode,
            //                          errorResult.errmsg),
            //            null, errorResult);
            //    }
            //}

            T result = js.Deserialize<T>(returnText);
            return result;
        }

        /// <summary>
        /// 发起Post请求
        /// </summary>
        /// <typeparam name="T">返回数据类型（Json对应的实体）</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="cookieContainer">CookieContainer，如果不需要则设为null</param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <returns></returns>
        public static T PostFileGetJson<T>(string url, CookieContainer cookieContainer = null, Dictionary<string, string> fileDictionary = null, Dictionary<string, string> postDataDictionary = null, Encoding encoding = null, int timeOut = Config.TIME_OUT, string appId = "", string appSecret = "", string accessToken = "", string method="POST")
        {
            using (MemoryStream ms = new MemoryStream())
            {
                postDataDictionary.FillFormDataStream(ms); //填充formData
                string returnText = HttpUtility.RequestUtility.HttpPost(url, cookieContainer, ms, fileDictionary, null, encoding, timeOut, appId, appSecret, accessToken, method);
                var result = GetResult<T>(returnText);
                return result;
            }

        }

        ///// <summary>
        ///// 发起Post请求
        ///// </summary>
        ///// <typeparam name="T">返回数据类型（Json对应的实体）</typeparam>
        ///// <param name="url">请求Url</param>
        ///// <param name="cookieContainer">CookieContainer，如果不需要则设为null</param>
        ///// <param name="fileStream">文件流</param>
        ///// <param name="timeOut">代理请求超时时间（毫秒）</param>
        ///// <param name="checkValidationResult">验证服务器证书回调自动验证</param>
        ///// <returns></returns>
        //public static T PostGetJson<T>(string url, CookieContainer cookieContainer = null, Stream fileStream = null, Encoding encoding = null, int timeOut = Config.TIME_OUT, bool checkValidationResult = false)
        //{
        //    string returnText = HttpUtility.RequestUtility.HttpPost(url, cookieContainer, fileStream, null, null, encoding, timeOut: timeOut, checkValidationResult: checkValidationResult);
        //    var result = GetResult<T>(returnText);
        //    return result;
        //}

        public static T PostGetJson<T>(string url, CookieContainer cookieContainer = null, Dictionary<string, string> formData = null, Encoding encoding = null, string appId = "", string appSecret = "", int timeOut = Config.TIME_OUT)
        {
            string returnText = HttpUtility.RequestUtility.HttpPost(url, cookieContainer, formData, encoding,timeOut,appId,appSecret);
            var result = GetResult<T>(returnText);
            return result;
        }

        /// <summary>
        /// 使用Post方法上传数据并下载文件或结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="stream"></param>
        public static void Download(string url, string data, Stream stream)
        {
            WebClient wc = new WebClient();
            var file = wc.UploadData(url, "POST", Encoding.UTF8.GetBytes(string.IsNullOrEmpty(data) ? "" : data));
            foreach (var b in file)
            {
                stream.WriteByte(b);
            }
        }

        #endregion

        
    }
}
