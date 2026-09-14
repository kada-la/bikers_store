USE SALES_DB;
GO

-- ==========================================================
-- 14_suppliers_table.sql
-- Create Suppliers Reference Table
-- Designed for Unit4 ERP Integration (Shadow / Reference Cache)
-- ==========================================================

IF OBJECT_ID('store.Suppliers', 'U') IS NULL
BEGIN
	CREATE TABLE store.Suppliers (
		SupplierId NVARCHAR(50) NOT NULL, -- Alphanumeric Supplier ID from Unit4 ERP
		SupplierName NVARCHAR(200) NOT NULL, -- Cached display name for local queries/reports
		Status CHAR(1) NOT NULL DEFAULT 'N', -- Status: 'N' (Normal/Active), 'C' (Closed/Inactive)
		Email NVARCHAR(100) NULL,
		PhoneNumber NVARCHAR(20) NULL,
		Address NVARCHAR(255) NULL,
		City NVARCHAR(50) NULL,
		PostalCode NVARCHAR(20) NULL,
		Country NVARCHAR(50) NULL,
		CreatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
		UpdatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
		LastSyncedAtUtc DATETIME2 NULL,
		CONSTRAINT PK_Suppliers PRIMARY KEY (SupplierId)
	);
END;
GO
