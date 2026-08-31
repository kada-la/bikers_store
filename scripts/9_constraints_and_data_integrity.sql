USE SALES_DB;
GO

-- Demonstrate how constraints prevent invalid data entry
-- this will fail with a UNIQUE KEY violation error
INSERT INTO store.customers (FirstName, LastName, Email, City, Country)
VALUES ('Jane', 'Doe', 'sarah.a@domain.com', 'Nairobi', 'Kenya');
GO

-- this will fail because ProductID 999 does not exist in store.products
-- because of the FOREIGN KEY constraint
INSERT INTO store.sale_items (SaleID, ProductID, Quantity, UnitPrice)
VALUES (1, 999, 2, 50.00);
GO