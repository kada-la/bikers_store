USE SALES_DB;
GO

-- ==========================================================
-- 15_add_supplier_to_products.sql
-- Associate Products with Suppliers in SALES_DB
-- ==========================================================

IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'store' AND TABLE_NAME = 'products' AND COLUMN_NAME = 'SupplierId'
)
BEGIN
    ALTER TABLE store.products ADD SupplierId NVARCHAR(50) NULL;

    ALTER TABLE store.products ADD CONSTRAINT FK_Products_Suppliers 
        FOREIGN KEY (SupplierId) REFERENCES store.Suppliers(SupplierId) 
        ON DELETE SET NULL;
END;
GO
