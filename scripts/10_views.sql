USE SALES_DB;
GO

-- create a view displaying: 
-- Sale ID 
-- Customer Name 
-- Product Name 
-- Quantity 
-- Total Sale Amount 
CREATE VIEW store.vw_sales_summary AS
SELECT
	s.SaleID,
	CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
	p.ProductName,
	si.Quantity,
	si.TotalAmount AS TotalSaleAmount
FROM store.sales s
JOIN store.sale_items si
	ON s.SaleID = si.SaleID
JOIN store.products p
	ON si.ProductID = p.ProductID
JOIN store.customers c
	ON s.CustomerID = c.CustomerID;
GO

-- Query the view to retrieve sales data
SELECT * FROM store.vw_sales_summary;
GO
