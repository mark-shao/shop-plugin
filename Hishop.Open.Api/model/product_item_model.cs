using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Open.Api
{
    /// <summary>
    /// 商品
    /// </summary>
    public class product_item_model
    {

        private IList<string> _picurl;
        private IList<product_sku_model> _skus;
       /// <summary>
        /// 商品分类编号
       /// </summary>
       public int cid { set; get; }

       /// <summary>
       /// 商品分类名称
       /// </summary>
       public string cat_name { set; get; }

       /// <summary>
       /// 品牌编号
       /// </summary>
       public int brand_id { set; get; }

       /// <summary>
       /// 品牌名称
       /// </summary>
       public string brand_name { set; get; }

       /// <summary>
       /// 商品类型编号
       /// </summary>
       public int type_id { set; get; }


       /// <summary>
       /// 商品类型名称
       /// </summary>
       public string type_name { set; get; }

       /// <summary>
       /// 商品编号
       /// </summary>
       public int num_iid { set; get; }

       /// <summary>
       /// 商品货号
       /// </summary>
       public string outer_id { set; get; }

       /// <summary>
       /// 商品名称
       /// </summary>
       public string title { set; get; }

       /// <summary>
       /// 商品主图，多集合
       /// </summary>
       public IList<string> pic_url
       {
           set { _picurl = value; }
           get
           {
               if (_picurl == null)
                   _picurl = new List<string>();
               return _picurl;
           }
       }

       /// <summary>
       /// 商品描述
       /// </summary>
       public string desc;

       /// <summary>
       /// 移动端商品描述
       /// </summary>
       public string wap_desc;
       /// <summary>
       /// 商品发布时间
       /// </summary>
       public DateTime? list_time { set; get; }


       /// <summary>
       /// 商品更新时间
       /// </summary>
       public DateTime? modified { set; get; }

       /// <summary>
       /// 商品排序号
       /// </summary>
       public int display_sequence { get; set; }

       /// <summary>
       /// 商品状态
       /// </summary>
       public string approve_status { set; get; }

       /// <summary>
       /// 销售数量
       /// </summary>
       public int sold_quantity { set; get; }

       /// <summary>
       ///  商品地区
       /// </summary>
       public string location { get; set; }

        /// <summary>
       /// 商品扩展属性
        /// </summary>
       public string props_name { get; set; }

       /// <summary>
       /// 商品库存
       /// </summary>
       public int sub_stock 
       {
           get
           {
               int num = 0;
               foreach (product_sku_model sku in skus)
                   num += sku.quantity;
               return num;
           }
       }

        /// <summary>
        /// 规格集
        /// </summary>
       public IList<product_sku_model> skus
       {
           set { _skus = value; }
           get
           {
               if (_skus == null)
                   _skus = new List<product_sku_model>();
               return _skus;
           }
       }
    }
}
