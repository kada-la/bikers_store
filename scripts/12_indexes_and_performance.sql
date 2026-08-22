USE SALES_DB;
GO

-- create an index on frequently queried columns 
CREATE NONCLUSTERED INDEX idx_sales_saledate ON store.sales (SaleDate);
GO

CREATE NONCLUSTERED INDEX idx_sales_customerid ON store.sales (CustomerID);
GO

-- briefly explain why indexes are important
-- Without an index, if you search for an order placed on a specific day,
-- SQL Server has to perform a Table Scan—meaning it looks through every
-- single row in the table from top to bottom (like reading an entire textbook
-- cover-to-cover just to find one mention of a word).

-- An index creates a specialized, sorted lookup pointer structure (like an index
-- at the back of a textbook). Instead of searching everything, SQL Server looks
-- up the value in the index and jumps straight to the exact memory location of
-- the row it needs, radically slashing disk activity and execution time.

-- demonstrate query performance improvement using indexes
-- Turn on statistics to see the computational impact in the messages tab
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

-- Run a query targeting our indexed column
SELECT SaleID, CustomerID, SaleDate 
FROM store.sales 
WHERE SaleDate >= DATEADD(DAY, -30, GETDATE());

-- Turn statistics back off
SET STATISTICS IO OFF;
SET STATISTICS TIME OFF;
GO
