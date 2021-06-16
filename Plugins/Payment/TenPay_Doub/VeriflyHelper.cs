using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;

namespace Hishop.Plugins.Payment.TenPay_Doub
{
  public  class VeriflyHelper
    {
     
      public VeriflyHelper()
        {
            parameters = new Hashtable();
        }


        /** 应答的参数 */
        protected Hashtable parameters;

        /** 获取密钥 */
        public string Key { set; get; }

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
        /** 获取带参数的请求URL  @return String */
        public virtual string getRequestURL()
        {
            this.createSign();

            StringBuilder sb = new StringBuilder();
            ArrayList akeys = new ArrayList(parameters.Keys);
            akeys.Sort();
            foreach (string k in akeys)
            {
                string v = (string)parameters[k];
                if (null != v && "key".CompareTo(k) != 0)
                {
                    sb.Append(k + "=" +Globals.UrlEncode(v, "") + "&");
                }
            }

            //去掉最后一个&
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return  "?"+ sb.ToString();
        }

        /**
        * 创建md5摘要,规则是:按参数名称a-z排序,遇到空值的参数不参加签名。
        */
        protected virtual void createSign()
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
            string sign = Globals.GetMD5(sb.ToString());
            this.setParameter("sign", sign);
            //debug信息
        //    this.setDebugInfo(sb.ToString() + " => sign:" + sign);
        }

    }
}
