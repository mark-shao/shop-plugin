using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.TransferManager
{
    /// <summary>
    /// 数据导入适配器抽象类
    /// </summary>
    public abstract class ImportAdapter
    {

        /// <summary>
        /// 数据来源对象
        /// </summary>
        public abstract Target Source { get; }

        /// <summary>
        /// 数据导入对象
        /// </summary>
        public abstract Target ImportTo { get; }

        public abstract string PrepareDataFiles(params object[] initParams);

        /// <summary>
        /// 建立商品类型映射关系
        /// </summary>
        /// <param name="initParams"></param>
        /// <returns></returns>
        public abstract object[] CreateMapping(params object[] initParams);

        /// <summary>
        /// 根据所给参数获取商品类型和基本信息
        /// </summary>
        /// <param name="importParams"></param>
        /// <returns></returns>
        public abstract object[] ParseIndexes(params object[] importParams);

        /// <summary>
        /// 根据给定的参数解析并返回商品信息
        /// </summary>
        /// <param name="importParams"></param>
        /// <returns></returns>
        public abstract object[] ParseProductData(params object[] importParams);

    }
}