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

namespace Transfers.HishopImporters
{
    public class Hishop5_4_2_from_Hishop5_4_2 : ImportAdapter
    {
        private const string IndexFilename = "indexes.xml";
        private const string ProductFilename = "products.xml";

        private readonly Target _importTo;
        private readonly Target _source;
        private readonly DirectoryInfo _baseDir;

        private DirectoryInfo _workDir;

        public Hishop5_4_2_from_Hishop5_4_2()
        {
            _importTo = new HishopTarget("5.4.2");
            _source = new HishopTarget("5.4.2");
            _baseDir = new DirectoryInfo(HttpContext.Current.Request.MapPath("~/storage/data/hishop"));
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
                MappingAttributes(mappedTypeId, mappingSet, attributeNodeList, indexesDoc);
            }

            mappingSet.AcceptChanges();
            return new object[] {mappingSet};
        }

        private void MappingAttributes(int mappedTypeId, DataSet mappingSet, XmlNodeList attributeNodeList, XmlDocument indexesDoc)
        {
            if (attributeNodeList == null || attributeNodeList.Count == 0)
                return;

            foreach(XmlNode attributeNode in attributeNodeList)
            {
                DataRow attributeRow = mappingSet.Tables["attributes"].NewRow();
                int mappedAttributeId = int.Parse(attributeNode.Attributes["mappedAttributeId"].Value);
                int selectedAttributeId = int.Parse(attributeNode.Attributes["selectedAttributeId"].Value);
                string selectAttributeName = attributeNode.Attributes["selectAttributeName"].Value;

                attributeRow["MappedAttributeId"] = mappedAttributeId;
                attributeRow["SelectedAttributeId"] = selectedAttributeId;
                attributeRow["MappedTypeId"] = mappedTypeId;
                attributeRow["AttributeName"] = selectAttributeName;
                if (selectedAttributeId==0)
                {
                    XmlNode indexAttNode = indexesDoc.SelectSingleNode("//attribute[attributeId[text()='" + mappedAttributeId + "']]");
                    attributeRow["AttributeName"] = indexAttNode.SelectSingleNode("attributeName").InnerText;
                    attributeRow["DisplaySequence"] = indexAttNode.SelectSingleNode("displaySequence").InnerText;
                    attributeRow["UsageMode"] = indexAttNode.SelectSingleNode("usageMode").InnerText;
                }

                mappingSet.Tables["attributes"].Rows.Add(attributeRow);

                XmlNodeList valueNodeList = attributeNode.SelectNodes("values/value");
                MappingValues(mappedAttributeId, selectedAttributeId, mappingSet, valueNodeList, indexesDoc);
            }
        }

        private void MappingValues(int mappedAttributeId, int selectedAttributeId, DataSet mappingSet, XmlNodeList valueNodeList, XmlDocument indexesDoc)
        {
            if (valueNodeList == null || valueNodeList.Count == 0)
                return;

            foreach(XmlNode valueNode in valueNodeList)
            {
                DataRow valueRow = mappingSet.Tables["values"].NewRow();
                string mappedValueId = valueNode.Attributes["mappedValueId"].Value;
                string selectedValueId = valueNode.Attributes["selectedValueId"].Value;

                valueRow["MappedValueId"] = mappedValueId;
                valueRow["SelectedValueId"] = selectedValueId;
                valueRow["MappedAttributeId"] = mappedAttributeId;
                valueRow["SelectedAttributeId"] = selectedAttributeId;

                if (selectedValueId == "0")
                {
                    XmlNode indexValueNode = indexesDoc.SelectSingleNode("//value[valueStr[text()='" + mappedValueId + "']]");
                    valueRow["ValueStr"] = indexValueNode.SelectSingleNode("valueStr").InnerText;
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
            dtType.Columns.Add(new DataColumn("TypeName") { DataType = Type.GetType("System.String") });
            dtType.Columns.Add(new DataColumn("Remark") { DataType = Type.GetType("System.String") });
            dtType.PrimaryKey = new DataColumn[] {dtType.Columns["MappedTypeId"]};

            DataTable dtAttributes = new DataTable("attributes");
            dtAttributes.Columns.Add(new DataColumn("MappedAttributeId") { Unique = true, DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("SelectedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("AttributeName") { DataType = Type.GetType("System.String") });
            dtAttributes.Columns.Add(new DataColumn("DisplaySequence") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("MappedTypeId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("UsageMode"){ DataType=Type.GetType("System.Int32")});
            dtAttributes.Columns.Add(new DataColumn("UseAttributeImage") { DataType = Type.GetType("System.String") });
            dtAttributes.PrimaryKey = new DataColumn[] {dtAttributes.Columns["MappedAttributeId"]};

            DataTable dtValues = new DataTable("values");
            dtValues.Columns.Add(new DataColumn("MappedValueId") { DataType=Type.GetType("System.String")});
            dtValues.Columns.Add(new DataColumn("SelectedValueId") { DataType = Type.GetType("System.String") });
            dtValues.Columns.Add(new DataColumn("MappedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtValues.Columns.Add(new DataColumn("SelectedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtValues.Columns.Add(new DataColumn("DisplaySequence") { DataType = Type.GetType("System.Int32") });
            dtValues.Columns.Add(new DataColumn("ValueStr") { DataType = Type.GetType("System.String") });
            dtValues.Columns.Add(new DataColumn("ImageUrl") { DataType = Type.GetType("System.String") });

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

            string virtualPath = "/Storage/Album/";
            string savePath = HttpContext.Current.Request.MapPath(virtualPath + DateTime.Now.Year + DateTime.Now.Month);
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            virtualPath = Path.Combine(virtualPath, DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString());

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

                productRow["ProductId"] = productId;
                productRow["SelectedTypeId"] = selectedTypeId;
                productRow["MappedTypeId"] = mappedTypeId;
                productRow["ProductName"] = productNode.SelectSingleNode("productName").InnerText;
                if (!string.IsNullOrEmpty(productNode.SelectSingleNode("SKU").InnerText))
                    productRow["SKU"] = productNode.SelectSingleNode("SKU").InnerText;
                else
                    productRow["SKU"] = GenerateSKU(8, productId);
                productRow["ShortDescription"] = productNode.SelectSingleNode("shortDescription").InnerText;
                productRow["Unit"] = productNode.SelectSingleNode("unit").InnerText;
                productRow["Description"] = productNode.SelectSingleNode("description").InnerText;
                productRow["Title"] = productNode.SelectSingleNode("title").InnerText;
                productRow["Meta_Description"] = productNode.SelectSingleNode("meta_Description").InnerText;
                productRow["Meta_Keywords"] = productNode.SelectSingleNode("meta_Keywords").InnerText;
                if (productNode.SelectSingleNode("marketPrice").InnerText.Length > 0)
                     productRow["marketPrice"] = decimal.Parse(productNode.SelectSingleNode("marketPrice").InnerText);
                if (includeCostPrice && productNode.SelectSingleNode("CostPrice") != null 
                    && !string.IsNullOrEmpty(productNode.SelectSingleNode("CostPrice").InnerText))
                     productRow["CostPrice"] = decimal.Parse(productNode.SelectSingleNode("CostPrice").InnerText);
                productRow["SalePrice"] = decimal.Parse(productNode.SelectSingleNode("SalePrice").InnerText);

                productRow["ImageUrl1"] = string.Empty;
                productRow["ImageUrl2"] = string.Empty;
                productRow["ImageUrl3"] = string.Empty;
                productRow["ImageUrl4"] = string.Empty;
                productRow["ImageUrl5"] = string.Empty;

                #region Images
                XmlNodeList imageNodeList = productNode.SelectNodes("images/image");
                if (imageNodeList != null && imageNodeList.Count > 0)
                {
                    int index = 0;
                    foreach (XmlNode imageNode in imageNodeList)
                    {
                        string imageUrl = imageNode.InnerText;  
                        if (imageUrl.Length > 0 && File.Exists(Path.Combine(workDir + "\\images2", imageUrl)))
                        {
                            File.Copy(Path.Combine(workDir + "\\images2", imageUrl),
                                      context.Request.MapPath("~" + virtualPath + imageUrl), true);
                            index++;
                            switch (index)
                            {
                                case 1:
                                    productRow["ImageUrl1"] = virtualPath + imageUrl;
                                    break;
                                case 2:
                                    productRow["ImageUrl2"] = virtualPath + imageUrl;
                                    break;
                                case 3:
                                    productRow["ImageUrl3"] = virtualPath + imageUrl;
                                    break;
                                case 4:
                                    productRow["ImageUrl4"] = virtualPath + imageUrl;
                                    break;
                                case 5:
                                    productRow["ImageUrl5"] = virtualPath + imageUrl;
                                    break;
                            }
                        }
                    }
                }
                #endregion

                XmlNodeList attributeNodeList = productNode.SelectNodes("productAttributes/productAttribute");
                loadProductAttributes(productId, attributeNodeList, productSet, mappingSet);

                decimal price = decimal.Parse(productNode.SelectSingleNode("SalePrice").InnerText);
                
                XmlNodeList valueNodeList = productNode.SelectNodes("skus/sku");
                int stock = loadProductSkus(productId, price, valueNodeList, productSet, mappingSet, includeCostPrice, includeStock);
                if (includeStock)
                    productRow["Stock"] = stock;

                productSet.Tables["products"].Rows.Add(productRow);
            }

            return new object[] {productSet};
        }

        private int loadProductSkus(int productId, decimal price, XmlNodeList valueNodeList, DataSet productSet, DataSet mappingSet, 
            bool includeCostPrice, bool includeStock)
        {
            int stock = 0;
            if (valueNodeList == null || valueNodeList.Count == 0)
            {
                return stock;
            }

            foreach (XmlNode valueNode in valueNodeList)
            {
                DataRow valueRow = productSet.Tables["skus"].NewRow();
                string mappedSkuId = valueNode.SelectSingleNode("skuId").InnerText;

                valueRow["MappedSkuId"] = mappedSkuId;
                valueRow["ProductId"] = productId;
                valueRow["SKU"] = valueNode.SelectSingleNode("SKU").InnerText;

                if (includeStock)
                {
                    valueRow["Stock"] = int.Parse(valueNode.SelectSingleNode("stock").InnerText);
                    stock += int.Parse(valueNode.SelectSingleNode("stock").InnerText);
                }
                valueRow["Price"] = decimal.Parse(valueNode.SelectSingleNode("Price").InnerText);

                XmlNodeList itemNodeList = valueNode.SelectNodes("skuItems/skuItem");
                loadSkuItems(mappedSkuId, productId, itemNodeList, productSet, mappingSet, valueNode.SelectSingleNode("skuId").InnerText);

                valueRow["NewSkuId"] = valueNode.SelectSingleNode("skuId").InnerText;
                productSet.Tables["skus"].Rows.Add(valueRow);
            }

            return stock;
        }

        private void loadSkuItems(string mappedSkuId, int mappedProductId, XmlNodeList itemNodeList, DataSet productSet, DataSet mappingSet, string sku)
        {
            if (itemNodeList == null || itemNodeList.Count == 0)
                return;


            foreach(XmlNode itemNode in itemNodeList)
            {
                DataRow itemRow = productSet.Tables["skuItems"].NewRow();

                int mappedAttributeId = int.Parse(itemNode.SelectSingleNode("attributeId").InnerText);
                string mappedValueId = itemNode.SelectSingleNode("ValueStr").InnerText;
                DataRow[] mappedAttrs = mappingSet.Tables["values"].Select("MappedAttributeId=" + mappedAttributeId);
                DataRow[] mappedValues = mappingSet.Tables["values"].Select(
                            "MappedAttributeId=" + itemNode.SelectSingleNode("attributeId").InnerText + "AND " +
                            "MappedValueId='" + itemNode.SelectSingleNode("ValueStr").InnerText + "'");

                //当商品中的规格属性在index.xml文件中存在时进行添加
                if (mappedAttrs != null && mappedAttrs.Length > 0 && mappedValues != null && mappedValues.Length > 0)
                {
                    int selectedAttributeId = (int)mappedAttrs[0]["SelectedAttributeId"];
                    string selectedAttributeName =
                        (string)mappingSet.Tables["attributes"].Select(
                            "MappedAttributeId=" + itemNode.SelectSingleNode("attributeId").InnerText)[0]["AttributeName"];
                    string selectedValueId = (string)mappedValues[0]["SelectedValueId"];

                    itemRow["MappedProductId"] = mappedProductId;
                    itemRow["NewSkuId"] = sku;
                    itemRow["MappedSkuId"] = mappedSkuId;
                    itemRow["SelectedAttributeId"] = selectedAttributeId;
                    itemRow["MappedAttributeId"] = mappedAttributeId;
                    itemRow["SelectedAttributeName"] = selectedAttributeName;
                    itemRow["SelectedValueId"] = selectedValueId;
                    itemRow["MappedValueId"] = mappedValueId;

                    productSet.Tables["skuItems"].Rows.Add(itemRow);
                }
            }
        }

        private void loadProductAttributes(int productId, XmlNodeList attributeNodeList, DataSet productSet, DataSet mappingSet)
        {
            if (attributeNodeList == null || attributeNodeList.Count == 0)
                return;

            foreach(XmlNode attributeNode in attributeNodeList)//选中的属性
            {

                int mappedAttributeId = int.Parse(attributeNode.SelectSingleNode("AttributeId").InnerText);
                string mappedValueId = attributeNode.SelectSingleNode("ValueStr").InnerText;

                DataRow[] mappedAttrs = mappingSet.Tables["attributes"].Select("MappedAttributeId=" + mappedAttributeId);
                DataRow[] mappedValues = mappingSet.Tables["values"].Select("MappedValueId='" + mappedValueId + "'");
                //当商品中的扩展属性在index.xml文件中存在时进行添加
                if (mappedAttrs != null && mappedAttrs.Length > 0 && mappedValues != null && mappedValues.Length > 0)
                {
                    int selectedAttributeId = (int)mappedAttrs[0]["SelectedAttributeId"];
                    string selectedValueId = (string)mappedValues[0]["SelectedValueId"];

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
            dtProducts.Columns.Add(new DataColumn("SKU") { DataType = Type.GetType("System.String") });
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
            dtProducts.Columns.Add(new DataColumn("CostPrice") { DataType = Type.GetType("System.Decimal") });
            dtProducts.Columns.Add(new DataColumn("SalePrice") { DataType = Type.GetType("System.Decimal") });
            dtProducts.Columns.Add(new DataColumn("Stock") { DataType = Type.GetType("System.Int32") });
            dtProducts.PrimaryKey = new DataColumn[] { dtProducts.Columns["ProductId"] };

            DataTable dtAttributes = new DataTable("attributes");
            dtAttributes.Columns.Add(new DataColumn("ProductId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("SelectedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("MappedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtAttributes.Columns.Add(new DataColumn("SelectedValueId") { DataType = Type.GetType("System.String") });
            dtAttributes.Columns.Add(new DataColumn("MappedValueId") { DataType = Type.GetType("System.String") });
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
            dtSkus.Columns.Add(new DataColumn("Stock") { DataType = Type.GetType("System.Int32") });
            dtSkus.Columns.Add(new DataColumn("Price") { DataType = Type.GetType("System.Decimal") });
            dtSkus.PrimaryKey = new DataColumn[] { dtSkus.Columns["MappedSkuId"] };

            DataTable dtSkuItems = new DataTable("skuItems");
            dtSkuItems.Columns.Add(new DataColumn("MappedSkuId") { DataType = Type.GetType("System.String") });
            dtSkuItems.Columns.Add(new DataColumn("NewSkuId") { DataType = Type.GetType("System.String") });
            dtSkuItems.Columns.Add(new DataColumn("MappedProductId") { DataType = Type.GetType("System.Int32") });
            dtSkuItems.Columns.Add(new DataColumn("SelectedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtSkuItems.Columns.Add(new DataColumn("MappedAttributeId") { DataType = Type.GetType("System.Int32") });
            dtSkuItems.Columns.Add(new DataColumn("SelectedAttributeName") { DataType = Type.GetType("System.String") });
            dtSkuItems.Columns.Add(new DataColumn("SelectedValueId") { DataType = Type.GetType("System.String") });
            dtSkuItems.Columns.Add(new DataColumn("MappedValueId") { DataType = Type.GetType("System.String") });
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

        private string GenerateSKU(int count,int productId)
        {
            int rand;
            char code;

            string orderId = string.Empty;
            Random random = new Random(productId);
            
            for (int i = 0; i < count; i++)
            {
                rand = random.Next(productId*i*count*123);
                if (count == 8)
                    rand /= 18;
                else
                    rand /= 23;
                code = (char)('0' + (char)(rand % 10));
                orderId += code.ToString();
            }

            return orderId;
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
