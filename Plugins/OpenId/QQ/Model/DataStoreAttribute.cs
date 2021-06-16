using System;

namespace Hishop.Plugins.OpenId.QQ.Model
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    internal sealed class DataStoreAttribute : Attribute
    {
        public string Name { get; set; }
        public bool PrimaryKey { get; set; }

        public DataStoreAttribute()
        {
            this.PrimaryKey = false;
        }
    }
}
