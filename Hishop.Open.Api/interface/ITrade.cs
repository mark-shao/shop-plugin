using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hishop.Open.Api
{
    /// <summary>
    /// 交易API
    /// </summary>
    public interface ITrade
    {
        /// <summary>
        /// 获取当前商家的订单列表（根据创建时间）
        /// </summary>
        /// <remarks>
        /// <p>1. 返回的数据结果是以订单的创建时间倒序排列的。 </p>
        /// <p>2. 返回的数据结果只包含了订单的部分数据，可通过Hishop.Open.Api.ITrade.GetTrade获取订单详情。</p>
        /// <p>注意:status字段的说明，如果该字段不传，接口默认只查4种类型订单，非默认查询的订单是不返回。遇到订单查不到的情况的，通常都是这个原因造成。解决办法就是status加上订单类型就可正常返回了。</p>
        /// </remarks>
        /// <param name="start_created" type="Date" required="可选">获取时间区间内订单的开始时间。格式:yyyy-MM-dd HH:mm:ss</param>
        /// <param name="end_created" type="Date" required="可选" >获取时间区间内订单的结束时间。格式:yyyy-MM-dd HH:mm:ss</param>
        /// <param name="status" type="String" required="可选">交易状态<a href='orderstatus.html' target='_blank'>（查看可选值）</a>：WAIT_BUYER_PAY（等待买家付款）、
        /// WAIT_SELLER_SEND_GOODS （等待商家发货）、 WAIT_BUYER_CONFIRM_GOODS（等待买家确认收货）、
        /// TRADE_CLOSED （交易关闭）、TRADE_FINISHED（交易成功） 默认查询所有交易状态的数据，
        /// 除了默认值外每次只能查询一种状态</param>
        /// <param name="buyer_uname" type="String" required="可选">买家帐号</param>
        /// <param name="page_no" type="Number" required="可选">页码。取值范围:大于零的整数; 默认值:1</param>
        /// <param name="page_size" type="Number" required="可选">每页条数。取值范围:大于零的整数; 默认值:40;最大值:100</param>
        /// <returns>
        /// {
        ///     "trades_sold_get_response":{
        ///         "total_results": 100,
        ///         "has_next": true,
        ///         "trades":[{
        ///             "tid":"201001010001",
        ///             "buyer_memo":"上衣要大一号",
        ///             "seller_memo":"好的",
        ///             "seller_flag":"1",
        ///             "discount_fee":"200.07",
        ///             "status":"WAIT_BUYER_PAY",
        ///             "close_memo":"到期自动关闭",
        ///             "created":"2000-01-01 00:00:00",
        ///             "modified":"2000-01-01 00:00:00",
        ///             "pay_time":"2000-01-01 00:00:00",
        ///             "consign_time":"2000-01-01 00:00:00",
        ///             "end_time":"2000-01-01 00:00:00",
        ///             "buyer_uname":"testuser",
        ///             "buyer_email":"test1064@test.com",
        ///             "buyer_nick":"我在测试",
        ///             "buyer_area":"浙江省杭州市",
        ///             "receiver_name":"东方不败",
        ///             "receiver_state":"湖北省",
        ///             "receiver_city":"宜昌市",
        ///             "receiver_district":"伍家岗区",
        ///             "receiver_town":"大公桥街道",
        ///             "receiver_address":"五一大道湘域中央805",
        ///             "receiver_zip":"223700",
        ///             "receiver_mobile":"13512501826",
        ///             "seller_id":"210030",
        ///             "seller_name":"自营店",
        ///             "seller_mobile":"13512501826",
        ///             "invoice_fee":"0",
        ///             "invoice_title":"XXXX有限公司",
        ///             "payment":"300.45",
        ///             "storeId":"0",
        ///             "orders":[{
        ///                 "sku_id":"5937146",
        ///                 "num_id":"98",
        ///                 "outer_sku_id":"50786-4",
        ///                 "title":"山寨版测试机器",
        ///                 "sku_properties_name":"颜色:桔色;尺码:M",           
        ///                 "price":"200.07",
        ///                 "num":1,
        ///                 "pic_path":"http://img08.test.net/bao/uploaded/i8/091917.jpg",
        ///                 "refund_status":"WAIT_BUYER_PAY"
        ///             }] 
        ///         }]
        ///     }
        /// }
        /// </returns>
        /// <example>
        /// [
        ///     {"Key":"total_results","Type":"Int","Value":"100","Description":"搜索到的交易信息总数","HasList":"false"},
        ///     {"Key":"has_next","Type":"Boolean","Value":"true","Description":"是否存在下一页","HasList":"false"},
        ///     {"Key":"trades","Type":"Trade","Value":"","Description":"搜索到的交易信息列表，返回的Trade和Order中包含的字段信息","HasList":"true","List":
        ///         [
        ///             {"Key":"tid","Type":"String","Value":"201001010001","Description":"交易编号","HasList":"false"},
        ///             {"Key":"buyer_memo","Type":"String","Value":"上衣要大一号","Description":"买家备注","HasList":"false"},
        ///             {"Key":"seller_memo","Type":"String","Value":"好的","Description":"卖家备注","HasList":"false"},
        ///             {"Key":"seller_flag","Type":"String","Value":"1","Description":"卖家标记","HasList":"false"},
        ///             {"Key":"discount_fee","Type":"Decimal","Value":"200.07","Description":"订单折扣","HasList":"false"},
        ///             {"Key":"status","Type":"Sring","Value":"WAIT_BUYER_PAY","Description":"订单状态","HasList":"false"},
        ///             {"Key":"close_memo","Type":"Sring","Value":"到期自动关闭","Description":"关闭理由","HasList":"false"},
        ///             {"Key":"created","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"下单时间","HasList":"false"},
        ///             {"Key":"modified","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"修改时间","HasList":"false"},
        ///             {"Key":"pay_time","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"支付时间","HasList":"false"},
        ///             {"Key":"consign_time","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"发货时间","HasList":"false"},
        ///             {"Key":"end_time","Type":"Sring","DateTime":"2000-01-01 00:00:00","Description":"订单完成时间","HasList":"false"},
        ///             {"Key":"buyer_uname","Type":"Sring","Value":"testuser","Description":"下单人用户名","HasList":"false"},
        ///             {"Key":"buyer_email","Type":"Sring","Value":"test1064@test.com","Description":"下单人邮箱","HasList":"false"},
        ///             {"Key":"buyer_nick","Type":"Sring","Value":"我在测试","Description":"买家昵称","HasList":"false"},
        ///             {"Key":"buyer_area","Type":"Sring","Value":"浙江省杭州市","Description":"下单人买家地区","HasList":"false"},
        ///             {"Key":"receiver_name","Type":"Sring","Value":"东方不败","Description":"收货人姓名","HasList":"false"},
        ///             {"Key":"receiver_state","Type":"Sring","Value":"湖北省","Description":"收货地址所在省","HasList":"false"},
        ///             {"Key":"receiver_city","Type":"Sring","Value":"宜昌市","Description":"收货地址所在市","HasList":"false"},
        ///             {"Key":"receiver_district","Type":"Sring","Value":"伍家岗区","Description":"收货地址所在区","HasList":"false"},
        ///             {"Key":"receiver_town","Type":"Sring","Value":"大公桥街道","Description":"收货地址所在的城镇街道","HasList":"false"},
        ///             {"Key":"receiver_address","Type":"Sring","Value":"五一大道湘域中央805","Description":"收货详细地址","HasList":"false"},
        ///             {"Key":"receiver_zip","Type":"Sring","Value":"223700","Description":"收货地区邮编","HasList":"false"},
        ///             {"Key":"receiver_mobile","Type":"Sring","Value":"13512501826","Description":"收货联系人手机","HasList":"false"},
        ///             {"Key":"seller_id","Type":"Sring","Value":"210030","Description":"卖家编号","HasList":"false"},
        ///             {"Key":"seller_name","Type":"Sring","Value":"自营店","Description":"卖家用户名","HasList":"false"},
        ///             {"Key":"seller_mobile","Type":"Sring","Value":"13512501826","Description":"卖家联系人手机","HasList":"false"},
        ///             {"Key":"invoice_fee","Type":"Decimal","Value":"0.00","Description":"发票金额","HasList":"false"},
        ///             {"Key":"invoice_title","Type":"Sring","Value":"XXXX有限公司","Description":"发票抬头","HasList":"false"},
        ///             {"Key":"payment","Type":"Sring","Value":"300.45","Description":"实付金额。精确到2位小数;单位:元。如:200.07，表示:200元7分","HasList":"false"},
        ///             {"Key":"storeId","Type":"Int","Value":"0","Description":"门店编号，默认返回0(此值针对云商城系统才存在，大于0表示来自门店的订单，等于0表示商城订单)","HasList":"false"},
        ///             {"Key":"orders","Type":"orders","Value":"","Description":"子订单列表","HasList":"true","List":
        ///                 [
        ///                     {"Key":"sku_id","Type":"Sring","Value":"5937146","Description":"商品规格号","HasList":"false"},
        ///                     {"Key":"num_id","Type":"Sring","Value":"98","Description":"商品编号","HasList":"false"},
        ///                     {"Key":"outer_sku_id","Type":"Sring","Value":"50786-4","Description":"商品货号","HasList":"false"},
        ///                     {"Key":"title","Type":"Sring","Value":"山寨版测试机器","Description":"商品名称","HasList":"false"},
        ///                     {"Key":"sku_properties_name","Type":"Sring","Value":"颜色:桔色;尺码:M","Description":"规格属性名称","HasList":"false"},
        ///                     {"Key":"price","Type":"Decimal","Value":"200.07","Description":"商品单价","HasList":"false"},
        ///                     {"Key":"num","Type":"Int","Value":"1","Description":"商品数量","HasList":"false"},
        ///                     {"Key":"pic_path","Type":"String","Value":"200.07","Description":"图片地址","HasList":"false"},
        ///                     {"Key":"refund_status","Type":"String","Value":"WAIT_BUYER_PAY","Description":"商品状态","HasList":"false"}
        ///                 ]
        ///             }
        ///         ]
        ///     }
        /// ]
        /// </example>
        string GetSoldTrades(DateTime? start_created, DateTime? end_created, string status, string buyer_uname, int page_no, int page_size);

        /// <summary>
        /// 查询订单的增量交易数据（根据修改时间） 
        /// </summary>
        /// <remarks>
        /// <p>1. 一次请求只能查询时间跨度为一天的增量交易记录，即<font color='red'>end_modified - start_modified小于等于1天</font>。</p>
        /// <p>2. 返回的数据结果是以订单的修改时间倒序排列的，通过从后往前翻页的方式可以避免漏单问题。</p>
        /// <p>注意:status字段的说明，如果该字段不传，接口默认只查4种类型订单，非默认查询的订单是不返回。遇到订单查不到的情况的，通常都是这个原因造成。解决办法就是status加上订单类型就可正常返回了。</p>
        /// </remarks>
        /// <param name="start_modified" type="Date" required="必须">查询修改开始时间(修改时间跨度不能大于一天)。格式:yyyy-MM-dd HH:mm:ss</param>
        /// <param name="end_modified" type="Date" required="必须">查询修改结束时间，必须大于修改开始时间(修改时间跨度不能大于一天)，格式:yyyy-MM-dd HH:mm:ss。建议使用30分钟以内的时间跨度，能大大提高响应速度和成功率</param>
        /// <param name="status" type="String" required="可选">交易状态<a href='orderstatus.html' target='_blank'>（查看可选值）</a>：WAIT_BUYER_PAY（等待买家付款）、 WAIT_SELLER_SEND_GOODS （等待商家发货）、 WAIT_BUYER_CONFIRM_GOODS（等待买家确认收货）、TRADE_CLOSED （交易关闭）、TRADE_FINISHED（交易成功） 默认查询所有交易状态的数据，除了默认值外每次只能查询一种状态</param>
        /// <param name="buyer_uname" type="String" required="可选">买家帐号</param>
        /// <param name="page_no" type="Number" required="可选">页码。取值范围:大于零的整数; 默认值:1</param>
        /// <param name="page_size" type="Number" required="可选">每页条数。取值范围:大于零的整数; 默认值:40;最大值:100（建议使用40~50，可以提高成功率，减少超时数量）</param>
        /// <returns>
        /// {
        ///     "trades_sold_get_response":{
        ///         "total_results": 100,
        ///         "has_next": true,
        ///         "trades":[{
        ///             "tid":"201001010001",  
        ///             "buyer_memo":"上衣要大一号",
        ///             "seller_memo":"好的",
        ///             "seller_flag":"1",
        ///             "discount_fee":"200.07",
        ///             "status":"WAIT_BUYER_PAY",
        ///             "close_memo":"到期自动关闭",
        ///             "created":"2000-01-01 00:00:00",
        ///             "modified":"2000-01-01 00:00:00",
        ///             "pay_time":"2000-01-01 00:00:00",
        ///             "consign_time":"2000-01-01 00:00:00",
        ///             "end_time":"2000-01-01 00:00:00",
        ///             "buyer_uname":"testuser",
        ///             "buyer_email":"test1064@test.com",
        ///             "buyer_nick":"我在测试",
        ///             "buyer_area":"浙江省杭州市",
        ///             "receiver_name":"东方不败",
        ///              "receiver_state":"湖北省",
        ///             "receiver_city":"宜昌市",
        ///             "receiver_district":"伍家岗区",
        ///             "receiver_town":"大公桥街道",
        ///             "receiver_address":"五一大道湘域中央805",
        ///             "receiver_zip":"223700",
        ///             "receiver_mobile":"13512501826",
        ///             "seller_id":"210030",
        ///             "seller_name":"自营店",
        ///             "seller_mobile":"13512501826",
        ///             "invoice_fee":"0",
        ///             "invoice_title":"XXXX有限公司",
        ///             "payment":"300.45",
        ///             "storeId":"0",
        ///             "orders":[{
        ///                 "sku_id":"5937146",
        ///                 "num_id":"98",
        ///                 "outer_sku_id":"50786-4",
        ///                 "title":"山寨版测试机器",
        ///                 "sku_properties_name":"颜色:桔色;尺码:M",           
        ///                 "price":"200.07",
        ///                 "num":1,
        ///                 "pic_path":"http://img08.test.net/bao/uploaded/i8/091917.jpg",
        ///                 "refund_status":"WAIT_BUYER_PAY"
        ///             }] 
        ///         }]
        ///     }
        /// }
        /// </returns>
        /// <example>
        /// [
        ///     {"Key":"total_results","Type":"Int","Value":"100","Description":"搜索到的交易信息总数","HasList":"false"},
        ///     {"Key":"has_next","Type":"Boolean","Value":"true","Description":"是否存在下一页","HasList":"false"},
        ///     {"Key":"trades","Type":"Trade","Value":"","Description":"搜索到的交易信息列表，返回的Trade和Order中包含的字段信息","HasList":"true","List":
        ///         [
        ///             {"Key":"tid","Type":"String","Value":"201001010001","Description":"交易编号","HasList":"false"},
        ///             {"Key":"buyer_memo","Type":"String","Value":"上衣要大一号","Description":"买家备注","HasList":"false"},
        ///             {"Key":"seller_memo","Type":"String","Value":"好的","Description":"卖家备注","HasList":"false"},
        ///             {"Key":"seller_flag","Type":"String","Value":"1","Description":"卖家标记","HasList":"false"},
        ///             {"Key":"discount_fee","Type":"Decimal","Value":"200.07","Description":"订单折扣","HasList":"false"},
        ///             {"Key":"status","Type":"Sring","Value":"WAIT_BUYER_PAY","Description":"订单状态","HasList":"false"},
        ///             {"Key":"close_memo","Type":"Sring","Value":"到期自动关闭","Description":"关闭理由","HasList":"false"},
        ///             {"Key":"created","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"下单时间","HasList":"false"},
        ///             {"Key":"modified","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"修改时间","HasList":"false"},
        ///             {"Key":"pay_time","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"支付时间","HasList":"false"},
        ///             {"Key":"consign_time","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"发货时间","HasList":"false"},
        ///             {"Key":"end_time","Type":"Sring","DateTime":"2000-01-01 00:00:00","Description":"订单完成时间","HasList":"false"},
        ///             {"Key":"buyer_uname","Type":"Sring","Value":"testuser","Description":"下单人用户名","HasList":"false"},
        ///             {"Key":"buyer_email","Type":"Sring","Value":"test1064@test.com","Description":"下单人邮箱","HasList":"false"},
        ///             {"Key":"buyer_nick","Type":"Sring","Value":"我在测试","Description":"买家昵称","HasList":"false"},
        ///             {"Key":"buyer_area","Type":"Sring","Value":"浙江省杭州市","Description":"下单人买家地区","HasList":"false"},
        ///             {"Key":"receiver_name","Type":"Sring","Value":"东方不败","Description":"收货人姓名","HasList":"false"},
        ///             {"Key":"receiver_state","Type":"Sring","Value":"湖北省","Description":"收货地址所在省","HasList":"false"},
        ///             {"Key":"receiver_city","Type":"Sring","Value":"宜昌市","Description":"收货地址所在市","HasList":"false"},
        ///             {"Key":"receiver_district","Type":"Sring","Value":"伍家岗区","Description":"收货地址所在区","HasList":"false"},
        ///             {"Key":"receiver_town","Type":"Sring","Value":"大公桥街道","Description":"收货地址所在的城镇街道","HasList":"false"},
        ///             {"Key":"receiver_address","Type":"Sring","Value":"五一大道湘域中央805","Description":"收货详细地址","HasList":"false"},
        ///             {"Key":"receiver_zip","Type":"Sring","Value":"223700","Description":"收货地区邮编","HasList":"false"},
        ///             {"Key":"receiver_mobile","Type":"Sring","Value":"13512501826","Description":"收货联系人手机","HasList":"false"},
        ///             {"Key":"seller_id","Type":"Sring","Value":"210030","Description":"卖家编号","HasList":"false"},
        ///             {"Key":"seller_name","Type":"Sring","Value":"自营店","Description":"卖家用户名","HasList":"false"},
        ///             {"Key":"seller_mobile","Type":"Sring","Value":"13512501826","Description":"卖家联系人手机","HasList":"false"},
        ///             {"Key":"invoice_fee","Type":"Decimal","Value":"0.00","Description":"发票金额","HasList":"false"},
        ///             {"Key":"invoice_title","Type":"Sring","Value":"XXXX有限公司","Description":"发票抬头","HasList":"false"},
        ///             {"Key":"payment","Type":"Sring","Value":"300.45","Description":"实付金额。精确到2位小数;单位:元。如:200.07，表示:200元7分","HasList":"false"},
        ///             {"Key":"storeId","Type":"Int","Value":"0","Description":"门店编号，默认返回0(此值针对云商城系统才存在，大于0表示来自门店的订单，等于0表示商城订单)","HasList":"false"},
        ///             {"Key":"orders","Type":"orders","Value":"","Description":"子订单列表","HasList":"true","List":
        ///                 [
        ///                     {"Key":"sku_id","Type":"Sring","Value":"5937146","Description":"商品规格号","HasList":"false"},
        ///                     {"Key":"num_id","Type":"Sring","Value":"98","Description":"商品编号","HasList":"false"},
        ///                     {"Key":"outer_sku_id","Type":"Sring","Value":"50786-4","Description":"商品货号","HasList":"false"},
        ///                     {"Key":"title","Type":"Sring","Value":"山寨版测试机器","Description":"商品名称","HasList":"false"},
        ///                     {"Key":"sku_properties_name","Type":"Sring","Value":"颜色:桔色;尺码:M","Description":"规格属性名称","HasList":"false"},
        ///                     {"Key":"price","Type":"Decimal","Value":"200.07","Description":"商品单价","HasList":"false"},
        ///                     {"Key":"num","Type":"Int","Value":"1","Description":"商品数量","HasList":"false"},
        ///                     {"Key":"pic_path","Type":"String","Value":"200.07","Description":"图片地址","HasList":"false"},
        ///                     {"Key":"refund_status","Type":"String","Value":"WAIT_BUYER_PAY","Description":"商品状态","HasList":"false"}
        ///                 ]
        ///             }
        ///         ]
        ///     }
        /// ]
        /// </example>
        string GetIncrementSoldTrades(DateTime start_modified, DateTime end_modified, string status, string buyer_uname, int page_no, int page_size);
        
        /// <summary>
        /// 获取单笔交易的详细信息
        /// </summary>
        /// <remarks>
        /// 获取单笔交易的详细信息
        /// </remarks>
        /// <param name="tid" type="String" required="必须 ">交易编号</param>
        /// <returns>
        /// {
        ///     "trade_get_response":
        ///     {
        ///         "trade":{
        ///             "tid":"201001010001",  
        ///             "buyer_memo":"上衣要大一号",
        ///             "seller_memo":"好的",
        ///             "seller_flag":"1",
        ///             "discount_fee":"200.07",
        ///             "status":"WAIT_BUYER_PAY",
        ///             "close_memo":"到期自动关闭",
        ///             "created":"2000-01-01 00:00:00",
        ///             "modified":"2000-01-01 00:00:00",
        ///             "pay_time":"2000-01-01 00:00:00",
        ///             "consign_time":"2000-01-01 00:00:00",
        ///             "end_time":"2000-01-01 00:00:00",
        ///             "buyer_uname":"testuser",
        ///             "buyer_email":"test1064@test.com",
        ///             "buyer_nick":"我在测试",
        ///             "buyer_area":"浙江省杭州市",
        ///             "receiver_name":"东方不败",
        ///             "receiver_state":"湖北省",
        ///             "receiver_city":"宜昌市",
        ///             "receiver_district":"伍家岗区",
        ///             "receiver_town":"大公桥街道",
        ///             "receiver_address":"五一大道湘域中央805",
        ///             "receiver_zip":"223700",
        ///             "receiver_mobile":"13512501826",
        ///             "seller_id":"210030",
        ///             "seller_name":"自营店",
        ///             "seller_mobile":"13512501826",
        ///             "invoice_fee":"0",
        ///             "invoice_title":"XXXX有限公司",
        ///             "payment":"300.45",
        ///             "orders":[{
        ///                 "sku_id":"5937146",
        ///                 "num_id":"98",
        ///                 "outer_sku_id":"50786-4",
        ///                 "title":"山寨版测试机器",
        ///                 "sku_properties_name":"颜色:桔色;尺码:M",           
        ///                 "price":"200.07",
        ///                 "num":1,
        ///                 "pic_path":"http://img08.test.net/bao/uploaded/i8/091917.jpg",
        ///                 "refund_status":"WAIT_BUYER_PAY"
        ///             }]
        ///         }
        ///     }
        /// }
        /// </returns>
        /// <example>
        /// [
        ///     {"Key":"trades","Type":"Trade","Value":"","Description":"搜索到的交易信息列表，返回的Trade和Order中包含的字段信息","HasList":"true","List":
        ///         [
        ///             {"Key":"tid","Type":"String","Value":"201001010001","Description":"交易编号","HasList":"false"},
        ///             {"Key":"buyer_memo","Type":"String","Value":"上衣要大一号","Description":"买家备注","HasList":"false"},
        ///             {"Key":"seller_memo","Type":"String","Value":"好的","Description":"卖家备注","HasList":"false"},
        ///             {"Key":"seller_flag","Type":"String","Value":"1","Description":"卖家标记","HasList":"false"},
        ///             {"Key":"discount_fee","Type":"Decimal","Value":"200.07","Description":"订单折扣","HasList":"false"},
        ///             {"Key":"status","Type":"Sring","Value":"WAIT_BUYER_PAY","Description":"订单状态","HasList":"false"},
        ///             {"Key":"close_memo","Type":"Sring","Value":"到期自动关闭","Description":"关闭理由","HasList":"false"},
        ///             {"Key":"created","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"下单时间","HasList":"false"},
        ///             {"Key":"modified","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"修改时间","HasList":"false"},
        ///             {"Key":"pay_time","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"支付时间","HasList":"false"},
        ///             {"Key":"consign_time","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"发货时间","HasList":"false"},
        ///             {"Key":"end_time","Type":"Sring","DateTime":"2000-01-01 00:00:00","Description":"订单完成时间","HasList":"false"},
        ///             {"Key":"buyer_uname","Type":"Sring","Value":"testuser","Description":"下单人用户名","HasList":"false"},
        ///             {"Key":"buyer_email","Type":"Sring","Value":"test1064@test.com","Description":"下单人邮箱","HasList":"false"},
        ///             {"Key":"buyer_nick","Type":"Sring","Value":"我在测试","Description":"买家昵称","HasList":"false"},
        ///             {"Key":"buyer_area","Type":"Sring","Value":"浙江省杭州市","Description":"下单人买家地区","HasList":"false"},
        ///             {"Key":"receiver_name","Type":"Sring","Value":"东方不败","Description":"收货人姓名","HasList":"false"},
        ///             {"Key":"receiver_state","Type":"Sring","Value":"湖北省","Description":"收货地址所在省","HasList":"false"},
        ///             {"Key":"receiver_city","Type":"Sring","Value":"宜昌市","Description":"收货地址所在市","HasList":"false"},
        ///             {"Key":"receiver_district","Type":"Sring","Value":"伍家岗区","Description":"收货地址所在区","HasList":"false"},
        ///             {"Key":"receiver_town","Type":"Sring","Value":"大公桥街道","Description":"收货地址所在的城镇街道","HasList":"false"},
        ///             {"Key":"receiver_address","Type":"Sring","Value":"五一大道湘域中央805","Description":"收货详细地址","HasList":"false"},
        ///             {"Key":"receiver_zip","Type":"Sring","Value":"223700","Description":"收货地区邮编","HasList":"false"},
        ///             {"Key":"receiver_mobile","Type":"Sring","Value":"13512501826","Description":"收货联系人手机","HasList":"false"},
        ///             {"Key":"seller_id","Type":"Sring","Value":"210030","Description":"卖家编号","HasList":"false"},
        ///             {"Key":"seller_name","Type":"Sring","Value":"自营店","Description":"卖家用户名","HasList":"false"},
        ///             {"Key":"seller_mobile","Type":"Sring","Value":"13512501826","Description":"卖家联系人手机","HasList":"false"},
        ///             {"Key":"invoice_fee","Type":"Decimal","Value":"0.00","Description":"发票金额","HasList":"false"},
        ///             {"Key":"invoice_title","Type":"Sring","Value":"XXXX有限公司","Description":"发票抬头","HasList":"false"},
        ///             {"Key":"payment","Type":"Sring","Value":"300.45","Description":"实付金额。精确到2位小数;单位:元。如:200.07，表示:200元7分","HasList":"false"},
        ///             {"Key":"orders","Type":"orders","Value":"","Description":"子订单列表","HasList":"true","List":
        ///                 [
        ///                     {"Key":"sku_id","Type":"Sring","Value":"5937146","Description":"商品规格号","HasList":"false"},
        ///                     {"Key":"num_id","Type":"Sring","Value":"98","Description":"商品编号","HasList":"false"},
        ///                     {"Key":"outer_sku_id","Type":"Sring","Value":"50786-4","Description":"商品货号","HasList":"false"},
        ///                     {"Key":"title","Type":"Sring","Value":"山寨版测试机器","Description":"商品名称","HasList":"false"},
        ///                     {"Key":"sku_properties_name","Type":"Sring","Value":"颜色:桔色;尺码:M","Description":"规格属性名称","HasList":"false"},
        ///                     {"Key":"price","Type":"Decimal","Value":"200.07","Description":"商品单价","HasList":"false"},
        ///                     {"Key":"num","Type":"Int","Value":"1","Description":"商品数量","HasList":"false"},
        ///                     {"Key":"pic_path","Type":"String","Value":"200.07","Description":"图片地址","HasList":"false"},
        ///                     {"Key":"refund_status","Type":"String","Value":"WAIT_BUYER_PAY","Description":"商品状态","HasList":"false"}
        ///                 ]
        ///             }
        ///         ]
        ///     }
        /// ]
        /// </example>
        /// 
        string GetTrade(string tid);

        /// <summary>
        /// 订单发货
        /// </summary>
        /// <remarks>
        /// 订单发货
        /// </remarks>
        /// <param name="tid" type="String" required="必须 ">交易编号</param>
        /// <param name="company_name" type="String" required="必须">物流公司名称</param>
        /// <param name="out_sid" type="String" required="必须 ">运单号</param>
        /// <returns>
        /// {
        ///     "logistics_send_response":{
        ///         "shipping":{"is_success":true}
        ///     }
        /// }
        /// </returns>
        /// <example>
        /// [
        ///     {"Key":"shipping","Type":"shiping","Value":"","Description":"发货订单结果","HasList":"true","List":
        ///         [
        ///             {"Key":"is_success","Type":"Boolean","Value":"true","Description":"发货成功与否","HasList":"false"}
        ///         ]
        ///     }
        ///     
        /// ]
        /// </example>
        string SendLogistic(string tid, string company_name,string out_sid);

        /// <summary>
        /// 修改交易备注
        /// </summary>
        /// <remarks>
        /// 可重复调用本接口更新交易备注，本接口同时具有添加备注的功能
        /// </remarks>
        /// <param name="tid" type="String" required="必须 ">交易编号</param>
        /// <param name="memo" type="String" required="可选">备注文本</param>
        /// <param name="flag" type="Number" required="可选">备注标记,云商城系统的可选值分别是1~6的正整数，实际情况请参考对接的系统</param>
        /// <returns>
        /// {
        ///     "trade_memo_update_response":{
        ///         "trade":{ "tid":"2231958349","modified":"2000-01-01 00:00:00"}
        ///     }
        /// }
        /// </returns>
        /// <example>
        /// [
        ///     {"Key":"trade","Type":"trade","Value":"","Description":"订单集合对象","HasList":"true","List":
        ///         [
        ///             {"Key":"tid","Type":"String","Value":"2231958349","Description":"订单编号","HasList":"false"},
        ///             {"Key":"modified","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"更新时间","HasList":"false"}
        ///         ]
        ///     }
        ///     
        /// ]
        /// </example>
        string UpdateTradeMemo(string tid, string memo, int flag);


        /// <summary>
        /// 更新物流信息
        /// </summary>
        /// <remarks>
        /// 更新订单物流信息
        /// </remarks>
        /// <param name="tid" type="String" required="必须 ">交易编号</param>
        /// <param name="company_name" type="String" required="必须">物流公司名称</param>
        /// <param name="out_sid" type="String" required="必须 ">运单号</param>
        /// <returns>
        /// {
        ///     "logistics_change_response":{
        ///         "shipping":{"is_success":true}
        ///     }
        /// }
        /// </returns>
        /// <example>
        /// [
        ///     {"Key":"shipping","Type":"shiping","Value":"","Description":"更新订单物流结果","HasList":"true","List":
        ///         [
        ///             {"Key":"is_success","Type":"Boolean","Value":"true","Description":"更新成功与否","HasList":"false"}
        ///         ]
        ///     }
        ///     
        /// ]
        /// </example>
        string ChangLogistics(string tid, string company_name, string out_sid);
    }
}
