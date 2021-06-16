using System;
using System.Collections.Generic;

namespace Hishop.Weixin.Pay.Util
{
    internal class PayDictionary : Dictionary<string, string>
    {
        public void Add(string key, object value)
        {
            string strValue;

            if (value == null)
            {
                strValue = null;
            }
            else if (value is string)
            {
                strValue = (string)value;
            }
            else if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                strValue = dateTime.ToString("yyyyMMddHHmmss");
            }
            else if (value is Nullable<DateTime>)
            {
                Nullable<DateTime> dateTime = value as Nullable<DateTime>;
                strValue = dateTime.Value.ToString("yyyyMMddHHmmss");
            }
            else if (value is decimal)
            {
                strValue = String.Format("{0:F2}", value);
            }
            else if (value is Nullable<decimal>)
            {
                strValue = String.Format("{0:F0}", (value as Nullable<decimal>).Value);
            }
            else
            {
                strValue = value.ToString();
            }

            this.Add(key, strValue);
        }

        public new void Add(string key, string value)
        {
            if (!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(value))
            {
                base.Add(key, value);
            }
        }
    }
}
