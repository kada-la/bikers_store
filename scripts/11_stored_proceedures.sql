USE SALES_DB;
GO

-- create a stored procedure to insert a new customer 
CREATE PROCEDURE store.usp_AddCustomer
	@FirstName NVARCHAR(50),
	@LastName NVARCHAR(50),
	@Email NVARCHAR(100),
	@PhoneNumber NVARCHAR(20),
	@City NVARCHAR(50),
	@Country NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO store.customers (FirstName, LastName, Email, PhoneNumber, City, Country)
	VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @City, @Country);
END;
GO

-- create a stored procedure to retrieve all sales for a given customer
CREATE PROCEDURE store.usp_GetCustomerSales
	@CustomerID INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		s.SaleID,
		s.SaleDate,
		p.ProductName,
		si.Quantity,
		si.UnitPrice,
		si.TotalAmount
	FROM store.sales s
	JOIN store.sale_items si ON s.SaleID = si.SaleID
	JOIN store.products p ON si.ProductID = p.ProductID
	WHERE s.CustomerID = @CustomerID
	ORDER BY s.SaleDate DESC;
END;
GO

-- execute the stored procedures and display results 
-- execute to insert a new customer
EXEC store.usp_AddCustomer 
	@FirstName = 'Patrick', 
	@LastName = 'Omondi', 
	@Email = 'p.omondi@email.co.ke',
	@PhoneNumber = '0755667788',
	@City = 'Nairobi',
	@Country = 'Kenya';
GO

-- execute to view sales history for a specific customer
EXEC store.usp_GetCustomerSales @CustomerID = 2;
GO
