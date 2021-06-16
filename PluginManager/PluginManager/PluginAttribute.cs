using System;

namespace Hishop.Plugins
{
    /// <summary>
    /// 用于为插件设置一个显示名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PluginAttribute : Attribute
    {

        public PluginAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 获取或设置插件的显示名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取或设置插件在同类型插件列表中的显示位置
        /// </summary>
        public int Sequence { get; set; }

    }
}