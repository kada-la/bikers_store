# Assignment – Business Systems Focus
### SQL Server / SSMS — CIFOR-ICRAF Internship

**Due:** Monday, July 6, 2026
**Assigned:** Wednesday, July 1, 2026

---

## Objective

- Build practical SQL skills for Business Systems and process-based environments
- Create, manage, and query a relational database using SQL Server
- Generate system-driven reports that support business decision-making

---

## Question 1: Environment Setup

- [x] Download and install SQL Server (Express Edition)
- [x] Download and install SQL Server Management Studio (SSMS)
- [x] Connect to your local SQL Server instance using SSMS
- [x] Create a database named `SALES_DB`

---

## Question 2: Database & Table Creation

Using `SALES_DB`, create the following tables:

- [x] `CUSTOMERS`
- [x] `PRODUCTS`
- [x] `SALES`

**Requirements:**
- Use a pure SQL statement to create at least one of the tables
- Each table has a `PRIMARY KEY`
- Appropriate data types are used
- Mandatory fields are set to `NOT NULL`

---

## Question 3: Data Insertion

- [x] Insert the provided records into `CUSTOMERS`
- [x] Insert at least 10 records into `PRODUCTS`
- [x] Insert at least 15 records into `SALES`

**Requirements:**
- Each sale references an existing customer
- Each sale references an existing product

---

## Question 4: Data Retrieval (SELECT Queries)

Write SQL queries to:

- [x] Display all customers
- [x] Retrieve customers from a specific city or country
- [x] Display all products sorted by price (highest to lowest)
- [x] Retrieve all sales made in the last 30 days
- [x] Display all sales for a specific customer

---

## Question 5: Filtering & Sorting

Write SQL queries to:

- [x] Retrieve products priced above a specified amount
- [x] Display customers whose names start with a specific letter
- [x] Sort sales by transaction date (most recent first)
- [x] Display the top 5 most expensive products

---

## Question 6: Aggregate Functions

Write SQL queries to:

- [x] Count the total number of customers
- [x] Calculate the total sales value
- [x] Calculate the average product price
- [x] Identify the highest and lowest product prices
- [x] Calculate total sales per product

---

## Question 7: Joins (Business Process Reporting)

Write SQL queries to:

- [x] Display sales with customer names and product names
- [x] Calculate total sales per customer
- [x] Calculate total revenue per product
- [x] Identify customers who have never made a purchase

---

## Question 8: Update & Delete Operations

Write SQL queries to:

- [x] Update a customer's contact details
- [x] Increase all product prices by a specified percentage
- [x] Delete a sales record based on a condition
- [x] Delete customers with no associated sales

---

## Question 9: Constraints & Data Integrity

- [x] Add `FOREIGN KEY` constraints: `SALES → CUSTOMERS`
- [x] Add `FOREIGN KEY` constraints: `SALES → PRODUCTS`
- [x] Add a `UNIQUE` constraint to customer email addresses
- [x] Demonstrate how constraints prevent invalid data entry (e.g. attempt an invalid insert and capture the error)

---

## Question 10: Views

- [x] Create a view displaying: Sale ID, Customer Name, Product Name, Quantity, Total Sale Amount
- [x] Query the view to retrieve sales data

---

## Question 11: Stored Procedures

- [x] Create a stored procedure to insert a new customer
- [x] Create a stored procedure to retrieve all sales for a given customer
- [x] Execute both stored procedures and display results

---

## Question 12: Indexes & Performance

- [x] Create an index on frequently queried columns
- [x] Briefly explain why indexes are important
- [x] Demonstrate query performance improvement using indexes (e.g. `SET STATISTICS TIME ON` / execution plan before and after)

---

## Question 13: Business Scenario Task

- [x] Assume the database supports a sales organization
- [x] Identify three management reports required from the system
- [x] Write SQL queries to generate each report

---

## Submission Requirements

- [x] SQL scripts for all tasks
- [x] Screenshots showing successful execution
- [x] Brief explanations where required

