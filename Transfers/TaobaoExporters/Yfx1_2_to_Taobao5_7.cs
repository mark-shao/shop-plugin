using System;
using System.Collections.Generic;
using System.Text;
using Hishop.TransferManager;
using System.IO;
using System.Data;
using System.Web;
using Ionic.Zip;
using Ionic.Zlib;

namespace Hishop.Transfers.TaobaoExporters
{
    public class Yfx1_2_to_Taobao5_7 : ExportAdapter
    {
        private const string ExportVersion = "5.7";
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

        public Yfx1_2_to_Taobao5_7()
        {
            _exportTo = new TbTarget(ExportVersion);
            _source = new YfxTarget("1.2");
        }

        public Yfx1_2_to_Taobao5_7(params object[] exportParams)
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
            string productFormat = "\"{0}\"\t{1}\t\"{2}\"\t{3}\t\"{4}\"\t\"{5}\"\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t\"{19}\"\t\"{20}\"\t\"{21}\"" +
                "\t{22}\t{23}\t\"{24}\"\t{25}\t\"{26}\"\t{27}\t\"{28}\"\t\"{29}\"\t\"{30}\"\t\"{31}\"\t\"{32}\"\t\"{33}\"\t\"{34}\"\t{35}\t{36}\t{37}\t{38}\t\"{39}\"\t\"{40}\"\t{41}" +
                "\t{42}\t\"{43}\"\t\"{44}\"\t{45}\t{46}\t\"{47}\"\t{48}\t{49}\t{50}\t{51}\t\"{52}\"\t\"{53}\"\t\"{54}\"\t\"{55}\"\t{56}\t\"{57}\"\t\"{58}\"\t\"{59}\"\t\"{60}\"\t{61}\t\"{62}\"\r\n";

            sb.Append("version 1.00\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\r\n");
            sb.Append("title\tcid\tseller_cids\tstuff_status\tlocation_state\tlocation_city\titem_type\tprice\tauction_increment\tnum\tvalid_thru\tfreight_payer\tpost_fee\tems_fee");
            sb.Append("\texpress_fee\thas_invoice\thas_warranty\tapprove_status\thas_showcase\tlist_time\tdescription\tcateProps\tpostage_id\thas_discount\tmodified\tupload_fail_msg");
            sb.Append("\tpicture_status\tauction_point\tpicture\tvideo\tskuProps\tinputPids\tinputValues\touter_id\tpropAlias\tauto_fill\tnum_id\tlocal_cid\tnavigation_type\tuser_name\tsyncStatus");
            sb.Append("\tis_lighting_consigment\tis_xinpin\tfoodparame\tfeatures\tbuyareatype\tglobal_stock_type\tglobal_stock_country\tsub_stock_type\titem_size\titem_weight\tsell_promise\tcustom_design_flag");
            sb.Append("\twireless_desc\tbarcode\tsku_barcode\tnewprepay\tsubtitle\tcpv_memo\tinput_custom_cpv\tqualification\tadd_qualification\to2o_bind_service\r\n");
            sb.Append("宝贝名称\t宝贝类目\t店铺类目\t新旧程度\t省\t城市\t出售方式\t宝贝价格\t加价幅度\t宝贝数量\t有效期\t运费承担\t平邮\tEMS\t快递\t发票\t保修\t放入仓库\t橱窗推荐\t开始时间\t宝贝描述");
            sb.Append("\t宝贝属性\t邮费模版ID\t会员打折\t修改时间\t上传状态\t图片状态\t返点比例\t新图片\t视频\t销售属性组合\t用户输入ID串\t用户输入名-值对\t商家编码\t销售属性别名\t代充类型\t数字ID\t本地ID");
            sb.Append("\t宝贝分类\t账户名称\t宝贝状态\t闪电发货\t新品\t食品专项\t尺码库\t采购地\t库存类型\t国家地区\t库存计数\t物流体积\t物流重量\t退换货承诺\t定制工具\t无线详情\t商品条形码\tsku 条形码\t7天退货\t宝贝卖点\t属性值备注\t自定义属性值\t商品资质\t增加商品资质\t关联线下服务\r\n");

            string description;

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

                string imageName = string.Empty;
                if (row["ImageUrl1"] != DBNull.Value)
                    imageName += CopyImage((string)row["ImageUrl1"], 1);
                if (row["ImageUrl2"] != DBNull.Value)
                    imageName += CopyImage((string)row["ImageUrl2"], 2);
                if (row["ImageUrl3"] != DBNull.Value)
                    imageName += CopyImage((string)row["ImageUrl3"], 3);
                if (row["ImageUrl4"] != DBNull.Value)
                    imageName += CopyImage((string)row["ImageUrl4"], 4);
                if (row["ImageUrl5"] != DBNull.Value)
                    imageName += CopyImage((string)row["ImageUrl5"], 5);

                DataRow[] srows = _exportData.Tables["skus"].Select("ProductId=" + row["ProductId"].ToString(), "SalePrice desc");
                string stock = "0";
                int stockint = 0;
                string freight_payer = "1"; //seller:1 buyer=2
                string PostFee = "0";
                string ExpressFee = "0";
                string EMSFee = "0";
                string inputPids = "";
                string Cid = "";
                string inputValues = "";
                string productcode = Convert.ToString(row["productcode"]); //商家编码
                string cateProps = ""; //宝贝属性
                string skuall = "";
                string skuprop = "";
                string skuq = "";
                string skup = "";
                string skuout = "";
                string LocationState = "";
                string LocationCity = "";
                string HasInvoice = "";
                string HasWarranty = "";
                string HasDiscount = "";
                string StuffStatus = "";
                DataRow[] tbrows = _exportData.Tables["TaobaoSku"].Select("ProductId=" + row["Productid"].ToString());
                if (tbrows.Length > 0)
                {
                    if (_includeStock)
                    {
                        if (tbrows[0]["SkuQuantities"] != null && tbrows[0]["SkuQuantities"].ToString() != "")
                        {
                            string[] strnum = null;
                            if (tbrows[0]["SkuQuantities"].ToString().Contains(","))
                            {
                                strnum = tbrows[0]["SkuQuantities"].ToString().Split(',');
                            }
                            else
                            {
                                strnum = new string[1];
                                strnum[0] = tbrows[0]["SkuQuantities"].ToString();
                            }
                            foreach (var num in strnum)
                            {
                                stockint += Convert.ToInt32(num);
                            }
                        }
                        else
                            stockint += Convert.ToInt32(tbrows[0]["Num"]);
                    }

                    LocationState = Convert.ToString(tbrows[0]["LocationState"]);
                    LocationCity = Convert.ToString(tbrows[0]["LocationCity"]);
                    HasInvoice = Convert.ToString(tbrows[0]["HasInvoice"]).ToLower() == "true" ? "1" : "0";
                    HasWarranty = Convert.ToString(tbrows[0]["HasWarranty"]).ToLower() == "true" ? "1" : "0";
                    HasDiscount = Convert.ToString(tbrows[0]["HasDiscount"]).ToLower() == "true" ? "1" : "0";
                    StuffStatus = tbrows[0]["StuffStatus"].ToString() == "new" ? "1" : "0";
                    if (Convert.ToString(tbrows[0]["FreightPayer"]) == "buyer")
                    {
                        freight_payer = "2";
                        PostFee = Convert.ToString(tbrows[0]["PostFee"]);
                        ExpressFee = Convert.ToString(tbrows[0]["ExpressFee"]);
                        EMSFee = Convert.ToString(tbrows[0]["EMSFee"]);
                    }
                    Cid = Convert.ToString(tbrows[0]["Cid"]);
                    cateProps = Convert.ToString(tbrows[0]["PropertyAlias"]);
                    inputPids = Convert.ToString(tbrows[0]["inputpids"]);
                    inputValues = Convert.ToString(tbrows[0]["inputstr"]);
                    skuq = Convert.ToString(tbrows[0]["SkuQuantities"]);
                    skup = Convert.ToString(tbrows[0]["skuPrices"]);
                    skuprop = Convert.ToString(tbrows[0]["SkuProperties"]);
                    skuout = Convert.ToString(tbrows[0]["SkuOuterIds"]);
                    if (!string.IsNullOrEmpty(skuq))
                    {
                        string[] q = skuq.Split(',');
                        string[] p = skup.Split(',');
                        string[] outsku = skuout.Split(',');
                        string[] prop = skuprop.Split(',');
                        for (int i = 0; i < q.Length; i++)
                        {
                            skuall += p[i] + ":" + q[i] + ":" + outsku[i] + ":" + prop[i] + ";";
                        }
                    }

                }
                else
                {
                    if (_includeStock && srows.Length > 0)
                    {

                        foreach (DataRow srow in srows)
                        {
                            stockint += (int)srow["Stock"];
                        }
                    }
                }
                stock = stockint.ToString();
                if (srows.Length > 0)
                {
                    sb.AppendFormat(productFormat,
                             Trim(Convert.ToString(row["ProductName"])), Cid, "", StuffStatus, LocationState, LocationCity, "1", srows[0]["SalePrice"], "",
                             stock, "14", freight_payer, PostFee, EMSFee, ExpressFee, HasInvoice, HasWarranty, "0", "0",
                             "", description, cateProps, "0",
                            HasDiscount, DateTime.Now, "100", "", "0", imageName, string.Empty,
                             skuall, inputPids, inputValues, productcode, string.Empty, "0", "0", "0", "1", "", "1", "112", "242", "", "", "0", "-1", "", "2", "1", "1", "1", "", "", "", "", "1", "", "", "", "", "1", "");
                }

            }

            return sb.Remove(sb.Length - 2, 2).ToString();
        }
        private string CopyImage(string imageUrl, int index)
        {
            string imageName = string.Empty;
            if (!imageUrl.StartsWith("http://"))
            {
                imageUrl = Trim(imageUrl);
                string filePath = HttpContext.Current.Request.MapPath("~" + imageUrl);
                if (File.Exists(filePath))
                {
                    FileInfo file = new FileInfo(filePath);
                    string filename = file.Name.ToLower();
                    if (filename.EndsWith(".jpg") || filename.EndsWith(".gif") || filename.EndsWith(".jpeg") || filename.EndsWith(".png") || filename.EndsWith(".bmp"))
                    {
                        filename = filename.Replace(file.Extension.ToLower(), ".tbi");
                        file.CopyTo(Path.Combine(_productImagesDir.FullName, filename), true);
                        imageName += filename.Replace(".tbi", string.Format(":1:{0}:|;", index - 1));
                    }
                }
            }
            return imageName;
        }

        private string Trim(string str)
        {
            while (str.StartsWith("\""))
                str = str.Substring(1);

            while (str.EndsWith("\""))
                str = str.Substring(0, str.Length - 1);

            return str;
        }
    }
}
