﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;

// 有关程序集的常规信息通过下列属性集
// 控制。更改这些属性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("SMS")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("长沙海商网络技术有限公司")]
[assembly: AssemblyProduct("手机短信发送插件")]
[assembly: AssemblyCopyright("版权所有 (C) 2002-2010 长沙海商网络技术有限公司")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 属性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("fe797000-8c86-4fd9-b563-ab45d4f5d093")]

// 程序集的版本信息由下面四个值组成:
//
//      主版本
//      次版本 
//      内部版本号
//      修订号
//
// 可以指定所有这些值，也可以使用“内部版本号”和“修订号”的默认值，
// 方法是按如下所示使用“*”:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.*")]
[assembly: AssemblyInformationalVersion("1.0.0")]

#if DEBUG
[assembly: AssemblyDescription("Debug")]
#else
[assembly: AssemblyDescription("Release")]
#endif

[assembly: CLSCompliant(true)]