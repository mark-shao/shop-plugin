using System;
using System.Collections.Generic;
using System.Text;
using Hishop.TransferManager;
using System.IO;
using System.Data;
using System.Web;
using System.Xml;
using System.Globalization;
using Ionic.Zip;
using Ionic.Zlib;

namespace Transfers.PaipaiExporters
{
    public class Hishop5_4_2_to_paipai4_0 : ExportAdapter
    {
        private const string ExportVersion = "4.0";
        private const string ProductFilename = "products.csv";

        private readonly Encoding _encoding = Encoding.Unicode;
        private readonly string _zipFilename;
        private readonly Target _exportTo;
        private readonly Target _source;
        private readonly DirectoryInfo _baseDir;
        private readonly DataSet _exportData;
        private readonly bool _includeImages;
        private readonly bool _includeCostPrice;
        private readonly bool _includeStock;
        private readonly string _flag;

        private DirectoryInfo _workDir, _productImagesDir;

        public Hishop5_4_2_to_paipai4_0()
        {
            _exportTo = new PPTarget(ExportVersion);
            _source = new HishopTarget("5.4.2");
        }

        public Hishop5_4_2_to_paipai4_0(params object[] exportParams)
            : this()
        {
            _exportData = (DataSet)exportParams[0]; // exportParams[0] - DataSet exportData
            _includeCostPrice = (bool)exportParams[1]; // exportParams[1] - bool includeCostPrice
            _includeStock = (bool)exportParams[2]; // exportParams[2] - bool includeStock
            _includeImages = (bool)exportParams[3]; // exportParams[3] - bool includeImages
            _baseDir = new DirectoryInfo(HttpContext.Current.Request.MapPath("~/storage/data/paipai"));
            _flag = DateTime.Now.ToString("yyyyMMddHHmmss");
            _zipFilename = string.Format("paipai.{0}.{1}.zip", ExportVersion, _flag);
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
            string descriptionFilename;
            string imageUrl;

            StringBuilder sb = new StringBuilder();
            string productFormat = "\r\n-1\t\"{0}\"\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t{6}\t\"{7}\"\t{8}\t{9}\t{10}\t" +
                "\"{11}\"\t\"{12}\"\t{13}\t{14}\t{15}\t{16}\t\"{17}\"\t{18}\t{19}\t{20}\t{21}\t{22}\t\"{23}\"\t" +
                "\"{24}\"\t\"{25}\"\t\"{26}\"\t\"{27}\"\t\"{28}\"\t\"{29}\"\t{30}\t{31}\t{32}\t{33}\t{34}\t{35}\t\"{36}\"\t" +
                "{37}\t\"{38}\"\t{39}\t\"{40}\"";
            sb.Append("\"id\"\t\"商品名称\"\t\"出售方式\"\t\"商品类目\"\t\"店铺类目\"\t\"商品数量\"\t\"商品重量\"\t\"有效期\"\t\"定时上架\"\t\"新旧程度\"\t\"价格\"\t\"加价幅度\"\t");
            sb.Append("\"省\"\t\"市\"\t\"运费承担\"\t\"平邮\"\t\"快递\"\t\"EMS\"\t\"购买限制\"\t\"付款方式\"\t\"有发票\"\t\"有保修\"\t\"支持财付通\"\t\"自动重发\"\t\"错误原因\"\t");
            sb.Append("\"图片\"\t\"图片2\"\t\"图片3\"\t\"图片4\"\t\"图片5\"\t\"商品详情\"\t\"上架选项\"\t\"皮肤风格\"\t\"属性\"\t\"诚保\"\t\"假一陪三\"\t\"橱窗\"\t\"库存属性\"\t");
            sb.Append("\"产品ID\"\t\"商家编码\"\t\"尺码对照表\"\t\"版本\"");

            foreach (DataRow row in _exportData.Tables["products"].Rows)
            {
                descriptionFilename = "{" + Guid.NewGuid().ToString() + "}.htm";
                string fulldescriptionFilename = Path.Combine(_productImagesDir.FullName, descriptionFilename);
                using (StreamWriter myWriter = new StreamWriter(fulldescriptionFilename, false, System.Text.Encoding.GetEncoding("gb2312")))
                {
                    myWriter.Write(row["Description"].ToString());
                }

                DataRow[] dtImages = _exportData.Tables["ProductImages"].Select("ProductId=" + row["ProductId"].ToString());
                string[] nameImages = { "", "", "", "", "" };
                for (int i = 0; i < dtImages.Length; i++)
                {
                    if (i >= 5)
                        break;
                    DataRow imgRow = dtImages[i];
                    imageUrl = imgRow["ImageUrl"].ToString();
                    if (File.Exists(HttpContext.Current.Request.MapPath("~" + imageUrl)))
                    {
                        FileInfo file = new FileInfo(HttpContext.Current.Request.MapPath("~" + imageUrl));
                        imageUrl = file.Name.ToLower();
                        nameImages[i] = imageUrl;
                        file.CopyTo(Path.Combine(_productImagesDir.FullName, imageUrl), true);
                    }
                }

                DataRow[] srows = _exportData.Tables["skus"].Select("ProductId=" + row["ProductId"].ToString(), "Price desc");
                decimal skuPrice = 0;
                if (srows != null && srows.Length > 0)
                    skuPrice = Convert.ToDecimal(srows[0]["Price"].ToString());
                int stock = 0;
                if (_includeStock)
                {
                    foreach (DataRow srow in srows)
                    {
                        stock += (int)srow["Stock"];
                    }
                }

                sb.AppendFormat(productFormat, row["ProductName"], "b", "0", "0", stock, row["Weight"], "7", "1970-1-1  8:00:00", "1", skuPrice + Convert.ToDecimal(row["SalePrice"]), "",
                     "", "", "1", "0.00", "0.00", "0.00", "", "0", "2", "2", "1", "0", "", nameImages[0], nameImages[1], nameImages[2], nameImages[3], nameImages[4], descriptionFilename, 
                     "2", "0", "0", "0", "0", "1", "", "0", row["SKU"], "0", "拍拍助理-商品管理 4.0 [54]");
            }
            return sb.ToString();
        }
    }
}
