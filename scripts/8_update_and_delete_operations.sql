USE SALES_DB;
GO

-- update a customer's contact details
UPDATE store.customers
SET Email = 'new.email@example.com',
	PhoneNumber = '123-456-7890'
WHERE CustomerID = 2;
GO

-- increase all product prices by a specified percentage
UPDATE store.products
SET Price = Price * 1.10;
GO

-- delete a sales record based on a condition
DELETE FROM store.sales
WHERE SaleID = 3;
GO

-- delete customers with no associated sales
DELETE FROM store.customers
WHERE CustomerID NOT IN (
	SELECT DISTINCT CustomerID
	FROM store.sales
	WHERE CustomerID IS NOT NULL
);
