using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using Hishop.TransferManager;
using Ionic.Zip;
using Ionic.Zlib;

namespace Hishop.Transfers.YfxExporters
{
    public class Yfx1_2_to_Yfx1_2 : ExportAdapter
    {

        private const string ExportVersion = "2.1";
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

        public Yfx1_2_to_Yfx1_2()
        {
            _exportTo = new YfxTarget(ExportVersion);
            _source = new YfxTarget("2.1");
        }

        public Yfx1_2_to_Yfx1_2(params object[] exportParams)
            : this()
        {
            _exportData = (DataSet) exportParams[0]; // exportParams[0] - DataSet exportData
            _includeCostPrice = (bool) exportParams[1]; // exportParams[1] - bool includeCostPrice
            _includeStock = (bool) exportParams[2]; // exportParams[2] - bool includeStock
            _includeImages = (bool) exportParams[3]; // exportParams[3] - bool includeImages
            _url = (string)exportParams[4];
            _applicationPath = (string)exportParams[5];
            _baseDir = new DirectoryInfo(HttpContext.Current.Request.MapPath("~/storage/data/yfx"));
            _flag = DateTime.Now.ToString("yyyyMMddHHmmss");
            _zipFilename = string.Format("YFX.{0}.{1}.zip", ExportVersion, _flag);
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
                    indexWriter.WriteElementString("useAttributeImage", arow["UseAttributeImage"].ToString());

                    indexWriter.WriteStartElement("values");
                    DataRow[] vrows =
                        _exportData.Tables["values"].Select("AttributeId=" + arow["AttributeId"].ToString());
                    foreach (DataRow vrow in vrows)
                    {
                        indexWriter.WriteStartElement("value");
                        indexWriter.WriteElementString("valueId", vrow["ValueId"].ToString());
                        indexWriter.WriteElementString("displaySequence", vrow["DisplaySequence"].ToString());
                        TransferHelper.WriteCDataElement(indexWriter, "valueStr", vrow["ValueStr"].ToString());
                        TransferHelper.WriteImageElement(indexWriter, "image", _includeImages, vrow["ImageUrl"].ToString(), _typeImagesDir);
                        indexWriter.WriteEndElement();// end of value
                    }
                    indexWriter.WriteEndElement();// end of values
                    indexWriter.WriteEndElement();// end of attribute
                }
                indexWriter.WriteEndElement();// end of attributes
                indexWriter.WriteEndElement();// end of type
            }

            indexWriter.WriteEndElement();// end of types

            _exportData.Tables.Remove("values");
            _exportData.Tables.Remove("attributes");
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
                TransferHelper.WriteCDataElement(productWriter, "productCode", prow["ProductCode"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "shortDescription", prow["ShortDescription"].ToString());
                productWriter.WriteElementString("unit", prow["Unit"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "description",
                    prow["Description"].ToString().Replace(string.Format("src=\"{0}/Storage/master/gallery", _applicationPath), string.Format("src=\"{0}/Storage/master/gallery", _url)));
                TransferHelper.WriteCDataElement(productWriter, "title", prow["Title"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "meta_Description", prow["Meta_Description"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "meta_Keywords", prow["Meta_Keywords"].ToString());
                productWriter.WriteElementString("saleStatus", prow["SaleStatus"].ToString());


                if (prow["ImageUrl1"] != DBNull.Value && !prow["ImageUrl1"].ToString().StartsWith("http://"))
                    TransferHelper.WriteImageElement(productWriter, "image1", _includeImages, prow["ImageUrl1"].ToString(), _productImagesDir);

                if (prow["ImageUrl2"] != DBNull.Value && !prow["ImageUrl2"].ToString().StartsWith("http://"))
                    TransferHelper.WriteImageElement(productWriter, "image2", _includeImages, prow["ImageUrl2"].ToString(), _productImagesDir);

                if (prow["ImageUrl3"] != DBNull.Value && !prow["ImageUrl3"].ToString().StartsWith("http://"))
                    TransferHelper.WriteImageElement(productWriter, "image3", _includeImages, prow["ImageUrl3"].ToString(), _productImagesDir);

                if (prow["ImageUrl4"] != DBNull.Value && !prow["ImageUrl4"].ToString().StartsWith("http://"))
                    TransferHelper.WriteImageElement(productWriter, "image4", _includeImages, prow["ImageUrl4"].ToString(), _productImagesDir);

                if (prow["ImageUrl5"] != DBNull.Value && !prow["ImageUrl5"].ToString().StartsWith("http://"))
                    TransferHelper.WriteImageElement(productWriter, "image5", _includeImages, prow["ImageUrl5"].ToString(), _productImagesDir);

                productWriter.WriteElementString("marketPrice", prow["MarketPrice"].ToString());
                productWriter.WriteElementString("lowestSalePrice", prow["LowestSalePrice"].ToString());
                productWriter.WriteElementString("penetrationStatus", prow["PenetrationStatus"].ToString());
                productWriter.WriteElementString("hasSKU", prow["HasSKU"].ToString());

                DataRow[] parows = _exportData.Tables["productAttributes"].Select("ProductId=" + prow["ProductId"].ToString());
                productWriter.WriteStartElement("attributes");
                foreach (DataRow parow in parows)
                {
                    productWriter.WriteStartElement("attribute");
                    productWriter.WriteElementString("attributeId", parow["AttributeId"].ToString());
                    productWriter.WriteElementString("valueId", parow["ValueId"].ToString());
                    productWriter.WriteEndElement();// end of attribute
                }
                productWriter.WriteEndElement();// end of attributes

                DataRow[] srows = _exportData.Tables["skus"].Select("ProductId=" + prow["ProductId"].ToString());
                productWriter.WriteStartElement("skus");

                foreach (DataRow srow in srows)
                {
                    productWriter.WriteStartElement("sku");
                    productWriter.WriteElementString("skuId", srow["SkuId"].ToString());
                    productWriter.WriteElementString("sKU", srow["SKU"].ToString());

                    if (_includeCostPrice)
                        productWriter.WriteElementString("costPrice", srow["CostPrice"].ToString());

                    productWriter.WriteElementString("weight", srow["Weight"].ToString());

                    if (_includeStock)
                        productWriter.WriteElementString("stock", srow["Stock"].ToString());

                    productWriter.WriteElementString("alertStock", srow["AlertStock"].ToString());
                    productWriter.WriteElementString("salePrice", srow["SalePrice"].ToString());
                    productWriter.WriteElementString("purchasePrice", srow["PurchasePrice"].ToString());

                    DataRow[] skrows = _exportData.Tables["skuItems"].Select("SkuId='" + srow["SkuId"].ToString() + "'");
                    productWriter.WriteStartElement("skuItems");

                    foreach (DataRow skrow in skrows)
                    {
                        productWriter.WriteStartElement("skuItem");
                        productWriter.WriteElementString("skuId", skrow["SkuId"].ToString());
                        productWriter.WriteElementString("attributeId", skrow["AttributeId"].ToString());
                        productWriter.WriteElementString("valueId", skrow["ValueId"].ToString());
                        productWriter.WriteEndElement();// end of skuItem
                    }

                    productWriter.WriteEndElement();// end of skuItems
                    productWriter.WriteEndElement();// end of sku
                }

                productWriter.WriteEndElement();// end of skus
                productWriter.WriteEndElement();// end of product

            }

            productWriter.WriteEndElement();// end of products

            _exportData.Tables.Remove("skuItems");
            _exportData.Tables.Remove("skus");
            _exportData.Tables.Remove("products");
            _exportData.Tables.Remove("productAttributes");
                
        }

    }
}