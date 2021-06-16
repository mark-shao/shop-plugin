using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Hishop.Plugins.Payment.AlipayWX
{
    /// <summary>
    /// 类名：Submit
    /// 功能：支付宝各接口请求提交类
    /// 详细：构造支付宝各接口表单HTML文本，获取远程HTTP数据
    /// 版本：3.0
    /// 日期：2012-07-11
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考
    /// </summary>
    public class Submit
    {
        /// 生成要请求给支付宝的参数数组
        /// </summary>
        /// <param name="sParaTemp">提交前的参数数组</param>
        /// <param name="input_charset">编码格式</param>
        /// <param name="key">MD5校验码</param>
        /// <param name="sign_type">签名类型</param>
        /// <returns>请求字符串</returns>
        private static string BuildRequestParaToString(SortedDictionary<string, string> sParaTemp, string input_charset, string key, string sign_type)
        {
            //待签名请求参数数组
            Dictionary<string, string> sPara = new Dictionary<string, string>();
            sPara = BuildRequestPara(sParaTemp, input_charset, key, sign_type);

            //把参数组中所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            string strRequestData;
            strRequestData = Function.CreateLinkString(sPara);
            return strRequestData;
        }


        /// <summary>
        /// 生成要请求给支付宝的参数数组
        /// </summary>
        /// <param name="sParaTemp">请求前的参数数组</param>
        /// <param name="input_charset">编码格式</param>
        /// <param name="key">MD5校验码</param>
        /// <param name="sign_type">签名类型</param>
        /// <returns>要请求的参数数组</returns>
        private static Dictionary<string, string> BuildRequestPara(SortedDictionary<string, string> sParaTemp, string input_charset, string key, string sign_type)
        {
            //签名结果
            string mysign = "";

            //获得签名结果
            mysign = Function.BuildMysign(sParaTemp, key, sign_type, input_charset);

            Dictionary<string, string> sPara = Function.FilterPara(sParaTemp);
            sPara.Add("sign", mysign);
            return sPara;
        }
        
        
        /// <summary>
        /// 构造HTTP的POST请求
        /// </summary>
        /// <param name="sParaTemp">请求参数数组</param>
        /// <param name="gateway">网关地址</param>
        /// <param name="input_charset">编码格式</param>
        /// <param name="key">MD5校验码</param>
        /// <param name="sign_type">签名类型</param>
        /// <returns>支付宝返回处理结果</returns>
        public static string SendPostInfo(SortedDictionary<string, string> sParaTemp, string gateway, string input_charset, string key, string sign_type)
        {
            //待请求参数数组字符串
            string strRequestData = BuildRequestParaToString(sParaTemp, input_charset, key, sign_type);

            //把数组转换成流中所需字节数组类型
            Encoding code = Encoding.GetEncoding(input_charset);
            byte[] bytesRequestData = code.GetBytes(strRequestData);

            //设置HttpWebRequest基本信息
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(gateway);
            request.Method = "post";
            request.ContentType = "application/x-www-form-urlencoded";

            //填充POST数据
            request.ContentLength = bytesRequestData.Length;

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bytesRequestData, 0, bytesRequestData.Length);
            }
            using (WebResponse wr = request.GetResponse())
            {
                //在这里对接收到的页面内容进行处理
                Stream myStream = wr.GetResponseStream();
                //获取数据必须用UTF8格式
                StreamReader sr = new StreamReader(myStream, code);
                string strResult = sr.ReadToEnd();
                return strResult;
            }
        }

        /// <summary>
        /// 构造HTTP的POST请求并提交
        /// </summary>
        /// <param name="sParaTemp">请求参数数组</param>
        /// <param name="gateway">网关地址</param>
        /// <param name="input_charset">编码格式</param>
        /// <param name="key">MD5校验码</param>
        /// <param name="sign_type">签名类型</param>
        /// <returns>跳转</returns>
        public static string SendPostRedirect(string req_url,SortedDictionary<string, string> sParaTemp, string gateway, string input_charset, string key, string sign_type)
        {
            //待请求参数数组字符串
            string strRequestData = BuildRequestParaToString(sParaTemp, input_charset, key, sign_type);
            //返回请求字符串
            return req_url + "?" + strRequestData;
        }
    }
}