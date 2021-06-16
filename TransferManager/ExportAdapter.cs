using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Hishop.TransferManager
{
    /// <summary>
    /// 数据导出适配器抽象类
    /// </summary>
    public abstract class ExportAdapter
    {

        /// <summary>
        /// 数据来源对象
        /// </summary>
        public abstract Target Source { get; }

        /// <summary>
        /// 数据导出对象
        /// </summary>
        public abstract Target ExportTo { get; }

        /// <summary>
        /// 执行导出操作
        /// </summary>
        public abstract void DoExport();

    }
}