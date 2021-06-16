using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.TransferManager
{
    /// <summary>
    /// 数据互导对象接口
    /// </summary>
    public class Target
    {

        public Target(string name, int major, int minor, int build)
            : this(name, new Version(major, minor, build))
        {
        }

        public Target(string name, string versionString)
            : this(name, new Version(versionString))
        {
        }

        public Target(string name, Version version)
        {
            Name = name;
            Version = version;
        }

        public override string ToString()
        {
            return Name + Version.ToString();
        }

        public string Name { get; private set; }
        public Version Version { get; private set; }

    }
}