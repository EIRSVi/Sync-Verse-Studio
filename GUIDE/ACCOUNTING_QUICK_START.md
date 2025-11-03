# Accounting System - Quick Start Guide

## Step 1: Run Database Migration

### Option A: Using SQL Server Management Studio (SSMS)
1. Open SQL Server Management Studio
2. Connect to your database server: `DESKTOP-6RCREN5\MSSQLSERVER01`
3. Click "New Query"
4. Open the file: `Database/AddAccountingTables.sql`
5. Make sure the correct database is selected (POSDB or khmerdatabase)
6. Click "Execute" or press F5
7. Wait for the success message: "Accounting tables created successfully!"

### Option B: Using Command Line
```bash
sqlcmd -S DESKTOP-6RCREN5\MSSQLSERVER01 -d POSDB -i Database\AddAccountingTables.sql
```

## Step 2: Verify Installation

1. **Check Tables Created**:
   ```sql
   SELECT * FROM INFORMATION_SCHEMA.TABLES 
   WHERE TABLE_NAME IN ('GeneralLedgerEntries', 'Purchases', 'PurchaseItems', 'FinancialAccounts')
   ```

2. **Check Financial Accounts**:
   ```sql
   SELECT * FROM FinancialAccounts ORDER BY AccountCode
   ```
   You should see 14 default accounts.

## Step 3: Run the Application

1. Open the solution in Visual Studio or your IDE
2. Build the solution (Ctrl+Shift+B)
3. Run the application (F5)
4. Login with your credentials

## Step 4: Access Accounting Reports

### For Administrator:
1. Login as Administrator
2. Look for "Accounting Reports" in the sidebar menu
3. Click to open the accounting reports view

### For Cashier:
1. Login as Cashier
2. Look for "Accounting Reports" in the sidebar menu
3. Click to open the accounting reports view

### For Inventory Clerk:
1. Login as Inventory Clerk
2. Look for "Accounting Reports" in the sidebar menu
3. Click to open the accounting reports view

## Step 5: Test the System

### Test 1: Make a Sale
1. Go to "Cashier (POS)" or "Point of Sale"
2. Add products to cart
3. Complete a sale
4. Go to "Accounting Reports"
5. Check the following tabs:
   - **General Ledger**: Should show debit to Cash and credit to Sales Revenue
   - **Sales Day Book**: Should show your sale
   - **Cash Book**: Should show cash received
   - **Financial Statements**: Income Statement should show revenue

### Test 2: View Financial Statements
1. Go to "Accounting Reports"
2. Click on "Financial Statements" tab
3. You should see:
   - **Left side**: Balance Sheet (Assets, Liabilities, Equity)
   - **Right side**: Income Statement (Revenue, Expenses, Net Income)

### Test 3: Filter by Date Range
1. At the top of Accounting Reports, you'll see date pickers
2. Select a start date (e.g., last month)
3. Select an end date (e.g., today)
4. Click "Refresh"
5. All reports will update to show only transactions in that date range

## Common Issues and Solutions

### Issue 1: "Table already exists" error
**Solution**: The tables are already created. You can skip the migration or drop the tables first:
```sql
DROP TABLE IF EXISTS PurchaseItems;
DROP TABLE IF EXISTS Purchases;
DROP TABLE IF EXISTS GeneralLedgerEntries;
DROP TABLE IF EXISTS FinancialAccounts;
```
Then run the migration script again.

### Issue 2: Menu item not showing
**Solution**: 
- Make sure you rebuilt the solution after adding the new files
- Check that you're logged in with the correct user role
- Restart the application

### Issue 3: No data in reports
**Solution**:
- Make sure you have made at least one sale
- Check the date range filter - it might be excluding your transactions
- Verify the database connection is working

### Issue 4: Foreign key constraint errors
**Solution**:
- Make sure the Payments table exists before running the migration
- If you get FK errors, comment out the payment-related foreign key section and run it separately

## What Each Report Shows

### General Ledger
- **All accounting entries** in the system
- Shows debits and credits for every transaction
- Links to source documents (sales, purchases, payments)

### Sales Day Book
- **All sales transactions**
- Customer information
- Payment methods
- Sales totals

### Purchases Day Book
- **All purchase orders**
- Supplier information
- Purchase amounts
- Payment status

### Cash Book
- **All cash movements**
- Cash in (receipts)
- Cash out (payments)
- Running balance

### General Journal
- **Manual journal entries**
- Adjustments
- Corrections

### Financial Statements
- **Balance Sheet**: Financial position (what you own and owe)
- **Income Statement**: Financial performance (profit or loss)

## Quick Tips

1. **Date Range**: Always check your date range filter. Default is last 30 days.

2. **Refresh**: Click the "Refresh" button after making transactions to see updated data.

3. **Export**: Right-click on any grid to copy data (future feature: export to Excel/PDF).

4. **Sorting**: Click column headers to sort data.

5. **Search**: Use Ctrl+F to search within reports (future feature).

## Next Steps

1. **Make some test transactions** to see how the system works
2. **Review the financial statements** to understand your business position
3. **Set up regular reporting schedule** (daily, weekly, monthly)
4. **Train your staff** on how to use the accounting reports
5. **Backup your database** regularly

## Need Help?

- Check the full documentation: `GUIDE/ACCOUNTING_SYSTEM_COMPLETE.md`
- Review the database schema: `Database/AddAccountingTables.sql`
- Check the code: `Views/AccountingReportsView.cs`

## Success Checklist

- [ ] Database migration completed successfully
- [ ] Application builds without errors
- [ ] Can login to the application
- [ ] "Accounting Reports" menu item is visible
- [ ] Can open accounting reports view
- [ ] All 6 tabs are visible (General Ledger, Sales Day Book, Purchases Day Book, Cash Book, General Journal, Financial Statements)
- [ ] Date range filters work
- [ ] Made a test sale and it appears in reports
- [ ] Financial statements show data

---

**Ready to go!** Your accounting system is now fully operational. Start making transactions and watch the reports populate automatically!
