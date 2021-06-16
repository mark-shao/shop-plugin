using System;
using System.Collections.Generic;
using System.Text;

using Hishop.Plugins;
using Hishop.Plugins.SupportedPlugins;
using Hidistro.Entities.Commodities;
using System.Data;
using System.Collections;
using System.Globalization;

namespace Hishop.Plugins.DataConvert.Taobao
{
    [Plugin("易趣导入接口")]
    public class YiquImport : DataImport
    {
        private int _categoryId;
        private bool _isSell;
        private int _maxSequence;

        public YiquImport(int categoryId, bool isSell, int maxSequence)
        {
            _categoryId = categoryId;
            _isSell = isSell;
            _maxSequence = maxSequence;
        }

        private ProductInfo FillProductInfo(string[] aryline)
        {
            ProductInfo product = new ProductInfo();
            product.CategoryId = _categoryId;
            product.Upselling = _isSell;
            product.AddedDate = DateTime.Now;
            // 商家编码
            product.SKU = Substring(aryline[0]);
            // 商品名称
            product.ProductName = Substring(aryline[1]);
            // 简单介绍
            product.ShortDescription = aryline[2];
            // 描述
            product.Description = Substring(aryline[3].Replace("\"\"", "\""));
            // 详细页标题
            product.Title = Substring(aryline[4]);
            // 详细页描述
            product.MetaDescription = Substring(aryline[5]);
            // 详细页搜索关键字
            product.MetaKeywords = Substring(aryline[6]);
            // 计量单位 
            product.Unit = Substring(aryline[7]);
            // 重量
            if (int.Parse(aryline[8], 0) > 0)
                product.Weight = Convert.ToInt32(Substring(aryline[8]));
            // 商品图片
            //if (!string.IsNullOrEmpty(aryline[9]))
            //    productToAdd.InFocusImageUrl = "/Storage/Original/" + Substring(aryline[9]);
            // 成本价
            product.CostPrice = decimal.Parse(Substring(aryline[10]), 0);
            // 市场价
            product.MarketPrice = decimal.Parse(Substring(aryline[11]), 0);
            // 销售价
            product.SalePrice = decimal.Parse(Substring(aryline[12]), 0);
            // 库存
            if (int.Parse(aryline[13], 0) > 0)
                product.Stock = Convert.ToInt32(aryline[13]);

            return product;
        }

        /// <summary>
        /// 检查商品信息
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        bool CheckProductInfo(ProductInfo product)
        {

            // 目前没有对信息的验证
            //
            if (string.IsNullOrEmpty(product.ProductName))
                return false;

            if (product.SalePrice == 0)
                return false;


            return true;
        }

        public override string UploadProducts(System.IO.StreamReader read)
        {
            int i = 0;
            int failNum = 0;
            int successNum = 0;
            string failPt = string.Empty;
            string strline;
            string[] aryline;

            while ((strline = read.ReadLine()) != null)
            {
                i++;
                if (i > 1 && strline.EndsWith(",,0,0,0,0,0")) // 过滤掉标题,同时过滤掉无用的列
                {
                    try
                    {
                        aryline = strline.Split(new char[] { ',' });
                        ProductInfo productToAdd = FillProductInfo(aryline);
                        if (productToAdd.Stock == 0)
                            productToAdd.Stock = 100;

                        productToAdd.DisplaySequence = _maxSequence;

                        if (string.IsNullOrEmpty(productToAdd.SKU))
                        {
                            Random rand = new Random();
                            // 自动生成一个商家编码
                            productToAdd.SKU = string.Format("{0}{1}", rand.Next(9).ToString() + rand.Next(9) + rand.Next(9) + rand.Next(9) + rand.Next(9), i);
                        }

                        Dictionary<string, SKUItem> skus = new Dictionary<string, SKUItem>();
                        SKUItem sku = new SKUItem();
                        sku.SKU = productToAdd.SKU;
                        sku.Stock = productToAdd.Stock;
                        skus.Add(sku.SKU, sku);

                        ArrayList uploadImages = new ArrayList();
                        //if (!string.IsNullOrEmpty(productToAdd.InFocusImageUrl))
                        //    uploadImages.Add("/Storage/Album/" + productToAdd.InFocusImageUrl.Substring(productToAdd.InFocusImageUrl.LastIndexOf("/") + 1));

                        if (!CheckProductInfo(productToAdd))
                        {
                            failNum++;
                            if (string.IsNullOrEmpty(failPt))
                                failPt = (i - 1).ToString(CultureInfo.InvariantCulture);
                            else
                                failPt += "、" + (i - 1).ToString(CultureInfo.InvariantCulture);

                            continue;
                        }


                        //if (ProductHelper.AddProduct(productToAdd, skus, null, uploadImages, null) == ProductActionStatus.Success)
                        //{
                        //    successNum++;
                        //    maxSequence++;
                        //}
                        //else
                        //{
                        //    failNum++;
                        //    if (string.IsNullOrEmpty(failPt))
                        //        failPt = (i-1).ToString(CultureInfo.InvariantCulture);
                        //    else
                        //        failPt += "、" + (i-1).ToString(CultureInfo.InvariantCulture);
                        //}
                    }
                    catch
                    {
                        failNum++;
                        if (string.IsNullOrEmpty(failPt))
                            failPt = (i - 1).ToString(CultureInfo.InvariantCulture);
                        else
                            failPt += "、" + (i - 1).ToString(CultureInfo.InvariantCulture);
                    }

                }
            }
            if (string.IsNullOrEmpty(failPt))
                return string.Format("成功上传{0}件商品，无失败上传商品", successNum.ToString(CultureInfo.InvariantCulture));
            else
                return string.Format("成功上传{0}件商品，同时 csv 中第{1}行记录的商品失败请检查商品名称、一口价是否填写以及商家编码是否和网店中的商品商家编码重复",
                    successNum.ToString(CultureInfo.InvariantCulture), failPt);
        }
    }
}
