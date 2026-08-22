USE SALES_DB;
GO
-- Three reports management would realistically ask for, and why.

-- Report 1: Revenue by Product Category
-- Why management needs this: tells the business which category
-- (Mountain, Road, Electric, Accessories) is actually driving revenue,
-- which informs purchasing focus, supplier negotiation, and marketing spend.
SELECT
	cat.CategoryName,
	COUNT(DISTINCT si.SaleItemID)	AS LineItemsSold,
	SUM(si.Quantity)				AS UnitsSold,
	SUM(si.TotalAmount)				AS TotalRevenue
FROM store.sale_items si
JOIN store.products p
	ON si.ProductID = p.ProductID
JOIN store.categories cat
	ON p.CategoryID = cat.CategoryID
GROUP BY cat.CategoryName
ORDER BY TotalRevenue DESC;
GO

-- Report 2: Top Customers by Total Spend
-- Why management needs this: identifies the highest-value customers so
-- the business knows who to prioritize for loyalty offers, account
-- management, or early access to new stock.
SELECT TOP 5
	c.CustomerID,
	CONCAT(c.FirstName, ' ', c.LastName)	AS CustomerName,
	c.City,
	c.Country,
	COUNT(DISTINCT s.SaleID)	AS NumberOfTransactions,
	SUM(si.TotalAmount)			AS TotalSpend
FROM store.customers c
JOIN store.sales s
	ON c.CustomerID = s.CustomerID
JOIN store.sale_items si
	ON s.SaleID = si.SaleID
GROUP BY c.CustomerID, c.FirstName, c.LastName, c.City, c.Country
ORDER BY TotalSpend DESC;
GO

-- Report 3: Sales Performance by County
-- Why management needs this: a Kenya-based sales org needs a regional
-- view — which counties are generating the most revenue and traffic —
-- to guide decisions on stock allocation, delivery logistics, or where
-- to open the next physical outlet.
SELECT
	c.County,
	COUNT(DISTINCT s.SaleID)			AS NumberOfTransactions,
	SUM(si.TotalAmount)					AS TotalRevenue,
	CAST(AVG(si.TotalAmount) AS DECIMAL(10,2)) AS AvgLineValue
FROM store.customers c
JOIN store.sales s
	ON c.CustomerID = s.CustomerID
JOIN store.sale_items si
	ON s.SaleID = si.SaleID
GROUP BY c.County
ORDER BY TotalRevenue DESC;
GO