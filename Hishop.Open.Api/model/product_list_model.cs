using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Open.Api
{
    public class product_list_model
    {

       private ArrayList _picurl;
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
       public ArrayList pic_url
       {
           set { _picurl = value; }
           get
           {
               if (_picurl == null)
                   _picurl = new ArrayList();
               return _picurl;
           }
       }
       /// <summary>
       /// 商品发布时间
       /// </summary>
       public DateTime? list_time { set; get; }


       /// <summary>
       /// 商品更新时间
       /// </summary>
       public DateTime? modified { set; get; }


       /// <summary>
       /// 商品状态
       /// </summary>
       public string approve_status { set; get; }

       /// <summary>
       /// 销售数量
       /// </summary>
       public int sold_quantity { set; get; }

       /// <summary>
       /// 商品库存
       /// </summary>
       public int num { set; get; }

       /// <summary>
       /// 商品单价
       /// </summary>
       public decimal price { set; get; }
    }
}
