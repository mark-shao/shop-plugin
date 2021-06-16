using System;
using System.Collections.Generic;
using System.Text;

using Hishop.Plugins;
using Hishop.Plugins.SupportedPlugins;
using System.Data;

namespace Hishop.Plugins.DataConvert.Taobao
{
    [Plugin("Taobao导出接口")]
    public class TaobaoExport : DataExport
    {
        public override string SaveCVS(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            string productFormat = "{0}\"\t{1}\t\"{2}\"\t{3}\t\"{4}\"\t\"{5}\"\t\"{6}\"\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t\"{22}\"\t{23}\t\"{24}\"\t{25}\t\"{26}\"\t{27}\t{28}\t{29}\t{30}\t\"{31}\"\t{32}\t{33}\t{34}\t\"{35}\"\t\"{36}\"\t\"{37}\"\t\"{38}\"\t\n\"";
            sb.Append("宝贝名称\t宝贝类目\t店铺类目\t新旧程度\t省\t城市\t出售方式\t宝贝价格\t加价幅度\t宝贝数量\t有效期\t运费承担\t平邮\tEMS\t快递\t付款方式\t支付宝\t发票\t保修\t自动重发\t放入仓库\t橱窗推荐\t发布时间\t心情故事\t宝贝描述\t宝贝图片\t宝贝属性\t团购价\t最小团购件数\t邮费模版ID\t会员打折\t修改时间\t上传状态\t图片状态\t返点比例\t新图片\t销售属性组合\t用户输入ID串\t用户输入名-值对\n\"");

            string description;
            string imageUrl;
            string imageName = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                if (row["Description"] != DBNull.Value)
                {
                    description = Trim((string)row["Description"]);
                }
                else
                    description = string.Empty;

                if (row["ShortDescription"] != DBNull.Value)
                {
                    string shortDescription = Trim(Convert.ToString(row["ShortDescription"]).Trim());
                    if (!string.IsNullOrEmpty(shortDescription) && shortDescription.Length > 0)
                        description = shortDescription + "<br/>" + description;
                }

                description = description.Replace("\r\n", "");
                description = description.Replace("\r", "").Replace("\n", "");
                description = description.Replace("\"", "\"\"");

                if (row["ImageUrl1"] != DBNull.Value)
                {
                    imageUrl = Trim((string)row["ImageUrl1"]);
                    string filename = imageUrl.Substring(imageUrl.LastIndexOf("/"));

                    if (filename.EndsWith(".jpg"))
                    {
                        filename = filename.Replace(".jpg", ".tbi");
                    }

                    imageName = filename.Replace(".tbi", ":0:0:|;");
                }
                else
                {
                    imageUrl = string.Empty;
                    imageName = string.Empty;
                }

                sb.AppendFormat(productFormat,
                            Trim(Convert.ToString(row["ProductName"])), "0", "0", "", "", "", "", row["SalePrice"], 0,
                            row["Stock"], 0, "", 0, 0,
                            0, "", "0", "0",
                            "0", "0", "0", "0",
                            "1980-1-1  0:00:00", "", description, imageUrl, "", 0,
                            "", 0, "0", row["AddedDate"], "200", "0", "0", imageName,
                            string.Empty, string.Empty, string.Empty);
            }
            // 输出
            return sb.ToString();
        }
    }
}
