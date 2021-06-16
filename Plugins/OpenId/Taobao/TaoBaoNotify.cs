using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Xml;
using System.Web;
using System.Security.Cryptography;

namespace Hishop.Plugins.OpenId.Taobao
{
    public class TaoBaoNotify : OpenIdNotify
    {
        private readonly string NickName = "NickName";
        private readonly string TokenKey = "TokenKey";
        private const string ReUrl = "ReturnUrl";
        private readonly SortedDictionary<string, string> parameters;
        public TaoBaoNotify(NameValueCollection _parameters)
        {
            parameters = new SortedDictionary<string, string>();
            string[] requestItem = _parameters.AllKeys;

            for (int i = 0; i < requestItem.Length; i++)
            {
                parameters.Add(requestItem[i], _parameters[requestItem[i]]);
            }
            parameters.Remove("HIGW");
            parameters.Remove("HITO");
        }

        public string Base64ToString(string str)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(str));
        }
        /// <summary>
        /// 解析回调地址中top_parameters中的值
        /// </summary>
        /// <param name="key">取值关键词</param>
        /// <returns></returns>
        public string GetParameters(string parameters, string key)
        {
            string ret = string.Empty;
            try
            {
                string str = Base64ToString(parameters);
                string[] param = str.Split('&');
                for (int i = 0; i < param.Length; i++)
                {
                    string[] info = param[i].Split('=');
                    if (info[0].ToLower() == key.ToLower())
                    {
                        ret = info[1];
                        break;
                    }
                }
            }
            catch
            {
                //
            }
            return ret;
        }


        public override void Verify(int timeout, string configXml)
        {
            bool isValid = false;
            string msg = "验证失败";
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(configXml);
            string openId = string.Empty;


            //try
            //{
            //    if (parameters["top_parameters"] != null && parameters["top_sign"] != null)
            //    {
            //        byte[] bytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(parameters["top_parameters"] + doc.FirstChild.SelectSingleNode("AppSecret").InnerText));
            //        string sign = System.Convert.ToBase64String(bytes);
            //        if(sign==parameters["top_sign"])
            //        {
            //        string nick = GetParameters(parameters["top_parameters"].ToString(), "nick");
            //        if (!string.IsNullOrEmpty(nick))
            //        {                   
            //            openId = nick;
            //            isValid = true;
            //        }
            //        }
            //        else
            //        {
            //            isValid=false;
            //            msg="签名不正确";
            //        }
            //    }
            //}

            //catch
            //{
            //    isValid = false;
            //}

            //验证
            if (parameters["code"] != null)
            {
                string verifier = parameters["code"];
                string key = doc.FirstChild.SelectSingleNode("AppKey").InnerText;
                string secret = doc.FirstChild.SelectSingleNode("AppSecret").InnerText;
                string token = "";
                try
                {
                    HttpCookie requrl = HttpContext.Current.Request.Cookies[ReUrl];
                    if (requrl == null)
                        requrl = new HttpCookie(ReUrl);
                    string redirect_uri = requrl.Value;

                    IDictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("grant_type", "authorization_code");
                    param.Add("code", verifier);
                    param.Add("redirect_uri", redirect_uri);
                    param.Add("client_id", key);
                    param.Add("client_secret", secret);
                    WebUtils util = new WebUtils();
                    token = util.DoPost("https://oauth.taobao.com/token", param);
                      
                    if (!string.IsNullOrEmpty(token))
                    {
                        LitJson.JsonData jd = LitJson.JsonMapper.ToObject(token);
                        if (jd["access_token"] != null)
                        {
                            HttpCookie cookieKey = HttpContext.Current.Request.Cookies[TokenKey];
                            if (cookieKey == null)
                                cookieKey = new HttpCookie(TokenKey);
                            cookieKey.Value = (string)jd["access_token"];
                            cookieKey.Expires = DateTime.Now.AddHours(2);//过期时间是2小时
                            HttpContext.Current.Response.Cookies.Add(cookieKey);

                            if (jd["taobao_user_nick"] != null)
                            {
                                string taobao_name = System.Web.HttpContext.Current.Server.UrlDecode((string)jd["taobao_user_nick"]).Substring(0, 1);
                                HttpCookie name = HttpContext.Current.Request.Cookies[NickName];
                                if (name == null)
                                    name = new HttpCookie(NickName);
                                //name.Value = System.Web.HttpContext.Current.Server.UrlEncode((string)jd["taobao_user_nick"]);
                                name.Value = System.Web.HttpContext.Current.Server.UrlEncode(GenerateRndString(8, taobao_name));
                                name.Expires = DateTime.Now.AddHours(2);//过期时间是2小时

                                HttpContext.Current.Response.Cookies.Add(name);
                                if (jd["taobao_open_uid"] != null)
                                {
                                    openId = (string)jd["taobao_open_uid"];
                                }
                                //openId = (string)jd["taobao_user_nick"];
                            }
                            isValid = true;

                        }
                        else
                        {
                            isValid = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Response.Write(ex.Message);
                    Dictionary<string, string> exParam = new Dictionary<string, string>();
                    exParam.Add("grant_type", "authorization_code");
                    exParam.Add("code", verifier);
                    exParam.Add("client_id", key);
                    exParam.Add("client_secret", secret);
                    exParam.Add("ErrorMessage", ex.Message);
                    exParam.Add("StackTrace", ex.StackTrace);
                    exParam.Add("token", token);
                    if (ex.InnerException != null)
                    {
                        exParam.Add("InnerException", ex.InnerException.ToString());
                    }
                    if (ex.GetBaseException() != null)
                        exParam.Add("BaseException", ex.GetBaseException().Message);
                    if (ex.TargetSite != null)
                        exParam.Add("TargetSite", ex.TargetSite.ToString());
                    exParam.Add("ExSource", ex.Source);
                    OpenIdLog.AppendLog(exParam, "", "", "淘宝信任登录错误", LogType.Taobao);

                    isValid = false;
                }

            }

            if (isValid)
                OnAuthenticated(openId);
            else
                OnFailed(msg);
        }

        private string GenerateRndString(int length, string prefix)
        {
            int rand;
            char code;
            string randomcode = String.Empty;

            //生成一定长度的验证码
            Random random = new Random();
            while (randomcode.Length < 10)
            {
                rand = random.Next();

                if (rand % 3 == 0)
                    code = (char)('a' + (char)(rand % 26));
                else
                    code = (char)('0' + (char)(rand % 10));

                randomcode += code.ToString();
            }
            return prefix + randomcode;
        }
    }
}
