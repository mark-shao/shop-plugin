using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Open.Api
{
    /// <summary>
    /// 规格
    /// </summary>
    public class product_sku_model
    {
        
       /// <summary>
        /// 商品规格编号
       /// </summary>
       public string sku_id { set; get; }


       /// <summary>
       /// 商品货号
       /// </summary>
       public string outer_sku_id { set; get; }

       /// <summary>
       /// 库存
       /// </summary>
       public int quantity { set; get; }


       /// <summary>
       /// 价格
       /// </summary>
       public decimal price { set; get; }


       /// <summary>
       /// 规格名称，规格值
       /// </summary>
       public string sku_properties_name { set; get; }
    }
}
