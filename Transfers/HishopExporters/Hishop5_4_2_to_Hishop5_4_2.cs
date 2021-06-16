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

namespace Transfers.HishopExporters
{
    public class Hishop5_4_2_to_Hishop5_4_2 : ExportAdapter
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
        private readonly string _flag;

        private DirectoryInfo _workDir, _typeImagesDir, _productImagesDir;

        public Hishop5_4_2_to_Hishop5_4_2()
        {
            _exportTo = new HishopTarget(ExportVersion);
            _source = new HishopTarget("5.4.2");
        }

        public Hishop5_4_2_to_Hishop5_4_2(params object[] exportParams)
            : this()
        {
            _exportData = (DataSet)exportParams[0]; // exportParams[0] - DataSet exportData
            _includeCostPrice = (bool)exportParams[1]; // exportParams[1] - bool includeCostPrice
            _includeStock = (bool)exportParams[2]; // exportParams[2] - bool includeStock
            _includeImages = (bool)exportParams[3]; // exportParams[3] - bool includeImages
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

            DataSet skuData = ReadySkuData();
            foreach (DataRow trow in _exportData.Tables["types"].Rows)
            {
                indexWriter.WriteStartElement("type");
                indexWriter.WriteElementString("typeId", trow["TypeId"].ToString());
                TransferHelper.WriteCDataElement(indexWriter, "typeName", trow["TypeName"].ToString());
                TransferHelper.WriteCDataElement(indexWriter, "remark", trow["Remark"].ToString());

                indexWriter.WriteStartElement("attributes");
                //输出非规格属性
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

                //输出规格属性
                DataRow[] attrRows = skuData.Tables["attributes"].Select("typeId='" + trow["TypeId"].ToString() + "'", "attributeId asc");
                foreach (DataRow attrRow in attrRows)
                {
                    IList<string> skuValues = new List<string>();
                    indexWriter.WriteStartElement("attribute");
                    indexWriter.WriteElementString("attributeId", attrRow["AttributeId"].ToString());
                    TransferHelper.WriteCDataElement(indexWriter, "attributeName", attrRow["AttributeName"].ToString());
                    indexWriter.WriteElementString("displaySequence", attrRow["DisplaySequence"].ToString());
                    indexWriter.WriteElementString("usageMode", attrRow["UsageMode"].ToString());

                    indexWriter.WriteStartElement("values");
                    DataRow[] vrows =
                        skuData.Tables["values"].Select("AttributeId=" + attrRow["AttributeId"].ToString());
                    foreach (DataRow vrow in vrows)
                    {
                        if (!skuValues.Contains(vrow["ValueStr"].ToString().Trim()))
                        {
                            indexWriter.WriteStartElement("value");
                            TransferHelper.WriteCDataElement(indexWriter, "valueStr", vrow["ValueStr"].ToString());
                            indexWriter.WriteEndElement();// end of value
                            skuValues.Add(vrow["ValueStr"].ToString().Trim());
                        }
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
                TransferHelper.WriteCDataElement(productWriter, "SKU", prow["SKU"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "shortDescription", prow["ShortDescription"].ToString());
                productWriter.WriteElementString("unit", prow["Unit"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "description", prow["Description"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "title", prow["Title"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "meta_Description", prow["Meta_Description"].ToString());
                TransferHelper.WriteCDataElement(productWriter, "meta_Keywords", prow["Meta_Keywords"].ToString());
                productWriter.WriteElementString("Upselling", (bool)prow["Upselling"] ? "1" : "0");
                productWriter.WriteElementString("marketPrice", string.IsNullOrEmpty(prow["MarketPrice"].ToString()) ? "0" : prow["MarketPrice"].ToString());

                //获得该商品的规格DataRow
                DataRow[] srows = _exportData.Tables["skus"].Select("ProductId=" + prow["ProductId"].ToString(), "Price desc");
                //DataRow srow1 = srows[0]; //第一行数据规格

                productWriter.WriteElementString("Weight", string.IsNullOrEmpty(prow["Weight"].ToString()) ? "0" : prow["Weight"].ToString());
                if (_includeCostPrice)
                    productWriter.WriteElementString("CostPrice", string.IsNullOrEmpty(prow["CostPrice"].ToString()) ? "0" : prow["CostPrice"].ToString());
                productWriter.WriteElementString("SalePrice", prow["SalePrice"].ToString());

                productWriter.WriteStartElement("skus");
                int sum = 0;
                foreach (DataRow srow in srows)
                {
                    string atts = (string)srow["AttributeNames"];
                    string vals = (string)srow["AttributeValues"];
                    int count = 0;
                    if (!string.IsNullOrEmpty(atts))
                    {
                        string[] attrNames = atts.Split(',');
                        string[] attrValues = vals.Split(',');
                        for (int i = 0; i < attrNames.Length; i++)
                        {
                            int typeId = 0;
                            int.TryParse(prow["typeId"].ToString(), out typeId);

                            DataRow[] attrows = _exportData.Tables["skuItems"].Select("TypeId=" + typeId + " AND attributeName='" + attrNames[i].ToString() + "' AND valueStr='" + attrValues[i].ToString() + "'");
                            if (attrows.Length > 0)
                            {
                                count++;
                            }
                        }
                    }
                    //此规格的所有规格属性都不是手写的
                    if (atts.Split(',').Length == count)
                    {
                        productWriter.WriteStartElement("sku");
                        productWriter.WriteElementString("skuId", srow["SKU"].ToString());
                        productWriter.WriteElementString("SKU", srow["SKU"].ToString());

                        if (_includeStock)
                            productWriter.WriteElementString("stock", srow["Stock"].ToString());
                        productWriter.WriteElementString("Price", srow["Price"].ToString());

                        productWriter.WriteStartElement("skuItems");

                        if (!string.IsNullOrEmpty(atts))
                        {
                            string[] attrNames = atts.Split(',');
                            string[] attrValues = vals.Split(',');
                            for (int i = 0; i < attrNames.Length; i++)
                            {
                                int typeId = 0;
                                int.TryParse(prow["typeId"].ToString(), out typeId);
                                DataRow[] attrows = _exportData.Tables["skuItems"].Select("TypeId=" + typeId + " AND attributeName='" + attrNames[i].ToString() + "' AND valueStr='" + attrValues[i].ToString() + "'");
                                if (attrows.Length > 0)
                                {
                                    productWriter.WriteStartElement("skuItem");
                                    productWriter.WriteElementString("skuId", srow["SKU"].ToString());
                                    productWriter.WriteElementString("attributeId", attrows[0]["AttributeId"].ToString());
                                    productWriter.WriteElementString("attributeName", attrows[0]["AttributeName"].ToString());
                                    productWriter.WriteElementString("ValueStr", attrows[0]["ValueStr"].ToString());
                                    productWriter.WriteEndElement();// end of skuItem
                                }
                            }
                        }
                        productWriter.WriteEndElement();// end of skuItems
                        productWriter.WriteEndElement();// end of sku
                        sum++;
                    }

                }
                if (sum == 0)//如果 没有规格，添加一个默认空规格
                {
                    productWriter.WriteStartElement("sku");
                    productWriter.WriteElementString("skuId", prow["SKU"].ToString());
                    productWriter.WriteElementString("SKU", prow["SKU"].ToString());

                    if (_includeStock)
                        productWriter.WriteElementString("stock", "100");
                    productWriter.WriteElementString("Price", "0");

                    productWriter.WriteStartElement("skuItems");
                    productWriter.WriteEndElement();// end of skuItems
                    productWriter.WriteEndElement();// end of sku
                }

                productWriter.WriteEndElement();// end of skus

                DataRow[] parows = _exportData.Tables["productAttributes"].Select("ProductId=" + prow["ProductId"].ToString());
                productWriter.WriteStartElement("productAttributes");
                foreach (DataRow paRow in parows)
                {
                    int typeId = 0;
                    int.TryParse(prow["typeId"].ToString(), out typeId);
                    int attributeId = 0;
                    int.TryParse(paRow["attributeId"].ToString(), out attributeId);
                    DataRow[] writeAttrows = _exportData.Tables["skuItems"].Select("TypeId=" + typeId + " AND attributeId=" +
                        attributeId + " AND valueStr='" + paRow["valueStr"] + "'");
                    if (writeAttrows.Length > 0)
                    {
                        productWriter.WriteStartElement("productAttribute");

                        productWriter.WriteElementString("AttributeId", paRow["AttributeId"].ToString());
                        TransferHelper.WriteCDataElement(productWriter, "ValueStr", paRow["ValueStr"].ToString());

                        productWriter.WriteEndElement();// end of productAttribute
                    }
                }
                productWriter.WriteEndElement();// end of productAttributes

                productWriter.WriteStartElement("images");
                DataRow[] imgrows = _exportData.Tables["ProductImages"].Select("ProductId=" + prow["ProductId"].ToString());
                foreach (DataRow imgRow in imgrows)
                {
                    TransferHelper.WriteImageElement(productWriter, "image", _includeImages, imgRow["ImageUrl"].ToString(), _productImagesDir);
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

        DataSet ReadySkuData()
        {
            DataSet skuData = GetSkuDataSet();
            IList<int> skuList = new List<int>();
            foreach (DataRow trow in _exportData.Tables["types"].Rows)
            {
                DataRow[] proRows = _exportData.Tables["products"].Select("typeId='" + trow["TypeId"].ToString() + "'");
                foreach (DataRow proRow in proRows)
                {
                    DataRow[] skuRows = _exportData.Tables["skus"].Select("ProductId=" + proRow["productId"]);
                    foreach (DataRow skuRow in skuRows)
                    {
                        string atts = (string)skuRow["AttributeNames"];
                        string vals = (string)skuRow["AttributeValues"];

                        if (!string.IsNullOrEmpty(atts))
                        {
                            string[] attrNames = atts.Split(',');
                            string[] attrValues = vals.Split(',');
                            for (int i = 0; i < attrNames.Length; i++)
                            {
                                DataRow[] attributerows = _exportData.Tables["skuItems"].Select("typeId=" + trow["TypeId"].ToString() + " AND attributeName='"
                                    + attrNames[i].ToString() + "' AND valueStr='" + attrValues[i].ToString() + "' AND UsageMode=2", " attributeId asc");
                                if (attributerows.Length > 0)
                                {
                                    if (!skuList.Contains((int)attributerows[0]["attributeId"]))
                                    {
                                        DataRow attrRow = skuData.Tables["attributes"].NewRow();
                                        attrRow["attributeName"] = attrNames[i];
                                        attrRow["attributeId"] = attributerows[0]["attributeId"];
                                        attrRow["DisplaySequence"] = attributerows[0]["DisplaySequence"];
                                        attrRow["TypeId"] = trow["TypeId"];
                                        attrRow["UsageMode"] = 2;
                                        skuData.Tables["attributes"].Rows.Add(attrRow);
                                        skuList.Add((int)attributerows[0]["attributeId"]);
                                    }
                                    DataRow valueRow = skuData.Tables["values"].NewRow();
                                    valueRow["attributeId"] = attributerows[0]["attributeId"];
                                    valueRow["ValueStr"] = attrValues[i];
                                    skuData.Tables["values"].Rows.Add(valueRow);

                                }
                            }
                        }
                    }
                }
            }
            return skuData;
        }

        private DataSet GetSkuDataSet()
        {
            DataSet mappingSet = new DataSet();

            DataTable dtAttributes = new DataTable("attributes");
            dtAttributes.Columns.Add(new DataColumn("AttributeId") { Unique = true, DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("AttributeName") { DataType = Type.GetType("System.String") });
            dtAttributes.Columns.Add(new DataColumn("DisplaySequence") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("TypeId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("UsageMode") { DataType = Type.GetType("System.Int32") });
            dtAttributes.PrimaryKey = new DataColumn[] { dtAttributes.Columns["AttributeId"] };

            DataTable dtValues = new DataTable("values");
            dtValues.Columns.Add(new DataColumn("AttributeId") { DataType = Type.GetType("System.Int32") });
            dtValues.Columns.Add(new DataColumn("DisplaySequence") { DataType = Type.GetType("System.Int32") });
            dtValues.Columns.Add(new DataColumn("ValueStr") { DataType = Type.GetType("System.String") });

            mappingSet.Tables.Add(dtAttributes);
            mappingSet.Tables.Add(dtValues);

            return mappingSet;
        }
    }
}
