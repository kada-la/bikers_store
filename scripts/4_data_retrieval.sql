USE SALES_DB;
GO

-- display all customers
SELECT * FROM store.customers;
GO

-- retrieve customers from a specific city
SELECT * FROM store.customers
WHERE city = 'Nairobi';
GO

-- or country
SELECT * FROM store.customers
WHERE country = 'Kenya';
GO

-- display all products sorted by price (highest to lowest)
SELECT * FROM store.products
WHERE price IS NOT NULL
ORDER BY price DESC;

-- retrieve all sales made in the last 30 days
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
WHERE s.SaleDate >= DATEADD(DAY, -30, GETDATE())
ORDER BY s.SaleDate DESC;

-- display all sales for a specific customer
SELECT
	s.SaleID,
	s.SaleDate,
	p.ProductName,
	si.Quantity,
	si.UnitPrice,
	si.TotalAmount
FROM store.sales s
JOIN store.sale_items si
	ON s.SaleID = si.SaleID
JOIN store.products p
	ON si.ProductID = p.ProductID
WHERE s.CustomerID = 2;