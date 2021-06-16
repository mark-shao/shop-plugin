using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;

namespace Hishop.Plugins.Payment.TenPay_Doub
{
   public class ClientVerifly
    {
       protected Hashtable parameters;

       public ClientVerifly()
        {
            parameters = new Hashtable();
        }

       public string key { set; get; }

       protected string content;

       public string getContent()
       {
           return this.content;
       }


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
           sb.Append("key=" + key);
           string sign = Globals.GetMD5(sb.ToString()).ToLower();
           return getParameter("sign").ToLower().Equals(sign);
       }


          public virtual void setContent(string content)
       {
           this.content = content;
           XmlDocument xmlDoc = new XmlDocument();
           xmlDoc.LoadXml(content);
           XmlNode root = xmlDoc.SelectSingleNode("root");
           XmlNodeList xnl = root.ChildNodes;

           foreach (XmlNode xnf in xnl)
           {
               this.setParameter(xnf.Name, xnf.InnerXml);
           }
       }

    }
}
