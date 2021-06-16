using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.Plugins
{
    public class PluginItem
    {

        public virtual string FullName { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string Logo { get; set; }

        public virtual string ShortDescription { get; set; }

        public virtual string Description { get; set; }

        public virtual string ToJsonString()
        {
            return "{\"FullName\":\"" + FullName + "\",\"DisplayName\":\"" + DisplayName + "\",\"Logo\":\"" + Logo +
                "\",\"ShortDescription\":\"" + ShortDescription + "\",\"Description\":\"" + Description + "\"}";
        }

        public virtual string ToXmlString()
        {
            return "<xml><FullName>" + FullName + "</FullName><DisplayName>" + DisplayName + "</DisplayName><Logo>" +
                Logo + "</Logo><ShortDescription>" + ShortDescription + "</ShortDescription><Description>" + Description + "</Description></xml>";
        }

    }
}