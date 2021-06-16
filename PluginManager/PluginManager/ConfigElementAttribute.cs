using System;

namespace Hishop.Plugins
{
    /// <summary>
    /// 标识插件的属性为需要用户设置信息的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ConfigElementAttribute : Attribute
    {

        public ConfigElementAttribute(string name)
        {
            InputType = InputType.TextBox;
            Name = name;
            Nullable = true;
        }

        /// <summary>
        /// 获取属性的名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取或设置属性的说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置属性值的输入方式
        /// </summary>
        public InputType InputType { get; set; }

        /// <summary>
        /// 获取或设置属性可选值集合
        /// </summary>
        public string[] Options { get; set; }

        /// <summary>
        /// 获取或设置属性值可否为空
        /// </summary>
        public bool Nullable { get; set; }
        /// <summary>
        /// 是否隐藏部分字符串
        /// </summary>
        public bool HiddenPart { get; set; }

    }
}