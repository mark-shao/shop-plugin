using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.TransferManager
{
    public class TbTarget : Target
    {

        public const string TargetName = "淘宝助理";

        public TbTarget(Version version)
            : base(TargetName, version)
        {
        }

        public TbTarget(string versionString)
            : base(TargetName, versionString)
        {
        }

        public TbTarget(int major, int minor, int build)
            : base(TargetName, major, minor, build)
        {
        }

    }
}