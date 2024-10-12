# SGS.OAD.DB

# Features

- 支援多種目標框架 (Target Framework)
  - .NET Framework `net47` `net471` `net472` `net48` `net481`
  - .NET `net6` `net8`
  - .NET Standard `2.0`
- 最低限度套件依賴，盡可能使用原生套件避免衝突
- 語法以相容所有目標框架為主，避免使用新版本語法糖
- 參考 Clean 與 Onion Architecture Pattern
- 導入 Fluent Design Pattern
- 支援非同步設計，提升執行效率
- 支援依賴注入，提升擴充性，可自行實作服務注入
- 支援歷程記錄，可以更好的了解套件使用狀況

> 💡目標框架參考微軟官方開發框架生命週期制定
> - [.NET Framework](https://learn.microsoft.com/zh-tw/lifecycle/products/microsoft-net-framework) 支援 LTS 版本，不支援有結束日期版本
> - [.NET / .NET Core](https://learn.microsoft.com/zh-tw/lifecycle/products/microsoft-net-and-net-core) 支援 .NET LTS 版本 (不支援 .NET Core)

# Installation

# How to Use

# Use Cases

# Future Improvement

- DES 加密應改為 AES
- 使用 HTTPS 端點
- 建立後端 WebAPI (並加入歷程記錄)

# References