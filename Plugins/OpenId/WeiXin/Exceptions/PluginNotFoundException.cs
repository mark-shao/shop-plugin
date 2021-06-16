using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Plugins.OpenId.WeiXin.Exceptions
{
    public class PluginNotFoundException : Exception
    {
        public PluginNotFoundException()
        {
        }

        public PluginNotFoundException(string message):base(message)
        {
        }

        public PluginNotFoundException(string message, Exception inner): base(message, inner)
        {
        }
    }
}
