USE SALES_DB;
GO

-- display sales with customer names and product names
SELECT
	s.SaleID,
	s.SaleDate,
	c.FirstName,
	c.LastName,
	p.ProductName,
	si.Quantity,
	si.UnitPrice,
	si.TotalAmount
FROM store.sales s
JOIN store.sale_items si
	ON s.SaleID = si.SaleID
JOIN store.products p
	ON si.ProductID = p.ProductID
JOIN store.customers c
	ON s.CustomerID = c.CustomerID
GO

-- calculate total sales per customer
SELECT 
	c.FirstName,
	c.LastName,
	SUM(si.TotalAmount) AS TotalSalesPerCustomer
FROM store.customers c
JOIN store.sales s
	ON c.CustomerID = s.CustomerID
JOIN store.sale_items si
	ON s.SaleID = si.SaleID
GROUP BY c.FirstName, c.LastName;
GO

-- calculate total revenue per product
SELECT 
	p.ProductName,
	SUM(si.TotalAmount) AS TotalRevenuePerProduct
FROM store.products p
JOIN store.sale_items si
	ON p.ProductID = si.ProductID
GROUP BY p.ProductName;
GO

-- identify customers who have never made a purchase
SELECT
	c.CustomerID,
	c.FirstName,
	c.LastName,
	c.Email
FROM store.customers c
LEFT JOIN store.sales s
	ON c.CustomerID = s.CustomerID
WHERE s.SaleID IS NULL;
GO