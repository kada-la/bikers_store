USE SALES_DB;
GO

-- retrieve products priced above a specified amount
SELECT ProductName, Price FROM store.products
WHERE Price > 800.00
ORDER BY Price ASC;

-- display customers whose names start with a specific letter
SELECT FirstName, LastName, Email FROM store.customers
WHERE FirstName LIKE 'A%' OR LastName LIKE 'A%'

-- sort sales by transaction date (most recent first)
SELECT
	s.SaleID,
	s.SaleDate,
	s.CustomerID,
	p.ProductName,
	si.Quantity,
	si.UnitPrice,
	si.TotalAmount
FROM store.sales s
JOIN store.sale_items si
	ON s.SaleID = si.SaleID
JOIN store.products p
	ON si.ProductID = p.ProductID
ORDER BY s.SaleDate DESC;

-- display the top 5 most expensive products
SELECT TOP 5
	ProductID,
	ProductName,
	Price,
	StockQuantity,
	IsActive
FROM store.products
ORDER BY Price DESC;