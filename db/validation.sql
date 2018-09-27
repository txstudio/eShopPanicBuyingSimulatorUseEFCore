USE [eShopUseEFCore]
GO

--取得商品目前現有的庫存量
SELECT a.[ProductNo]
	,b.[Schema]
	,b.[Name]
	,a.[Storage]
FROM [Products].[ProductStorages] a
	INNER JOIN [Products].[ProductMains] b ON a.[ProductNo] = b.[No]
GO

--取得所有已下訂的商品數量
SELECT a.[ProductNo]
	,b.[Schema]
	,b.[Name]
	,SUM([Quantity]) [TotalQuantity]
FROM [Orders].[OrderDetails] a
	INNER JOIN [Products].[ProductMains] b ON a.[ProductNo] = b.[No]
GROUP BY a.[ProductNo]
	,b.[Schema]
	,b.[Name]
GO

--檢視事件紀錄
SELECT [No]
	,[EventDateTime]
	,[MemberGUID]
	,[ProductSchema]
	,[ProductName]
	,[OrginalStorage]
	,[Quantity]
	,[Elapsed]
	,[IsSuccess]
	,[Exception]
	,[Retry]
FROM [Events].[EventLogs]
ORDER BY [No] ASC
GO

--從第一筆與最後一筆訂單時間取得測試所需時間
DECLARE @Start DATETIMEOFFSET
DECLARE @End DATETIMEOFFSET

SET @Start = (
	SELECT TOP(1) [OrderDate] FROM [Orders].[OrderMains]
	ORDER BY [OrderDate] ASC
)
SET @End = (
	SELECT TOP(1) [OrderDate] FROM [Orders].[OrderMains]
	ORDER BY [OrderDate] DESC
)
SELECT @Start, @End, DATEDIFF(SECOND,@Start,@End)
GO

/* 驗證訂單序號與訂單編號是否有誤 */
SELECT [No]
	,[Schema]
	,[OrderDate]
	,[MemberGUID]
FROM [Orders].[OrderMains]
ORDER BY [No] ASC
GO

SELECT [No]
	,[Schema]
	,[OrderDate]
	,[MemberGUID] 
FROM [Orders].[OrderMains]
ORDER BY [Schema] ASC
GO