using System;
using System.Collections.Generic;
using System.Text;

using Hishop.Plugins;
using Hishop.Plugins.SupportedPlugins;
using System.Data;

namespace Hishop.Plugins.DataConvert.Hishop
{
    [Plugin("Hishop导出接口")]
    public class HishopExport : DataExport
    {
        public override string SaveCVS(DataTable dt)
        {
            string description;
            string imageUrl;

            StringBuilder sb = new StringBuilder();
            string productFormat = "\r\n\"{0}\"\t\"{1}\"\t{2}\t\"{3}\"\t\"{4}\"\t\"{5}\"\t\"{6}\"\t\"{7}\"\t{8}\t\"{9}\"\t{10}\t{11}\t{12}\t{13}";
            sb.Append("\"商家编码\"\t\"商品名称\"\t\"简单介绍\"\t\"详细描述\"\t\"详细页标题\"\t \"详细页描述\"\t\"详细页搜索关键字\"\t\"计量单位\"\t\"重量\"\t\"清晰图\"\t\"成本价\"\t\"市场价\"\t\"销售价\"\t\"库存\"");

            foreach (DataRow row in dt.Rows)
            {
                if (row["Description"] != DBNull.Value)
                {
                    description = (string)row["Description"];
                    description = description.Replace("\r\n", "");
                    description = description.Replace("\r", "").Replace("\n", "");
                    description = description.Replace(',', '，').Replace("\"", "\"\"");
                }
                else
                    description = string.Empty;

                if (row["ImageUrl1"] != DBNull.Value)
                {
                    imageUrl = (string)row["ImageUrl1"];
                }
                else
                    imageUrl = string.Empty;

                sb.AppendFormat(productFormat,
                    row["SKU"], row["ProductName"], row["ShortDescription"], description, row["Title"], row["Meta_Description"], row["Meta_Keywords"], row["Unit"], row["Weight"], imageUrl,
                    row["CostPrice"], row["MarketPrice"], row["SalePrice"], row["Stock"]);
            }
            // 输出
            return sb.ToString();
        }
    }
}
