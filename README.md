# SGS.OAD.DB

# Features

- 支援多種目標框架 (Target Framework)
  - .NET Framework `net47` `net471` `net472` `net48` `net481`
  - .NET `net6` `net8`
  - .NET Standard `2.0`
- 最低限度依賴，僅 .NET Framework 需引用 System.Net.Http
- 其餘功能皆使用目標框架原生 API 實現
- 語法以相容所有目標框架優先，避免使用新版本語法糖
- 參考 Clean 與 Onion Architecture Pattern
- 導入 Fluent Design Pattern
- 支援非同步設計，提供非同步方法
- 支援 DI 依賴注入，可自行實作服務注入

> 💡目標框架參考微軟官方開發框架生命週期制定
> - [.NET Framework](https://learn.microsoft.com/zh-tw/lifecycle/products/microsoft-net-framework) 支援 LTS 版本，不支援有結束日期版本
> - [.NET / .NET Core](https://learn.microsoft.com/zh-tw/lifecycle/products/microsoft-net-and-net-core) 支援 .NET LTS 版本 (不支援 .NET Core)

📂📁🗂️🗃️📄📝📜📑🧾💻🖥️⚙️🛠️📦🔧🔍🧪🔬✅❌🚦🗜️📚📦🗄️📊💾📅🚀🔒🔖

# Installation

- nuget.config
- NuGet Package Manager

# How to Use

## Quick Start

```cs
using SGS.OAD.DB;

// a quick start to getting the connection string
var connectionString = DbInfoBuilder
    .Init()                 // 1. initialize builder
    .SetServer("TWDB000")   // 2. set server name
    .SetDatabase("SGSLims") // 3. set database name
    .Build()                // 4. build DbInfo data object
    .ConnectionString;      // 5. get your connection string ❤️
```

## Step by Step

```cs
// 1. create builder
var builder = DbInfoBuilder.Init()
    .SetServer("TWDB000")
    .SetDatabase("SGSLims");
// 2. build data object
var db = builder.Build();
// 3. get the connection string
var connectionString = db.ConnectionString;      
```

# Use Cases

# Architecture

```js
📁 SGS.OAD.DB
  📁 Builders     //建構器
  📁 Enums        //列舉
  📁 Models       //資料結構、DTO
  📁 Services     //服務
  📁 Utilities    //共用工具
  📄 config.xml   //組態檔、參數檔
```

# Future Improvement

- 實作解密服務
- 開發管理工具
- DES 加密應改為 AES
- 改用 HTTPS 端點
- 重構後端 WebAPI
- WebAPI 建立完整歷程記錄

# References