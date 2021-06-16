using System.Reflection;
using System.Runtime.InteropServices;
using System;

// 有关程序集的常规信息通过下列属性集
// 控制。更改这些属性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("PaymentInterface")]

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 属性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("40a51082-b14c-4d21-aadb-1a7c33a8db38")]

// 公司及版权信息
[assembly: AssemblyCompany("长沙海商网络技术有限公司")]
[assembly: AssemblyCopyright("版权所有 (C) 2002-2010 长沙海商网络技术有限公司")]

// 程序集的版本信息由下面四个值组成:
//
//      主版本
//      次版本 
//      内部版本号
//      修订号
//
// 可以指定所有这些值，也可以使用“修订号”和“内部版本号”的默认值，
// 方法是按如下所示使用“*”:

// 产品及版本信息
[assembly: AssemblyProduct("支付接口")]
[assembly: AssemblyInformationalVersion("1.0.2")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 文件及程序集版本信息
[assembly: AssemblyVersion("1.0.2.*")]

// 注明程序集是调试或发布版本
#if DEBUG
[assembly: AssemblyDescription("Debug")]
#else
[assembly: AssemblyDescription("Release")]
#endif

[assembly: CLSCompliant(true)]