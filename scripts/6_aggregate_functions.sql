USE SALES_DB;
GO

-- count the total number of customers
SELECT COUNT(*) AS TotalCustomers FROM store.customers;
GO

-- calculate the total sales value
SELECT SUM(TotalAmount) AS TotalSalesValue FROM store.sale_items;
GO

-- calculate the average product price
SELECT AVG(Price) AS AverageProductPrice FROM store.products;
GO

-- identify the highest and lowest product prices
SELECT
	MAX(Price) AS HighestProductPrice,
	MIN(Price) AS LowestProductPrice
	FROM store.products;
GO

-- calculate total sales per product
SELECT 
	p.ProductName,
	SUM(si.TotalAmount) AS TotalSalesPerProduct
FROM store.sale_items si
JOIN store.products p
	ON si.ProductID = p.ProductID
GROUP BY p.ProductName
ORDER BY TotalSalesPerProduct DESC;