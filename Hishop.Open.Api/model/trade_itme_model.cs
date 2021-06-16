using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Open.Api
{
    /// <summary>
    /// 交易子订单列目
    /// </summary>
    public class trade_itme_model
    {
       /// <summary>
       /// 规格号
       /// </summary>
       public string sku_id { set; get; }

       /// <summary>
       /// 商品编号
       /// </summary>
       public string num_id { set; get; }

       /// <summary>
       /// 商品货号
       /// </summary>
       public string outer_sku_id { set; get; }

       /// <summary>
       /// 商品名称
       /// </summary>
       public string title { set; get; }

       /// <summary>
       /// 规格名称
       /// </summary>
       public string sku_properties_name { set; get; }


       /// <summary>
       /// 商品价格
       /// </summary>
       public decimal price { set; get; } 


       /// <summary>
       /// 购买数量
       /// </summary>
       public int num { set; get; }


       /// <summary>
       /// 商品图片url地址
       /// </summary>
       public string pic_path { set; get; }


       /// <summary>
       /// 商品状态
       /// </summary>
       public string refund_status { set; get; }
    }
}
