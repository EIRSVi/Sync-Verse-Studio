# 🚀 START HERE - Accounting System Setup

## ⚠️ IMPORTANT: Follow These Steps in Order

### Step 1: Run Database Migration (REQUIRED FIRST!)
**You MUST do this before running the application!**

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your server: `DESKTOP-6RCREN5\MSSQLSERVER01`
3. Select your database: `POSDB`
4. Click **"New Query"**
5. Open the file: `Database/AddAccountingTables.sql`
6. Click **"Execute"** (or press F5)
7. Wait for success message: **"Accounting tables created successfully!"**

**Without this step, the Accounting Reports menu will not work!**

---

### Step 2: Rebuild the Application

**Option A: Using Visual Studio**
1. Open the solution in Visual Studio
2. Go to **Build** menu
3. Click **"Clean Solution"**
4. Then click **"Rebuild Solution"**
5. Wait for build to complete (should say "Build succeeded")

**Option B: Using Command Line**
```bash
dotnet clean
dotnet build
```

---

### Step 3: Run the Application

1. Press **F5** in Visual Studio (or click Start)
2. Login with your credentials
3. Look at the sidebar menu

---

### Step 4: Find "Accounting Reports"

**For Administrator:**
- Dashboard
- Users
- Products
- Customer Management
- Categories
- Suppliers
- Analytics
- **👉 Accounting Reports** ← HERE!
- Audit Logs

**For Cashier:**
- Dashboard
- Invoices
- Payment Links
- Online Store
- Cashier (POS)
- Products
- Clients
- **👉 Accounting Reports** ← HERE!
- Reports

**For Inventory Clerk:**
- Dashboard
- Products
- Categories
- Suppliers
- Inventory
- **👉 Accounting Reports** ← HERE!
- Reports

---

### Step 5: Click "Accounting Reports"

You should see a new window with **6 tabs**:
1. General Ledger
2. Sales Day Book
3. Purchases Day Book
4. Cash Book
5. General Journal
6. Financial Statements

---

## 🐛 Troubleshooting

### Problem: "Accounting Reports" menu item not showing

**Solution 1: Did you rebuild?**
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

**Solution 2: Did you run the database migration?**
- Check if tables exist:
```sql
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('GeneralLedgerEntries', 'FinancialAccounts')
```
- If no results, run `Database/AddAccountingTables.sql`

**Solution 3: Restart the application**
- Close the application completely
- Rebuild the solution
- Run again

---

### Problem: Error when clicking "Accounting Reports"

**Check database connection:**
```sql
-- Run this in SSMS to verify tables exist
SELECT COUNT(*) FROM FinancialAccounts
-- Should return 14
```

**If tables don't exist:**
- Run `Database/AddAccountingTables.sql` in SSMS

---

### Problem: Reports are empty

**This is normal if:**
- You haven't made any sales yet
- The date range filter excludes your transactions

**To test:**
1. Go to POS/Cashier
2. Make a test sale
3. Go back to Accounting Reports
4. You should see the sale in:
   - General Ledger
   - Sales Day Book
   - Cash Book
   - Financial Statements

---

## ✅ Quick Verification

Run these SQL queries to verify setup:

```sql
-- 1. Check if tables exist (should return 4 rows)
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('GeneralLedgerEntries', 'Purchases', 'PurchaseItems', 'FinancialAccounts')

-- 2. Check financial accounts (should return 14 rows)
SELECT COUNT(*) FROM FinancialAccounts

-- 3. Check account details
SELECT AccountCode, AccountName, AccountType 
FROM FinancialAccounts 
ORDER BY AccountCode
```

Expected accounts:
- 1000 - Cash
- 1100 - Accounts Receivable
- 1200 - Inventory
- 1500 - Equipment
- 2000 - Accounts Payable
- 2100 - Loans Payable
- 3000 - Owner's Capital
- 3100 - Retained Earnings
- 4000 - Sales Revenue
- 4100 - Other Income
- 5000 - Cost of Goods Sold
- 6000 - Operating Expenses
- 6100 - Utilities
- 6200 - Rent

---

## 📸 What You Should See

### In the Sidebar Menu:
```
┌─────────────────────────┐
│ SyncVerse               │
│ POS SYSTEM              │
├─────────────────────────┤
│ NAVIGATION              │
├─────────────────────────┤
│ 📊 Dashboard            │
│ 👥 Users                │
│ 📦 Products             │
│ ...                     │
│ 💰 Accounting Reports   │ ← This should be visible!
│ ...                     │
└─────────────────────────┘
```

### When You Click It:
```
┌─────────────────────────────────────────────┐
│ 💰 Accounting Reports                       │
│ General Ledger • Books of Entry • Financial │
├─────────────────────────────────────────────┤
│ [General Ledger] [Sales Day Book] [Purchases│
│  Day Book] [Cash Book] [General Journal]    │
│  [Financial Statements]                     │
├─────────────────────────────────────────────┤
│                                             │
│  (Report data appears here)                 │
│                                             │
└─────────────────────────────────────────────┘
```

---

## 🎯 Still Not Working?

### Check Build Output:
1. In Visual Studio, go to **View** → **Output**
2. Look for any errors during build
3. Make sure it says "Build succeeded"

### Check Error List:
1. In Visual Studio, go to **View** → **Error List**
2. Should show 0 errors
3. If there are errors, read them carefully

### Verify Files Exist:
```bash
# Run in PowerShell
Test-Path "Views\AccountingReportsView.cs"
Test-Path "Models\AccountingModels.cs"
Test-Path "Services\AccountingService.cs"
Test-Path "Database\AddAccountingTables.sql"
```
All should return `True`

---

## 📞 Need More Help?

1. **Check the full documentation:**
   - `GUIDE/ACCOUNTING_QUICK_START.md`
   - `GUIDE/ACCOUNTING_SYSTEM_COMPLETE.md`

2. **Check the installation checklist:**
   - `ACCOUNTING_INSTALLATION_CHECKLIST.md`

3. **Review the code:**
   - `Views/AccountingReportsView.cs` - The UI
   - `Views/MainDashboard.cs` - Menu items (lines 240-280)

---

## 🎉 Success!

Once you see "Accounting Reports" in the menu and can click it:
1. ✅ Database migration successful
2. ✅ Application built correctly
3. ✅ Menu items loaded
4. ✅ Ready to use!

Make a test sale and watch it appear in all the accounting reports automatically!

---

**Remember: The database migration MUST be run first, or nothing will work!**
