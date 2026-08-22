USE SALES_DB;
GO

-- Insert categories
INSERT INTO store.categories (CategoryName, Description) VALUES
('Mountain Bikes', 'Bikes built for rugged off-road trails and steep terrain.'),
('Road Bikes', 'Lightweight bikes optimized for speed on paved surfaces.'),
('Electric Bikes', 'Bikes equipped with integrated electric motors for pedal ass'),
('Accessories', 'Essential riding gear including helmets, lights, and locks.');
GO

INSERT INTO store.customers (FirstName, LastName, Email, PhoneNumber, Address, City, County, PostalCode, Country) VALUES
('Michael', 'Njenga', 'm.njenga@email.co.ke', '0711223344', '12 Tom Mboya St', 'Nairobi', 'Nairobi', '00100', 'Kenya'),
('Sarah', 'Achieng', 'sarah.a@domain.com', '0722334455', '45 Milimani Rd', 'Kisumu', 'Kisumu', '40100', 'Kenya'),
('David', 'Kimani', 'kimani.d@webmail.ke', '0733445566', '78 Gakere Rd', 'Nyeri', 'Nyeri', '10100', 'Kenya'),
('Emma', 'Watson', 'emma.w@global.com', '+15550199', '234 Elm St', 'New York', 'New York', '10001', 'USA'),
('John', 'Oliver', 'j.oliver@ukmail.co.uk', '+44207946', '89 Piccadilly', 'London', 'Greater London', 'W1J', 'UK'),
('Grace', 'Moraa', 'moraa.g@fastmail.com', '0744556677', '102 Digo Rd', 'Mombasa', 'Mombasa', '80100', 'Kenya');
GO

INSERT INTO store.products (ProductName, CategoryID, Description, Price, StockQuantity) VALUES
('Apex Trail Blazer', 1, 'Full-suspension mountain bike with 12-speed drivetrain.', 1450.00, 12),
('Summit Ridge 29', 1, 'Hardtail trail bike with lightweight aluminum frame.', 899.00, 20),
('Veloce Carbon Pro', 2, 'Aerodynamic carbon fiber road bike for competitive racing.', 2800.00, 5),
('Street Commuter X', 2, 'Durable urban road bike built for daily city travel.', 650.00, 15),
('Volt Volt-E Commuter', 3, 'Long-range electric hybrid bike with a 500Wh battery.', 3200.00, 4),
('E-Mountain Beast', 3, 'High-torque electric mountain bike for steep climbs.', 4100.00, 3),
('Aero Stream Helmet', 4, 'Impact-resistant ventilation helmet with MIPS safety.', 120.00, 50),
('Lumen Ultra LED Set', 4, '1200 lumen rechargeable front and rear smart lights.', 75.00, 40),
('Titanium U-Lock', 4, 'Heavy-duty maximum security anti-theft shackle lock.', 95.00, 25),
('MudSlinger Fender Set', 4, 'Quick-release snap-on front and rear mudguards.', 40.00, 30);
GO

INSERT INTO store.sales (CustomerID, SaleDate) VALUES
(1, DATEADD(DAY, -2, GETDATE())),   -- Invvoice 1: 2 daus agp (Within 30 days)
(2, DATEADD(DAY, -10, GETDATE())),  -- Invoice 2: 10 days ago (Within 30 days)
(3, DATEADD(DAY, -18, GETDATE())),  -- Invoice 3: 18 days ago (Within 30 days)
(4, DATEADD(DAY, -25, GETDATE())),  -- Invoice 4: 25 days ago (Within 30 days)
(1, DATEADD(DAY, -45, GETDATE())),  -- Invoice 5: 45 days ago (OUTSIDE 30 days)
(5, DATEADD(DAY, -60, GETDATE()));  -- Invoice 6: 60 days ago (OUTSIDE 30 days)
GO

INSERT INTO store.sale_items (SaleID, ProductID, Quantity, UnitPrice) VALUES
(1, 1, 1, 1450.00), -- Invoice 1, Item 1
(1, 7, 1, 120.00),  -- Invoice 1, Item 2
(1, 8, 1, 75.00),   -- Invoice 1, Item 3
(2, 2, 2, 899.00),  -- Invoice 2, Item 4
(2, 7, 2, 120.00),  -- Invoice 2, Item 5
(3, 4, 1, 650.00),  -- Invoice 3, Item 6
(3, 9, 1, 95.00),   -- Invoice 3, Item 7
(4, 5, 1, 3200.00), -- Invoice 4, Item 8
(4, 7, 1, 120.00),  -- Invoice 4, Item 9
(4, 8, 1, 75.00),   -- Invoice 4, Item 10
(4, 10, 1, 40.00),  -- Invoice 4, Item 11
(5, 3, 1, 2800.00), -- Invoice 5, Item 12
(5, 7, 1, 120.00),  -- Invoice 5, Item 13
(6, 2, 1, 899.00),  -- Invoice 6, Item 14
(6, 9, 1, 95.00);   -- Invoice 6, Item 15
GO