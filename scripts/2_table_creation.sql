USE SALES_DB;
GO

-- create schema for the sales database
CREATE SCHEMA store;
GO

--- create tables for the sales database
CREATE TABLE store.categories (
	CategoryID INT IDENTITY(1,1) PRIMARY KEY,
	CategoryName NVARCHAR(50) NOT NULL,
	Description NVARCHAR(255),
	IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE store.customers (
	CustomerID INT IDENTITY(1,1) PRIMARY KEY,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	Email NVARCHAR(100) UNIQUE NOT NULL,
	PhoneNumber NVARCHAR(15),
	Address NVARCHAR(255),
	City NVARCHAR(50),
	County NVARCHAR(50),
	PostalCode NVARCHAR(10),
	Country NVARCHAR(50)
);

CREATE TABLE store.products (
	ProductID INT IDENTITY(1,1) PRIMARY KEY,
	ProductName NVARCHAR(100) NOT NULL,
	CategoryID INT NOT NULL,
	Description NVARCHAR(255),
	Price DECIMAL(10, 2) NOT NULL,
	StockQuantity INT NOT NULL,
	IsActive BIT NOT NULL DEFAULT 1,
	CONSTRAINT FK_Products_Categories
		FOREIGN KEY (CategoryID)
		REFERENCES store.categories(CategoryID)
);

CREATE TABLE store.sales (
	SaleID INT IDENTITY(1,1) PRIMARY KEY,
	CustomerID INT NOT NULL,
	SaleDate DATETIME NOT NULL DEFAULT GETDATE(),
	CONSTRAINT FK_Sales_Customers
		FOREIGN KEY (CustomerID)
		REFERENCES store.customers(CustomerID)
);

CREATE TABLE store.sale_items (
	SaleItemID INT IDENTITY(1,1) PRIMARY KEY,
	SaleID INT NOT NULL,
	ProductID INT NOT NULL,
	Quantity INT NOT NULL,
	UnitPrice DECIMAL(10, 2) NOT NULL,
	TotalAmount AS (Quantity * UnitPrice) PERSISTED,
	CONSTRAINT FK_SaleItems_Sales
		FOREIGN KEY (SaleID)
		REFERENCES store.sales(SaleID)
		ON DELETE CASCADE,
	CONSTRAINT FK_SaleItems_Products
		FOREIGN KEY (ProductID)
		REFERENCES store.products(ProductID)
);
