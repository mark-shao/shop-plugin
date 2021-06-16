using System;

namespace Hishop.TransferManager
{
    public class YfxTarget : Target
    {

        public const string TargetName = "分销商城";

        public YfxTarget(Version version)
            : base(TargetName, version)
        {
        }

        public YfxTarget(string versionString)
            : base(TargetName, versionString)
        {
        }

        public YfxTarget(int major, int minor, int build)
            : base(TargetName, major, minor, build)
        {
        }

    }
}