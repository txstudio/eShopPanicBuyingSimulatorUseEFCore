# 電子商務網站搶購模擬 - 使用 Entity Framework Core

這是一個模擬電子商務網站遇到在大量商品訂購需求中是否會有商品超賣的情況 - 使用 Entity Framework Core 進行資料存取

本專案使用資料庫與程式語言如下

- [Microsoft SQL Server in Docker](https://hub.docker.com/r/microsoft/mssql-server-linux/)
- [.NET Core 2.1 ConsoleApplication](https://docs.microsoft.com/zh-tw/dotnet/core/)

> 本範例程式碼有一個對應的 Stored Procedure 版本，並使用 ADO.NET 進行資料存取
> [txstudio/eShopPanicBuyingSimulatorUseStoreProcedure](https://github.com/txstudio/eShopPanicBuyingSimulatorUseStoreProcedure)

## 資料庫

範例資料庫使用 Microsoft SQL Server in Docker 的 Image 建立，可以使用下列指令碼啟用 Container

```
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Pa$$w0rd' -p 1433:1433 -d
```

## .NET Core 應用程式

使用 Entity Framework Core 進行資料庫存取，並透過 Code First 的方式建立與初始化資料表物件

> 在資料夾中找不到初始化資料庫的 Transact-SQL 檔案是正常的

## 參考資料
- [Concurrency Tokens - EF Core | Microsoft Docs](https://docs.microsoft.com/en-us/ef/core/modeling/concurrency)
- [Advanced Entity Framework Scenarios for an MVC Web Application (10 of 10) - No-Tracking Queries](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/advanced-entity-framework-scenarios-for-an-mvc-web-application#no-tracking-queries)
- [EntityFramework.Docs/Sample.cs at master · aspnet/EntityFramework.Docs · GitHub](https://github.com/aspnet/EntityFramework.Docs/blob/master/samples/core/Saving/Saving/Concurrency/Sample.cs)
