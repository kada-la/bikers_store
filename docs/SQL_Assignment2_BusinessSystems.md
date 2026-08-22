# Intern Assignment: Sales Management System (ASP.NET Core MVC)

## Project Goal

Develop a **Sales Management System** using **ASP.NET Core MVC**, **Entity Framework Core**, **SQL Server**, and **Clean Architecture**. The application will use the existing **SALES_DB** database.

---

## Learning Objectives

- Understand ASP.NET Core MVC.
- Apply Clean Architecture principles.
- Use Entity Framework Core with SQL Server.
- Implement CRUD operations.
- Use Dependency Injection and the Repository Pattern.
- Create Razor Views with Bootstrap.
- Write clean, maintainable C# code.
- Use Git for version control.
- Use AI responsibly to improve productivity.

---

## Technologies

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server (SALES_DB)
- Bootstrap 5
- Razor Views
- Git & GitHub

---

## Database

Use the existing **SALES_DB** database.

- Create Entity Framework models for the existing tables.
- Implement repositories for the existing tables.
- **Do not modify the database structure unless approval is given.**

---

## Expected Modules

- Dashboard
- Categories
- Customers
- Products
- Sales
- Sale Items
- Reports

---

## Module Requirements

### 1. Categories

- View all categories
- Add category
- Edit category
- Delete category
- Search categories

---

### 2. Customers

- View all customers
- Register customer
- Edit customer details
- Delete customer
- Search customers

---

### 3. Products

- View products
- Add product
- Edit product
- Delete product
- Filter by category
- Search by product name

---

### 4. Sales

- Create a new sale
- Select customer
- Add multiple products
- Specify quantity
- Automatically calculate totals
- Save sale and sale items

---

### 5. Reports

- Sales summary
- Daily sales
- Monthly sales
- Top-selling products
- Sales by customer

---

## Business Rules

- Product price must be greater than zero.
- A sale must contain at least one item.
- Sale totals must be calculated automatically.
- A customer must exist before creating a sale.
- A product must exist before adding it to a sale.

---

## User Interface

The application should include:

- Responsive Bootstrap layout
- Dashboard cards
- Navigation menu
- Tables with search functionality
- Create/Edit forms
- Confirmation page before deleting records

---

## Deliverables

- Complete source code
- Architecture diagram