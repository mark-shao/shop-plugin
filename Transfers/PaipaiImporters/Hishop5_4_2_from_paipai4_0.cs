using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Xml;
using System.Data;
using System.Globalization;
using Hishop.TransferManager;
using Ionic.Zip;
using LumenWorks.Framework.IO.Csv;

namespace Transfers.PaipaiImporters
{
    public class Hishop5_4_2_from_paipai4_0 : ImportAdapter
    {
       private const string ProductFilename = "products.csv";

        private readonly Target _importTo;
        private readonly Target _source;
        private readonly DirectoryInfo _baseDir;

        private DirectoryInfo _workDir;

        public Hishop5_4_2_from_paipai4_0()
        {
            _importTo = new HishopTarget("5.4.2");
            _source = new PPTarget("4.0");
            _baseDir = new DirectoryInfo(HttpContext.Current.Request.MapPath("~/storage/data/paipai"));
        }

        public override object[] ParseProductData(params object[] importParams)
        {
            string workDir = (string)importParams[0]; // initParams[1] - workDir

            const string virtualPath = "/Storage/Album/";
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
                    productRow["SalePrice"] = decimal.Parse(csv[10]);
                    if (!string.IsNullOrEmpty(csv[6]))
                    {
                        productRow["Weight"] = int.Parse(csv[6]);
                    }
                    if (!string.IsNullOrEmpty(csv[5]))
                    {
                        productRow["Stock"] = int.Parse(csv[5]);
                    }

                    productRow["ProductName"] = Trim(csv[1]);
                    if (!string.IsNullOrEmpty(csv[30]))
                    {
                        string descriptionFile = Path.Combine(workDir + "\\products", csv[30]);
                        if (File.Exists(descriptionFile))
                            productRow["Description"] = File.ReadAllText(descriptionFile, Encoding.GetEncoding("gb2312"));
                    }

                    string picName = Substring(csv[25]);
                    if (!string.IsNullOrEmpty(picName))
                    {
                        picName = picName.Substring(picName.LastIndexOf("\\") + 1);
                        if (File.Exists(Path.Combine(workDir + "\\products", picName)))
                        {
                            File.Copy(Path.Combine(workDir + "\\products", picName), context.Request.MapPath("~" + virtualPath + picName), true);
                            productRow["ImageUrl1"] = virtualPath + picName;
                        }
                    }

                    picName = Substring(csv[26]);
                    if (!string.IsNullOrEmpty(picName))
                    {
                        picName = picName.Substring(picName.LastIndexOf("\\") + 1);
                        if (File.Exists(Path.Combine(workDir + "\\products", picName)))
                        {
                            File.Copy(Path.Combine(workDir + "\\products", picName), context.Request.MapPath("~" + virtualPath + picName), true);
                            productRow["ImageUrl2"] = virtualPath + picName;
                        }
                    }

                    picName = Substring(csv[27]);
                    if (!string.IsNullOrEmpty(picName))
                    {
                        picName = picName.Substring(picName.LastIndexOf("\\") + 1);
                        if (File.Exists(Path.Combine(workDir + "\\products", picName)))
                        {
                            File.Copy(Path.Combine(workDir + "\\products", picName), context.Request.MapPath("~" + virtualPath + picName), true);
                            productRow["ImageUrl3"] = virtualPath + picName;
                        }
                    }

                    picName = Substring(csv[28]);
                    if (!string.IsNullOrEmpty(picName))
                    {
                        picName = picName.Substring(picName.LastIndexOf("\\") + 1);
                        if (File.Exists(Path.Combine(workDir + "\\products", picName)))
                        {
                            File.Copy(Path.Combine(workDir + "\\products", picName), context.Request.MapPath("~" + virtualPath + picName), true);
                            productRow["ImageUrl4"] = virtualPath + picName;
                        }
                    }

                    picName = Substring(csv[29]);
                    if (!string.IsNullOrEmpty(picName))
                    {
                        picName = picName.Substring(picName.LastIndexOf("\\") + 1);
                        if (File.Exists(Path.Combine(workDir + "\\products", picName)))
                        {
                            File.Copy(Path.Combine(workDir + "\\products", picName), context.Request.MapPath("~" + virtualPath + picName), true);
                            productRow["ImageUrl5"] = virtualPath + picName;
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

        #region 拍拍导入中不需要实现的方法
        public override object[] CreateMapping(params object[] initParams)
        {
            throw new NotImplementedException();
        }

        public override object[] ParseIndexes(params object[] importParams)
        {
            throw new NotImplementedException();
        }
        #endregion

        string Substring(string str)
        {
            if (str.StartsWith("\""))
            {
                str = str.Substring(1);
            }

            if (str.EndsWith("\""))
            {
                str = str.Substring(0, str.Length - 1);
            }

            return str;
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
