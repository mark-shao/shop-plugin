using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.TransferManager
{
    public class HishopTarget : Target
    {

        public const string TargetName = "Hishop";

        public HishopTarget(Version version)
            : base(TargetName, version)
        {
        }

        public HishopTarget(string versionString)
            : base(TargetName, versionString)
        {
        }

        public HishopTarget(int major, int minor, int build)
            : base(TargetName, major, minor, build)
        {
        }

    }
}