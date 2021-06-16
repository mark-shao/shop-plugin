using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hishop.Open.Api
{
    /// <summary>
    /// 商品API
    /// </summary>
    public interface IProduct
    {
        /// <summary>
        /// 获取当前商家的商品列表
        /// </summary>
        /// <remarks>
        ///  获取当前商家的商品信息，可根据商品时间段或商品状态进行条件筛选
        /// </remarks>
        /// <param name="start_modified" type="Date" required="可选">起始的修改时间</param>
        /// <param name="end_modified" type="Date" required="可选">结束的修改时间 </param>
        /// <param name="approve_status" type="String" required="可选">商品状态On_Sale(出售中)/(Un_Sale)下架区/(In_Stock)仓库中 默认查询所有状态的数据，除了默认值外每次只能查询一种状态</param>
        /// <param name="q" type="String" required="可选">搜索关键字，可搜索商品的名称。</param>
        /// <param name="order_by" type="String" required="可选">排序方式。格式为column:asc/desc ，column可选值:display_sequence（默认顺序） create_time(创建时间),sold_quantity（商品销量）;默认商品排序编号升序(diplay_sequence值越小在前)。如按照商品排序编号升序排序方式为display_sequence:asc</param>
        /// <param name="page_no" type="Number" required="可选">页码。取值范围:大于零的整数; 默认值:1</param>
        /// <param name="page_size" type="Number" required="可选">每页条数。取值范围:大于零的整数; 默认值:40;最大值:100</param>
        /// <returns>
        /// {
        ///      "products_get_response":{
        ///          "total_results":150,
        ///          "items":[{
        ///              "cid":132443,
        ///              "cat_name":"服务类",
        ///              "brand_id":"45",
        ///              "brand_name":"谷歌",
        ///              "type_id":"1",
        ///              "type_name":"商品类型",
        ///              "num_iid":1489161932,
        ///              "outer_id":"34143554352",
        ///              "title":"Google test item",
        ///              "pic_url":["http:\/\/img03.taobao.net\/bao\/uploaded\/i3\/T1HXdXXgPSt0JxZ2.8_070458.jpg","http:\/\/img02.taobao.net\/bao\/uploaded\/i3\/T1HXdXXgPSt0JxZ2.8_070458.jpg"],
        ///              "list_time":"2009-10-22 14:22:06",
        ///              "modified":"2000-01-01 00:00:00",
        ///              "approve_status":"On_Sale",
        ///              "sold_quantity":8888,
        ///              "num":8888,
        ///              "price":"5.00"
        ///         }]
        ///     }
        /// }
        /// </returns>
        /// <example>
        /// [
        ///     {"Key":"total_results","Type":"Int","Value":150,"Description":"总条数","HasList":"false"},
        ///     {"Key":"items","Type":"items","Value":"","Description":"返回商品具体的字段信息","HasList":"true","List":
        ///         [
        ///             {"Key":"cid","Type":"Int","Value":"132443","Description":"商品分类编号","HasList":"false"},
        ///             {"Key":"cat_name","Type":"String","Value":"服务类","Description":"商品类目名称","HasList":"false"},
        ///             {"Key":"brand_id","Type":"Int","Value":"45","Description":"品牌编号","HasList":"false"},
        ///             {"Key":"brand_name","Type":"String","Value":"谷歌","Description":"品牌名称","HasList":"false"},
        ///             {"Key":"type_id","Type":"Int","Value":"1","Description":"商品类型","HasList":"false"},
        ///             {"Key":"type_name","Type":"String","Value":"商品类型","Description":"商品类型","HasList":"false"},
        ///             {"Key":"num_iid","Type":"Int","Value":"1489161932","Description":"商品编号","HasList":"false"},
        ///             {"Key":"outer_id","Type":"String","Value":"34143554352","Description":"商品货号","HasList":"false"},
        ///             {"Key":"title","Type":"String","Value":"Google test item","Description":"商品名称","HasList":"false"},
        ///             {"Key":"pic_url","Type":"List","Value":["http:\/\/img\/038.jpg"],"Description":"商品图片主图，存在多图就以数组形式返回，中间用逗号隔开","HasList":"false"},
        ///             {"Key":"list_time","Type":"DateTime","Value":"2009-10-22 14:22:06","Description":"商品创建时间","HasList":"false"},
        ///             {"Key":"modified","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"商品修改时间","HasList":"false"},
        ///             {"Key":"approve_status","Type":"String","Value":"On_Sale","Description":"商品状态","HasList":"false"},
        ///             {"Key":"sold_quantity","Type":"Int","Value":"88888","Description":"销售数量","HasList":"false"},
        ///             {"Key":"num","Type":"Int","Value":"88888","Description":"商品库存","HasList":"false"},
        ///             {"Key":"price","Type":"Decimal","Value":"88888","Description":"商品一口价","HasList":"false"}
        ///         ]
        ///     }
        ///     
        /// ]
        /// </example>
        string GetSoldProducts(DateTime? start_modified, DateTime? end_modified, string approve_status, string q, string order_by, int page_no, int page_size);

        /// <summary>
        /// 获取指定商品的详情信息
        /// </summary>
        /// <remarks>
        ///  获取指定商品的某件商品详细信息
        /// </remarks>
        /// <param name="num_iid " type="Number" required="必须">商品数字ID</param>
        /// <returns>
        /// {
        ///      "product_get_response":{
        ///         "item":{
        ///              "cid":132443,
        ///              "cat_name":"服装"
        ///              "type_id":20,
        ///              "type_name":"情侣装",
        ///              "brand_id":12,
        ///              "brand_name":"阿迪达斯",
        ///              "num_iid":1489161932,
        ///              "outer_id":"34143554352",
        ///              "title":"Google test item",
        ///              "pic_url":["http:\/\/img03.taobao.net\/bao\/uploaded\/i3\/T1HXdXXgPSt0JxZ2.8_070458.jpg","http:\/\/img03.taobao.net\/bao\/uploaded\/i3\/T1HXdXXgPSt0JxZ2.8_070458.jpg"],
        ///              "desc":"这是一个好商品",
        ///              "wap_desc":"Wap宝贝详情",
        ///              "list_time":"2009-10-22 14:22:06",
        ///              "modified":"2000-01-01 00:00:00",
        ///              "display_sequence":10,
        ///              "approve_status":"On_Sale",
        ///              "sold_quantity":8888,
        ///              "location":{"city":"杭州", "state":"浙江" },
        ///              "props_name":"适合季节#cln#[春季,秋季]#scln#适合年龄段#cln#[少年,青年]",
        ///              "sub_stock":1,
        ///              "skus":[{
        ///                  "sku_id":123,
        ///                  "outer_sku_id":"50786-4",
        ///                  "quantity":3,
        ///                  "price":"200.07",
        ///                  "sku_properties_name":"颜色:桔色;尺码:M"
        ///             }]      
        ///         }
        ///     }
        /// }
        /// </returns>
        /// <example>
        /// [
        ///     {"Key":"item","Type":"items","Value":"","Description":"返回商品的详细字段信息","HasList":"true","List":
        ///         [
        ///             {"Key":"cid","Type":"Int","Value":"132443","Description":"商品分类编号","HasList":"false"},
        ///             {"Key":"cat_name","Type":"String","Value":"服装","Description":"商品类目名称","HasList":"false"},
        ///             {"Key":"brand_id","Type":"Int","Value":"45","Description":"品牌编号","HasList":"false"},
        ///             {"Key":"brand_name","Type":"String","Value":"谷歌","Description":"品牌名称","HasList":"false"},
        ///             {"Key":"type_id","Type":"Int","Value":"1","Description":"商品类型","HasList":"false"},
        ///             {"Key":"type_name","Type":"String","Value":"商品类型","Description":"商品类型","HasList":"false"},
        ///             {"Key":"num_iid","Type":"Int","Value":"1489161932","Description":"商品编号","HasList":"false"},
        ///             {"Key":"outer_id","Type":"String","Value":"34143554352","Description":"商品货号","HasList":"false"},
        ///             {"Key":"title","Type":"String","Value":"Google test item","Description":"商品名称","HasList":"false"},
        ///             {"Key":"pic_url","Type":"List","Value":["http:\/\/img\/038.jpg"],"Description":"商品图片主图，存在多图就以数组形式返回，中间用逗号隔开","HasList":"false"},
        ///             {"Key":"desc","Type":"String","Value":"这是一个好商品","Description":"pc端的商品详情","HasList":"false"},
        ///             {"Key":"wap_desc","Type":"String","Value":"Wap宝贝详情","Description":"移动端的商品详情","HasList":"false"},
        ///             {"Key":"list_time","Type":"DateTime","Value":"2009-10-22 14:22:06","Description":"商品创建时间","HasList":"false"},
        ///             {"Key":"modified","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"商品修改时间","HasList":"false"},
        ///             {"Key":"display_sequence","Type":"Int","Value":"10","Description":"商品排序号","HasList":"false"},
        ///             {"Key":"approve_status","Type":"String","Value":"On_Sale","Description":"商品状态","HasList":"false"},
        ///             {"Key":"sold_quantity","Type":"Int","Value":"88888","Description":"销售数量","HasList":"false"},
        ///             {"Key":"location","Type":"String","Value":"{'city':'杭州', 'state':'浙江'}","Description":"商品配送地区(此值为Himall系统存在)","HasList":"false"},
        ///             {"Key":"props_name","Type":"Int","Value":"适合季节#cln#[春季,秋季]#scln#适合年龄段#cln#[少年,青年]","Description":"商品扩展属性，(注：属性名称中的冒号':'被转换为：'#cln#'; 分号';'被转换为：'#scln#' )","HasList":"false"},
        ///             {"Key":"sub_stock","Type":"Int","Value":"1","Description":"商品库存","HasList":"false"},
        ///             {"Key":"skus","Type":"skus","Value":"","Description":"商品其他规格型号","HasList":"true","List":
        ///                 [
        ///                     {"Key":"sku_id","Type":"String","Value":"123","Description":"商品规格号","HasList":"false"},
        ///                     {"Key":"outer_sku_id","Type":"String","Value":"50786-4","Description":"商品货号","HasList":"false"},
        ///                     {"Key":"quantity","Type":"Int","Value":"3","Description":"商品数量","HasList":"false"},
        ///                     {"Key":"price","Type":"Decimal","Value":"200.07","Description":"商品单价","HasList":"false"},
        ///                     {"Key":"sku_properties_name","Type":"Int","Value":"132443","Description":"商品规格名称","HasList":"false"}
        ///                 ]
        ///             }
        ///         ]
        ///     }
        ///     
        /// ]
        /// </example>
        string GetProduct(int num_iid);

        /// <summary>
        /// 商品/SKU库存修改(提供按照全量或增量形式修改宝贝/SKU库存的功能)
        /// </summary>
        /// <remarks>
        ///  
        /// </remarks>
        /// <param name="num_iid " type="Number" required="必须">商品数字ID</param>
        /// <param name="sku_id" type="String" required="可选">规格唯一编号。如果不填默认修改商品所有库存，如果填上则修改该SKU的库存 </param>
        /// <param name="quantity" type="Number" required="必须">库存修改值，必选。当全量更新库存时，quantity必须为大于等于0的正整数；当增量更新库存时，quantity为整数，可小于等于0。若增量更新时传入的库存为负数，则负数与实际库存之和不能小于0。比如当前实际库存为1，传入增量更新quantity=-1，库存改为0 </param>
        /// <param name="type" type="Number" required="可选">库存更新方式，可选。1为全量更新，2为增量更新。如果不填，默认为全量更新。 </param>
        /// <returns>
        /// {
        ///     "product_quantity_update_response":{
        ///         "item":{
        ///              "cid":132443,
        ///              "cat_name":"服装"
        ///              "type_id":20,
        ///              "type_name":"情侣装",
        ///              "brand_id":12,
        ///              "brand_name":"阿迪达斯",
        ///              "num_iid":1489161932,
        ///              "outer_id":"34143554352",
        ///              "title":"Google test item",
        ///              "pic_url":["http:\/\/img03.taobao.net\/bao\/uploaded\/i3\/T1HXdXXgPSt0JxZ2.8_070458.jpg","http:\/\/img03.taobao.net\/bao\/uploaded\/i3\/T1HXdXXgPSt0JxZ2.8_070458.jpg"],
        ///              "desc":"这是一个好商品",
        ///              "wap_desc":"Wap宝贝详情",
        ///              "list_time":"2009-10-22 14:22:06",
        ///              "modified":"2000-01-01 00:00:00",
        ///              "display_sequence":10,
        ///              "approve_status":"On_Sale",
        ///              "sold_quantity":8888,
        ///              "location":{"city":"杭州", "state":"浙江" },
        ///              "props_name":"适合季节#cln#[春季,秋季]#scln#适合年龄段#cln#[少年,青年]",
        ///              "sub_stock":1,
        ///              "skus":[{
        ///                  "sku_id":123,
        ///                  "outer_sku_id":"50786-4",
        ///                  "quantity":3,
        ///                  "price":"200.07",
        ///                  "sku_properties_name":"颜色:桔色;尺码:M"
        ///             }]      
        ///         }
        ///     }
        /// }
        /// </returns>
        /// <example>
        /// [
        ///     {"Key":"item","Type":"items","Value":"","Description":"返回商品的详细字段信息","HasList":"true","List":
        ///         [
        ///             {"Key":"cid","Type":"Int","Value":"132443","Description":"商品分类编号","HasList":"false"},
        ///             {"Key":"cat_name","Type":"String","Value":"服装","Description":"商品类目名称","HasList":"false"},
        ///             {"Key":"brand_id","Type":"Int","Value":"45","Description":"品牌编号","HasList":"false"},
        ///             {"Key":"brand_name","Type":"String","Value":"谷歌","Description":"品牌名称","HasList":"false"},
        ///             {"Key":"type_id","Type":"Int","Value":"1","Description":"商品类型","HasList":"false"},
        ///             {"Key":"type_name","Type":"String","Value":"商品类型","Description":"商品类型","HasList":"false"},
        ///             {"Key":"num_iid","Type":"Int","Value":"1489161932","Description":"商品编号","HasList":"false"},
        ///             {"Key":"outer_id","Type":"String","Value":"34143554352","Description":"商品货号","HasList":"false"},
        ///             {"Key":"title","Type":"String","Value":"Google test item","Description":"商品名称","HasList":"false"},
        ///             {"Key":"pic_url","Type":"List","Value":["http:\/\/img\/038.jpg"],"Description":"商品图片主图，存在多图就以数组形式返回，中间用逗号隔开","HasList":"false"},
        ///             {"Key":"desc","Type":"String","Value":"这是一个好商品","Description":"pc端的商品详情","HasList":"false"},
        ///             {"Key":"wap_desc","Type":"String","Value":"Wap宝贝详情","Description":"移动端的商品详情","HasList":"false"},
        ///             {"Key":"list_time","Type":"DateTime","Value":"2009-10-22 14:22:06","Description":"商品创建时间","HasList":"false"},
        ///             {"Key":"modified","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"商品修改时间","HasList":"false"},
        ///             {"Key":"display_sequence","Type":"Int","Value":"10","Description":"商品排序号","HasList":"false"},
        ///             {"Key":"approve_status","Type":"String","Value":"On_Sale","Description":"商品状态","HasList":"false"},
        ///             {"Key":"sold_quantity","Type":"Int","Value":"88888","Description":"销售数量","HasList":"false"},
        ///             {"Key":"location","Type":"String","Value":"{'city':'杭州', 'state':'浙江'}","Description":"商品配送地区(此值为Himall系统存在)","HasList":"false"},
        ///             {"Key":"props_name","Type":"Int","Value":"适合季节#cln#[春季,秋季]#scln#适合年龄段#cln#[少年,青年]","Description":"商品扩展属性，(注：属性名称中的冒号':'被转换为：'#cln#'; 分号';'被转换为：'#scln#' )","HasList":"false"},
        ///             {"Key":"sub_stock","Type":"Int","Value":"1","Description":"商品库存","HasList":"false"},
        ///             {"Key":"skus","Type":"skus","Value":"","Description":"商品其他规格型号","HasList":"true","List":
        ///                 [
        ///                     {"Key":"sku_id","Type":"String","Value":"123","Description":"商品规格号","HasList":"false"},
        ///                     {"Key":"outer_sku_id","Type":"String","Value":"50786-4","Description":"商品货号","HasList":"false"},
        ///                     {"Key":"quantity","Type":"Int","Value":"3","Description":"商品数量","HasList":"false"},
        ///                     {"Key":"price","Type":"Decimal","Value":"200.07","Description":"商品单价","HasList":"false"},
        ///                     {"Key":"sku_properties_name","Type":"Int","Value":"132443","Description":"商品规格名称","HasList":"false"}
        ///                 ]
        ///             }
        ///         ]
        ///     }
        ///     
        /// ]
        /// </example>
        string UpdateProductQuantity(int num_iid, string sku_id, int quantity, int type);

        /// <summary>
        /// 修改商品销售状态 (上架， 下架， 入库)
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        /// <param name="num_iid " type="Number" required="必须">商品数字ID</param>
        /// <param name="approve_status " type="String" required="必须">商品状态（On_Sale）出售中/（Un_Sale）下架区/(In_Stock)仓库中,查看<a href='productstatus.html' target='_blank'>商品状态</a></param>
        /// <returns>
        /// {
        ///     "product_quantity_update_response":{
        ///         "item":{
        ///              "cid":132443,
        ///              "cat_name":"服装"
        ///              "type_id":20,
        ///              "type_name":"情侣装",
        ///              "brand_id":12,
        ///              "brand_name":"阿迪达斯",
        ///              "num_iid":1489161932,
        ///              "outer_id":"34143554352",
        ///              "title":"Google test item",
        ///              "pic_url":["http:\/\/img03.taobao.net\/bao\/uploaded\/i3\/T1HXdXXgPSt0JxZ2.8_070458.jpg","http:\/\/img03.taobao.net\/bao\/uploaded\/i3\/T1HXdXXgPSt0JxZ2.8_070458.jpg"],
        ///              "desc":"这是一个好商品",
        ///              "wap_desc":"Wap宝贝详情",
        ///              "list_time":"2009-10-22 14:22:06",
        ///              "modified":"2000-01-01 00:00:00",
        ///              "display_sequence":10,
        ///              "approve_status":"On_Sale",
        ///              "sold_quantity":8888,
        ///              "location":{"city":"杭州", "state":"浙江" },
        ///              "props_name":"适合季节#cln#[春季,秋季]#scln#适合年龄段#cln#[少年,青年]",
        ///              "sub_stock":1,
        ///              "skus":[{
        ///                  "sku_id":123,
        ///                  "outer_sku_id":"50786-4",
        ///                  "quantity":3,
        ///                  "price":"200.07",
        ///                  "sku_properties_name":"颜色:桔色;尺码:M"
        ///             }]      
        ///         }
        ///     }
        /// }
        /// </returns>
        /// <example>
        /// [
        ///     {"Key":"item","Type":"items","Value":"","Description":"返回商品的详细字段信息","HasList":"true","List":
        ///         [
        ///             {"Key":"cid","Type":"Int","Value":"132443","Description":"商品分类编号","HasList":"false"},
        ///             {"Key":"cat_name","Type":"String","Value":"服装","Description":"商品类目名称","HasList":"false"},
        ///             {"Key":"brand_id","Type":"Int","Value":"45","Description":"品牌编号","HasList":"false"},
        ///             {"Key":"brand_name","Type":"String","Value":"谷歌","Description":"品牌名称","HasList":"false"},
        ///             {"Key":"type_id","Type":"Int","Value":"1","Description":"商品类型","HasList":"false"},
        ///             {"Key":"type_name","Type":"String","Value":"商品类型","Description":"商品类型","HasList":"false"},
        ///             {"Key":"num_iid","Type":"Int","Value":"1489161932","Description":"商品编号","HasList":"false"},
        ///             {"Key":"outer_id","Type":"String","Value":"34143554352","Description":"商品货号","HasList":"false"},
        ///             {"Key":"title","Type":"String","Value":"Google test item","Description":"商品名称","HasList":"false"},
        ///             {"Key":"pic_url","Type":"List","Value":["http:\/\/img\/038.jpg"],"Description":"商品图片主图，存在多图就以数组形式返回，中间用逗号隔开","HasList":"false"},
        ///             {"Key":"desc","Type":"String","Value":"这是一个好商品","Description":"pc端的商品详情","HasList":"false"},
        ///             {"Key":"wap_desc","Type":"String","Value":"Wap宝贝详情","Description":"移动端的商品详情","HasList":"false"},
        ///             {"Key":"list_time","Type":"DateTime","Value":"2009-10-22 14:22:06","Description":"商品创建时间","HasList":"false"},
        ///             {"Key":"modified","Type":"DateTime","Value":"2000-01-01 00:00:00","Description":"商品修改时间","HasList":"false"},
        ///             {"Key":"display_sequence","Type":"Int","Value":"10","Description":"商品排序号","HasList":"false"},
        ///             {"Key":"approve_status","Type":"String","Value":"On_Sale","Description":"商品状态","HasList":"false"},
        ///             {"Key":"sold_quantity","Type":"Int","Value":"88888","Description":"销售数量","HasList":"false"},
        ///             {"Key":"location","Type":"String","Value":"{'city':'杭州', 'state':'浙江'}","Description":"商品配送地区(此值为Himall系统存在)","HasList":"false"},
        ///             {"Key":"props_name","Type":"Int","Value":"适合季节#cln#[春季,秋季]#scln#适合年龄段#cln#[少年,青年]","Description":"商品扩展属性，(注：属性名称中的冒号':'被转换为：'#cln#'; 分号';'被转换为：'#scln#' )","HasList":"false"},
        ///             {"Key":"sub_stock","Type":"Int","Value":"1","Description":"商品库存","HasList":"false"},
        ///             {"Key":"skus","Type":"skus","Value":"","Description":"商品其他规格型号","HasList":"true","List":
        ///                 [
        ///                     {"Key":"sku_id","Type":"String","Value":"123","Description":"商品规格号","HasList":"false"},
        ///                     {"Key":"outer_sku_id","Type":"String","Value":"50786-4","Description":"商品货号","HasList":"false"},
        ///                     {"Key":"quantity","Type":"Int","Value":"3","Description":"商品数量","HasList":"false"},
        ///                     {"Key":"price","Type":"Decimal","Value":"200.07","Description":"商品单价","HasList":"false"},
        ///                     {"Key":"sku_properties_name","Type":"Int","Value":"132443","Description":"商品规格名称","HasList":"false"}
        ///                 ]
        ///             }
        ///         ]
        ///     }
        ///     
        /// ]
        /// </example>
        string UpdateProductApproveStatus(int num_iid, string approve_status);

    }
}
