using System;
using System.Collections.Generic;
using System.Text;
using Hishop.TransferManager;
using System.IO;
using System.Data;
using System.Web;
using Ionic.Zlib;
using Ionic.Zip;

namespace Hishop.Transfers.TaobaoExporters
{
    public class Yfx1_2_to_Taobao4_6 : ExportAdapter
    {
        private const string ExportVersion = "4.6";
        private const string ProductFilename = "products.csv";

        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly string _zipFilename;
        private readonly Target _exportTo;
        private readonly Target _source;
        private readonly DirectoryInfo _baseDir;
        private readonly DataSet _exportData;
        private readonly bool _includeImages;
        private readonly bool _includeCostPrice;
        private readonly bool _includeStock;
        private readonly string _url;
        private readonly string _applicationPath;
        private readonly string _flag;

        private DirectoryInfo _workDir, _productImagesDir;

        public Yfx1_2_to_Taobao4_6()
        {
            _exportTo = new TbTarget(ExportVersion);
            _source = new YfxTarget("1.2");
        }

        public Yfx1_2_to_Taobao4_6(params object[] exportParams)
            : this()
        {
            _exportData = (DataSet)exportParams[0]; // exportParams[0] - DataSet exportData
            _includeCostPrice = (bool)exportParams[1]; // exportParams[1] - bool includeCostPrice
            _includeStock = (bool)exportParams[2]; // exportParams[2] - bool includeStock
            _includeImages = (bool)exportParams[3]; // exportParams[3] - bool includeImages
            _url = (string)exportParams[4];
            _applicationPath = (string)exportParams[5];
            _baseDir = new DirectoryInfo(HttpContext.Current.Request.MapPath("~/storage/data/taobao"));
            _flag = DateTime.Now.ToString("yyyyMMddHHmmss");
            _zipFilename = string.Format("taobao.{0}.{1}.zip", ExportVersion, _flag);
        }

        public override void DoExport()
        {
            // 创建工作目录
            _workDir = _baseDir.CreateSubdirectory(_flag);
            _productImagesDir = _workDir.CreateSubdirectory("products");            

            string productFilePath = Path.Combine(_workDir.FullName, ProductFilename);
            // 创建商品信息文件
            using (FileStream productStream = new FileStream(productFilePath, FileMode.Create, FileAccess.Write))
            {
                string productsString = GetProductCSV();
                UnicodeEncoding utf8BOM = new UnicodeEncoding();
                int byteCount = utf8BOM.GetByteCount(productsString);
                byte[] prefix = utf8BOM.GetPreamble();
                byte[] output = new byte[prefix.Length + byteCount];
                System.Buffer.BlockCopy(prefix, 0, output, 0, prefix.Length);
                utf8BOM.GetBytes(productsString.ToCharArray(), 0, productsString.Length, output, prefix.Length);

                productStream.Write(output, 0, output.Length);
            }

            // 打包下载
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionLevel = CompressionLevel.Default;
                zip.AddFile(productFilePath, "");
                zip.AddDirectory(_productImagesDir.FullName, _productImagesDir.Name);

                HttpResponse response = HttpContext.Current.Response;
                response.ContentType = "application/x-zip-compressed";
                response.ContentEncoding = _encoding;
                response.AddHeader("Content-Disposition", "attachment; filename=" + _zipFilename);
                response.Clear();

                zip.Save(response.OutputStream);
                _workDir.Delete(true);

                response.Flush();
                response.Close();
            }
        }

        public override Target ExportTo
        {
            get { return _exportTo; }
        }

        public override Target Source
        {
            get { return _source; }
        }

        private string GetProductCSV()
        {
            // 构建要输出的内容
            //
            StringBuilder sb = new StringBuilder();
            string productFormat = "{0}\"\t{1}\t\"{2}\"\t{3}\t\"{4}\"\t\"{5}\"\t\"{6}\"\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t\"{22}\"\t{23}\t\"{24}\"\t{25}\t\"{26}\"\t{27}\t{28}\t{29}\t{30}\t\"{31}\"\t{32}\t{33}\t{34}\t\"{35}\"\t\"{36}\"\t\"{37}\"\t\"{38}\"\t\n\"";
            sb.Append("宝贝名称\t宝贝类目\t店铺类目\t新旧程度\t省\t城市\t出售方式\t宝贝价格\t加价幅度\t宝贝数量\t有效期\t运费承担\t平邮\tEMS\t快递\t付款方式\t支付宝\t发票\t保修\t自动重发\t放入仓库\t橱窗推荐\t发布时间\t心情故事\t宝贝描述\t宝贝图片\t宝贝属性\t团购价\t最小团购件数\t邮费模版ID\t会员打折\t修改时间\t上传状态\t图片状态\t返点比例\t新图片\t销售属性组合\t用户输入ID串\t用户输入名-值对\n\"");

            string description;
            string imageUrl;
            string imageName = string.Empty;

            foreach (DataRow row in _exportData.Tables["products"].Rows)
            {
                if (row["Description"] != DBNull.Value)
                {
                    description = Trim((string)row["Description"]);
                    description = description.Replace(string.Format("src=\"{0}/Storage/master/gallery", _applicationPath), string.Format("src=\"{0}/Storage/master/gallery", _url));
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
                    string filePath = HttpContext.Current.Request.MapPath("~" + imageUrl);

                    if (File.Exists(filePath))
                    {
                        FileInfo file = new FileInfo(filePath);
                        string filename = file.Name.ToLower();
                        if (filename.EndsWith(".jpg"))
                        {
                            filename = filename.Replace(".jpg", ".tbi");
                            file.CopyTo(Path.Combine(_productImagesDir.FullName, filename), true);
                            imageName = filename.Replace(".tbi", ":0:0:|;");
                        }
                    }
                }
                else
                {
                    imageUrl = string.Empty;
                    imageName = string.Empty;
                }

                DataRow[] srows = _exportData.Tables["skus"].Select("ProductId=" + row["ProductId"].ToString(), "SalePrice desc");
                int stock = 0;
                if (_includeStock)
                {
                    foreach (DataRow srow in srows)
                    {
                        stock += (int)srow["Stock"];
                    }
                }

                sb.AppendFormat(productFormat,
                            Trim(Convert.ToString(row["ProductName"])), "0", "0", "0", "", "", "b", srows[0]["SalePrice"], "0",
                            stock, "14", "0", "0", "0", "0", "", "", "0", "0", "0", "0", "0",
                            "1980-1-1  0:00", "", description, "", "", "0",
                            "0", 0, "0", DateTime.Now, "100", "", "0", imageName, string.Empty,
                            string.Empty, ",", ",", string.Empty, string.Empty, "0", string.Empty);
            }
            return sb.Remove(sb.Length - 2, 2).ToString();
        }

        private static string Trim(string str)
        {
            while (str.StartsWith("\""))
                str = str.Substring(1);

            while (str.EndsWith("\""))
                str = str.Substring(0, str.Length - 1);

            return str;
        }
    }
}
