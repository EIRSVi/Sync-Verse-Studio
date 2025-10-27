---
inclusion: always
---

# Project Structure

## Root Organization

The repository has a dual structure with root-level folders (legacy) and the main project in `syncversestudio/` folder.

### Active Project Location
All active development happens in: `syncversestudio/`

### Root-Level Folders (Reference/Legacy)
- `Models/` - Legacy model definitions (reference only)
- `Views/` - Legacy view files (reference only)
- `Services/` - Legacy service files (reference only)
- `Data/` - Legacy data context (reference only)
- `Database/` - SQL scripts for manual database setup
- `GUIDE/` - Extensive documentation and implementation guides
- `assets/` - Shared assets (images, audio, branding)

## Main Project Structure: `syncversestudio/`

```
syncversestudio/
├── Data/
│   ├── ApplicationDbContext.cs      # EF Core DbContext with all DbSets
│   └── Migrations/                  # EF Core migration files
├── Models/
│   ├── User.cs                      # User entity with UserRole enum
│   ├── Product.cs                   # Product entity
│   ├── Category.cs, Supplier.cs     # Supporting entities
│   ├── Sale.cs, SaleItem.cs         # Sales transaction entities
│   ├── Customer.cs                  # Customer entity
│   ├── Invoice.cs, InvoiceItem.cs   # Invoice entities
│   ├── Payment.cs, PaymentLink.cs   # Payment entities
│   ├── HeldTransaction.cs           # Hold transaction feature
│   ├── InventoryMovement.cs         # Stock tracking
│   ├── AuditLog.cs                  # Audit trail
│   └── OnlineStoreIntegration.cs    # E-commerce sync
├── Services/
│   ├── AuthenticationService.cs     # Login, permissions, audit logging
│   └── DatabaseInitializer.cs       # Database seeding
├── Views/
│   ├── LoginForm.cs                 # Entry point form
│   ├── MainDashboard.cs             # Role-based main dashboard
│   ├── EnhancedCashierDashboardView.cs  # Modern POS interface
│   ├── ProductManagementView.cs     # Product CRUD
│   ├── CategoryManagementView.cs    # Category management
│   ├── CustomerManagementView.cs    # Customer management
│   ├── UserManagementView.cs        # User administration
│   ├── AnalyticsView.cs             # Reports and analytics
│   ├── InventoryView.cs             # Stock management
│   ├── SalesView.cs                 # Sales history
│   └── CashierDashboard/            # Cashier-specific views
├── Helpers/
│   ├── ProductImageHelper.cs        # Image handling utilities
│   └── BrandTheme.cs                # UI theming
├── SQL/                             # SQL scripts
├── bin/, obj/                       # Build output (ignored)
├── Program.cs                       # Application entry point
├── GlobalUsings.cs                  # Global using directives
├── syncversestudio.csproj           # Project file
└── app.manifest                     # Windows app manifest
```

## Key Architectural Layers

### 1. Data Layer (`Data/`)
- `ApplicationDbContext`: Central EF Core context
- Fluent API configurations for all entities
- Seed data for initial setup (admin user, categories, suppliers)
- Connection string hardcoded in OnConfiguring method

### 2. Domain Layer (`Models/`)
- Entity classes with data annotations
- Navigation properties for relationships
- Computed properties (NotMapped) for business logic
- Enums: UserRole, PaymentMethod, SaleStatus, etc.

### 3. Business Layer (`Services/`)
- `AuthenticationService`: User login, permissions, session management
- Password hashing with BCrypt
- Audit logging for security events

### 4. Presentation Layer (`Views/`)
- Windows Forms with custom drawing (GDI+)
- Role-based UI with permission checks
- Material design components
- FontAwesome icons throughout

## Naming Conventions

### Files
- PascalCase for all C# files: `ProductManagementView.cs`
- Views end with `View.cs` or `Form.cs`
- Services end with `Service.cs`

### Code
- PascalCase for classes, methods, properties
- camelCase for private fields with `_` prefix: `_context`
- UPPER_CASE for constants
- Async methods end with `Async` suffix

### Database
- PascalCase table names (matches entity names)
- Foreign keys: `{Entity}Id` (e.g., `CategoryId`)
- Navigation properties match entity names

## Important Patterns

### Entity Relationships
- One-to-Many: Category → Products, Supplier → Products
- Many-to-One: Sale → User (Cashier), Sale → Customer
- One-to-Many: Sale → SaleItems, Invoice → InvoiceItems
- Cascade deletes configured via Fluent API

### Data Access
- Always use `ApplicationDbContext` via `using` statement or dispose properly
- Async operations preferred: `await _context.SaveChangesAsync()`
- LINQ for queries: `.Where()`, `.Include()`, `.FirstOrDefaultAsync()`

### UI Patterns
- Forms inherit from `Form` base class
- Event handlers: `btnName_Click`, `txtName_TextChanged`
- DataGridView for tabular data
- Custom painting in `OnPaint` overrides
