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

namespace Hishop.Transfers.HishopExporters
{
    public class Yfx1_2_to_Hishop5_4_2 : ExportAdapter
    {
        private const string ExportVersion = "5.4.2";
        private const string IndexFilename = "indexes.xml";
        private const string ProductFilename = "products.xml";

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

        private DirectoryInfo _workDir, _typeImagesDir, _productImagesDir;

        public Yfx1_2_to_Hishop5_4_2()
        {
            _exportTo = new HishopTarget(ExportVersion);
            _source = new YfxTarget("1.2");
        }

        public Yfx1_2_to_Hishop5_4_2(params object[] exportParams)
            : this()
        {
            _exportData = (DataSet)exportParams[0]; // exportParams[0] - DataSet exportData
            _includeCostPrice = (bool)exportParams[1]; // exportParams[1] - bool includeCostPrice
            _includeStock = (bool)exportParams[2]; // exportParams[2] - bool includeStock
            _includeImages = (bool)exportParams[3]; // exportParams[3] - bool includeImages
            _url = (string)exportParams[4];
            _applicationPath = (string)exportParams[5];
            _baseDir = new DirectoryInfo(HttpContext.Current.Request.MapPath("~/storage/data/Hishop"));
            _flag = DateTime.Now.ToString("yyyyMMddHHmmss");
            _zipFilename = string.Format("Hishop.{0}.{1}.zip", ExportVersion, _flag);
        }

        public override void DoExport()
        {
            // 创建工作目录
            _workDir = _baseDir.CreateSubdirectory(_flag);
            _typeImagesDir = _workDir.CreateSubdirectory("images1");
            _productImagesDir = _workDir.CreateSubdirectory("images2");

            string indexFilePath = Path.Combine(_workDir.FullName, IndexFilename);
            // 创建商品类型文件
            using (FileStream indexStream = new FileStream(indexFilePath, FileMode.Create, FileAccess.Write))
            {
                XmlWriter indexWriter = new XmlTextWriter(indexStream, _encoding);

                indexWriter.WriteStartDocument();
                indexWriter.WriteStartElement("indexes");
                indexWriter.WriteAttributeString("version", ExportVersion);
                indexWriter.WriteAttributeString("QTY", _exportData.Tables["products"].Rows.Count.ToString(CultureInfo.InvariantCulture));
                indexWriter.WriteAttributeString("includeCostPrice", _includeCostPrice.ToString());
                indexWriter.WriteAttributeString("includeStock", _includeStock.ToString());
                indexWriter.WriteAttributeString("includeImages", _includeImages.ToString());

                WriteIndexes(indexWriter);

                indexWriter.WriteEndElement();
                indexWriter.WriteEndDocument();

                indexWriter.Close();
            }

            string productFilePath = Path.Combine(_workDir.FullName, ProductFilename);
            // 创建商品信息文件
            using (FileStream productStream = new FileStream(productFilePath, FileMode.Create, FileAccess.Write))
            {
                XmlWriter productWriter = new XmlTextWriter(productStream, _encoding);

                productWriter.WriteStartDocument();
                productWriter.WriteStartElement("products");

                WriteProducts(productWriter);

                productWriter.WriteEndElement();
                productWriter.WriteEndDocument();

                productWriter.Close();
            }

            // 打包下载
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionLevel = CompressionLevel.Default;
                zip.AddFile(indexFilePath, "");
                zip.AddFile(productFilePath, "");
                zip.AddDirectory(_typeImagesDir.FullName, _typeImagesDir.Name);
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

        private void WriteIndexes(XmlWriter indexWriter)
        {
            indexWriter.WriteStartElement("types");

            foreach (DataRow trow in _exportData.Tables["types"].Rows)
            {
                indexWriter.WriteStartElement("type");
                indexWriter.WriteElementString("typeId", trow["TypeId"].ToString());
                TransferHelper.WriteCDataElement(indexWriter, "typeName", trow["TypeName"].ToString());
                TransferHelper.WriteCDataElement(indexWriter, "remark", trow["Remark"].ToString());

                indexWriter.WriteStartElement("attributes");
                DataRow[] attrows = _exportData.Tables["attributes"].Select("TypeId=" + trow["TypeId"].ToString());
                foreach (DataRow arow in attrows)
                {
                    indexWriter.WriteStartElement("attribute");
                    indexWriter.WriteElementString("attributeId", arow["AttributeId"].ToString());
                    TransferHelper.WriteCDataElement(indexWriter, "attributeName", arow["AttributeName"].ToString());
                    indexWriter.WriteElementString("displaySequence", arow["DisplaySequence"].ToString());
                    indexWriter.WriteElementString("usageMode", arow["UsageMode"].ToString());

                    indexWriter.WriteStartElement("values");
                    DataRow[] vrows =
                        _exportData.Tables["values"].Select("AttributeId=" + arow["AttributeId"].ToString());
                    foreach (DataRow vrow in vrows)
                    {
                        indexWriter.WriteStartElement("value");
                        TransferHelper.WriteCDataElement(indexWriter, "valueStr", vrow["ValueStr"].ToString());
                        indexWriter.WriteEndElement();// end of value
                    }
                    indexWriter.WriteEndElement();// end of values
                    indexWriter.WriteEndElement();// end of attribute
                }
                indexWriter.WriteEndElement();// end of attributes
                indexWriter.WriteEndElement();// end of type
            }

            indexWriter.WriteEndElement();// end of types

            
            _exportData.Tables.Remove("types");
        }

        private void WriteProducts(XmlWriter productWriter)
        {
            productWriter.WriteStartElement("products");

            foreach (DataRow prow in _exportData.Tables["products"].Rows)
            {
                productWriter.WriteStartElement("product");

                productWriter.WriteElementString("productId", prow["ProductId"].ToString());
                productWriter.WriteElementString("typeId", prow["TypeId"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "productName", prow["ProductName"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "SKU", prow["ProductCode"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "shortDescription", prow["ShortDescription"].ToString());
                productWriter.WriteElementString("unit", prow["Unit"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "description", 
                    prow["Description"].ToString().Replace(string.Format("src=\"{0}/Storage/master/gallery", _applicationPath), string.Format("src=\"{0}/Storage/master/gallery", _url)));
                TransferHelper.WriteCDataElement(productWriter, "title", prow["Title"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "meta_Description", prow["Meta_Description"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "meta_Keywords", prow["Meta_Keywords"].ToString());
                productWriter.WriteElementString("Upselling", prow["SaleStatus"].ToString() == "1" ? "1" : "0");
                productWriter.WriteElementString("marketPrice", prow["MarketPrice"].ToString());

                DataRow[] srows = _exportData.Tables["skus"].Select("ProductId=" + prow["ProductId"].ToString(), "SalePrice");
                DataRow srow1 = srows[0];

                productWriter.WriteElementString("Weight", srow1["Weight"].ToString());
                if (_includeCostPrice)
                    productWriter.WriteElementString("CostPrice", srow1["CostPrice"].ToString());
                productWriter.WriteElementString("SalePrice", srow1["SalePrice"].ToString());

                productWriter.WriteStartElement("skus");
                foreach (DataRow srow in srows)
                {
                    productWriter.WriteStartElement("sku");
                    productWriter.WriteElementString("skuId", srow["SkuId"].ToString());
                    productWriter.WriteElementString("SKU", srow["SKU"].ToString());

                    if (_includeStock)
                        productWriter.WriteElementString("stock", srow["Stock"].ToString());
                    productWriter.WriteElementString("Price", ((decimal)srow["SalePrice"] - (decimal)srow1["SalePrice"]).ToString());

                    DataRow[] skrows = _exportData.Tables["skuItems"].Select("SkuId='" + srow["SkuId"].ToString() + "'");
                    productWriter.WriteStartElement("skuItems");

                    foreach (DataRow skrow in skrows)
                    {
                        DataRow[] attrows = _exportData.Tables["attributes"].Select("AttributeId=" + skrow["AttributeId"].ToString());
                        DataRow[] vrows = _exportData.Tables["values"].Select("ValueId=" + skrow["ValueId"].ToString());

                        productWriter.WriteStartElement("skuItem");
                        productWriter.WriteElementString("skuId", skrow["SkuId"].ToString());
                        productWriter.WriteElementString("attributeId", skrow["AttributeId"].ToString());
                        productWriter.WriteElementString("attributeName", attrows[0]["AttributeName"].ToString());
                        productWriter.WriteElementString("ValueStr", vrows[0]["ValueStr"].ToString());
                        productWriter.WriteEndElement();// end of skuItem
                    }

                    productWriter.WriteEndElement();// end of skuItems
                    productWriter.WriteEndElement();// end of sku
                }

                productWriter.WriteEndElement();// end of skus

                DataRow[] parows = _exportData.Tables["productAttributes"].Select("ProductId=" + prow["ProductId"].ToString());
                productWriter.WriteStartElement("productAttributes");
                foreach (DataRow paRow in parows)
                {
                    productWriter.WriteStartElement("productAttribute");

                    productWriter.WriteElementString("AttributeId", paRow["AttributeId"].ToString());
                    DataRow[] vrows = _exportData.Tables["values"].Select("ValueId=" + paRow["ValueId"].ToString());
                    TransferHelper.WriteCDataElement(productWriter, "ValueStr", vrows[0]["ValueStr"].ToString());

                    productWriter.WriteEndElement();// end of productAttribute
                }
                productWriter.WriteEndElement();// end of productAttributes

                productWriter.WriteStartElement("images");
                if (prow["ImageUrl1"].ToString().Length > 0)
                {
                    TransferHelper.WriteImageElement(productWriter, "image", _includeImages, prow["ImageUrl1"].ToString(), _productImagesDir);
                }
                if (prow["ImageUrl2"].ToString().Length > 0)
                {
                    TransferHelper.WriteImageElement(productWriter, "image", _includeImages, prow["ImageUrl2"].ToString(), _productImagesDir);
                }
                if (prow["ImageUrl3"].ToString().Length > 0)
                {
                    TransferHelper.WriteImageElement(productWriter, "image", _includeImages, prow["ImageUrl3"].ToString(), _productImagesDir);
                }
                if (prow["ImageUrl4"].ToString().Length > 0)
                {
                    TransferHelper.WriteImageElement(productWriter, "image", _includeImages, prow["ImageUrl4"].ToString(), _productImagesDir);
                }
                if (prow["ImageUrl5"].ToString().Length > 0)
                {
                    TransferHelper.WriteImageElement(productWriter, "image", _includeImages, prow["ImageUrl5"].ToString(), _productImagesDir);
                }
                productWriter.WriteEndElement();// end of images


                productWriter.WriteEndElement();// end of product
            }

            productWriter.WriteEndElement();// end of products

            _exportData.Tables.Remove("productAttributes");
            _exportData.Tables.Remove("skuItems");
            _exportData.Tables.Remove("skus");
            _exportData.Tables.Remove("products");

            _exportData.Tables.Remove("values");
            _exportData.Tables.Remove("attributes");
        }
    }
}
