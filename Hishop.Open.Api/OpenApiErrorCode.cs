using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Hishop.Open.Api
{
    /// <summary>
    /// 错误码
    /// </summary>
   public enum OpenApiErrorCode
   {
       #region  系统级错误
       /// <summary>
       /// 开发者权限不足
       /// </summary>
       /// <value>1</value>
       /// <remarks>针对某些接口的调用会设置部分权限或次数调用，请了解要调用的接口是否有足够的权限</remarks>
       [Description("开发者权限不足")]
       Insufficient_ISV_Permissions=1,

       /// <summary>
       /// 用户权限不足
       /// </summary>
       /// <value>2</value>
       /// <remarks>在调用接口过程中，可能某个会员或某个角色没有权限获取部分接口资料</remarks>
       [Description("用户权限不足")]
       Insufficient_User_Permissions=2,

       /// <summary>
       /// 远程服务出错
       /// </summary>
       /// <value>3</value>
       /// <remarks>在调用接口过程中，可能某个会员或某个角色没有权限获取部分接口资料</remarks>
       [Description("远程服务出错")]
       Remote_Service_Error = 3,


       /// <summary>
       /// 缺少方法名参数
       /// </summary>
       /// <value>4</value>
       /// <remarks>调用接口过程中缺少参数</remarks>
       [Description("缺少方法名参数")]
       Missing_Method = 4,


       /// <summary>
       /// 不存在的方法名
       /// </summary>
       /// <value>5</value>
       /// <remarks>调用了不存在的接口，接口无效</remarks>
       [Description("不存在的方法名")]
       Invalid_Method = 5,


       /// <summary>
       /// 非法数据格式
       /// </summary>
       /// <value>6</value>
       /// <remarks>传入的参数格式不符合接口规范要求</remarks>
       [Description("非法数据格式")]
       Invalid_Format = 6,

       /// <summary>
       /// 缺少签名参数
       /// </summary>
       /// <value>7</value>
       /// <remarks>接口缺少系统参数，sign</remarks>
       [Description("缺少签名参数")]
       Missing_Signature = 7,

       /// <summary>
       /// 非法签名
       /// </summary>
       /// <value>8</value>
       /// <remarks>签名(sign)错误，签名值(sign)不对</remarks>
       [Description("非法签名")]
       Invalid_Signature = 8,


       /// <summary>
       /// 缺少AppKey参数
       /// </summary>
       /// <value>9</value>
       /// <remarks>接口缺少系统参数,AppKey</remarks>
       [Description("缺少AppKey参数")]
       Missing_App_Key = 9,


       /// <summary>
       /// 非法的AppKey参数
       /// </summary>
       /// <value>10</value>
       /// <remarks>AppKey传入的值不对</remarks>
       [Description("非法的AppKey参数")]
       Invalid_App_Key= 10,

       /// <summary>
       /// 缺少时间戳参数
       /// </summary>
       /// <value>12</value>
       /// <remarks>接口缺少系统参数,timestamp</remarks>
       [Description("缺少时间戳参数")]
       Missing_Timestamp = 12,


       /// <summary>
       /// 非法的时间戳参数
       /// </summary>
       /// <value>13</value>
       /// <remarks>系统参数timestamp时间格式不符合接口规范要求</remarks>
       [Description("非法的时间戳参数")]
       Invalid_Timestamp = 13,


       /// <summary>
       /// 缺少必选参数
       /// </summary>
       /// <value>14</value>
       /// <remarks>检查接口的必填参数是否缺少或为空值</remarks>
       [Description("缺少必选参数")]
       Missing_Required_Arguments = 14,



       /// <summary>
       /// 非法的参数
       /// </summary>
       /// <value>15</value>
       /// <remarks>传入了接口不允许的非法参数值</remarks>
       [Description("非法的参数")]
       	Invalid_Arguments = 15,

       /// <summary>
       /// 请求被禁止
       /// </summary>
       /// <value>16</value>
       /// <remarks>服务端终止请求，以防恶意调用</remarks>
       [Description("请求被禁止")]
       	Forbidden_Request = 16,



       [Description("参数错误")]
       Parameter_Error = 17,

       /// <summary>
       /// 系统错误
       /// </summary>
       /// <value>18</value>
       /// <remarks>请重新尝试调用</remarks>
       [Description("系统错误")]
       System_Error = 18,
       #endregion


       #region  业务错误


       /// <summary>
       /// 缺少业务参数
       /// </summary>
       /// <value>501</value>
       /// <remarks>请重新尝试调用</remarks>
       [Description("缺少参数")]
       Missing_Parameters=501,


       /// <summary>
       /// 需要绑定用户昵称
       /// </summary>
       /// <value>502</value>
       /// <remarks>在调用会员的部分接口时，需要先绑定</remarks>
       [Description("需要绑定用户昵称")]
       Need_Binding_User = 502,


       /// <summary>
       /// 业务参数格式错误
       /// </summary>
       /// <value>503</value>
       /// <remarks>业务参数的格式不符合接口规范要求</remarks>
       [Description("参数格式错误")]
       	Parameters_Format_Error = 503,

       /// <summary>
       /// 查询不到指定的交易记录
       /// </summary>
       /// <value>504</value>
       /// <remarks>检查是否传入了正确的交易编号(tid)</remarks>
       [Description("交易不存在")]
       	Trade_not_Exists = 504,

       /// <summary>
       /// 非法交易
       /// </summary>
       /// <value>505</value>
       /// <remarks>重新调用</remarks>
       [Description("非法交易")]
       	Trade_is_Invalid = 505,


       /// <summary>
       /// 用户不存在
       /// </summary>
       /// <value>506</value>
       /// <remarks>检查指定的用户名是否输入正确</remarks>
       [Description("用户不存在")]
       User_not_Exists = 506,

       /// <summary>
       /// 非法的交易订单（或子订单）ID
       /// </summary>
       /// <value>507</value>
       /// <remarks>检查交易订单是否为当前用户的订单或者交易号正确</remarks>
       [Description("非法的交易订单（或子订单）ID")]
       Biz_Order_ID_is_Invalid = 507,

       /// <summary>
       /// 交易备注超出长度限制
       /// </summary>
       /// <value>508</value>
       /// <remarks>输入的交易备注号长度不符合系统要求</remarks>
       [Description("交易备注超出长度限制")]
       Trade_Memo_Too_Long=508,

       /// <summary>
       /// 页码条数超出长度限制
       /// </summary>
       /// <value>509</value>
       /// <remarks>page_no取值范围:大于零的整数; 默认值:1,page_size取值范围:大于零的整数; 默认值:40;最大值:100</remarks>
       [Description("页码条数超出长度限制")]
       Page_Size_Too_Long = 509,

       /// <summary>
       /// 开始时间晚于结束时间
       /// </summary>
       /// <value>510</value>
       /// <remarks>查询的开始时间要比结束时间要早</remarks>
       [Description("开始时间晚于结束时间")]
       Time_Start_End = 510,

       /// <summary>
       /// 结束时间晚于当前时间
       /// </summary>
       /// <value>511</value>
       /// <remarks>结束时间不要超过当前系统时间</remarks>
       [Description("结束时间晚于当前时间")]
       Time_End_Now = 511,

       /// <summary>
       /// 开始时间晚于当前时间
       /// </summary>
       /// <value>512</value>
       /// <remarks>开始时间不要超过当前系统时间</remarks>
       [Description("开始时间晚于当前时间")]
       Time_Start_Now=512,

       /// <summary>
       /// 物流公司不存在
       /// </summary>
       /// <value>513</value>
       /// <remarks>查询的物流公司不存在</remarks>
       [Description("物流公司不存在")]
       Company_not_Exists = 513,

       /// <summary>
       /// 运单号太长
       /// </summary>
       /// <value>514</value>
       /// <remarks>输入的运单号不符合格式</remarks>
       [Description("运单号太长")]
       Out_Sid_Too_Long=514,


       /// <summary>
       /// 商品库存不足
       /// </summary>
       /// <value>515</value>
       /// <remarks>商品库存不够，无法发货</remarks>
       [Description("商品库存不足")]
       Product_Stock_Lack=515,

       /// <summary>
       /// 订单状态不允许进行发货
       /// </summary>
       /// <value>516</value>
       /// <remarks>交易订单状态必须在已付款的状态或者货到付款的订单才允许发货</remarks>
       [Description("订单状态不允许进行发货")]
       Trade_Status_Send=516,

       /// <summary>
       /// 配送方式不存在
       /// </summary>
       /// <value>517</value>
       /// <remarks>添加要发货的配送方式</remarks>
       [Description("配送方式不存在")]
       ShippingMode_not_Exists = 517,


       /// <summary>
       /// 交易标记值不在指定范围之内
       /// </summary>
       /// <value>518</value>
       /// <remarks>交易备注标记只允许在1~6的正整数</remarks>
       [Description("交易标记值不在指定范围之内")]
       Trade_Flag_Too_Long = 518,


       /// <summary>
       /// 状态不在指定范围之内
       /// </summary>
       /// <value>519</value>
       /// <remarks>交易查询的状态不在规定状态范围内</remarks>
       [Description("状态不在指定范围之内")]
       Trade_Status_is_Invalid = 519,

       /// <summary>
       /// 查询条件(修改时间)跨度不能超过一天
       /// </summary>
       /// <value>520</value>
       /// <remarks>查询交易增量的开始和结束时间跨度不能超过一天</remarks>
       [Description("查询条件(修改时间)跨度不能超过一天")]
       Time_StartModified_AND_EndModified=520,

       /// <summary>
       /// 订单来自门店的订单
       /// </summary>
       /// <value>521</value>
       /// <remarks>选择正确的订单类型</remarks> 
       [Description("来自门店的订单")]
       Trade_is_Store=521,

       /// <summary>
       /// 不符合打印订单的要求
       /// </summary>
       /// <value>522</value>
       /// <remarks>检查订单的状态是否存在售后问题</remarks> 
       [Description("订单状态不允许打印")]
       Trade_Status_Print = 522,

       /// <summary>
       /// 打印订单失败
       /// </summary>
       /// <value>523</value>
       /// <remarks>更新订单物流信息和打印状态失败</remarks> 
       [Description("订单打印失败")]
       Trade_Print_Faild = 523,

       /// <summary>
       /// 打印订单失败
       /// </summary>
       /// <value>601</value>
       /// <remarks>获取单件商品失败</remarks> 
       [Description("商品不存在")]
       Product_Not_Exists = 601,

       /// <summary>
       /// 状态不在指定范围之内
       /// </summary>
       /// <value>602</value>
       /// <remarks>商品查询的状态不在规定状态范围内</remarks>
       [Description("状态不在指定范围之内")]
       Product_Status_is_Invalid = 602,

       /// <summary>
       /// 状态不在指定范围之内
       /// </summary>
       /// <value>603</value>
       /// <remarks>修改商品库存失败</remarks>
       [Description("修改商品库存失败")]
       Product_UpdateeQuantity_Faild = 603,

       /// <summary>
       /// 状态不在指定范围之内
       /// </summary>
       /// <value>604</value>
       /// <remarks>修改商品状态失败</remarks>
       [Description("修改商品状态失败")]
       Product_ApproveStatus_Faild = 604,
       #endregion
   }

   public static class OpenApiErrorMessage
   {
       public static string GetEnumDescription(Enum enumSubitem)
       {
           string strValue = enumSubitem.ToString();

           FieldInfo fieldinfo = enumSubitem.GetType().GetField(strValue);
           Object[] objs = fieldinfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
           if (objs == null || objs.Length == 0)
           {
               return strValue;
           }
           else
           {
               DescriptionAttribute da = (DescriptionAttribute)objs[0];
               return da.Description;
           }

       }


       public static string ShowErrorMsg(Enum enumSubitem,string fields)
       {
           string submsg=GetEnumDescription(enumSubitem).Replace("_"," ");
           string format="{{\"error_response\":{{\"code\":\"{0}\",\"msg\":\"{1}:{2}\",\"sub_msg\":\"{3}\"}}}}";
           return string.Format(format, Convert.ToInt16(enumSubitem).ToString(), enumSubitem.ToString().Replace("_", " "), fields, submsg);
       }

   }
}
