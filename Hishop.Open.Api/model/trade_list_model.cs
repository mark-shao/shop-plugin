using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Open.Api
{
    /// <summary>
    /// 交易订单字段说明
    /// </summary>
    public class trade_list_model
    {

       private List<trade_itme_model> _orders;
     /// <summary>
     /// 订单号
     /// </summary>
       public string tid { set; get; }

       
       /// <summary>
       /// 买家备注
       /// </summary>
       public string buyer_memo { set; get; }

       /// <summary>
       /// 卖家备注
       /// </summary>
       public string seller_memo { set; get; }

       /// <summary>
       /// 商家标记
       /// </summary>
       public string seller_flag { set; get; }


       /// <summary>
       /// 订单折扣
       /// </summary>
       public decimal discount_fee { set; get; }


       /// <summary>
       /// 订单状态
       /// </summary>
       public string status { set; get; }


       /// <summary>
       /// 关闭理由
       /// </summary>
       public string close_memo { set; get; }


       /// <summary>
       /// 创建时间
       /// </summary>
       public DateTime? created { set; get; }


       /// <summary>
       /// 修改时间
       /// </summary>
       public DateTime? modified { set; get; }

       /// <summary>
       /// 支付时间
       /// </summary>
       public DateTime? pay_time { set; get; }

       /// <summary>
       /// 发货时间
       /// </summary>
       public DateTime? consign_time{set;get;}


       /// <summary>
       /// 完成时间
       /// </summary>
       public DateTime? end_time { set; get; }

       /// <summary>
       /// 下单人用户名
       /// </summary>
       public string buyer_uname { set; get; }



       /// <summary>
       /// 下单人邮箱
       /// </summary>
       public string buyer_email { set; get; }


       /// <summary>
       /// 买家昵称
       /// </summary>
       public string buyer_nick { set; get; }


       /// <summary>
       /// 下单人买家地区
       /// </summary>
       public string buyer_area { set; get; }


       /// <summary>
       /// 收货人姓名
       /// </summary>
       public string receiver_name { set; get; }

       /// <summary>
       /// 收货地址省
       /// </summary>
       public string receiver_state { set; get; }

       /// <summary>
       /// 收货所在的市
       /// </summary>
       public string receiver_city { set; get; }


       /// <summary>
       /// 收货所在的区
       /// </summary>
       public string receiver_district { set; get; }

       /// <summary>
       /// 收货所在的镇/街道
       /// </summary>
       public string receiver_town { set; get; }


       /// <summary>
       /// 收货详细地址
       /// </summary>
       public string receiver_address { set; get; }


       /// <summary>
       /// 收货地址邮编
       /// </summary>
       public string receiver_zip { set; get; }


       /// <summary>
       /// 收货人联系方式
       /// </summary>
       public string receiver_mobile { set; get; }

       /// <summary>
       /// 卖家编号
       /// </summary>
       public string seller_id { set; get; }

       /// <summary>
       /// 卖家用户名
       /// </summary>
       public string seller_name { set; get; }


       /// <summary>
       /// 卖家联系方式
       /// </summary>
       public string seller_mobile { set; get; }

       /// <summary>
       ///  发票金额
       /// </summary>
       public decimal invoice_fee { set; get; }



       /// <summary>
       /// 发票抬头
       /// </summary>
       public string invoice_title { set; get; }

       /// <summary>
       /// 订单支付金额
       /// </summary>
       public decimal payment { set; get; }

       /// <summary>
       /// 店铺编号
       /// </summary>
       public string storeId { set; get; }

       /// <summary>
       /// 订单列目
       /// </summary>
       public List<trade_itme_model> orders 
       {
           set { _orders = value; }
           get {
               if (_orders == null)
                   _orders=new List<trade_itme_model>();
               return _orders;
           }
       }

    }
}
