using System;
using System.Collections.Generic;
using System.Text;
using SDK.yop.utils;
using SDK.enums;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using Newtonsoft.Json.Linq;
using SDK.common;
using System.Globalization;
using System.Collections; //使用Hashtable时，必须引入这个命名空间
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SDK.yop.client
{
    public class YopClient3
    {
        protected static Dictionary<string, List<string>> uriTemplateCache = new Dictionary<string, List<string>>();
        /// <summary>
        /// 自动补全请求
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodOrUri"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected static string richRequest(string methodOrUri, YopRequest request)
        {
            Assert.notNull(methodOrUri, "method name or rest uri");
            if (methodOrUri.StartsWith(request.getServerRoot()))
            {
                methodOrUri = methodOrUri.Substring(request.getServerRoot().Length + 1);
            }
            bool isRest = methodOrUri.StartsWith("/rest/");//检查是否以指定字符开头
            String serverUrl = request.getServerRoot();

            if (isRest)
            {
                methodOrUri = mergeTplUri(methodOrUri, request);
                serverUrl += methodOrUri;
                string version = Regex.Match(methodOrUri, "(?<=/rest/v).*?(?=/)").Value;
                if (StringUtils.isNotBlank(version))
                {
                    request.setVersion(version);
                }
            }
            else
            {
                serverUrl += "/command?" + YopConstants.METHOD + "=" + methodOrUri;

            }
            request.setMethod(methodOrUri);
            return serverUrl;
        }

        /// <summary>
        /// 模板URL自动补全参数
        /// </summary>
        /// <param name="tplUri"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected static string mergeTplUri(string tplUri, YopRequest request)
        {
            string uri = tplUri;
            if (tplUri.IndexOf("{") < 0)
            {
                return uri;
            }
            List<string> dynaParamNames = uriTemplateCache[tplUri];
            if (dynaParamNames == null)
            {
                dynaParamNames = new List<string>();

                dynaParamNames.Add(RegexUtil.GetResResult("\\{([^\\}]+)\\}", tplUri));

                uriTemplateCache.Add(tplUri, dynaParamNames);
            }

            foreach (string dynaParamName in dynaParamNames)
            {
                string value = request.removeParam(dynaParamName);
                Assert.notNull(value, dynaParamName + " must be specified");
                uri = uri.Replace("{" + dynaParamName + "}", value);
            }

            return uri;
        }
        /// <summary>
        /// 发起post请求，以YopResponse对象返回
        /// </summary>
        /// <param name="apiUri">目标地址或命名模式的method</param>
        /// <param name="request">客户端请求对象</param>
        /// <returns>响应对象</returns>
        public static YopResponse postRsa(String methodOrUri, YopRequest request)
        {
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("11:");
            string content = postRsaString(methodOrUri, request);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("content:" + content);
            //System.Diagnostics.Debug.WriteLine("请求结果：" + content + "\n");

            YopResponse response = null;
            if (request.getFormat() == FormatType.json)
            {
                response = (YopResponse)JsonConvert.DeserializeObject(content, typeof(YopResponse));
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.LoadXml(content);
                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
                JObject jo = JObject.Parse(jsonText);
                string strValue = jo["response"].ToString();
                response = (YopResponse)JsonConvert.DeserializeObject(strValue, typeof(YopResponse));
            }
            if (response.isSuccess())//请求失败不验签 yongfeng.zhang 20180410
            {
                handleRsaResult(request, response, content);
            }
            return response;
        }

        /// <summary>
        /// 发起post请求，以字符串返回
        /// </summary>
        /// <param name="apiUri">目标地址或命名模式的method</param>
        /// <param name="request">客户端请求对象</param>
        /// <returns>字符串形式的响应</returns>
        public static String postRsaString(String methodOrUri, YopRequest request)
        {
            // 创建日志对象
            StringBuilder log = new StringBuilder();
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("22:methodOrUri:" + methodOrUri);
            string serverUrl = richRequest(methodOrUri, request);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("serverUrl:"+ serverUrl);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("请求参数:" + request.toQueryString());
            log.Append("serverUrl：" + serverUrl + "\r\n");
            log.Append("请求参数：" + request.toQueryString() + "\r\n");

            //signAndEncrypt(request);
            request.setAbsoluteURL(serverUrl);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("33:");
            //request.encoding("");

            Hashtable headers = SignRsaParameter(methodOrUri, request);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("44:");

            //哈希表(Hashtable)顺序输出
            foreach (DictionaryEntry de in headers)
            {
                log.Append(de.Key + ":" + de.Value + "\r\n");
            }
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("55:");

            //请求网站
            request.encoding("");
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("66:");
            Stream stream = HttpUtils.PostAndGetHttpWebResponse(request, "POST", headers).GetResponseStream();
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("77:");
            string content = new StreamReader(stream, Encoding.UTF8).ReadToEnd();

            log.Append("请求结果：" + content + "\r\n");
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("88:"+ content);
            //写入日志文件
            SoftLog.LogStr(log.ToString(), "Request");
            return content;

        }

        private static Hashtable SignRsaParameter(String methodOrUri, YopRequest request)
        {

            Assert.notNull(request.getSecretKey(), "secretKey must be specified");
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-1:");
            string appKey = request.getParamValue(YopConstants.APP_KEY);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-2:"+ appKey);
            if (StringUtils.isBlank(appKey))
            {
                appKey = StringUtils.trimToNull(request
                        .getParamValue(YopConstants.CUSTOMER_NO));
            }

            Assert.notNull(request.getSecretKey(), "secretKey must be specified");

            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-3:" + appKey);
            string timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-4:" + timestamp);
            String requestId = Guid.NewGuid().ToString();
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-5:" + requestId);
            YopConfig.setYopRequestId(requestId);
            ////Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-6:" + requestId);
            //string requestId = "654321";
            //string timestamp = "2016-02-25T08:57:48Z";

            //string[] headers = new string[] { };

            Hashtable headers = new Hashtable();

            headers.Add("x-yop-request-id", requestId);
            headers.Add("x-yop-date", timestamp);

            string protocolVersion = "yop-auth-v2";
            string EXPIRED_SECONDS = "1800";

            string authString = protocolVersion + "/" + appKey + "/" + timestamp + "/" + EXPIRED_SECONDS;
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-7authString:" + authString);


            List<string> headersToSignSet = new List<string>();
            headersToSignSet.Add("x-yop-request-id");
            headersToSignSet.Add("x-yop-date");

            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-8authString:" + authString);
            if (StringUtils.isBlank(request.getCustomerNo()))
            {
                //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-9appKey:" + appKey);
                headers.Add("x-yop-appkey", appKey);
                headersToSignSet.Add("x-yop-appkey");
            }
            else
            {
                //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-10appKey:" + appKey);
                headers.Add("x-yop-customerid", appKey);
                headersToSignSet.Add("x-yop-customerid");
            }


            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-11:methodOrUri:"+ methodOrUri);
            // Formatting the URL with signing protocol.
            string canonicalURI = HttpUtils.getCanonicalURIPath(methodOrUri);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-12:canonicalURI:"+ canonicalURI);

            // Formatting the query string with signing protocol.
            string canonicalQueryString = getCanonicalQueryString(request, true);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-13:canonicalQueryString:" + canonicalQueryString);

            //Sorted the headers should be signed from the request.
            SortedDictionary<String, String> headersToSign = getHeadersToSign(headers, headersToSignSet);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-14");

            // Formatting the headers from the request based on signing protocol.
            string canonicalHeader = getCanonicalHeaders(headersToSign);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-15：canonicalHeader：" + canonicalHeader);

            string signedHeaders = "";
            if (headersToSignSet != null)
            {
                foreach (string key in headersToSign.Keys)
                {

                    string value = (string)headersToSign[key];
                    if (signedHeaders == "")
                    {
                        signedHeaders += "";
                    }
                    else
                    {

                        signedHeaders += ";";
                    }
                    signedHeaders += key;


                }
                signedHeaders = signedHeaders.ToLower();
            }

            string canonicalRequest = authString + "\n" + "POST" + "\n" + canonicalURI + "\n" + canonicalQueryString + "\n" + canonicalHeader;
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-16：canonicalRequest：" + canonicalRequest);

            string private_key = request.getSecretKey();
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-17：canonicalRequest：" + private_key);

            string signToBase64 = SHA1withRSA.sign(canonicalRequest, private_key, "UTF-8");
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-18：signToBase64：" + signToBase64);

            signToBase64 = HttpUtils.Base64UrlEncode(signToBase64);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-19：signToBase64：" + signToBase64);

            signToBase64 += "$SHA256";

            headers.Add("Authorization", "YOP-RSA2048-SHA256 " + protocolVersion + "/" + appKey + "/" + timestamp + "/" + EXPIRED_SECONDS + "/" + signedHeaders + "/" + signToBase64);
            //Hishop.Plugins.Payment.YeePay.YeepayRequest.AppendLog("00-20：canonicalRequest：" + canonicalRequest);
            //Console.Write("headers:" + headers + "<br>");
            return headers;
        }
        private static void signAndEncrypt(String apiUri, YopRequest request)
        {

        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="apiUri">目标地址或命名模式的method</param>
        /// <param name="request">客户端请求对象</param>
        /// <returns>响应对象</returns>
        public static YopResponse uploadRsa(String apiUri, YopRequest request)
        {
            string content = uploadRsaForString(apiUri, request);

            YopResponse response = null;
            if (request.getFormat() == FormatType.json)
            {
                response = (YopResponse)JsonConvert.DeserializeObject(content, typeof(YopResponse));
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.LoadXml(content);
                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
                JObject jo = JObject.Parse(jsonText);
                string strValue = jo["response"].ToString();
                response = (YopResponse)JsonConvert.DeserializeObject(strValue, typeof(YopResponse));
            }

            handleRsaResult(request, response, content);
            return response;
        }
        /// <summary>
        /// 发起文件上传请求，以字符串返回
        /// </summary>
        /// <param name="apiUri">目标地址或命名模式的method</param>
        /// <param name="request">客户端请求对象</param>
        /// <returns>字符串形式的响应</returns>
        public static String uploadRsaForString(String apiUri, YopRequest request)
        {
            string serverUrl = richRequest(apiUri, request);
            //signAndEncrypt(request);
            request.setAbsoluteURL(serverUrl);
            //request.encoding("");
            string strTemp = request.getParamValue("_file");

            request.removeParam("_file");

            Hashtable headers = SignRsaParameter(apiUri, request);

            request.addParam("_file", strTemp);
            //请求网站
            //request.encoding("");

            Stream stream = HttpUtils.PostAndGetHttpWebResponse(request, "PUT", headers).GetResponseStream();
            string content = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
            return content;



        }


        protected static void handleRsaResult(YopRequest request, YopResponse response, String content)
        {

            response.format = request.getFormat();
            string ziped = string.Empty;
            if (response.isSuccess())
            {
                string strResult = getBizResult(content, request.getFormat());
                //ziped = strResult.Replace("\t", "").Replace("\n","").Replace(" ","");
                //ziped = Regex.Replace(ziped, @"[/n/r]", ""); 

                // 先解密，极端情况可能业务正常，但返回前处理（如加密）出错，所以要判断是否有error
                if (StringUtils.isNotBlank(strResult) && response.error == null)
                {
                    ziped = strResult;

                    if (request.isEncrypt())
                    {
                        //string decryptResult = decrypt(request, strResult.Trim());
                        //response.stringResult = decryptResult;
                        //response.result = decryptResult;
                        //ziped = decryptResult.Replace("\t\n", "");
                    }
                    else
                    {
                        response.stringResult = strResult;
                    }
                }
            }

            response.validSign = isValidResult(ziped, response.sign, request.getYopPublicKey());


        }
        /// <summary>
        /// 对业务结果签名进行校验
        /// </summary>
        /// <param name="result"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        /// 

        public static bool isValidResult(String result, String sign, String publicKey)
        {
            string sb = "";
            if (result == null)
            {
                sb = "";
            }
            else
            {
                sb = result.Trim();
            }

            sb = sb.Replace("\t", "").Replace("\n", "").Replace(" ", "");

            return SHA1withRSA.verify(sb, HttpUtils.Base64UrlDecode(sign.Remove(sign.Length - 7, 7)), publicKey, "UTF-8");

        }



        private static SortedDictionary<String, String> getHeadersToSign(Hashtable headers, List<string> headersToSign)
        {
            SortedDictionary<String, String> ret = new SortedDictionary<string, string>();

            if (headersToSign != null)
            {
                List<string> tempSet = new List<string>();

                foreach (Object header in headersToSign)
                {
                    tempSet.Add((string)header.ToString().ToLower());
                }

                headersToSign = tempSet;
            }

            foreach (DictionaryEntry de in headers)
            {
                if (de.Value != null)
                {
                    if ((headersToSign == null && isDefaultHeaderToSign((string)de.Key)) || (headersToSign != null && headersToSign.Contains((String)de.Key.ToString().ToLower()) && (String)de.Key.ToString().ToLower() != "Authorization"))
                    {
                        ret.Add((string)de.Key, (string)de.Value);

                    }
                }
                Console.WriteLine("Key -- {0}; Value --{1}.", de.Key, de.Value);
            }

            return ret;
        }

        private static bool isDefaultHeaderToSign(string header)
        {
            header = header.Trim().ToLower();
            List<string> defaultHeadersToSign = new List<string>();
            defaultHeadersToSign.Add("host");
            defaultHeadersToSign.Add("content-length");
            defaultHeadersToSign.Add("content-type");
            defaultHeadersToSign.Add("content-md5");


            return header.StartsWith("x-yop-") || defaultHeadersToSign.Contains(header);


        }

        /**
 * @param $headers
 * @return string
 */
        public static string getCanonicalHeaders(SortedDictionary<String, String> headers)
        {
            if (headers == null)
            {
                return "";
            }

            List<string> headerStrings = new List<string>();

            foreach (string key in headers.Keys)
            {


                string value = (string)headers[key];

                if (key == null)
                {
                    continue;
                }
                if (value == null)
                {
                    value = "";
                }

                string kv = HttpUtils.normalize(key.Trim().ToLower());
                value = HttpUtils.normalize(value.Trim());
                headerStrings.Add(kv + ':' + value);

            }

            headerStrings.Sort();
            string StrQuery = "";

            foreach (Object kv in headerStrings)
            {
                if (StrQuery == "")
                {
                    StrQuery += "";
                }
                else
                {
                    StrQuery += "\n";
                }

                StrQuery += (string)kv;
            }

            return StrQuery;


        }

        /// <summary>
        /// 从完整返回结果中获取业务结果，主要用于验证返回结果签名
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private static string getBizResult(string content, FormatType format)
        {
            if (StringUtils.isBlank(content))
            {
                return content;
            }

            switch (format)
            {
                case FormatType.json:
                    string jsonStr = StringUtils.substringAfter(content, "\"result\" : ");
                    jsonStr = StringUtils.substringBeforeLast(jsonStr, "\"ts\"");
                    // 去除逗号
                    jsonStr = StringUtils.substringBeforeLast(jsonStr, ",");
                    //return jsonStr.Replace("\"", "");
                    return jsonStr;
                default:
                    string xmlStr = StringUtils.substringAfter(content, "</state>");
                    xmlStr = StringUtils.substringBeforeLast(xmlStr, "<ts>");
                    return xmlStr;
            }
        }

        /**
        * @param $YopRequest
        * @param $forSignature
        * @return string
        */
        public static string getCanonicalQueryString(YopRequest request, bool forSignature)
        {
            List<string> arrayList = new List<string>();

            string StrQuery = "";
            NameValueCollection paramMap = request.getParams();

            string[] values = null;

            foreach (string key in paramMap.Keys)
            {
                values = paramMap.GetValues(key);
                foreach (string value in values)
                {
                    if (forSignature && key.CompareTo("Authorization") == 0)
                    {
                        continue;
                    }
                    arrayList.Add(key + "=" + HttpUtils.UrlEncode(value, System.Text.Encoding.GetEncoding("UTF-8"), true));
                    //arrayList.Add(key + "=" + HttpUtils.UrlEncode(value));
                }
            }
            arrayList.Sort();


            for (int i = 0; i < arrayList.Count; i++)
            {
                if (StrQuery == "")
                {
                    StrQuery += "";
                }
                else
                {
                    StrQuery += "&";
                }

                StrQuery += (string)arrayList[i];
            }

            return StrQuery;


        }

    }
}
