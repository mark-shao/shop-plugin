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

namespace Hishop.Transfers.YfxImporters
{
    public class Yfx1_2_from_Yfx1_2 : ImportAdapter
    {

        private const string IndexFilename = "indexes.xml";
        private const string ProductFilename = "products.xml";

        private readonly Target _importTo;
        private readonly Target _source;
        private readonly DirectoryInfo _baseDir;

        private DirectoryInfo _workDir, _typeImagesDir, _productImagesDir;

        public Yfx1_2_from_Yfx1_2()
        {
            _importTo = new YfxTarget("2.1");
            _source = new YfxTarget("2.1");
            _baseDir = new DirectoryInfo(HttpContext.Current.Request.MapPath("~/storage/data/yfx"));
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

        /// <summary>
        /// 创建商品类型映射
        /// </summary>
        /// <param name="initParams"></param>
        /// <returns></returns>
        public override object[] CreateMapping(params object[] initParams)
        {
            XmlDocument doc = (XmlDocument)initParams[0]; // initParams[0] - 管理员后台操作对应的xml文档
            string workDir = (string)initParams[1]; // initParams[1] - workDir

            DataSet mappingSet = GetMappingSet();
            XmlDocument indexesDoc = new XmlDocument();

            indexesDoc.Load(Path.Combine(workDir, IndexFilename));
            XmlNodeList typeNodeList = doc.DocumentElement.SelectNodes("//type");

            foreach(XmlNode typeNode in typeNodeList)
            {
                DataRow typeRow = mappingSet.Tables["types"].NewRow();
                int mappedTypeId = int.Parse(typeNode.Attributes["mappedTypeId"].Value);
                int selectedTypeId = int.Parse(typeNode.Attributes["selectedTypeId"].Value);

                typeRow["MappedTypeId"] = mappedTypeId;
                typeRow["SelectedTypeId"] = selectedTypeId;

                if (selectedTypeId==0)
                {
                    XmlNode indexTypeNode = indexesDoc.SelectSingleNode("//type[typeId[text()='" + mappedTypeId + "']]");
                    typeRow["TypeName"] = indexTypeNode.SelectSingleNode("typeName").InnerText;
                    typeRow["Remark"] = indexTypeNode.SelectSingleNode("remark").InnerText;
                }

                mappingSet.Tables["types"].Rows.Add(typeRow);
                
                XmlNodeList attributeNodeList = typeNode.SelectNodes("attributes/attribute");
                MappingAttributes(mappedTypeId, mappingSet, attributeNodeList, indexesDoc, workDir);
            }

            mappingSet.AcceptChanges();
            return new object[] {mappingSet};
        }

        private void MappingAttributes(int mappedTypeId, DataSet mappingSet, XmlNodeList attributeNodeList, XmlDocument indexesDoc, string workDir)
        {
            if (attributeNodeList == null || attributeNodeList.Count == 0)
                return;

            foreach(XmlNode attributeNode in attributeNodeList)
            {
                DataRow attributeRow = mappingSet.Tables["attributes"].NewRow();
                int mappedAttributeId = int.Parse(attributeNode.Attributes["mappedAttributeId"].Value);
                int selectedAttributeId = int.Parse(attributeNode.Attributes["selectedAttributeId"].Value);

                attributeRow["MappedAttributeId"] = mappedAttributeId;
                attributeRow["SelectedAttributeId"] = selectedAttributeId;
                attributeRow["MappedTypeId"] = mappedTypeId;

                if (selectedAttributeId==0)
                {
                    XmlNode indexAttNode = indexesDoc.SelectSingleNode("//attribute[attributeId[text()='" + mappedAttributeId + "']]");
                    attributeRow["AttributeName"] = indexAttNode.SelectSingleNode("attributeName").InnerText;
                    attributeRow["DisplaySequence"] = indexAttNode.SelectSingleNode("displaySequence").InnerText;
                    attributeRow["UsageMode"] = indexAttNode.SelectSingleNode("usageMode").InnerText;
                    attributeRow["UseAttributeImage"] = indexAttNode.SelectSingleNode("useAttributeImage").InnerText;
                }

                mappingSet.Tables["attributes"].Rows.Add(attributeRow);

                XmlNodeList valueNodeList = attributeNode.SelectNodes("values/value");
                MappingValues(mappedAttributeId, selectedAttributeId, mappingSet, valueNodeList, indexesDoc, workDir);
            }
        }

        private void MappingValues(int mappedAttributeId, int selectedAttributeId, DataSet mappingSet, XmlNodeList valueNodeList, XmlDocument indexesDoc, string workDir)
        {
            if (valueNodeList == null || valueNodeList.Count == 0)
                return;

            const string virtualPath = "/Storage/master/sku/";

            foreach(XmlNode valueNode in valueNodeList)
            {
                DataRow valueRow = mappingSet.Tables["values"].NewRow();
                int mappedValueId = int.Parse(valueNode.Attributes["mappedValueId"].Value);
                int selectedValueId = int.Parse(valueNode.Attributes["selectedValueId"].Value);

                valueRow["MappedValueId"] = mappedValueId;
                valueRow["SelectedValueId"] = selectedValueId;
                valueRow["MappedAttributeId"] = mappedAttributeId;
                valueRow["SelectedAttributeId"] = selectedAttributeId;

                if (selectedValueId == 0)
                {
                    XmlNode indexValueNode = indexesDoc.SelectSingleNode("//value[valueId[text()='" + mappedValueId + "']]");
                    valueRow["DisplaySequence"] = indexValueNode.SelectSingleNode("displaySequence").InnerText;
                    valueRow["ValueStr"] = indexValueNode.SelectSingleNode("valueStr").InnerText;
                    string imageUrl =  indexValueNode.SelectSingleNode("image").InnerText;

                    if (imageUrl.Length > 0 && File.Exists(Path.Combine(workDir + "\\images1", imageUrl)))
                    {
                        File.Copy(Path.Combine(workDir + "\\images1", imageUrl),
                                  HttpContext.Current.Request.MapPath("~" + virtualPath + imageUrl), true);
                        valueRow["ImageUrl"] = virtualPath + imageUrl;
                    }
                }

                mappingSet.Tables["values"].Rows.Add(valueRow);
            }
        }

        private DataSet GetMappingSet()
        {
            DataSet mappingSet = new DataSet();

            DataTable dtType = new DataTable("types");
            dtType.Columns.Add(new DataColumn("MappedTypeId") { Unique = true, DataType = Type.GetType("System.Int32") });
            dtType.Columns.Add(new DataColumn("SelectedTypeId") {DataType = Type.GetType("System.Int32")});
            dtType.Columns.Add(new DataColumn("TypeName"));
            dtType.Columns.Add(new DataColumn("Remark"));
            dtType.PrimaryKey = new DataColumn[] {dtType.Columns["MappedTypeId"]};

            DataTable dtAttributes = new DataTable("attributes");
            dtAttributes.Columns.Add(new DataColumn("MappedAttributeId") { Unique = true, DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("SelectedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("AttributeName"));
            dtAttributes.Columns.Add(new DataColumn("DisplaySequence"));
            dtAttributes.Columns.Add(new DataColumn("MappedTypeId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("UsageMode"));
            dtAttributes.Columns.Add(new DataColumn("UseAttributeImage"));
            dtAttributes.PrimaryKey = new DataColumn[] {dtAttributes.Columns["MappedAttributeId"]};

            DataTable dtValues = new DataTable("values");
            dtValues.Columns.Add(new DataColumn("MappedValueId") { Unique = true, DataType = Type.GetType("System.Int32") });
            dtValues.Columns.Add(new DataColumn("SelectedValueId") { DataType = Type.GetType("System.Int32") });
            dtValues.Columns.Add(new DataColumn("MappedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtValues.Columns.Add(new DataColumn("SelectedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtValues.Columns.Add(new DataColumn("DisplaySequence"));
            dtValues.Columns.Add(new DataColumn("ValueStr"));
            dtValues.Columns.Add(new DataColumn("ImageUrl"));

            mappingSet.Tables.Add(dtType);
            mappingSet.Tables.Add(dtAttributes);
            mappingSet.Tables.Add(dtValues);

            return mappingSet;
        }

        public override object[] ParseIndexes(params object[] importParams)
        {
            string workPath = (string)importParams[0];//importParams[0] - 工作目录
            //string dir = (string) importParams[1];

            if (!Directory.Exists(workPath))
            {
                throw new DirectoryNotFoundException("directory:" + workPath + " does not found");
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(workPath, IndexFilename));
            XmlNode root = doc.DocumentElement.SelectSingleNode("/indexes");

            string version = root.Attributes["version"].Value;
            int qty = int.Parse(root.Attributes["QTY"].Value);
            bool includeCostPrice = bool.Parse(root.Attributes["includeCostPrice"].Value);
            bool includeStock = bool.Parse(root.Attributes["includeStock"].Value);
            bool includeImages = bool.Parse(root.Attributes["includeImages"].Value);

            //XmlNodeList imageNodeList = doc.DocumentElement.SelectNodes("//values/value/image[text() is not null]]");
            //if (imageNodeList != null && imageNodeList.Count >0)
            //{
            //    foreach(XmlNode imageNode in imageNodeList)
            //    {
            //        if(includeImages)
            //        {
            //            imageNode.InnerText = "/storage/data/yfx/" + dir + "/images1/" + imageNode.InnerText;
            //        }
            //    }
            //}
            string ptXmlStr = "<xml>" + root.OuterXml + "</xml>";

            return new object[] { version, qty, includeCostPrice, includeStock, includeImages, ptXmlStr };
        }

        public override object[] ParseProductData(params object[] importParams)
        {
            DataSet mappingSet = (DataSet)importParams[0];// importParams[0] - 商品类型映射
            string workDir = (string)importParams[1]; // initParams[1] - workDir
            bool includeCostPrice = (bool)importParams[2]; // initParams[2] - includeCostPrice
            bool includeStock = (bool)importParams[3]; // initParams[3] - includeStock
            bool includeImages = (bool)importParams[4]; // initParams[4] - includeImages

            const string virtualPath = "/Storage/master/product/images/";
            HttpContext context = HttpContext.Current;

            DataSet productSet = GetProductSet();
            XmlDocument productDoc = new XmlDocument();
            productDoc.Load(Path.Combine(workDir, ProductFilename));

            XmlNodeList productNodeList = productDoc.DocumentElement.SelectNodes("//product");
            foreach (XmlNode productNode in productNodeList)
            {
                DataRow productRow = productSet.Tables["products"].NewRow();

                int productId = int.Parse(productNode.SelectSingleNode("productId").InnerText);
                int mappedTypeId = 0, selectedTypeId = 0;

                if (productNode.SelectSingleNode("typeId").InnerText.Length > 0)
                {
                    mappedTypeId = int.Parse(productNode.SelectSingleNode("typeId").InnerText);
                    if (mappedTypeId != 0)
                        selectedTypeId = (int)mappingSet.Tables["types"].Select("MappedTypeId=" + mappedTypeId.ToString(CultureInfo.InvariantCulture))[0]["SelectedTypeId"];
                }

                bool hasSku = bool.Parse(productNode.SelectSingleNode("hasSKU").InnerText);

                productRow["ProductId"] = productId;
                productRow["SelectedTypeId"] = selectedTypeId;
                productRow["MappedTypeId"] = mappedTypeId;
                productRow["ProductName"] = productNode.SelectSingleNode("productName").InnerText;
                productRow["ProductCode"] = productNode.SelectSingleNode("productCode").InnerText;
                productRow["ShortDescription"] = productNode.SelectSingleNode("shortDescription").InnerText;
                productRow["Unit"] = productNode.SelectSingleNode("unit").InnerText;
                productRow["Description"] = productNode.SelectSingleNode("description").InnerText;
                productRow["Title"] = productNode.SelectSingleNode("title").InnerText;
                productRow["Meta_Description"] = productNode.SelectSingleNode("meta_Description").InnerText;
                productRow["Meta_Keywords"] = productNode.SelectSingleNode("meta_Keywords").InnerText;
                productRow["SaleStatus"] = int.Parse(productNode.SelectSingleNode("saleStatus").InnerText);

                string imageUrl1 = string.Empty;
                string imageUrl2 = string.Empty;
                string imageUrl3 = string.Empty;
                string imageUrl4 = string.Empty;
                string imageUrl5 = string.Empty;

                if (productNode.SelectSingleNode("image1") != null)
                    imageUrl1 = productNode.SelectSingleNode("image1").InnerText;

                if (productNode.SelectSingleNode("image2") != null)
                    imageUrl2 = productNode.SelectSingleNode("image2").InnerText;

                if (productNode.SelectSingleNode("image3") != null)
                    imageUrl3 = productNode.SelectSingleNode("image3").InnerText;

                if (productNode.SelectSingleNode("image4") != null)
                    imageUrl4 = productNode.SelectSingleNode("image4").InnerText;

                if (productNode.SelectSingleNode("image5") != null)
                    imageUrl5 = productNode.SelectSingleNode("image5").InnerText;

                productRow["ImageUrl1"] = imageUrl1;
                productRow["ImageUrl2"] = imageUrl2;
                productRow["ImageUrl3"] = imageUrl3;
                productRow["ImageUrl4"] = imageUrl4;
                productRow["ImageUrl5"] = imageUrl5;

                if (includeImages)
                {
                    if (imageUrl1.Length > 0 && File.Exists(Path.Combine(workDir+"\\images2",imageUrl1)))
                    {
                        File.Copy(Path.Combine(workDir + "\\images2", imageUrl1),
                                  context.Request.MapPath("~" + virtualPath + imageUrl1), true);
                        productRow["ImageUrl1"] = virtualPath + imageUrl1;
                    }

                    if (imageUrl2.Length > 0 && File.Exists(Path.Combine(workDir + "\\images2", imageUrl2)))
                    {
                        File.Copy(Path.Combine(workDir + "\\images2", imageUrl2),
                                  context.Request.MapPath("~" + virtualPath + imageUrl2), true);
                        productRow["ImageUrl2"] = virtualPath + imageUrl2;
                    }

                    if (imageUrl3.Length > 0 && File.Exists(Path.Combine(workDir + "\\images2", imageUrl3)))
                    {
                        File.Copy(Path.Combine(workDir + "\\images2", imageUrl3),
                                  context.Request.MapPath("~" + virtualPath + imageUrl3), true);
                        productRow["ImageUrl3"] = virtualPath + imageUrl3;
                    }

                    if (imageUrl4.Length > 0 && File.Exists(Path.Combine(workDir + "\\images2", imageUrl4)))
                    {
                        File.Copy(Path.Combine(workDir + "\\images2", imageUrl4),
                                  context.Request.MapPath("~" + virtualPath + imageUrl4), true);
                        productRow["ImageUrl4"] = virtualPath + imageUrl4;
                    }

                    if (imageUrl5.Length > 0 && File.Exists(Path.Combine(workDir + "\\images2", imageUrl5)))
                    {
                        File.Copy(Path.Combine(workDir + "\\images2", imageUrl5),
                                  context.Request.MapPath("~" + virtualPath + imageUrl5), true);
                        productRow["ImageUrl5"] = virtualPath + imageUrl5;
                    }
                }

                if (productNode.SelectSingleNode("marketPrice").InnerText.Length > 0)
                    productRow["MarketPrice"] = decimal.Parse(productNode.SelectSingleNode("marketPrice").InnerText);

                productRow["LowestSalePrice"] = decimal.Parse(productNode.SelectSingleNode("lowestSalePrice").InnerText);
                productRow["PenetrationStatus"] = int.Parse(productNode.SelectSingleNode("penetrationStatus").InnerText);
                productRow["HasSKU"] = hasSku;

                productSet.Tables["products"].Rows.Add(productRow);

                XmlNodeList attributeNodeList = productNode.SelectNodes("attributes/attribute");
                loadProductAttributes(productId, attributeNodeList, productSet, mappingSet);

                XmlNodeList valueNodeList = productNode.SelectNodes("skus/sku");
                loadProductSkus(productId, hasSku, valueNodeList, productSet, mappingSet, includeCostPrice, includeStock);
            }

            return new object[] {productSet};
        }

        private void loadProductSkus(int productId, bool hasSku, XmlNodeList valueNodeList, DataSet productSet, DataSet mappingSet, bool includeCostPrice, bool includeStock)
        {
            if (valueNodeList == null || valueNodeList.Count == 0)
                return;

            foreach(XmlNode valueNode in valueNodeList)
            {
                DataRow valueRow = productSet.Tables["skus"].NewRow();
                string mappedSkuId = valueNode.SelectSingleNode("skuId").InnerText;

                valueRow["MappedSkuId"] = mappedSkuId;
                valueRow["ProductId"] = productId;
                valueRow["SKU"] = valueNode.SelectSingleNode("sKU").InnerText;

                if (valueNode.SelectSingleNode("weight").InnerText.Length > 0)
                    valueRow["Weight"] = decimal.Parse(valueNode.SelectSingleNode("weight").InnerText);

                if (includeStock)
                    valueRow["Stock"] = int.Parse(valueNode.SelectSingleNode("stock").InnerText);

                valueRow["AlertStock"] = valueNode.SelectSingleNode("alertStock").InnerText;

                if (includeCostPrice && valueNode.SelectSingleNode("costPrice").InnerText.Length > 0)
                    valueRow["CostPrice"] = valueNode.SelectSingleNode("costPrice").InnerText;

                valueRow["SalePrice"] = valueNode.SelectSingleNode("salePrice").InnerText;
                valueRow["PurchasePrice"] = valueNode.SelectSingleNode("purchasePrice").InnerText;

                XmlNodeList itemNodeList = valueNode.SelectNodes("skuItems/skuItem");
                string newSkuId = loadSkuItems(mappedSkuId, productId, itemNodeList, productSet, mappingSet);

                valueRow["NewSkuId"] = hasSku ? newSkuId : "0";
                productSet.Tables["skus"].Rows.Add(valueRow);
            }
        }

        private string loadSkuItems(string mappedSkuId, int mappedProductId, XmlNodeList itemNodeList, DataSet productSet, DataSet mappingSet)
        {
            if (itemNodeList == null || itemNodeList.Count == 0)
                return "0";

            string newSkuId = "";
            foreach (XmlNode itemNode in itemNodeList)
            {
                newSkuId += mappingSet.Tables["values"].Select(
                    "MappedValueId=" + itemNode.SelectSingleNode("valueId").InnerText)[0]["SelectedValueId"].ToString() + "_";
            }

            newSkuId = newSkuId.Substring(0, newSkuId.Length - 1);

            foreach(XmlNode itemNode in itemNodeList)
            {
                int mappedAttributeId = int.Parse(itemNode.SelectSingleNode("attributeId").InnerText);
                int mappedValueId = int.Parse(itemNode.SelectSingleNode("valueId").InnerText);

                DataRow[] mappedAttrs = mappingSet.Tables["attributes"].Select("MappedAttributeId=" + mappedAttributeId);
                DataRow[] mappedValues = mappingSet.Tables["values"].Select("MappedValueId=" + mappedValueId);

                //当商品中的规格属性在index.xml文件中存在时进行添加
                if (mappedAttrs != null && mappedAttrs.Length > 0 && mappedValues != null && mappedValues.Length > 0)
                {
                    int selectedAttributeId = (int)mappedAttrs[0]["SelectedAttributeId"];
                    int selectedValueId = (int)mappedValues[0]["SelectedValueId"];

                    DataRow itemRow = productSet.Tables["skuItems"].NewRow();

                    itemRow["MappedProductId"] = mappedProductId;
                    itemRow["NewSkuId"] = newSkuId;
                    itemRow["MappedSkuId"] = mappedSkuId;
                    itemRow["SelectedAttributeId"] = selectedAttributeId;
                    itemRow["MappedAttributeId"] = mappedAttributeId;
                    itemRow["SelectedValueId"] = selectedValueId;
                    itemRow["MappedValueId"] = mappedValueId;

                    productSet.Tables["skuItems"].Rows.Add(itemRow);
                }
            }

            return newSkuId;
        }

        private void loadProductAttributes(int productId, XmlNodeList attributeNodeList, DataSet productSet, DataSet mappingSet)
        {
            if (attributeNodeList == null || attributeNodeList.Count == 0)
                return;

            foreach(XmlNode attributeNode in attributeNodeList)
            {
                int mappedAttributeId = int.Parse(attributeNode.SelectSingleNode("attributeId").InnerText);
                int mappedValueId = int.Parse(attributeNode.SelectSingleNode("valueId").InnerText);

                DataRow[] mappedAttrs = mappingSet.Tables["attributes"].Select("MappedAttributeId=" + mappedAttributeId);
                DataRow[] mappedValues = mappingSet.Tables["values"].Select("MappedValueId=" + mappedValueId);

                //当商品中的扩展属性在index.xml文件中存在时进行添加
                if (mappedAttrs != null && mappedAttrs.Length > 0 && mappedValues != null && mappedValues.Length > 0)
                {
                    int selectedAttributeId = (int)mappedAttrs[0]["SelectedAttributeId"];
                    int selectedValueId = (int)mappedValues[0]["SelectedValueId"];

                    DataRow attributeRow = productSet.Tables["attributes"].NewRow();

                    attributeRow["ProductId"] = productId;
                    attributeRow["SelectedAttributeId"] = selectedAttributeId;
                    attributeRow["MappedAttributeId"] = mappedAttributeId;
                    attributeRow["SelectedValueId"] = selectedValueId;
                    attributeRow["MappedValueId"] = mappedValueId;

                    productSet.Tables["attributes"].Rows.Add(attributeRow);
                }
            }
        }

        private DataSet GetProductSet()
        {
            DataSet productSet = new DataSet();

            DataTable dtProducts = new DataTable("products");
            dtProducts.Columns.Add(new DataColumn("ProductId") { Unique = true, DataType = Type.GetType("System.Int32") });
            dtProducts.Columns.Add(new DataColumn("SelectedTypeId") { DataType = Type.GetType("System.Int32") });
            dtProducts.Columns.Add(new DataColumn("MappedTypeId") { DataType = Type.GetType("System.Int32") });
            dtProducts.Columns.Add(new DataColumn("ProductName") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("ProductCode") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("ShortDescription") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("Unit") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("Description") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("Title") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("Meta_Description") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("Meta_Keywords") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("SaleStatus") { DataType = Type.GetType("System.Int32") });
            dtProducts.Columns.Add(new DataColumn("ImageUrl1") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("ImageUrl2") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("ImageUrl3") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("ImageUrl4") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("ImageUrl5") { DataType = Type.GetType("System.String") });
            dtProducts.Columns.Add(new DataColumn("MarketPrice") { DataType = Type.GetType("System.Decimal") });
            dtProducts.Columns.Add(new DataColumn("LowestSalePrice") { DataType = Type.GetType("System.Decimal") });
            dtProducts.Columns.Add(new DataColumn("PenetrationStatus") { DataType = Type.GetType("System.Int32") });
            dtProducts.Columns.Add(new DataColumn("HasSKU") { DataType = Type.GetType("System.Boolean") });
            dtProducts.PrimaryKey = new DataColumn[] { dtProducts.Columns["ProductId"] };

            DataTable dtAttributes = new DataTable("attributes");
            dtAttributes.Columns.Add(new DataColumn("ProductId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("SelectedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("MappedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("SelectedValueId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("MappedValueId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.PrimaryKey = new DataColumn[]
                                          {
                                              dtAttributes.Columns["ProductId"],
                                              dtAttributes.Columns["MappedAttributeId"],
                                              dtAttributes.Columns["MappedValueId"]
                                          };

            DataTable dtSkus = new DataTable("skus");
            dtSkus.Columns.Add(new DataColumn("MappedSkuId") { Unique = true, DataType = Type.GetType("System.String") });
            dtSkus.Columns.Add(new DataColumn("NewSkuId") { DataType = Type.GetType("System.String") });
            dtSkus.Columns.Add(new DataColumn("ProductId") { DataType = Type.GetType("System.Int32") });
            dtSkus.Columns.Add(new DataColumn("SKU") { DataType = Type.GetType("System.String") });
            dtSkus.Columns.Add(new DataColumn("Weight") { DataType = Type.GetType("System.Decimal") });
            dtSkus.Columns.Add(new DataColumn("Stock") { DataType = Type.GetType("System.Int32") });
            dtSkus.Columns.Add(new DataColumn("AlertStock") { DataType = Type.GetType("System.Int32") });
            dtSkus.Columns.Add(new DataColumn("CostPrice") { DataType = Type.GetType("System.Decimal") });
            dtSkus.Columns.Add(new DataColumn("SalePrice") { DataType = Type.GetType("System.Decimal") });
            dtSkus.Columns.Add(new DataColumn("PurchasePrice") { DataType = Type.GetType("System.Decimal") });
            dtSkus.PrimaryKey = new DataColumn[] { dtSkus.Columns["MappedSkuId"] };

            DataTable dtSkuItems = new DataTable("skuItems");
            dtSkuItems.Columns.Add(new DataColumn("MappedSkuId") { DataType = Type.GetType("System.String") });
            dtSkuItems.Columns.Add(new DataColumn("NewSkuId") { DataType = Type.GetType("System.String") });
            dtSkuItems.Columns.Add(new DataColumn("MappedProductId") { DataType = Type.GetType("System.Int32") });
            dtSkuItems.Columns.Add(new DataColumn("SelectedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtSkuItems.Columns.Add(new DataColumn("MappedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtSkuItems.Columns.Add(new DataColumn("SelectedValueId") { DataType = Type.GetType("System.Int32") });
            dtSkuItems.Columns.Add(new DataColumn("MappedValueId") { DataType = Type.GetType("System.Int32") });
            dtSkuItems.PrimaryKey = new DataColumn[]
                                          {
                                              dtSkuItems.Columns["MappedSkuId"],
                                              dtSkuItems.Columns["MappedAttributeId"]
                                          };

            productSet.Tables.Add(dtProducts);
            productSet.Tables.Add(dtAttributes);
            productSet.Tables.Add(dtSkus);
            productSet.Tables.Add(dtSkuItems);

            return productSet;
        }

        public override Target Source
        {
            get { return _source; }
        }

        public override Target ImportTo
        {
            get { return _importTo; }
        }

    }
}