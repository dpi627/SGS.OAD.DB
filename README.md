![](https://img.shields.io/badge/SGS-OAD-orange) 
![](https://img.shields.io/badge/proj-Database%20Connection%20String-purple) 
![](https://img.shields.io/badge/-4.7-3484D2?logo=dotnet)
![](https://img.shields.io/badge/-4.8-3484D2?logo=dotnet)
![](https://img.shields.io/badge/-Standard%202.0-056473?logo=dotnet)
![](https://img.shields.io/badge/-6-512BD4?logo=dotnet)
![](https://img.shields.io/badge/-8-512BD4?logo=dotnet)
![](https://img.shields.io/badge/-NuGet-004880?logo=nuget)
![](https://img.shields.io/badge/-Git-666?logo=git)
![](https://img.shields.io/badge/-GitHub-666?logo=github)
![](https://img.shields.io/badge/-Gitea-666?logo=gitea)
![](https://img.shields.io/badge/Anthropic-191919?logo=anthropic)
![](https://img.shields.io/badge/OpenAI-412991?logo=openai) 
![](https://img.shields.io/badge/GitHub_Copilot-555?logo=githubcopilot)
![](https://img.shields.io/badge/draw.io-555?logo=diagrams.net)
![](https://img.shields.io/badge/Markdown-555?logo=markdown)

# 🛡️SGS.OAD.DB

SGS Taiwan 內部套件，由 OAD 開發，透過呼叫內部網路服務 (Web API) 取得加密過的資料庫使用者帳號與密碼。資料會於套件內部進行解密，組成可用之資料庫連線字串。旨在避免將帳號密碼儲存於專案內，從而提升資安層級與管理彈性。

# ✨Features

- 支援多種目標框架 (Target Framework)
  - .NET Framework `net47` `net471` `net472` `net48` `net481`
  - .NET `net6` `net8`
  - .NET Standard `2.0`
- 最低限度依賴，功能盡可能使用目標框架原生 API 實現
- 語法以相容所有目標框架優先，避免使用新版本語法糖
- 參考 Clean 與 Onion Architecture Pattern 實現關注點分離
- 導入 Fluent Design Pattern 提升使用體驗
- 支援非同步設計，提供非同步方法提升系統效率
- 支援 Dependency Injection，可自行實作服務注入
- 開發快取機制，避免後端 Web API 負載過重

> 💡目標框架參考微軟官方開發框架 [.NET Framework](https://learn.microsoft.com/zh-tw/lifecycle/products/microsoft-net-framework) 與 [.NET / .NET Core](https://learn.microsoft.com/zh-tw/lifecycle/products/microsoft-net-and-net-core) 之生命週期制定

# 💻Installation

透過 NuGet Package Manager 進行安裝，安裝之前需先新增內部 Package Source

## ➕Add Interal Package Source

- 開啟 Tools > Options，搜尋 Package Sources，加入自訂來源
- 或於方案或專案目錄加入檔案 `nuget.config` 內容如下:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <!-- {SourceName} is up to you -->
    <add key="{SourceName}" value="\\twfs007\SGSSHARE\OAD\nuget" />
  </packageSources>
</configuration>
```

>💡`nuget.config` 放置於方案目錄可以作用於方案內所有專案

## 📦NuGet Package Manager

- 加入自訂來源後，可透過 NuGet Package Manager 搜尋並安裝
- 如果搜尋不到，請檢查 Package Source 是否為自訂來源或 All

![](./assets/nuget-package-manager.png)

# 🛠️How to Use

引用命名空間

```cs
using SGS.OAD.DB;
```

## 🚀Quick Start

直接取得連線字串

```cs
// a quick start to getting the connection string
var connectionString = DbInfoBuilder
    .Init()                 // 1. initialize builder
    .SetServer("TWDB000")   // 2. set server name
    .SetDatabase("SGSLims") // 3. set database name
    .Build()                // 4. build DbInfo data object
    .ConnectionString;      // 5. get your connection string ❤️
```

## 🧑‍💻Step by Step

依序建立 `builder` > `db object`，然後取得連線字串 `db.ConnectionString`

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

# 🔗Connection String

連線字串樣板與獲取範例如下，常用參數應均已具備

```cs
//Data Source={server};Initial Catalog={database};User ID={uid};Password={pwd};Application Name={app};Connect Timeout={timeout};TrustServerCertificate={servercertificate};
"Data Source=TWDB009;Initial Catalog=SGSLims;User ID=db_user;Password=1234;Application Name=SYSOP;Connect Timeout=30;TrustServerCertificate=True;"
```

# 📚Use Cases

除了前述基本使用方法，以下也介紹一些其他使用情境

## ⚙️Other Builder Settings

除了伺服器與資料庫必須設定，其餘皆為選擇性(已有預設值)，可依照需求自行修改設定。

```cs
var builder = DbInfoBuilder.Init()
    .SetServer("TWDB000")
    .SetDatabase("SGSLims")
    .SetAppName("MyAppName")     // 應用程式名稱(建議設定)
    .SetTimeout(60)              // 連線逾時，預設 30
    .SetServerCertificate(false) // 信任資料庫憑證，預設 true
    .SetLanguage(ProgramLanguage.Delphi) // 程式語言，預設 C#
    .SetDatabaseRole(DatabaseRole.db_owner); // 角色，預設 db_datawriter
```

>💡應用程式名稱 `AppName` 建議設定，方便 DBA 管理連線來源

## 🌐Change Endpoint

套件內建 `ApiUrlBuilder` 用以建構 API 網址，包含替換端點以及其他設定

```cs
var builder = DbInfoBuilder.Init()
    .SetServer("TWDB000")
    .SetDatabase("SGSLims")
    .SetApiUrl(api => api
        .SetEndpoint("https://some.where.com:9527/api/")
        //.SetDatabaseRole(DatabaseRole.db_owner) //可於上層設定
        //.SetLanguage(ProgramLanguage.DelphiXE)  //可於上層設定
    );
```

## 🧩Dependency Injection

可以自行實作介面並注入，用以取代原本服務

```cs
using SGS.OAD.DB.Services.Interfaces;

// implement your own services
IUserInfoService userInfoService = new YourUserInfoService();
IDecryptService decryptService = new YourDecryptService();

var builder = DbInfoBuilder
    // inject your own services
    .Init(userInfoService, decryptService)
    .SetServer("TWDB000")
    .SetDatabase("SGSLims");
```

## ⏳Asynchronous

套件提供非同步方法，使用上可搭配 `.ConfigureAwait(false)`

```cs
var builder = DbInfoBuilder.Init()
    .SetServer("TWDB000")
    .SetDatabase("SGSLims");
    
// build database object asynchronously
var db = await builder.BuildAsync();
```

### ⚖️ConfigureAwait

- ASP.NET 具備「同步上下文」，ASP.NET Core 預設沒有
- 於 ASP.NET MVC 使用非同步方法結束時
  - 如須返回原執行緒 (例如處理 `HttpContext`、更新 View)
  ```cs
  // 預設 ConfigureAwait(true) 直接使用即可
  var db = await builder.BuildAsync();
  // 返回原始上下文以使用 ViewBag
  ViewBag.Data = db;
  ```
  - 如不須返回原執行緒
  ```cs
  // ConfigureAwait(fasle) 可避免切換上下文與部分 deadlock
  var db = await builder.BuildAsync().ConfigureAwait(false);
  // 不須返回原始上下文
  OtherProcess(db);
  ```

>💡「同步上下文」即 Synchronization Context，是 .NET 用於管理執行緒上下文的機制

## 🔐File I/O Account

這部分與資料庫連線字串無關，旨在利用此套件機制管理特殊I/O權限帳密(例如 efile_tw)。可避免帳號密碼留存於各系統組態檔，造成管理成本與資安風險。

```cs
var builder = DbInfoBuilder.Init()
    .SetServer("TWDB000")
    .SetDatabase("SGSLims");
    // set database role to "db_filewriter"
    .SetDatabaseRole(DatabaseRole.db_filewriter);

var db = builder.Build();

// get user id and password
Console.Write($"UID: {db.UserId}, PWD: {db.Password}");
```

## 💾Data Chahe

函式庫具備快取機制，避免連續呼叫，減輕後端 Web API 負載

```cs
// 000.UAT 首次創建，會呼叫 API
var db1 = DbInfoBuilder.Init().SetServer("000").SetDatabase("UAT").Build();
// 111.PRD 首次創建，會呼叫 API
var db2 = DbInfoBuilder.Init().SetServer("111").SetDatabase("PRD").Build();
// 000.UAT 重複創建，不會呼叫 API (已存在快取)
var builder = DbInfoBuilder.Init().SetServer("000").SetDatabase("UAT");
var db3 = builder.Build();

builder.ClearCache(); // 清除快取

// 000.UAT 重複創建，會呼叫 API (快取已清除)
builder = builder.SetServer("000").SetDatabase("UAT");
var db4 = builder.Build();
// 000.UAT 重新創建，會呼叫 API (重建 = 清除快取 + 創建)
builder = builder.SetServer("000").SetDatabase("UAT");
var db4 = builder.Rebuild();
```

>💡上述範例也支援非同步 `BuildAsync()`，函式庫已使用 Thread Safety 類別，避免非同步操作可能引起之 race condition 問題

# 🏗️Project Architecture

```js
📁 SGS.OAD.DB
  📁 Builders     //建構器
  📁 Enums        //列舉
  📁 Models       //資料結構、DTO
  📁 Services     //服務
  📁 Utilities    //共用工具
  📄 config.xml   //組態檔、參數檔
```

- 參考 Clean Architecture 與 Onion Architecture 設計架構，實現關注點分離
- Builder 採用 Fluent Design Pattern 提升操作體驗
- 服務部分使用 Interface 解耦，支援自行實作介面注入 (Dependency Injection)
- 系統參數或預設值使用 config.xml 管理 (避開 app.config 或 appsettings.json)
- config.xml 設定為嵌入資源，部署之後不可見，避免(也無法)修改

# 📋TODO

- 實作解密服務
- 開發管理工具
- 加密方法改為 AES
- 改用 HTTPS 端點
- 重構後端 WebAPI
- WebAPI 紀錄操作歷程
- 使用混淆器