using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using Hishop.TransferManager;
using Ionic.Zip;
using LumenWorks.Framework.IO.Csv;
using System.Text.RegularExpressions;

namespace Hishop.Transfers.TaobaoImporters
{
    public class Yfx1_2_from_Taobao5_0 : ImportAdapter
    {
        private const string ProductFilename = "products.csv";

        private readonly Target _importTo;
        private readonly Target _source;
        private readonly DirectoryInfo _baseDir;

        private DirectoryInfo _workDir, _productImagesDir;

        public Yfx1_2_from_Taobao5_0()
        {
            _importTo = new YfxTarget("2.1");
            _source = new TbTarget("5.7");
            _baseDir = new DirectoryInfo(HttpContext.Current.Request.MapPath("~/storage/data/taobao"));
        }

        public override object[] ParseProductData(params object[] importParams)
        {
            string workDir = (string)importParams[0]; // initParams[1] - workDir

            const string virtualPath = "/Storage/master/product/images/";
            HttpContext context = HttpContext.Current;

            DataTable dtProducts = GetProductSet();

            // 去掉前两行
            StreamReader sr = new StreamReader(Path.Combine(workDir, ProductFilename), Encoding.Unicode);
            string str = sr.ReadToEnd();
            sr.Close();

            str = str.Substring(str.IndexOf('\n') + 1);
            str = str.Substring(str.IndexOf('\n') + 1);

            StreamWriter sw = new StreamWriter(Path.Combine(workDir, ProductFilename), false, Encoding.Unicode);
            sw.Write(str);
            sw.Close();


            using (CsvReader csv = new CsvReader(new StreamReader(Path.Combine(workDir, ProductFilename), System.Text.Encoding.Default), true, '\t'))
            {
                int index = 0;
                while (csv.ReadNextRecord())
                {
                    index++;
                    DataRow productRow = dtProducts.NewRow();
                    Random rand = new Random();
                    productRow["SKU"] = csv["商家编码"];
                    productRow["SalePrice"] = decimal.Parse(csv["宝贝价格"]);
                    productRow["Num"] = 0;
                    if (!string.IsNullOrEmpty(csv["宝贝数量"]))
                    {
                        productRow["Num"] = productRow["Stock"] = Convert.ToInt64(csv["宝贝数量"]);
                    }

                    productRow["ProductName"] = Trim(csv["宝贝名称"]);
                    if (!string.IsNullOrEmpty(csv["宝贝描述"]))
                    {
                        productRow["Description"] = Trim(csv["宝贝描述"].Replace("\"\"", "\"").Replace("alt=\"\"", "").Replace("alt=\"", "").Replace("alt=''", ""));
                    }

                    string pic = Trim(csv["新图片"]);

                    if (!string.IsNullOrEmpty(pic))
                    {
                        if (pic.EndsWith(";"))
                        {
                            string[] picArrary = pic.Split(';');
                            for (int i = 0; i < picArrary.Length - 1; i++)
                            {
                                string name = picArrary[i].Substring(0, picArrary[i].IndexOf(":"));
                                string picName = name + ".jpg";

                                if (File.Exists(Path.Combine(workDir + "\\products", name + ".tbi")))
                                {
                                    File.Copy(Path.Combine(workDir + "\\products", name + ".tbi"), context.Request.MapPath("~" + virtualPath + picName), true);

                                    switch (i)
                                    {
                                        case 0:
                                            productRow["ImageUrl1"] = virtualPath + picName;
                                            break;
                                        case 1:
                                            productRow["ImageUrl2"] = virtualPath + picName;
                                            break;
                                        case 2:
                                            productRow["ImageUrl3"] = virtualPath + picName;
                                            break;
                                        case 3:
                                            productRow["ImageUrl4"] = virtualPath + picName;
                                            break;
                                        case 4:
                                            productRow["ImageUrl5"] = virtualPath + picName;
                                            break;
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (File.Exists(Path.Combine(workDir + "\\products", pic.Replace(".jpg", ".tbi"))))
                            {
                                File.Copy(Path.Combine(workDir + "\\products", pic.Replace(".jpg", ".tbi")), context.Request.MapPath("~" + virtualPath + pic), true);
                                productRow["ImageUrl1"] = virtualPath + pic;
                            }
                        }
                    }

                    productRow["Cid"] = 0;
                    if (!string.IsNullOrEmpty(csv["宝贝类目"]))
                    {
                        productRow["Cid"] = Convert.ToInt64(csv["宝贝类目"]);
                    }
                    productRow["StuffStatus"] = csv["新旧程度"] == "1" ? "new" : "second";
                    productRow["LocationState"] = csv["省"];
                    productRow["LocationCity"] = csv["城市"];
                    productRow["FreightPayer"] = csv["运费承担"] == "1" ? "seller" : "buyer";
                    try
                    {
                        productRow["PostFee"] = decimal.Parse(csv["平邮"]);
                        productRow["ExpressFee"] = decimal.Parse(csv["快递"]);
                        productRow["EMSFee"] = decimal.Parse(csv["EMS"]);
                    }
                    catch 
                    {
                        productRow["PostFee"] = 0m;
                        productRow["ExpressFee"] = 0m;
                        productRow["EMSFee"] = 0m;
                    }

                    productRow["HasInvoice"] = csv["发票"] == "1" ? true : false; 
                    productRow["HasWarranty"] = csv["保修"]== "1" ? true : false;
                    productRow["HasDiscount"] = csv["会员打折"] == "1" ? true : false;

                    if (!string.IsNullOrEmpty(csv["有效期"]))
                        productRow["ValidThru"] = long.Parse(csv["有效期"]);

                    if (!string.IsNullOrEmpty(csv["开始时间"]))
                        productRow["ListTime"] = DateTime.Parse(csv["开始时间"]);

                    productRow["PropertyAlias"] = csv["宝贝属性"];
                    productRow["InputPids"] = csv["用户输入ID串"];
                    productRow["InputStr"] = csv["用户输入名-值对"];

                    string skuPrices = string.Empty;
                    string skuQuantities = string.Empty;
                    string skuOuterIds = string.Empty;
                    string skuProperties = string.Empty;
                    string skusString = csv["销售属性组合"];
                    if (!string.IsNullOrEmpty(skusString))
                    {
                        string pat = "(?<Price>[^:]+):(?<Quantities>[^:]+):(?<Outid>[^:]*):(?<Skuprop>[^;]+;(?:\\d+:\\d+;)?)";
                        Regex reg = new Regex(pat, RegexOptions.IgnoreCase);

                        MatchCollection mc = reg.Matches(skusString);

                        foreach (Match match in mc)
                        {
                            skuQuantities += match.Groups["Quantities"] + ",";
                            skuPrices += match.Groups["Price"] + ",";
                            skuOuterIds += match.Groups["Outid"] + ",";
                            skuProperties += match.Groups["Skuprop"].ToString().Substring(0, match.Groups["Skuprop"].ToString().Length - 1) + ",";
                        }
                        if (skuQuantities.Length > 0)
                            skuQuantities = skuQuantities.Substring(0, skuQuantities.Length - 1);
                        if (skuPrices.Length > 0)
                            skuPrices = skuPrices.Substring(0, skuPrices.Length - 1);
                        if (skuOuterIds.Length > 0)
                            skuOuterIds = skuOuterIds.Substring(0, skuOuterIds.Length - 1);
                        if (skuProperties.Length > 0)
                            skuProperties = skuProperties.Substring(0, skuProperties.Length - 1);
                    }
                                        
                    productRow["SkuProperties"] = skuProperties;
                    productRow["SkuQuantities"] = skuQuantities;
                    productRow["SkuPrices"] = skuPrices;
                    productRow["SkuOuterIds"] = skuOuterIds;

                    dtProducts.Rows.Add(productRow);
                }
            }


            return new object[] { dtProducts };
        }

        private DataTable GetProductSet()
        {
            DataTable dtProducts = new DataTable("products");

            dtProducts.Columns.Add(new DataColumn("ProductName") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("Description") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("ImageUrl1") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("ImageUrl2") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("ImageUrl3") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("ImageUrl4") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("ImageUrl5") { DataType = Type.GetType("System.String") });

            dtProducts.Columns.Add(new DataColumn("SKU") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("Stock") { DataType = Type.GetType("System.Int32") });
            dtProducts.Columns.Add(new DataColumn("SalePrice") { DataType = Type.GetType("System.Decimal") });
            dtProducts.Columns.Add(new DataColumn("Weight") { DataType = Type.GetType("System.Decimal") });

            dtProducts.Columns.Add(new DataColumn("Cid") { DataType = Type.GetType("System.Int64") });
            dtProducts.Columns.Add(new DataColumn("StuffStatus") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("Num") { DataType = Type.GetType("System.Int64") });
            dtProducts.Columns.Add(new DataColumn("LocationState") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("LocationCity") { DataType = Type.GetType("System.String") });

            dtProducts.Columns.Add(new DataColumn("FreightPayer") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("PostFee") { DataType = Type.GetType("System.Decimal") });
            dtProducts.Columns.Add(new DataColumn("ExpressFee") { DataType = Type.GetType("System.Decimal") });
            dtProducts.Columns.Add(new DataColumn("EMSFee") { DataType = Type.GetType("System.Decimal") });

            dtProducts.Columns.Add(new DataColumn("HasInvoice") { DataType = Type.GetType("System.Boolean") });
            dtProducts.Columns.Add(new DataColumn("HasWarranty") { DataType = Type.GetType("System.Boolean") });
            dtProducts.Columns.Add(new DataColumn("HasDiscount") { DataType = Type.GetType("System.Boolean") });

            dtProducts.Columns.Add(new DataColumn("ValidThru") { DataType = Type.GetType("System.Int64") });
            dtProducts.Columns.Add(new DataColumn("ListTime") { DataType = Type.GetType("System.DateTime") });

            dtProducts.Columns.Add(new DataColumn("PropertyAlias") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("InputPids") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("InputStr") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("SkuProperties") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("SkuQuantities") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("SkuPrices") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("SkuOuterIds") { DataType = Type.GetType("System.String") });

            return dtProducts;
        }

        public override Target Source
        {
            get { return _source; }
        }

        public override Target ImportTo
        {
            get { return _importTo; }
        }

        public override string PrepareDataFiles(params object[] initParams)
        {
            string filename = (string)initParams[0]; // initParams[0] - 数据包文件
            _workDir = _baseDir.CreateSubdirectory(Path.GetFileNameWithoutExtension(filename));

            using (ZipFile zip = ZipFile.Read(Path.Combine(_baseDir.FullName, filename)))
            {
                foreach (ZipEntry file in zip)
                {
                    file.Extract(_workDir.FullName, ExtractExistingFileAction.OverwriteSilently);
                }
            }

            return _workDir.FullName;
        }

        #region 淘宝导入中不需要实现的方法
        public override object[] CreateMapping(params object[] initParams)
        {
            throw new NotImplementedException();
        }

        public override object[] ParseIndexes(params object[] importParams)
        {
            throw new NotImplementedException();
        }
        #endregion

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
