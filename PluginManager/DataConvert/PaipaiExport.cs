using System;
using System.Collections.Generic;
using System.Text;

using Hishop.Plugins;
using Hishop.Plugins.SupportedPlugins;
using System.Data;

namespace Hishop.Plugins.DataConvert.Paipai
{
    [Plugin("Paipai导出接口")]
    public class PaipaiExport : DataExport
    {
        public override string SaveCVS(DataTable dt)
        {
            string description;
            string imageUrl;

            StringBuilder sb = new StringBuilder();
            string productFormat = "\r\n-1,\"{0}\",\"{1}\",{2},\"{3}\",\"{4}\",{5},{6},\"{7}\",{8},{9},{10},\"{11}\",\"{12}\",{13},{14},{15},{16},{17},{18},{19},{20},{21},\"{22}\",{23},\"{24}\",\"{25}\",{26},{27},\"{28}\",\"{29}\",\"{30}\"";
            sb.Append("\"id\",\"商品名称\",\"出售方式\",\"商品类目\",\"店铺类目\", \"店铺类目名称\",\"商品数量\",\"有效期\",\"定时上架\",\"新旧程度\",\"价格\",\"加价幅度\",\"省\",\"市\",\"运费承担\",\"平邮\",\"快递\",\"购买限制\",\"付款方式\",\"有发票\",\"有保修\",\"支持财付通\",\"自动重发\",\"错误原因\",\"图片文件\",\"商品详情\",\"上架选项\",\"皮肤风格\",\"属性\",\"诚保\",\"橱窗\",\"版本\"");

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

                if (row["ShortDescription"] != DBNull.Value)
                    description = row["ShortDescription"].ToString().Replace("\r\n", "<br/>").Replace(',', '，').Replace('"', ' ') + "<br/>" + description;

                if (row["ImageUrl1"] != DBNull.Value)
                {
                    imageUrl = (string)row["ImageUrl1"];
                }
                else
                    imageUrl = string.Empty;

                sb.AppendFormat(productFormat,
                    row["ProductName"], "", "0", "0", "0",
                    row["Stock"], 0, "", "", row["SalePrice"], 0,
                    "0", "0", "", 0, 0,
                    "0", "", "0", "0",
                    "0", "0",
                    "0", imageUrl, description, "2", "0", "0", "32", "1", "3.0beta1sp5");
            }
            // 输出
            return sb.ToString();
        }
    }
}
