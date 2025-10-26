# Deployment Checklist - Enhanced Cashier Dashboard

## Pre-Deployment

### Database
- [ ] Backup existing database
- [ ] Review migration script: `Database/AddInvoicingAndPaymentTables.sql`
- [ ] Test migration on development database
- [ ] Verify all tables created successfully
- [ ] Check foreign key constraints
- [ ] Verify indexes created

### Code Review
- [ ] All new files compile without errors
- [ ] No diagnostic warnings
- [ ] Code follows project conventions
- [ ] Comments added for complex logic
- [ ] TODO items documented

### Dependencies
- [ ] .NET 8.0 Runtime installed
- [ ] SQL Server 2019+ available
- [ ] FontAwesome.Sharp NuGet package installed
- [ ] Entity Framework Core packages installed
- [ ] System.Windows.Forms.DataVisualization.Charting referenced

## Deployment Steps

### 1. Database Migration
```bash
# Backup first!
sqlcmd -S YOUR_SERVER -d POSDB -Q "BACKUP DATABASE POSDB TO DISK='C:\Backup\POSDB_backup.bak'"

# Run migration
sqlcmd -S YOUR_SERVER -d POSDB -i Database/AddInvoicingAndPaymentTables.sql

# Verify tables
sqlcmd -S YOUR_SERVER -d POSDB -Q "SELECT name FROM sys.tables WHERE name LIKE '%Invoice%' OR name LIKE '%Payment%'"
```

### 2. Build Application
```bash
# Clean build
dotnet clean
dotnet build --configuration Release

# Check for errors
# Should see: Build succeeded. 0 Warning(s). 0 Error(s).
```

### 3. Test on Staging
- [ ] Deploy to staging environment
- [ ] Test cashier login
- [ ] Verify dashboard loads
- [ ] Test POS transaction flow
- [ ] Verify invoice creation
- [ ] Check payment processing
- [ ] Test receipt options
- [ ] Verify stock updates

### 4. User Acceptance Testing
- [ ] Cashier tests dashboard
- [ ] Cashier tests POS workflow
- [ ] Manager reviews analytics
- [ ] Verify all menu items accessible
- [ ] Test with real products
- [ ] Test with real customers
- [ ] Process test transactions

### 5. Production Deployment
- [ ] Schedule maintenance window
- [ ] Notify users of new interface
- [ ] Deploy to production
- [ ] Run database migration
- [ ] Restart application
- [ ] Monitor for errors

## Post-Deployment

### Immediate Verification (First 5 Minutes)
- [ ] Cashier can login
- [ ] Dashboard displays correctly
- [ ] POS loads products
- [ ] Can complete a transaction
- [ ] Invoice creates in database
- [ ] Payment records correctly
- [ ] Stock updates properly

### First Hour Monitoring
- [ ] Check application logs
- [ ] Monitor database performance
- [ ] Watch for error messages
- [ ] Verify auto-refresh works
- [ ] Check chart rendering
- [ ] Monitor user feedback

### First Day Monitoring
- [ ] Review all transactions
- [ ] Check invoice numbering sequence
- [ ] Verify payment totals
- [ ] Monitor system performance
- [ ] Collect user feedback
- [ ] Document any issues

## Rollback Plan

### If Critical Issues Occur

1. **Stop Application**
   ```bash
   # Stop the application immediately
   ```

2. **Revert Code Changes**
   ```bash
   git revert <commit-hash>
   dotnet build
   ```

3. **Restore Database** (if needed)
   ```sql
   -- Only if database issues
   RESTORE DATABASE POSDB FROM DISK='C:\Backup\POSDB_backup.bak'
   ```

4. **Restart Application**
   ```bash
   dotnet run
   ```

### Rollback Triggers
- Application crashes on startup
- Database errors prevent transactions
- Data corruption detected
- Critical functionality broken
- Performance degradation > 50%

## Testing Scenarios

### Scenario 1: Walk-in Customer Purchase
1. Login as cashier
2. Navigate to Cashier (POS)
3. Keep "Walk-in Client" selected
4. Add 3 different products
5. Adjust quantities
6. Process cash payment
7. Choose print receipt
8. Verify invoice created
9. Check stock updated

**Expected Result:** ✅ Transaction completes, invoice #YYYYNNN created, stock reduced

### Scenario 2: Registered Customer Purchase
1. Login as cashier
2. Navigate to Cashier (POS)
3. Select registered customer
4. Add products to cart
5. Process card payment
6. Choose email receipt
7. Verify invoice linked to customer
8. Check payment record

**Expected Result:** ✅ Transaction completes, customer linked, payment recorded

### Scenario 3: Partial Payment
1. Login as cashier
2. Create transaction for 1000 KHR
3. Enter custom value: 500 KHR
4. Verify remaining shows 500 KHR
5. Pay button should be disabled
6. Enter custom value: 1000 KHR
7. Verify remaining shows 0 KHR
8. Pay button should be enabled
9. Complete payment

**Expected Result:** ✅ Partial payment logic works, button enables correctly

### Scenario 4: Save Transaction
1. Login as cashier
2. Add products to cart
3. Click save button (blue)
4. Verify transaction saved
5. Clear cart
6. Load saved transaction
7. Complete payment

**Expected Result:** ✅ Transaction saves and loads correctly

### Scenario 5: Cancel Transaction
1. Login as cashier
2. Add products to cart
3. Click cancel button (red X)
4. Confirm cancellation
5. Verify cart clears

**Expected Result:** ✅ Cart clears, no data saved

### Scenario 6: Dashboard Analytics
1. Login as cashier
2. View dashboard
3. Verify metrics display
4. Check line chart renders
5. Check donut chart renders
6. Verify latest invoices list
7. Wait 5 seconds
8. Verify auto-refresh

**Expected Result:** ✅ All dashboard elements display and update

## Performance Benchmarks

### Expected Performance
- Dashboard load: < 2 seconds
- POS load: < 3 seconds
- Product search: < 1 second
- Add to cart: < 0.5 seconds
- Payment processing: < 2 seconds
- Invoice creation: < 1 second
- Chart rendering: < 2 seconds

### Monitor These Metrics
- Database query time
- UI rendering time
- Memory usage
- CPU usage
- Network latency (if applicable)

## Security Checklist

- [ ] Role-based access working
- [ ] Cashier can only access cashier features
- [ ] Admin interface unchanged
- [ ] User authentication required
- [ ] Transaction logging enabled
- [ ] Audit trail functional
- [ ] No sensitive data in logs
- [ ] Connection strings secured

## User Training

### Training Materials Needed
- [ ] Quick Start Guide (created ✅)
- [ ] Video walkthrough (pending)
- [ ] FAQ document (pending)
- [ ] Troubleshooting guide (in docs)

### Training Topics
1. New dashboard overview
2. POS workflow changes
3. Client selection
4. Payment processing
5. Receipt options
6. Save/cancel transactions
7. Reading analytics

### Training Schedule
- Day 1: Cashiers (2 hours)
- Day 2: Managers (1 hour)
- Day 3: Q&A session (1 hour)

## Support Plan

### First Week Support
- On-site support available
- Hotline for critical issues
- Daily check-ins with users
- Quick response to feedback

### Ongoing Support
- Weekly review meetings
- Monthly feature updates
- Quarterly training refreshers
- Continuous improvement

## Success Criteria

### Must Have (Critical)
- ✅ Cashier can login
- ✅ Dashboard displays
- ✅ POS processes transactions
- ✅ Invoices create correctly
- ✅ Payments record properly
- ✅ Stock updates accurately

### Should Have (Important)
- ✅ Charts render correctly
- ✅ Auto-refresh works
- ✅ Receipt options available
- ✅ Client selection works
- ✅ Save transaction works

### Nice to Have (Enhancement)
- ⏳ PDF generation (future)
- ⏳ Email delivery (future)
- ⏳ SMS delivery (future)
- ⏳ Payment gateway integration (future)
- ⏳ Online store sync (future)

## Documentation Checklist

- [x] Database schema documented
- [x] UI implementation documented
- [x] Migration guide created
- [x] UI components reference created
- [x] Implementation summary created
- [x] Quick start guide created
- [x] Deployment checklist created
- [ ] Video tutorial (pending)
- [ ] API documentation (if needed)
- [ ] User manual (pending)

## Sign-Off

### Development Team
- [ ] Lead Developer: _________________ Date: _______
- [ ] QA Engineer: _________________ Date: _______
- [ ] Database Admin: _________________ Date: _______

### Business Team
- [ ] Product Owner: _________________ Date: _______
- [ ] Project Manager: _________________ Date: _______
- [ ] Business Analyst: _________________ Date: _______

### Operations Team
- [ ] System Admin: _________________ Date: _______
- [ ] Support Lead: _________________ Date: _______
- [ ] Training Lead: _________________ Date: _______

## Emergency Contacts

### Technical Issues
- Developer: [Contact Info]
- Database Admin: [Contact Info]
- System Admin: [Contact Info]

### Business Issues
- Product Owner: [Contact Info]
- Project Manager: [Contact Info]
- Support Lead: [Contact Info]

---

**Deployment Date:** ______________  
**Deployment Time:** ______________  
**Deployed By:** ______________  
**Status:** ⏳ Pending / ✅ Complete / ❌ Failed

---

**Version:** 1.0  
**Last Updated:** October 26, 2025  
**Next Review:** [Schedule next review date]
