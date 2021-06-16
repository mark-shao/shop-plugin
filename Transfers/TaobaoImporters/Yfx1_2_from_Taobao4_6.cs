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

namespace Hishop.Transfers.TaobaoImporters
{
    public class Yfx1_2_from_Taobao4_6 : ImportAdapter
    {
        private const string ProductFilename = "products.csv";

        private readonly Target _importTo;
        private readonly Target _source;
        private readonly DirectoryInfo _baseDir;

        private DirectoryInfo _workDir, _productImagesDir;

        public Yfx1_2_from_Taobao4_6()
        {
            _importTo = new YfxTarget("1.2");
            _source = new TbTarget("4.6");
            _baseDir = new DirectoryInfo(HttpContext.Current.Request.MapPath("~/storage/data/taobao"));
        }

        public override object[] ParseProductData(params object[] importParams)
        {
            string workDir = (string)importParams[0]; // initParams[1] - workDir

            const string virtualPath = "/Storage/master/product/images/";
            HttpContext context = HttpContext.Current;

            DataTable dtProducts = GetProductSet();
            using (CsvReader csv = new CsvReader(new StreamReader(Path.Combine(workDir, ProductFilename), System.Text.Encoding.Default), true, '\t'))
            {
                int index = 0;
                while (csv.ReadNextRecord())
                {
                    index++;
                    DataRow productRow = dtProducts.NewRow();
                    Random rand = new Random();
                    productRow["SKU"] = string.Format("{0}{1}", rand.Next(9).ToString() + rand.Next(9) + rand.Next(9) + rand.Next(9) + rand.Next(9), index);
                    productRow["SalePrice"] = decimal.Parse(csv[7]);
                    if (!string.IsNullOrEmpty(csv[9]))
                    {
                        productRow["Stock"] = int.Parse(csv[9]);
                    }

                    productRow["ProductName"] = Trim(csv[0]);
                    if (!string.IsNullOrEmpty(csv[24]))
                    {
                        productRow["Description"] = Trim(csv[24].Replace("\"\"", "\"").Replace("alt=\"\"", "").Replace("alt=\"", "").Replace("alt=''", ""));
                    }

                    string pic = Trim(csv[35]);
                    if (string.IsNullOrEmpty(pic))
                    {
                        pic = Trim(csv[25]);
                    }

                    if (!string.IsNullOrEmpty(pic))
                    {
                        if (pic.EndsWith(";"))
                        {
                            string[] picArrary = pic.Split(';');
                            for (int i = 0; i < picArrary.Length - 1; i++)
                            {
                                string picName = picArrary[i].Substring(0, picArrary[i].IndexOf(":")) + ".jpg";

                                if (File.Exists(Path.Combine(workDir + "\\products", picName.Replace(".jpg", ".tbi"))))
                                {
                                    File.Copy(Path.Combine(workDir + "\\products", picName.Replace(".jpg", ".tbi")), context.Request.MapPath("~" + virtualPath + picName), true);

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
            dtProducts.Columns.Add(new DataColumn("Weight") { DataType = Type.GetType("System.Int32") });

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
