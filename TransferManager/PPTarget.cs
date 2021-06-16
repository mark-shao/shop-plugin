using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.TransferManager
{
    public class PPTarget : Target
    {

        public const string TargetName = "拍拍助理";

        public PPTarget(Version version)
            : base(TargetName, version)
        {
        }

        public PPTarget(string versionString)
            : base(TargetName, versionString)
        {
        }

        public PPTarget(int major, int minor, int build)
            : base(TargetName, major, minor, build)
        {
        }

    }
}