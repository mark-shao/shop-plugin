using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.Plugins.OpenId.QQ.Model
{
    [DataStore(Name = "Users")]
    public class User
    {
        [DataStore(Name = "ID", PrimaryKey = true)]
        public Guid ID { get; set; }
        [DataStore(Name = "Login")]
        public string Login { get; set; }
        [DataStore(Name = "Password")]
        public string Password { get; set; }

    }
}
