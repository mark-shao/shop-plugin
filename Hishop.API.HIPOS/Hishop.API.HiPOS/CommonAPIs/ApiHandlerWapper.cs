using HiShop.API.Setting.Entities;
using HiShop.API.Setting.Utilities.HiPOSUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.API.HiPOS.CommonAPIs
{
    public static class ApiHandlerWapper
    {
        /// <summary>
        /// 异常时候最大尝试请求次数
        /// </summary>
        private readonly static int MAXGETTOKENCOUNT = 3;
        private static int getTokenCount = 0;
        /// <summary>
        /// 使用AccessToken进行操作时，如果遇到AccessToken错误的情况，重新获取AccessToken一次，并重试。
        /// 使用此方法之前必须使用AccessTokenContainer.Register(_appId, _appSecret);或JsApiTicketContainer.Register(_appId, _appSecret);方法对账号信息进行过注册，否则会出错。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fun"></param>
        /// <param name="accessTokenOrAppId">AccessToken或AppId。如果为null，则自动取已经注册的第一个appId/appSecret来信息获取AccessToken。</param>
        /// <param name="retryIfFaild">请保留默认值true，不用输入。</param>
        /// <returns></returns>
        public static T TryCommonApi<T>(Func<string, T> fun, string accessTokenOrAppId = null, bool retryIfFaild = true) where T : HiShopJsonResult
        {
            string appId = null;
            string accessToken = null;
            if (accessTokenOrAppId == null)
            {
                appId = AccessTokenContainer.GetFirstOrDefaultAppId();
                if (appId == null)
                {
                    throw new Exception("尚无已经注册的AppId，请先使用AccessTokenContainer.Register完成注册（全局执行一次即可）！");
                }
            }
            else if (ApiUtility.IsAppId(accessTokenOrAppId))
            {
                if (!AccessTokenContainer.CheckRegistered(accessTokenOrAppId))
                {
                    throw new Exception("此appId尚未注册，请先使用AccessTokenContainer.Register完成注册（全局执行一次即可）！");
                }

                appId = accessTokenOrAppId;
            }
            else
            {
                //accessToken
                accessToken = accessTokenOrAppId;
            }


            T result = null;

            try
            {
                if (accessToken == null)
                {
                    var accessTokenResult = AccessTokenContainer.GetTokenResult(appId, false);
                    accessToken = accessTokenResult.access_token;
                }
                result = fun(accessToken);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message) && getTokenCount < MAXGETTOKENCOUNT)
                {
                    getTokenCount++;
                    //尝试重新验证
                    var accessTokenResult = AccessTokenContainer.GetTokenResult(appId, true);
                    accessToken = accessTokenResult.access_token;
                    result = TryCommonApi(fun, appId, false);
                }
                else
                {
                    throw;
                }
            }

            return result;
        }

        /// <summary>
        /// 使用AccessToken进行操作时，如果遇到AccessToken错误的情况，重新获取AccessToken一次，并重试
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="fun">第一个参数为accessToken</param>
        /// <param name="retryIfFaild"></param>
        /// <returns></returns>
        [Obsolete("请使用TryCommonApi()方法")]
        public static T Do<T>(Func<string, T> fun, string appId, string appSecret, bool retryIfFaild = true)
            where T : WxJsonResult
        {
            T result = null;
            try
            {
                var accessToken = AccessTokenContainer.TryGetToken(appId, appSecret, false);
                result = fun(accessToken);
            }
            catch (Exception ex)
            {
                //尝试重新验证
                var accessToken = AccessTokenContainer.TryGetToken(appId, appSecret, true);
                result = Do(fun, appId, appSecret, false);

            }
            return result;
        }

    }
}
