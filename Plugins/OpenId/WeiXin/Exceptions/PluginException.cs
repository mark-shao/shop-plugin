using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Plugins.OpenId.WeiXin.Exceptions
{
    public class PluginException:Exception
    {
        public PluginException()
        {
        }

        public PluginException(string message):base(message)
        {
        }

        public PluginException(string message, Exception inner):base(message, inner)
        {
        }
    }
}
