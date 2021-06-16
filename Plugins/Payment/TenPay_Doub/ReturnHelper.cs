using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Web;
using System.Collections.Specialized;

namespace Hishop.Plugins.Payment.TenPay_Doub
{
  public  class ReturnHelper
    {
      protected Hashtable parameters;

        //获取服务器通知数据方式，进行参数获取
        public ReturnHelper (NameValueCollection collection)
        {
            parameters = new Hashtable();
            collection.Remove("HIGW");
            foreach (string k in collection)
            {
                
                string v = (string)collection[k];
                this.setParameter(k, v);
            }
        }
        /** 获取密钥 */
        public string Key{set;get;}

       /** 获取参数值 */
        public string getParameter(string parameter)
        {
            string s = (string)parameters[parameter];
            return (null == s) ? "" : s;
        }
        /** 设置参数值 */
        public void setParameter(string parameter, string parameterValue)
        {
            if (parameter != null && parameter != "")
            {
                if (parameters.Contains(parameter))
                {
                    parameters.Remove(parameter);
                }

                parameters.Add(parameter, parameterValue);
            }
        }

       /** 是否财付通签名,规则是:按参数名称a-z排序,遇到空值的参数不参加签名。 
         * @return boolean */
        public virtual Boolean isTenpaySign()
        {
            StringBuilder sb = new StringBuilder();

            ArrayList akeys = new ArrayList(parameters.Keys);
            akeys.Sort();

            foreach (string k in akeys)
            {
                string v = (string)parameters[k];
                if (null != v && "".CompareTo(v) != 0
                    && "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0)
                {
                    sb.Append(k + "=" + v + "&");
                }
            }
            sb.Append("key=" + Key);
            string sign =Globals.GetMD5(sb.ToString()).ToLower();
            return getParameter("sign").ToLower().Equals(sign);
        }
     
    }
}
