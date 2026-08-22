# Sales Management System — Architecture Plan
### For discussion with Eric Muli — pre-code review

**Stack:** .NET 10 LTS, ASP.NET Core MVC, EF Core, SQL Server (existing `SALES_DB`), Bootstrap 5

**Status:** Planning only — no code written yet.

---

## 1. Chosen Architecture: Clean Architecture, 4 projects

Standard layering, dependencies point inward only (outer layers depend on inner ones, never the reverse):

```
SalesManagementSystem.sln
└── src/
    ├── SalesManagementSystem.Domain          (entities only — zero dependencies)
    ├── SalesManagementSystem.Application      (interfaces, services, business rules — depends on Domain)
    ├── SalesManagementSystem.Infrastructure   (EF Core, repositories, DB config — depends on Application + Domain)
    └── SalesManagementSystem.Web              (MVC: controllers, views, DI wiring — depends on Application;
                                                 references Infrastructure only in Program.cs as composition root)
```

This is a deliberate, recognizable pattern (matches Microsoft's own reference architectures).

## 2. Data Access Strategy: EF Core, database-first, hand-finished

The brief says use the existing `SALES_DB` and don't modify its structure without approval — that settles this as **database-first**, not code-first. Concretely:

1. Run `Scaffold-DbContext` once against `SALES_DB` to reverse-engineer a starting `DbContext` and entity classes. Treat this as a **reference, not the final model** — scaffolded output goes in a throwaway folder, not committed as-is.
2. Hand-write clean POCO entities in `Domain` (no EF attributes, no navigation-property clutter) that mirror `store.categories`, `store.customers`, `store.products`, `store.sales`, `store.sale_items`.
3. Configure the actual EF ↔ table mapping in `Infrastructure` via `IEntityTypeConfiguration<T>` classes (one per entity) — this is what keeps the persistence model out of the Domain layer, which is the whole point of doing database-first *inside* Clean Architecture rather than just using whatever the scaffolder produces everywhere.

**One schema detail that needs explicit handling:** `sale_items.TotalAmount` is a `PERSISTED COMPUTED` column (`Quantity * UnitPrice`) at the database level. In the EF configuration this needs `.HasComputedColumnSql(..., stored: true)` so EF never tries to write to it and instead reads back the DB-calculated value after insert. This directly satisfies the brief's "sale total is calculated automatically" requirement — the calculation is already happening in SQL Server, not in application code, which is arguably a stronger guarantee than doing it in C#.

## 3. Repository + Unit of Work

- `IGenericRepository<T>` — basic CRUD (Get, GetAll, Add, Update, Remove) for Categories, Customers, Products.
- `ISaleRepository` — a dedicated interface, not generic CRUD, because creating a sale is a **composite operation**: one `Sale` row plus N `SaleItem` rows must be written together or not at all.
- `IUnitOfWork` — exposes each repository and a single `CompleteAsync()`. This is what makes the Sale + SaleItems write atomic — without it, Repository Pattern alone tends to drift toward one `SaveChanges()` per repository, which risks a half-written sale.

`DbContext`, repositories, and Unit of Work must all be registered as **Scoped** (one instance per HTTP request). Registering the repositories as Transient/Singleton while `DbContext` is Scoped is a common mistake that either breaks at startup (DI validation) or causes subtle cross-request state bugs.

## 4. Application Services (business logic layer)

One service per module, each depending only on repository interfaces (never on `DbContext` directly):

| Service | Responsibility |
|---|---|
| `CategoryService` | CRUD, search |
| `CustomerService` | CRUD, search, **sale-existence check before delete** (see §6) |
| `ProductService` | CRUD, filter by category, search by name, price validation |
| `SaleService` | Orchestrates sale creation: validate customer exists, validate each product exists, validate ≥1 line item, delegate total calculation to the DB, persist atomically via Unit of Work |
| `ReportService` | The five report queries — read-only, likely `.AsNoTracking()` |

Note on Reports: the query shapes for "Sales by customer" and the aggregate logic for "Top selling products" are close cousins of the Q13 business-scenario queries already written for the SQL assignment (top customers by spend, revenue by category) — that SQL doesn't need to be reinvented, just adapted into LINQ or reused as raw SQL via `FromSqlRaw` where a hand-written query is clearer than LINQ (grouped aggregates are often one of those cases).

## 5. Delete Behavior — this is schema-driven, not a UI choice

| Entity | Delete behavior | Why |
|---|---|---|
| **Category** | Soft delete (`IsActive = 0`) | The column already exists in the schema for this purpose. A hard `DELETE` would fail once any product references the category — no cascade is configured, deliberately. |
| **Product** | Soft delete (`IsActive = 0`) | Same reasoning — `sale_items.ProductID` has no cascade, so a hard delete on a product with sales history would throw a FK violation. Soft delete also means historical sales still show correct product names. |
| **Customer** | Hard delete, but only when the customer has zero sales | `CUSTOMERS` has no `IsActive` column, so this mirrors the Q8 rule already implemented in the SQL assignment ("delete customers with no associated sales") — same business rule, now enforced in `CustomerService` before the repository call, not just as an ad-hoc query. |
| **Sale / SaleItem** | Hard delete allowed | `sale_items → sales` already has `ON DELETE CASCADE` from the original schema design, so deleting a sale correctly removes its line items. |

"Confirmation page before delete" applies uniformly regardless of soft vs. hard — the view looks the same either way; only what happens on POST differs.

## 6. Business Rules → Where They're Enforced

| Rule | Enforcement point |
|---|---|
| Product price > 0 | Data Annotation (`[Range(0.01, decimal.MaxValue)]`) on the ViewModel, **and** re-checked in `ProductService` (defense in depth — don't rely on client validation alone) |
| Sale must contain ≥1 item | Checked in `SaleService.CreateSale` against the posted line-item list before any DB write |
| Sale total calculated automatically | Handled at the DB level via the computed column (see §2) |
| Customer must exist before sale | Populated via dropdown from existing customers (can't select what doesn't exist) + existence check in `SaleService` as defense in depth |
| Product must exist before adding to sale | Same pattern as above |

## 7. Modules → Controllers/Views

- **Dashboard** — single `Index` view, summary cards (total customers, total products, total sales, revenue) sourced from `ReportService`.
- **Categories / Customers / Products** — standard Index (list + search) / Create / Edit / Delete-confirm, following the same MVC scaffold shape for consistency across all three.
- **Sales** — Index (list), Create (the complex one — see §8), Details (view a sale with its line items).
- **"Sale Items" module** — the brief lists this separately in "Expected Modules" but the detailed requirements only describe item management *inside* sale creation ("add multiple products, specify quantity"). Reading is: **no standalone Sale Items controller/pages** — line items are managed entirely within the Sales Create/Details views.
- **Reports** — one controller, five actions/views matching the five required reports.

## 8. Open Discussion Point: Sale Creation UI

- **Option A — simple, server-rendered:** a fixed set of line rows (e.g. 5 blank rows) posted back on submit; less JavaScript, faster to build, less impressive UI.
- **Option B — dynamic, JS-enhanced:** an "add another product" button that adds rows client-side, with an AJAX call to fetch the selected product's price for live total calculation. More realistic UX, more time investment, more JavaScript to write and debug.

## 9. Git & AI Use

- Feature branches per module (`feature/categories`, `feature/sales`, etc.), merged into `main` after each is working.
- AI used for scaffolding acceleration and explaining unfamiliar patterns — every generated line reviewed and understood before committing, consistent with how the SQL assignment work was approached (reviewed and corrected rather than accepted as-is).

