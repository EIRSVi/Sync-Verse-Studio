# SYNCVERSE STUDIO

Point of Sale and Inventory Management System

A comprehensive retail management solution built with .NET 8.0 and Windows Forms.

---

## Overview

SYNCVERSE STUDIO is an enterprise-grade POS system that handles sales processing, inventory management, customer relationships, invoicing, and analytics.

Version: 1.0.0
Platform: Windows Desktop
Framework: .NET 8.0

---

## System Architecture

```mermaid
graph TB
    subgraph "Presentation Layer"
        A[Windows Forms UI]
        A1[Login]
        A2[Dashboard]
        A3[POS]
        A4[Products]
        A5[Analytics]
    end
    
    subgraph "Business Logic"
        B[Services]
        B1[Authentication]
        B2[Database Init]
    end
    
    subgraph "Data Access"
        C[Entity Framework Core]
        C1[DbContext]
        C2[Migrations]
    end
    
    subgraph "Database"
        D[(SQL Server)]
    end
    
    A --> B
    B --> C
    C --> D
    
    style A fill:#3B82F6,stroke:#2563EB,color:#fff
    style B fill:#22C55E,stroke:#16A34A,color:#fff
    style C fill:#A855F7,stroke:#9333EA,color:#fff
    style D fill:#EF4444,stroke:#DC2626,color:#fff
```

---

## User Roles

```mermaid
graph TD
    A[SYNCVERSE STUDIO] --> B[Administrator]
    A --> C[Cashier]
    A --> D[Inventory Clerk]
    
    B --> B1[User Management]
    B --> B2[System Config]
    B --> B3[Reports]
    B --> B4[Audit Logs]
    
    C --> C1[POS Operations]
    C --> C2[Customer Management]
    C --> C3[Sales Processing]
    C --> C4[Invoicing]
    
    D --> D1[Product Management]
    D --> D2[Categories]
    D --> D3[Suppliers]
    D --> D4[Stock Control]
    
    style A fill:#14B8A6,stroke:#0D9488,color:#fff
    style B fill:#3B82F6,stroke:#2563EB,color:#fff
    style C fill:#22C55E,stroke:#16A34A,color:#fff
    style D fill:#A855F7,stroke:#9333EA,color:#fff
```

---

## Application Flow

```mermaid
flowchart TD
    Start([Login]) --> Auth{Authenticate}
    Auth -->|Success| Role{User Role}
    Auth -->|Fail| Start
    
    Role -->|Admin| AdminDash[Admin Dashboard]
    Role -->|Cashier| CashierDash[Cashier Dashboard]
    Role -->|Clerk| ClerkDash[Inventory Dashboard]
    
    AdminDash --> AdminOps[User Management<br/>System Settings<br/>Analytics<br/>Audit Logs]
    
    CashierDash --> CashierOps[Point of Sale<br/>Customers<br/>Sales History<br/>Invoices]
    
    ClerkDash --> ClerkOps[Products<br/>Categories<br/>Suppliers<br/>Stock Reports]
    
    AdminOps --> Log[(Audit Log)]
    CashierOps --> Log
    ClerkOps --> Log
    
    style Start fill:#14B8A6,stroke:#0D9488,color:#fff
    style Auth fill:#F97316,stroke:#EA580C,color:#fff
    style Role fill:#A855F7,stroke:#9333EA,color:#fff
    style AdminDash fill:#3B82F6,stroke:#2563EB,color:#fff
    style CashierDash fill:#22C55E,stroke:#16A34A,color:#fff
    style ClerkDash fill:#EC4899,stroke:#DB2777,color:#fff
```

---

## POS Transaction Flow

```mermaid
flowchart TD
    Start([Start]) --> Select[Select Products]
    Select --> Cart[Add to Cart]
    Cart --> Stock{Stock Available}
    Stock -->|No| Error[Show Error]
    Stock -->|Yes| Review[Review Cart]
    Error --> Select
    
    Review --> Tax[Apply Tax]
    Tax --> Payment{Payment Method}
    
    Payment -->|Cash| Cash[Enter Amount]
    Payment -->|Card| Card[Process Card]
    Payment -->|QR| QR[Generate QR]
    
    Cash --> Customer
    Card --> Customer
    QR --> Customer
    
    Customer{Customer Type} -->|Walk-in| WalkIn[Walk-in]
    Customer -->|Existing| Existing[Select Customer]
    Customer -->|New| New[Create Customer]
    
    WalkIn --> Complete[Complete Sale]
    Existing --> Complete
    New --> Complete
    
    Complete --> Invoice[Generate Invoice]
    Invoice --> UpdateStock[Update Inventory]
    UpdateStock --> Receipt{Receipt}
    
    Receipt -->|Print| Print[Print]
    Receipt -->|Email| Email[Email]
    Receipt -->|View| View[View]
    
    Print --> End([Done])
    Email --> End
    View --> End
    
    style Start fill:#14B8A6,stroke:#0D9488,color:#fff
    style Complete fill:#22C55E,stroke:#16A34A,color:#fff
    style End fill:#3B82F6,stroke:#2563EB,color:#fff
    style Error fill:#EF4444,stroke:#DC2626,color:#fff
```

---

## Database Schema

### Complete Entity Relationship Diagram

```mermaid
erDiagram
    User ||--o{ Sale : "processes"
    User ||--o{ Invoice : "creates"
    User ||--o{ Payment : "handles"
    User ||--o{ AuditLog : "generates"
    User ||--o{ InventoryMovement : "records"
    User ||--o{ HeldTransaction : "holds"
    User ||--o{ PaymentLink : "creates"
    
    Category ||--o{ Product : "contains"
    Supplier ||--o{ Product : "supplies"
    
    Product ||--o{ SaleItem : "sold_in"
    Product ||--o{ InvoiceItem : "invoiced_in"
    Product ||--o{ InventoryMovement : "tracked"
    
    Customer ||--o{ Sale : "makes"
    Customer ||--o{ Invoice : "receives"
    Customer ||--o{ HeldTransaction : "holds"
    Customer ||--o{ PaymentLink : "pays_via"
    
    Sale ||--|{ SaleItem : "contains"
    Sale ||--o| Invoice : "generates"
    Sale ||--o{ Payment : "paid_by"
    
    Invoice ||--|{ InvoiceItem : "contains"
    Invoice ||--o{ Payment : "paid_by"
    Invoice ||--o{ PaymentLink : "linked_to"
    
    Payment ||--o| PaymentLink : "from"
    
    User {
        int Id PK
        string Username UK
        string Password
        string Email
        string FirstName
        string LastName
        string Role
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
    
    Product {
        int Id PK
        string Name
        string Description
        string Barcode UK
        string SKU UK
        int CategoryId FK
        int SupplierId FK
        decimal CostPrice
        decimal SellingPrice
        int Quantity
        int MinQuantity
        string ImagePath
        bool IsActive
        bool IsSyncedToOnlineStore
        datetime CreatedAt
    }
    
    Category {
        int Id PK
        string Name
        string Description
        bool IsActive
        datetime CreatedAt
    }
    
    Supplier {
        int Id PK
        string Name
        string ContactPerson
        string Phone
        string Email
        string Address
        bool IsActive
        datetime CreatedAt
    }
    
    Customer {
        int Id PK
        string FirstName
        string LastName
        string Phone
        string Email
        string Address
        int LoyaltyPoints
        datetime CreatedAt
    }
    
    Sale {
        int Id PK
        string InvoiceNumber UK
        int CustomerId FK
        int CashierId FK
        decimal TotalAmount
        decimal TaxAmount
        decimal DiscountAmount
        string PaymentMethod
        datetime SaleDate
        string Status
    }
    
    SaleItem {
        int Id PK
        int SaleId FK
        int ProductId FK
        string ProductName
        int Quantity
        decimal UnitPrice
        decimal TotalPrice
    }
    
    Invoice {
        int Id PK
        string InvoiceNumber UK
        int CustomerId FK
        string CustomerName
        int CreatedByUserId FK
        decimal SubTotal
        decimal TaxAmount
        decimal DiscountAmount
        decimal TotalAmount
        decimal PaidAmount
        decimal BalanceAmount
        string Status
        datetime InvoiceDate
        datetime DueDate
        string Notes
    }
    
    InvoiceItem {
        int Id PK
        int InvoiceId FK
        int ProductId FK
        string ProductName
        int Quantity
        decimal UnitPrice
        decimal TotalPrice
    }
    
    Payment {
        int Id PK
        string PaymentReference UK
        int InvoiceId FK
        int SaleId FK
        decimal Amount
        string PaymentMethod
        string Status
        string TransactionId
        string PaymentGateway
        int ProcessedByUserId FK
        datetime PaymentDate
    }
    
    InventoryMovement {
        int Id PK
        int ProductId FK
        int UserId FK
        string MovementType
        int Quantity
        string Reference
        datetime CreatedAt
    }
    
    AuditLog {
        int Id PK
        int UserId FK
        string Action
        string TableName
        int RecordId
        string OldValues
        string NewValues
        datetime Timestamp
        string IpAddress
    }
    
    HeldTransaction {
        int Id PK
        string TransactionCode UK
        int CustomerId FK
        string CustomerName
        int HeldByUserId FK
        decimal SubTotal
        decimal TaxAmount
        decimal TotalAmount
        string CartItemsJson
        datetime HeldAt
        bool IsCompleted
    }
    
    PaymentLink {
        int Id PK
        string LinkCode UK
        int InvoiceId FK
        int CustomerId FK
        decimal Amount
        string Description
        string Status
        datetime ExpiryDate
        datetime PaidAt
        int PaymentId FK
        int CreatedByUserId FK
        datetime CreatedAt
    }
```

### Data Table Relationships Explained

```mermaid
graph TB
    subgraph "User Management"
        U[User Table]
        U --> U1[Stores credentials]
        U --> U2[Defines roles]
        U --> U3[Tracks activity]
    end
    
    subgraph "Product Management"
        P[Product Table]
        C[Category Table]
        S[Supplier Table]
        
        C --> P
        S --> P
        P --> P1[Tracks inventory]
        P --> P2[Stores pricing]
        P --> P3[Manages stock levels]
    end
    
    subgraph "Sales Processing"
        SA[Sale Table]
        SI[SaleItem Table]
        CU[Customer Table]
        
        CU --> SA
        U --> SA
        SA --> SI
        P --> SI
        
        SA --> SA1[Records transactions]
        SI --> SI2[Line item details]
    end
    
    subgraph "Invoicing"
        I[Invoice Table]
        II[InvoiceItem Table]
        
        CU --> I
        U --> I
        SA --> I
        I --> II
        P --> II
        
        I --> I1[Billing information]
        II --> I2[Invoice line items]
    end
    
    subgraph "Payment Processing"
        PA[Payment Table]
        PL[PaymentLink Table]
        
        I --> PA
        SA --> PA
        U --> PA
        I --> PL
        CU --> PL
        U --> PL
        PA --> PL
        
        PA --> PA1[Payment records]
        PL --> PL1[Payment links]
    end
    
    subgraph "Tracking & Audit"
        IM[InventoryMovement Table]
        AL[AuditLog Table]
        HT[HeldTransaction Table]
        
        P --> IM
        U --> IM
        U --> AL
        U --> HT
        CU --> HT
        
        IM --> IM1[Stock movements]
        AL --> AL1[System audit trail]
        HT --> HT1[Suspended sales]
    end
    
    style U fill:#3B82F6,stroke:#2563EB,color:#fff
    style P fill:#22C55E,stroke:#16A34A,color:#fff
    style SA fill:#F59E0B,stroke:#D97706,color:#fff
    style I fill:#8B5CF6,stroke:#7C3AED,color:#fff
    style PA fill:#EC4899,stroke:#DB2777,color:#fff
    style AL fill:#6B7280,stroke:#4B5563,color:#fff
```

---

## Data Presentation Interface

### How Data Flows Through the System

```mermaid
graph LR
    subgraph "Data Source"
        DB[(SQL Server<br/>Database)]
    end
    
    subgraph "Data Access Layer"
        EF[Entity Framework<br/>Core]
        CTX[DbContext]
        
        DB --> EF
        EF --> CTX
    end
    
    subgraph "Business Logic"
        AUTH[Authentication<br/>Service]
        INIT[Database<br/>Initializer]
        
        CTX --> AUTH
        CTX --> INIT
    end
    
    subgraph "Presentation Layer"
        LOGIN[Login Form]
        DASH[Dashboard]
        POS[POS Interface]
        PROD[Product Management]
        CUST[Customer Management]
        RPT[Reports & Analytics]
        
        AUTH --> LOGIN
        LOGIN --> DASH
        DASH --> POS
        DASH --> PROD
        DASH --> CUST
        DASH --> RPT
    end
    
    subgraph "User Interaction"
        USER[User]
        
        USER --> LOGIN
        USER --> POS
        USER --> PROD
        USER --> CUST
        USER --> RPT
    end
    
    style DB fill:#EF4444,stroke:#DC2626,color:#fff
    style EF fill:#A855F7,stroke:#9333EA,color:#fff
    style AUTH fill:#22C55E,stroke:#16A34A,color:#fff
    style DASH fill:#3B82F6,stroke:#2563EB,color:#fff
    style USER fill:#F59E0B,stroke:#D97706,color:#fff
```

### Data Flow in POS Transaction

```mermaid
sequenceDiagram
    participant U as User/Cashier
    participant UI as POS Interface
    participant BL as Business Logic
    participant DB as Database
    participant PR as Printer/Display
    
    U->>UI: Select Products
    UI->>DB: Query Product Data
    DB-->>UI: Return Products
    UI-->>U: Display Products
    
    U->>UI: Add to Cart
    UI->>DB: Check Stock Availability
    DB-->>UI: Stock Status
    UI-->>U: Update Cart Display
    
    U->>UI: Apply Tax & Discount
    UI->>BL: Calculate Totals
    BL-->>UI: Return Calculated Amounts
    UI-->>U: Show Final Total
    
    U->>UI: Select Payment Method
    UI->>BL: Process Payment
    BL->>DB: Create Sale Record
    BL->>DB: Create Invoice Record
    BL->>DB: Create Payment Record
    BL->>DB: Update Product Quantities
    BL->>DB: Create Inventory Movements
    BL->>DB: Create Audit Log
    
    DB-->>BL: Confirm All Saved
    BL-->>UI: Transaction Complete
    
    UI->>PR: Generate Invoice/Receipt
    PR-->>U: Print/Display Receipt
    
    UI-->>U: Show Success Message
```

### Detailed POS Transaction Data Flow

```mermaid
flowchart TD
    Start([Cashier Opens POS]) --> Init[Initialize POS Interface]
    Init --> LoadData[Load Initial Data]
    
    LoadData --> L1[Query Active Products]
    LoadData --> L2[Query Categories]
    LoadData --> L3[Query Customers]
    LoadData --> L4[Load User Session]
    
    L1 --> Display[Display Product Grid]
    L2 --> Display
    L3 --> Display
    L4 --> Display
    
    Display --> UserAction{User Action}
    
    UserAction -->|Search| Search[Search Products by Name/Barcode]
    UserAction -->|Filter| Filter[Filter by Category]
    UserAction -->|Select| AddCart[Add Product to Cart]
    
    Search --> Display
    Filter --> Display
    
    AddCart --> ValidateStock{Stock > 0}
    ValidateStock -->|No| StockError[Show Stock Error]
    ValidateStock -->|Yes| CheckQty{Quantity Available}
    
    StockError --> Display
    
    CheckQty -->|Insufficient| QtyError[Show Quantity Error]
    CheckQty -->|Sufficient| UpdateCart[Update Cart]
    
    QtyError --> Display
    
    UpdateCart --> CalcSubtotal[Calculate Subtotal]
    CalcSubtotal --> CartDisplay[Display Cart Items]
    
    CartDisplay --> CartAction{Cart Action}
    
    CartAction -->|Add More| UserAction
    CartAction -->|Update Qty| UpdateQty[Update Item Quantity]
    CartAction -->|Remove Item| RemoveItem[Remove from Cart]
    CartAction -->|Clear Cart| ClearCart[Clear All Items]
    CartAction -->|Proceed| ApplyTax[Apply Tax Rate]
    
    UpdateQty --> CalcSubtotal
    RemoveItem --> CalcSubtotal
    ClearCart --> Display
    
    ApplyTax --> CalcTax[Tax = Subtotal × Tax Rate]
    CalcTax --> ApplyDiscount{Apply Discount}
    
    ApplyDiscount -->|Yes| CalcDiscount[Calculate Discount Amount]
    ApplyDiscount -->|No| CalcTotal
    
    CalcDiscount --> CalcTotal[Total = Subtotal + Tax - Discount]
    CalcTotal --> ShowTotal[Display Final Total]
    
    ShowTotal --> SelectCustomer{Select Customer}
    
    SelectCustomer -->|Walk-in| WalkIn[Use Walk-in Customer]
    SelectCustomer -->|Existing| ExistingCust[Select from List]
    SelectCustomer -->|New| NewCust[Create New Customer]
    
    NewCust --> SaveCust[Save Customer to DB]
    SaveCust --> CustomerSet
    
    WalkIn --> CustomerSet[Customer Set]
    ExistingCust --> CustomerSet
    
    CustomerSet --> PaymentMethod{Select Payment}
    
    PaymentMethod -->|Cash| CashPay[Cash Payment]
    PaymentMethod -->|Card| CardPay[Card Payment]
    PaymentMethod -->|QR/Mobile| QRPay[QR Payment]
    PaymentMethod -->|Mixed| MixedPay[Mixed Payment]
    
    CashPay --> EnterCash[Enter Cash Amount]
    EnterCash --> ValidateCash{Amount >= Total}
    ValidateCash -->|No| CashError[Show Error]
    ValidateCash -->|Yes| CalcChange[Calculate Change]
    CashError --> EnterCash
    CalcChange --> ProcessTrans
    
    CardPay --> CardDetails[Enter Card Details]
    CardDetails --> ProcessCard[Process Card Transaction]
    ProcessCard --> CardResult{Success}
    CardResult -->|No| CardFail[Show Card Error]
    CardResult -->|Yes| ProcessTrans
    CardFail --> PaymentMethod
    
    QRPay --> GenQR[Generate QR Code]
    GenQR --> DisplayQR[Display QR for Scanning]
    DisplayQR --> WaitScan[Wait for Payment Confirmation]
    WaitScan --> ProcessTrans
    
    MixedPay --> SplitPay[Split Payment Amounts]
    SplitPay --> ProcessTrans
    
    ProcessTrans[Process Transaction] --> BeginTrans[Begin Database Transaction]
    
    BeginTrans --> CreateSale[Create Sale Record]
    CreateSale --> CreateSaleItems[Create SaleItem Records]
    CreateSaleItems --> CreateInvoice[Create Invoice Record]
    CreateInvoice --> CreateInvoiceItems[Create InvoiceItem Records]
    CreateInvoiceItems --> CreatePayment[Create Payment Record]
    CreatePayment --> UpdateStock[Update Product Quantities]
    UpdateStock --> CreateMovements[Create Inventory Movements]
    CreateMovements --> UpdateCustomer[Update Customer Loyalty Points]
    UpdateCustomer --> CreateAudit[Create Audit Log Entry]
    
    CreateAudit --> CommitCheck{All Success}
    
    CommitCheck -->|No| Rollback[Rollback Transaction]
    CommitCheck -->|Yes| Commit[Commit Transaction]
    
    Rollback --> TransError[Show Transaction Error]
    TransError --> PaymentMethod
    
    Commit --> GenInvoice[Generate Invoice Document]
    GenInvoice --> ReceiptOption{Receipt Option}
    
    ReceiptOption -->|Print| PrintReceipt[Print Invoice]
    ReceiptOption -->|Email| EmailReceipt[Email Invoice]
    ReceiptOption -->|SMS| SMSReceipt[SMS Receipt Link]
    ReceiptOption -->|View| ViewReceipt[View on Screen]
    
    PrintReceipt --> Success
    EmailReceipt --> Success
    SMSReceipt --> Success
    ViewReceipt --> Success
    
    Success[Show Success Message] --> ResetPOS[Reset POS Interface]
    ResetPOS --> Display
    
    style Start fill:#14B8A6,stroke:#0D9488,color:#fff
    style Display fill:#3B82F6,stroke:#2563EB,color:#fff
    style ProcessTrans fill:#F59E0B,stroke:#D97706,color:#fff
    style Commit fill:#22C55E,stroke:#16A34A,color:#fff
    style Rollback fill:#EF4444,stroke:#DC2626,color:#fff
    style Success fill:#22C55E,stroke:#16A34A,color:#fff
    style StockError fill:#EF4444,stroke:#DC2626,color:#fff
    style QtyError fill:#EF4444,stroke:#DC2626,color:#fff
```

### POS Transaction State Machine

```mermaid
stateDiagram-v2
    [*] --> Idle
    Idle --> ProductSelection: Start Transaction
    
    ProductSelection --> CartBuilding: Add Product
    CartBuilding --> CartBuilding: Add/Update/Remove Items
    CartBuilding --> ProductSelection: Continue Shopping
    CartBuilding --> Calculation: Proceed to Payment
    
    Calculation --> TaxApplication: Calculate Subtotal
    TaxApplication --> DiscountApplication: Apply Tax
    DiscountApplication --> TotalCalculation: Apply Discount
    TotalCalculation --> CustomerSelection: Show Total
    
    CustomerSelection --> PaymentSelection: Customer Selected
    
    PaymentSelection --> CashPayment: Cash
    PaymentSelection --> CardPayment: Card
    PaymentSelection --> QRPayment: QR/Mobile
    PaymentSelection --> MixedPayment: Mixed
    
    CashPayment --> PaymentValidation: Enter Amount
    CardPayment --> PaymentValidation: Process Card
    QRPayment --> PaymentValidation: Scan QR
    MixedPayment --> PaymentValidation: Split Amounts
    
    PaymentValidation --> PaymentFailed: Validation Failed
    PaymentValidation --> TransactionProcessing: Validation Success
    
    PaymentFailed --> PaymentSelection: Retry
    
    TransactionProcessing --> DatabaseOperations: Begin Transaction
    
    DatabaseOperations --> TransactionFailed: Error Occurred
    DatabaseOperations --> TransactionSuccess: All Saved
    
    TransactionFailed --> PaymentSelection: Rollback & Retry
    
    TransactionSuccess --> InvoiceGeneration: Commit Transaction
    InvoiceGeneration --> ReceiptDelivery: Invoice Created
    
    ReceiptDelivery --> TransactionComplete: Receipt Delivered
    TransactionComplete --> Idle: Reset POS
    
    note right of ProductSelection
        Load products from database
        Display in grid with images
        Enable search and filter
    end note
    
    note right of CartBuilding
        Validate stock availability
        Calculate line totals
        Update cart display
    end note
    
    note right of DatabaseOperations
        Create Sale
        Create Invoice
        Create Payment
        Update Stock
        Create Movements
        Create Audit Log
    end note
```

### Payment Processing Flow

```mermaid
flowchart LR
    subgraph "Payment Input"
        PI1[Cash Payment]
        PI2[Card Payment]
        PI3[QR Payment]
        PI4[Mixed Payment]
    end
    
    subgraph "Validation"
        V1[Validate Amount]
        V2[Validate Card]
        V3[Validate QR Scan]
        V4[Validate Split]
    end
    
    subgraph "Processing"
        P1[Calculate Change]
        P2[Process Card Transaction]
        P3[Confirm QR Payment]
        P4[Process Multiple Payments]
    end
    
    subgraph "Database Operations"
        D1[Create Payment Record]
        D2[Update Invoice Status]
        D3[Update Sale Status]
        D4[Create Audit Log]
    end
    
    subgraph "Result"
        R1[Payment Success]
        R2[Payment Failed]
    end
    
    PI1 --> V1
    PI2 --> V2
    PI3 --> V3
    PI4 --> V4
    
    V1 --> P1
    V2 --> P2
    V3 --> P3
    V4 --> P4
    
    P1 --> D1
    P2 --> D1
    P3 --> D1
    P4 --> D1
    
    D1 --> D2
    D2 --> D3
    D3 --> D4
    
    D4 --> R1
    
    V1 -.->|Invalid| R2
    V2 -.->|Invalid| R2
    V3 -.->|Invalid| R2
    V4 -.->|Invalid| R2
    
    style PI1 fill:#3B82F6,stroke:#2563EB,color:#fff
    style PI2 fill:#3B82F6,stroke:#2563EB,color:#fff
    style PI3 fill:#3B82F6,stroke:#2563EB,color:#fff
    style PI4 fill:#3B82F6,stroke:#2563EB,color:#fff
    style R1 fill:#22C55E,stroke:#16A34A,color:#fff
    style R2 fill:#EF4444,stroke:#DC2626,color:#fff
```

### Data Presentation by Module

```mermaid
graph TB
    subgraph "Dashboard Module"
        D1[Load User Session]
        D2[Query Sales Data]
        D3[Calculate KPIs]
        D4[Display Metrics]
        
        D1 --> D2
        D2 --> D3
        D3 --> D4
    end
    
    subgraph "POS Module"
        P1[Load Products]
        P2[Display Product Grid]
        P3[Manage Shopping Cart]
        P4[Calculate Totals]
        P5[Process Payment]
        P6[Generate Invoice]
        
        P1 --> P2
        P2 --> P3
        P3 --> P4
        P4 --> P5
        P5 --> P6
    end
    
    subgraph "Product Module"
        PR1[Query Products]
        PR2[Load Categories]
        PR3[Load Suppliers]
        PR4[Display DataGridView]
        PR5[Enable CRUD Operations]
        
        PR1 --> PR4
        PR2 --> PR4
        PR3 --> PR4
        PR4 --> PR5
    end
    
    subgraph "Customer Module"
        C1[Query Customers]
        C2[Load Purchase History]
        C3[Calculate Loyalty Points]
        C4[Display Customer List]
        C5[Enable Management]
        
        C1 --> C4
        C2 --> C4
        C3 --> C4
        C4 --> C5
    end
    
    subgraph "Reports Module"
        R1[Select Date Range]
        R2[Query Transaction Data]
        R3[Aggregate Data]
        R4[Generate Charts]
        R5[Display Analytics]
        R6[Export Options]
        
        R1 --> R2
        R2 --> R3
        R3 --> R4
        R4 --> R5
        R5 --> R6
    end
    
    style D4 fill:#3B82F6,stroke:#2563EB,color:#fff
    style P6 fill:#22C55E,stroke:#16A34A,color:#fff
    style PR5 fill:#A855F7,stroke:#9333EA,color:#fff
    style C5 fill:#EC4899,stroke:#DB2777,color:#fff
    style R6 fill:#F59E0B,stroke:#D97706,color:#fff
```

### Real-Time Data Updates

```mermaid
graph TD
    A[User Action] --> B{Action Type}
    
    B -->|Create| C1[Insert to Database]
    B -->|Read| C2[Query from Database]
    B -->|Update| C3[Modify in Database]
    B -->|Delete| C4[Remove from Database]
    
    C1 --> D[Database Transaction]
    C2 --> D
    C3 --> D
    C4 --> D
    
    D --> E{Success?}
    
    E -->|Yes| F1[Commit Transaction]
    E -->|No| F2[Rollback Transaction]
    
    F1 --> G1[Update UI]
    F1 --> G2[Create Audit Log]
    F1 --> G3[Refresh Data Grid]
    F1 --> G4[Show Success Message]
    
    F2 --> H1[Show Error Message]
    F2 --> H2[Log Error]
    F2 --> H3[Restore Previous State]
    
    G1 --> I[User Sees Updated Data]
    G3 --> I
    G4 --> I
    
    H1 --> J[User Sees Error]
    H3 --> J
    
    style A fill:#14B8A6,stroke:#0D9488,color:#fff
    style F1 fill:#22C55E,stroke:#16A34A,color:#fff
    style F2 fill:#EF4444,stroke:#DC2626,color:#fff
    style I fill:#3B82F6,stroke:#2563EB,color:#fff
    style J fill:#F97316,stroke:#EA580C,color:#fff
```

### Interface Component Hierarchy

```mermaid
graph TB
    A[Main Application] --> B[Login Form]
    B --> C{Authentication}
    
    C -->|Admin| D[Admin Dashboard]
    C -->|Cashier| E[Cashier Dashboard]
    C -->|Clerk| F[Inventory Dashboard]
    
    D --> D1[User Management View]
    D --> D2[System Settings View]
    D --> D3[Analytics View]
    D --> D4[Audit Log View]
    
    E --> E1[POS View]
    E --> E2[Customer Management View]
    E --> E3[Sales History View]
    E --> E4[Invoice View]
    
    F --> F1[Product Management View]
    F --> F2[Category Management View]
    F --> F3[Supplier Management View]
    F --> F4[Inventory Reports View]
    
    E1 --> E1A[Product Grid Panel]
    E1 --> E1B[Shopping Cart Panel]
    E1 --> E1C[Payment Panel]
    E1 --> E1D[Customer Selection Panel]
    
    F1 --> F1A[Product List DataGrid]
    F1 --> F1B[Product Form Panel]
    F1 --> F1C[Image Upload Panel]
    F1 --> F1D[Action Buttons Panel]
    
    D3 --> D3A[KPI Cards]
    D3 --> D3B[Sales Chart]
    D3 --> D3C[Revenue Chart]
    D3 --> D3D[Product Performance Chart]
    
    style A fill:#14B8A6,stroke:#0D9488,color:#fff
    style D fill:#3B82F6,stroke:#2563EB,color:#fff
    style E fill:#22C55E,stroke:#16A34A,color:#fff
    style F fill:#A855F7,stroke:#9333EA,color:#fff
```

### Inventory Update Flow During Sale

```mermaid
flowchart TD
    Start([Sale Completed]) --> GetItems[Get All Sale Items]
    GetItems --> Loop{For Each Item}
    
    Loop -->|Next Item| GetProduct[Get Product by ID]
    GetProduct --> CurrentQty[Get Current Quantity]
    CurrentQty --> CalcNew[New Qty = Current - Sold]
    CalcNew --> UpdateProduct[Update Product Quantity]
    UpdateProduct --> CreateMovement[Create Inventory Movement]
    
    CreateMovement --> MovementData[Movement Data:<br/>Type: Sale<br/>Quantity: -Sold<br/>Reference: Invoice Number<br/>User: Cashier<br/>Timestamp: Now]
    
    MovementData --> CheckMin{Qty < MinQuantity}
    CheckMin -->|Yes| LowStock[Flag Low Stock Alert]
    CheckMin -->|No| NextItem
    
    LowStock --> NextItem{More Items}
    NextItem -->|Yes| Loop
    NextItem -->|No| Complete[Inventory Updated]
    
    Complete --> End([Done])
    
    style Start fill:#14B8A6,stroke:#0D9488,color:#fff
    style UpdateProduct fill:#F59E0B,stroke:#D97706,color:#fff
    style LowStock fill:#EF4444,stroke:#DC2626,color:#fff
    style Complete fill:#22C55E,stroke:#16A34A,color:#fff
```

### Customer Loyalty Points Calculation

```mermaid
flowchart LR
    subgraph "Transaction Data"
        T1[Total Amount]
        T2[Customer ID]
    end
    
    subgraph "Calculation"
        C1[Points = Total / 10]
        C2[Round Down]
    end
    
    subgraph "Database Update"
        D1[Get Current Points]
        D2[Add New Points]
        D3[Update Customer Record]
    end
    
    subgraph "Result"
        R1[Points Added]
        R2[Display to Cashier]
    end
    
    T1 --> C1
    T2 --> D1
    C1 --> C2
    C2 --> D2
    D1 --> D2
    D2 --> D3
    D3 --> R1
    R1 --> R2
    
    style T1 fill:#3B82F6,stroke:#2563EB,color:#fff
    style C1 fill:#F59E0B,stroke:#D97706,color:#fff
    style D3 fill:#A855F7,stroke:#9333EA,color:#fff
    style R1 fill:#22C55E,stroke:#16A34A,color:#fff
```

### Invoice Generation Process

```mermaid
flowchart TD
    Start([Generate Invoice]) --> GetData[Collect Invoice Data]
    
    GetData --> D1[Sale Information]
    GetData --> D2[Customer Details]
    GetData --> D3[Line Items]
    GetData --> D4[Payment Info]
    GetData --> D5[Company Info]
    
    D1 --> Format[Format Invoice Document]
    D2 --> Format
    D3 --> Format
    D4 --> Format
    D5 --> Format
    
    Format --> Template[Apply Invoice Template]
    Template --> AddHeader[Add Company Header]
    AddHeader --> AddCustomer[Add Customer Section]
    AddCustomer --> AddItems[Add Line Items Table]
    AddItems --> AddTotals[Add Totals Section]
    AddTotals --> AddPayment[Add Payment Details]
    AddPayment --> AddFooter[Add Footer & Terms]
    
    AddFooter --> OutputType{Output Type}
    
    OutputType -->|Print| PrintDoc[Send to Printer]
    OutputType -->|PDF| GenPDF[Generate PDF File]
    OutputType -->|Email| EmailDoc[Email PDF]
    OutputType -->|View| DisplayDoc[Display on Screen]
    
    PrintDoc --> Complete
    GenPDF --> Complete
    EmailDoc --> Complete
    DisplayDoc --> Complete
    
    Complete[Invoice Delivered] --> End([Done])
    
    style Start fill:#14B8A6,stroke:#0D9488,color:#fff
    style Format fill:#3B82F6,stroke:#2563EB,color:#fff
    style Complete fill:#22C55E,stroke:#16A34A,color:#fff
```

### Audit Log Creation

```mermaid
flowchart LR
    subgraph "Event Trigger"
        E1[User Action]
        E2[System Event]
    end
    
    subgraph "Capture Data"
        C1[User ID]
        C2[Action Type]
        C3[Table Name]
        C4[Record ID]
        C5[Old Values]
        C6[New Values]
        C7[Timestamp]
        C8[IP Address]
    end
    
    subgraph "Process"
        P1[Serialize Values to JSON]
        P2[Create Audit Log Entry]
    end
    
    subgraph "Storage"
        S1[(AuditLog Table)]
    end
    
    E1 --> C1
    E2 --> C1
    
    C1 --> P1
    C2 --> P1
    C3 --> P1
    C4 --> P1
    C5 --> P1
    C6 --> P1
    C7 --> P1
    C8 --> P1
    
    P1 --> P2
    P2 --> S1
    
    style E1 fill:#F59E0B,stroke:#D97706,color:#fff
    style P2 fill:#3B82F6,stroke:#2563EB,color:#fff
    style S1 fill:#EF4444,stroke:#DC2626,color:#fff
```

### Error Handling in POS

```mermaid
flowchart TD
    Action[User Action] --> Try{Try Operation}
    
    Try -->|Success| Success[Operation Complete]
    Try -->|Error| CatchError[Catch Exception]
    
    CatchError --> ErrorType{Error Type}
    
    ErrorType -->|Database| DBError[Database Error]
    ErrorType -->|Validation| ValError[Validation Error]
    ErrorType -->|Network| NetError[Network Error]
    ErrorType -->|Other| GenError[General Error]
    
    DBError --> LogError[Log Error Details]
    ValError --> LogError
    NetError --> LogError
    GenError --> LogError
    
    LogError --> Rollback{Transaction Active}
    
    Rollback -->|Yes| RollbackTrans[Rollback Transaction]
    Rollback -->|No| ShowError
    
    RollbackTrans --> RestoreState[Restore Previous State]
    RestoreState --> ShowError[Show Error Message]
    
    ShowError --> UserAction{User Decision}
    
    UserAction -->|Retry| Action
    UserAction -->|Cancel| Cancel[Cancel Operation]
    
    Success --> End([Done])
    Cancel --> End
    
    style Action fill:#14B8A6,stroke:#0D9488,color:#fff
    style Success fill:#22C55E,stroke:#16A34A,color:#fff
    style CatchError fill:#F97316,stroke:#EA580C,color:#fff
    style RollbackTrans fill:#EF4444,stroke:#DC2626,color:#fff
    style ShowError fill:#F59E0B,stroke:#D97706,color:#fff
```

### Multi-Payment Processing

```mermaid
flowchart TD
    Start([Mixed Payment Selected]) --> GetTotal[Get Total Amount]
    GetTotal --> InitPayments[Initialize Payment List]
    
    InitPayments --> AddPayment{Add Payment Method}
    
    AddPayment -->|Cash| CashAmount[Enter Cash Amount]
    AddPayment -->|Card| CardAmount[Enter Card Amount]
    AddPayment -->|QR| QRAmount[Enter QR Amount]
    
    CashAmount --> AddToList[Add to Payment List]
    CardAmount --> AddToList
    QRAmount --> AddToList
    
    AddToList --> CalcRemaining[Calculate Remaining Balance]
    CalcRemaining --> ShowRemaining[Display Remaining Amount]
    
    ShowRemaining --> CheckBalance{Balance = 0}
    
    CheckBalance -->|No| MorePayments{Add More}
    CheckBalance -->|Yes| ValidateAll[Validate All Payments]
    
    MorePayments -->|Yes| AddPayment
    MorePayments -->|No| BalanceError[Show Balance Error]
    
    BalanceError --> AddPayment
    
    ValidateAll --> ProcessEach[Process Each Payment]
    ProcessEach --> CreateRecords[Create Payment Records]
    CreateRecords --> UpdateInvoice[Update Invoice Status]
    UpdateInvoice --> Complete[Payment Complete]
    
    Complete --> End([Done])
    
    style Start fill:#14B8A6,stroke:#0D9488,color:#fff
    style ValidateAll fill:#F59E0B,stroke:#D97706,color:#fff
    style Complete fill:#22C55E,stroke:#16A34A,color:#fff
    style BalanceError fill:#EF4444,stroke:#DC2626,color:#fff
```

### Transaction Hold and Resume

```mermaid
flowchart LR
    subgraph "Hold Transaction"
        H1[Current Cart State]
        H2[Generate Hold Code]
        H3[Serialize Cart to JSON]
        H4[Save to HeldTransaction]
        H5[Clear Current Cart]
    end
    
    subgraph "Resume Transaction"
        R1[Enter Hold Code]
        R2[Query HeldTransaction]
        R3[Deserialize Cart JSON]
        R4[Restore Cart Items]
        R5[Restore Customer]
        R6[Restore Totals]
    end
    
    subgraph "Complete Held"
        C1[Process Payment]
        C2[Mark as Completed]
        C3[Delete Hold Record]
    end
    
    H1 --> H2
    H2 --> H3
    H3 --> H4
    H4 --> H5
    
    R1 --> R2
    R2 --> R3
    R3 --> R4
    R4 --> R5
    R5 --> R6
    
    R6 --> C1
    C1 --> C2
    C2 --> C3
    
    style H4 fill:#3B82F6,stroke:#2563EB,color:#fff
    style R4 fill:#F59E0B,stroke:#D97706,color:#fff
    style C2 fill:#22C55E,stroke:#16A34A,color:#fff
```

---

## Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Framework | .NET | 8.0 |
| Language | C# | 12 |
| UI | Windows Forms | 8.0 |
| ORM | Entity Framework Core | 8.0.0 |
| Database | SQL Server | 2019+ |
| Security | BCrypt.Net-Next | 4.0.3 |
| Icons | FontAwesome.Sharp | 6.3.0 |
| UI Theme | MaterialSkin.2 | 2.1.0 |
| QR Codes | QRCoder | 1.7.0 |
| Barcode | ZXing.Net | 0.16.9 |
| PDF | QuestPDF | 2023.12.6 |

---

## Installation

### Prerequisites

- Windows 10 (1809+) or Windows 11
- .NET 8.0 Runtime
- SQL Server 2019+ or SQL Server Express
- 4 GB RAM minimum (8 GB recommended)
- 500 MB disk space

### Setup Steps

```bash
# 1. Clone repository
git clone https://github.com/yourusername/syncversestudio.git
cd syncversestudio

# 2. Restore packages
dotnet restore syncversestudio/syncversestudio.csproj

# 3. Update connection string in ApplicationDbContext.cs
# Data Source=YOUR_SERVER;Initial Catalog=POSDB;Integrated Security=True;Trust Server Certificate=True

# 4. Create database
dotnet ef database update --project syncversestudio

# 5. Build application
dotnet build syncversestudio/syncversestudio.csproj --configuration Release

# 6. Run application
dotnet run --project syncversestudio/syncversestudio.csproj
```

### Default Login

Username: vi
Password: admin123

Change password immediately after first login.

---

## Key Features

### Point of Sale

- Modern cashier interface
- Product search and filtering
- Shopping cart with real-time calculations
- Multiple payment methods (Cash, Card, QR)
- Barcode scanning support
- Invoice printing
- Transaction hold and resume

### Inventory Management

- Product CRUD operations
- Category and supplier management
- Stock level tracking
- Low-stock alerts
- Product image management
- SKU and barcode support
- Inventory movement logging

### Customer Management

- Customer profiles
- Purchase history tracking
- Loyalty points system
- Walk-in customer support
- Data encryption

### Invoicing

- Automated invoice generation
- Multiple payment methods
- Partial payment tracking
- Invoice status management
- Tax calculation
- Professional printing

### Analytics

- Real-time sales dashboard
- Revenue and profit tracking
- Sales trend visualization
- Product popularity analysis
- Inventory performance reports
- Staff performance metrics

### Security

- Role-based access control
- BCrypt password hashing
- User authentication
- Audit logging
- Customer data encryption
- Session management

---

## Permission Matrix

| Feature | Administrator | Cashier | Inventory Clerk |
|---------|--------------|---------|-----------------|
| User Management | Yes | No | No |
| Product Management | Yes | No | Yes |
| Sales Operations | Yes | Yes | No |
| Customer Management | Yes | Yes | No |
| Inventory Adjustment | Yes | No | Yes |
| Reports | All | Sales | Inventory |
| System Configuration | Yes | No | No |
| Audit Logs | Yes | No | No |

---

## Configuration

### Database Connection

Windows Authentication:
```
Data Source=SERVER_NAME;Initial Catalog=POSDB;Integrated Security=True;Trust Server Certificate=True
```

SQL Authentication:
```
Data Source=SERVER_NAME;Initial Catalog=POSDB;User Id=USERNAME;Password=PASSWORD;Trust Server Certificate=True
```

LocalDB:
```
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=POSDB;Integrated Security=True;Trust Server Certificate=True
```

### Tax Configuration

Default tax rate: 10%
Modify in POS interface using tax rate control.

### Invoice Format

Format: INV-YYYYMMDD-HHMMSS
Example: INV-20251027-143052

---

## Usage Guide

### Administrator Tasks

1. Login as Administrator
2. Navigate to User Management
3. Create user accounts with appropriate roles
4. Configure system settings
5. Monitor audit logs
6. Generate reports and analytics

### Cashier Tasks

1. Login with cashier credentials
2. Click Cashier (POS) from dashboard
3. Select customer type
4. Add products to cart
5. Review cart and totals
6. Select payment method
7. Complete transaction
8. Print or email receipt

### Inventory Clerk Tasks

1. Login with clerk credentials
2. Navigate to Products
3. Add or edit products
4. Manage categories and suppliers
5. Monitor stock levels
6. Respond to low-stock alerts
7. Generate inventory reports

---

## Deployment Scenarios

### Single Store

```mermaid
graph TD
    A[Single PC] --> B[Application]
    B --> C[SQL Server LocalDB]
    C --> D[(Database)]
    
    style A fill:#3B82F6,stroke:#2563EB,color:#fff
    style B fill:#22C55E,stroke:#16A34A,color:#fff
    style C fill:#A855F7,stroke:#9333EA,color:#fff
    style D fill:#EF4444,stroke:#DC2626,color:#fff
```

### Multi-Terminal

```mermaid
graph TD
    A[Terminal 1] --> D[Network]
    B[Terminal 2] --> D
    C[Terminal 3] --> D
    D --> E[SQL Server]
    E --> F[(Database)]
    
    style A fill:#3B82F6,stroke:#2563EB,color:#fff
    style B fill:#3B82F6,stroke:#2563EB,color:#fff
    style C fill:#3B82F6,stroke:#2563EB,color:#fff
    style D fill:#22C55E,stroke:#16A34A,color:#fff
    style E fill:#A855F7,stroke:#9333EA,color:#fff
    style F fill:#EF4444,stroke:#DC2626,color:#fff
```

---

## Project Structure

```
syncversestudio/
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Migrations/
├── Models/
│   ├── User.cs
│   ├── Product.cs
│   ├── Sale.cs
│   ├── Invoice.cs
│   └── ...
├── Services/
│   ├── AuthenticationService.cs
│   └── DatabaseInitializer.cs
├── Views/
│   ├── LoginForm.cs
│   ├── MainDashboard.cs
│   └── CashierDashboard/
├── Helpers/
│   ├── ProductImageHelper.cs
│   └── BrandTheme.cs
└── Program.cs
```

---

## Development

### Build Commands

```bash
# Clean
dotnet clean syncversestudio/syncversestudio.csproj

# Restore
dotnet restore syncversestudio/syncversestudio.csproj

# Build Debug
dotnet build syncversestudio/syncversestudio.csproj --configuration Debug

# Build Release
dotnet build syncversestudio/syncversestudio.csproj --configuration Release

# Run
dotnet run --project syncversestudio/syncversestudio.csproj
```

### Database Migrations

```bash
# Create migration
dotnet ef migrations add MigrationName --project syncversestudio

# Apply migration
dotnet ef database update --project syncversestudio

# Remove migration
dotnet ef migrations remove --project syncversestudio

# Generate SQL script
dotnet ef migrations script --project syncversestudio --output migration.sql
```

---

## Security Features

### Authentication

- BCrypt password hashing (work factor 11)
- Automatic salt generation
- No plain text storage
- Session tracking

### Authorization

- Role-based access control
- Permission enforcement
- Feature-level restrictions

### Data Protection

- Customer data encryption
- Payment information protection
- API key encryption

### Audit Trail

All operations logged:
- User login/logout
- Data modifications
- Transactions
- Payments
- Inventory adjustments
- System changes

---

## Performance

| Operation | Time | Notes |
|-----------|------|-------|
| Application Startup | 2-3s | First launch slower |
| User Login | <1s | BCrypt verification |
| Dashboard Load | 1-2s | 1000+ transactions |
| Product Search | <500ms | 10,000+ products |
| Add to Cart | <100ms | Real-time |
| Complete Transaction | 1-2s | Database writes |
| Invoice Generation | <1s | PDF creation |
| Report Generation | 2-5s | Date range dependent |

### Scalability

- Products: 50,000+
- Transactions: 100,000+
- Customers: 10,000+
- Concurrent Users: 5-10
- Database Size: Up to 10 GB

---

## Troubleshooting

### Application won't start

- Verify .NET 8.0 Runtime installed
- Check database connection string
- Ensure SQL Server running
- Review application logs

### Database connection failed

- Verify SQL Server accessible
- Check connection string format
- Ensure database exists
- Verify user permissions

### Login fails

- Check user account active
- Verify credentials
- Review audit logs
- Ensure database accessible

### Products not showing images

- Verify image files exist
- Check file permissions
- Validate ImagePath
- Check file formats (JPG, PNG)

---

## Backup and Restore

### Manual Backup

```bash
# Backup
sqlcmd -S SERVER_NAME -E -Q "BACKUP DATABASE POSDB TO DISK='C:\Backup\POSDB.bak' WITH FORMAT"

# Restore
sqlcmd -S SERVER_NAME -E -Q "RESTORE DATABASE POSDB FROM DISK='C:\Backup\POSDB.bak' WITH REPLACE"
```

### Automated Backup

Schedule SQL Server Agent job for daily backups at 2 AM:

```sql
BACKUP DATABASE POSDB
TO DISK = 'C:\Backups\POSDB_' + CONVERT(VARCHAR, GETDATE(), 112) + '.bak'
WITH FORMAT, COMPRESSION, STATS = 10;
```

---

## Contributing

### Process

1. Fork repository
2. Create feature branch
3. Make changes
4. Test thoroughly
5. Commit with clear messages
6. Push to fork
7. Submit pull request

### Commit Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

Types: feat, fix, docs, style, refactor, test, chore

Example:
```
feat(pos): Add QR code payment support

Implemented QR code generation for mobile payments.

Closes #123
```

---

## Roadmap

### Version 1.1 (Q1 2026)

- Barcode scanner hardware integration
- Receipt printer integration
- Email notifications
- SMS notifications
- Advanced charts
- PDF reports
- Excel export

### Version 1.2 (Q2 2026)

- Multi-language support
- Dark mode
- Mobile app
- Cloud backup
- Enhanced analytics
- Loyalty program improvements
- Discount system

### Version 2.0 (Q3 2026)

- E-commerce integration
- Multi-store support
- Franchise management
- Inventory forecasting
- AI sales predictions
- Customer analytics
- Automated reordering

---

## License

MIT License

Copyright (c) 2025 SYNCVERSE STUDIO

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

---

## Support

### Documentation

Additional guides in GUIDE/ folder:
- Quick Start Guide
- Technology Stack
- Project Structure
- Brand Theme Guide
- Security Policy
- Deployment Checklist
- Migration Guide
- Analytics Feature
- Enhanced POS Documentation

### Contact

- Repository: https://github.com/EIRSVi/sync-verse-studio
- Issues: https://github.com/EIRSVi/sync-verse-studio/issues
- Email: support@syncverse.com

---

## Acknowledgments

### Development Team

Lead Developer: Vi

Contributors:
- PHA***NAK (100034879410842)
- pha******nn (100006581647309)
- Sa***Dy (100028267065321)
- CH****KLA (100074770834689)
- Ph****nna (100057666978328)

### Technologies

Special thanks to:
- Microsoft (.NET, Entity Framework Core)
- Open-source community
- All contributors and testers

---

SYNCVERSE STUDIO - Empowering Retail Excellence

Version 1.0.0 | October 2025
