using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;

namespace SyncVerseStudio.Services
{
    public class AccountingService
    {
        private readonly ApplicationDbContext _context;

        public AccountingService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create ledger entries for a sale
        public async Task CreateSaleLedgerEntries(Sale sale, int userId)
        {
            var entryNumber = await GenerateEntryNumber();

            // Debit: Cash/Bank (Asset increases)
            var debitEntry = new GeneralLedgerEntry
            {
                EntryNumber = entryNumber,
                EntryDate = sale.SaleDate,
                AccountName = "Cash",
                AccountType = AccountType.Asset,
                DebitAmount = sale.TotalAmount,
                CreditAmount = 0,
                Description = $"Sale - {sale.InvoiceNumber}",
                ReferenceNumber = sale.InvoiceNumber,
                BookOfEntry = BookOfEntry.SalesDayBook,
                RelatedSaleId = sale.Id,
                CreatedByUserId = userId
            };

            // Credit: Sales Revenue (Revenue increases)
            var creditEntry = new GeneralLedgerEntry
            {
                EntryNumber = entryNumber,
                EntryDate = sale.SaleDate,
                AccountName = "Sales Revenue",
                AccountType = AccountType.Revenue,
                DebitAmount = 0,
                CreditAmount = sale.TotalAmount,
                Description = $"Sale - {sale.InvoiceNumber}",
                ReferenceNumber = sale.InvoiceNumber,
                BookOfEntry = BookOfEntry.SalesDayBook,
                RelatedSaleId = sale.Id,
                CreatedByUserId = userId
            };

            _context.GeneralLedgerEntries.AddRange(debitEntry, creditEntry);

            // Record COGS (Cost of Goods Sold)
            var cogs = await CalculateCOGS(sale);
            if (cogs > 0)
            {
                var cogsDebitEntry = new GeneralLedgerEntry
                {
                    EntryNumber = await GenerateEntryNumber(),
                    EntryDate = sale.SaleDate,
                    AccountName = "Cost of Goods Sold",
                    AccountType = AccountType.Expense,
                    DebitAmount = cogs,
                    CreditAmount = 0,
                    Description = $"COGS - {sale.InvoiceNumber}",
                    ReferenceNumber = sale.InvoiceNumber,
                    BookOfEntry = BookOfEntry.SalesDayBook,
                    RelatedSaleId = sale.Id,
                    CreatedByUserId = userId
                };

                var cogsCreditEntry = new GeneralLedgerEntry
                {
                    EntryNumber = cogsDebitEntry.EntryNumber,
                    EntryDate = sale.SaleDate,
                    AccountName = "Inventory",
                    AccountType = AccountType.Asset,
                    DebitAmount = 0,
                    CreditAmount = cogs,
                    Description = $"COGS - {sale.InvoiceNumber}",
                    ReferenceNumber = sale.InvoiceNumber,
                    BookOfEntry = BookOfEntry.SalesDayBook,
                    RelatedSaleId = sale.Id,
                    CreatedByUserId = userId
                };

                _context.GeneralLedgerEntries.AddRange(cogsDebitEntry, cogsCreditEntry);
            }

            await _context.SaveChangesAsync();
        }

        // Create ledger entries for a purchase
        public async Task CreatePurchaseLedgerEntries(Purchase purchase, int userId)
        {
            var entryNumber = await GenerateEntryNumber();

            // Debit: Inventory (Asset increases)
            var debitEntry = new GeneralLedgerEntry
            {
                EntryNumber = entryNumber,
                EntryDate = purchase.PurchaseDate,
                AccountName = "Inventory",
                AccountType = AccountType.Asset,
                DebitAmount = purchase.TotalAmount,
                CreditAmount = 0,
                Description = $"Purchase - {purchase.PurchaseNumber}",
                ReferenceNumber = purchase.PurchaseNumber,
                BookOfEntry = BookOfEntry.PurchasesDayBook,
                RelatedPurchaseId = purchase.Id,
                CreatedByUserId = userId
            };

            // Credit: Accounts Payable or Cash (Liability increases or Asset decreases)
            var creditAccountName = purchase.PaidAmount >= purchase.TotalAmount ? "Cash" : "Accounts Payable";
            var creditAccountType = purchase.PaidAmount >= purchase.TotalAmount ? AccountType.Asset : AccountType.Liability;

            var creditEntry = new GeneralLedgerEntry
            {
                EntryNumber = entryNumber,
                EntryDate = purchase.PurchaseDate,
                AccountName = creditAccountName,
                AccountType = creditAccountType,
                DebitAmount = 0,
                CreditAmount = purchase.TotalAmount,
                Description = $"Purchase - {purchase.PurchaseNumber}",
                ReferenceNumber = purchase.PurchaseNumber,
                BookOfEntry = BookOfEntry.PurchasesDayBook,
                RelatedPurchaseId = purchase.Id,
                CreatedByUserId = userId
            };

            _context.GeneralLedgerEntries.AddRange(debitEntry, creditEntry);
            await _context.SaveChangesAsync();
        }

        // Create ledger entries for a payment
        public async Task CreatePaymentLedgerEntries(Payment payment, int userId)
        {
            var entryNumber = await GenerateEntryNumber();
            var isCashIn = payment.InvoiceId.HasValue || payment.SaleId.HasValue;

            if (isCashIn)
            {
                // Cash received
                var debitEntry = new GeneralLedgerEntry
                {
                    EntryNumber = entryNumber,
                    EntryDate = payment.PaymentDate,
                    AccountName = "Cash",
                    AccountType = AccountType.Asset,
                    DebitAmount = payment.Amount,
                    CreditAmount = 0,
                    Description = $"Payment Received - {payment.PaymentReference}",
                    ReferenceNumber = payment.PaymentReference,
                    BookOfEntry = BookOfEntry.CashBook,
                    RelatedPaymentId = payment.Id,
                    CreatedByUserId = userId
                };

                var creditEntry = new GeneralLedgerEntry
                {
                    EntryNumber = entryNumber,
                    EntryDate = payment.PaymentDate,
                    AccountName = "Accounts Receivable",
                    AccountType = AccountType.Asset,
                    DebitAmount = 0,
                    CreditAmount = payment.Amount,
                    Description = $"Payment Received - {payment.PaymentReference}",
                    ReferenceNumber = payment.PaymentReference,
                    BookOfEntry = BookOfEntry.CashBook,
                    RelatedPaymentId = payment.Id,
                    CreatedByUserId = userId
                };

                _context.GeneralLedgerEntries.AddRange(debitEntry, creditEntry);
            }
            else
            {
                // Cash paid out
                var debitEntry = new GeneralLedgerEntry
                {
                    EntryNumber = entryNumber,
                    EntryDate = payment.PaymentDate,
                    AccountName = "Accounts Payable",
                    AccountType = AccountType.Liability,
                    DebitAmount = payment.Amount,
                    CreditAmount = 0,
                    Description = $"Payment Made - {payment.PaymentReference}",
                    ReferenceNumber = payment.PaymentReference,
                    BookOfEntry = BookOfEntry.CashBook,
                    RelatedPaymentId = payment.Id,
                    CreatedByUserId = userId
                };

                var creditEntry = new GeneralLedgerEntry
                {
                    EntryNumber = entryNumber,
                    EntryDate = payment.PaymentDate,
                    AccountName = "Cash",
                    AccountType = AccountType.Asset,
                    DebitAmount = 0,
                    CreditAmount = payment.Amount,
                    Description = $"Payment Made - {payment.PaymentReference}",
                    ReferenceNumber = payment.PaymentReference,
                    BookOfEntry = BookOfEntry.CashBook,
                    RelatedPaymentId = payment.Id,
                    CreatedByUserId = userId
                };

                _context.GeneralLedgerEntries.AddRange(debitEntry, creditEntry);
            }

            await _context.SaveChangesAsync();
        }

        // Create manual journal entry
        public async Task CreateJournalEntry(string accountName, AccountType accountType, 
            decimal debitAmount, decimal creditAmount, string description, int userId)
        {
            var entryNumber = await GenerateEntryNumber();

            var entry = new GeneralLedgerEntry
            {
                EntryNumber = entryNumber,
                EntryDate = DateTime.Now,
                AccountName = accountName,
                AccountType = accountType,
                DebitAmount = debitAmount,
                CreditAmount = creditAmount,
                Description = description,
                BookOfEntry = BookOfEntry.GeneralJournal,
                CreatedByUserId = userId
            };

            _context.GeneralLedgerEntries.Add(entry);
            await _context.SaveChangesAsync();
        }

        // Initialize default financial accounts
        public async Task InitializeFinancialAccounts()
        {
            if (await _context.FinancialAccounts.AnyAsync())
                return;

            var accounts = new[]
            {
                // Assets
                new FinancialAccount { AccountCode = "1000", AccountName = "Cash", AccountType = AccountType.Asset, Category = FinancialStatementCategory.CurrentAssets },
                new FinancialAccount { AccountCode = "1100", AccountName = "Accounts Receivable", AccountType = AccountType.Asset, Category = FinancialStatementCategory.CurrentAssets },
                new FinancialAccount { AccountCode = "1200", AccountName = "Inventory", AccountType = AccountType.Asset, Category = FinancialStatementCategory.CurrentAssets },
                new FinancialAccount { AccountCode = "1500", AccountName = "Equipment", AccountType = AccountType.Asset, Category = FinancialStatementCategory.FixedAssets },
                
                // Liabilities
                new FinancialAccount { AccountCode = "2000", AccountName = "Accounts Payable", AccountType = AccountType.Liability, Category = FinancialStatementCategory.CurrentLiabilities },
                new FinancialAccount { AccountCode = "2100", AccountName = "Loans Payable", AccountType = AccountType.Liability, Category = FinancialStatementCategory.LongTermLiabilities },
                
                // Equity
                new FinancialAccount { AccountCode = "3000", AccountName = "Owner's Capital", AccountType = AccountType.Equity, Category = FinancialStatementCategory.OwnersEquity },
                new FinancialAccount { AccountCode = "3100", AccountName = "Retained Earnings", AccountType = AccountType.Equity, Category = FinancialStatementCategory.RetainedEarnings },
                
                // Revenue
                new FinancialAccount { AccountCode = "4000", AccountName = "Sales Revenue", AccountType = AccountType.Revenue, Category = FinancialStatementCategory.SalesRevenue },
                new FinancialAccount { AccountCode = "4100", AccountName = "Other Income", AccountType = AccountType.Revenue, Category = FinancialStatementCategory.OtherIncome },
                
                // Expenses
                new FinancialAccount { AccountCode = "5000", AccountName = "Cost of Goods Sold", AccountType = AccountType.Expense, Category = FinancialStatementCategory.CostOfGoodsSold },
                new FinancialAccount { AccountCode = "6000", AccountName = "Operating Expenses", AccountType = AccountType.Expense, Category = FinancialStatementCategory.OperatingExpenses },
                new FinancialAccount { AccountCode = "6100", AccountName = "Utilities", AccountType = AccountType.Expense, Category = FinancialStatementCategory.OperatingExpenses },
                new FinancialAccount { AccountCode = "6200", AccountName = "Rent", AccountType = AccountType.Expense, Category = FinancialStatementCategory.OperatingExpenses }
            };

            _context.FinancialAccounts.AddRange(accounts);
            await _context.SaveChangesAsync();
        }

        private async Task<decimal> CalculateCOGS(Sale sale)
        {
            var saleItems = await _context.SaleItems
                .Include(si => si.Product)
                .Where(si => si.SaleId == sale.Id)
                .ToListAsync();

            return saleItems.Sum(si => si.Quantity * si.Product.CostPrice);
        }

        private async Task<string> GenerateEntryNumber()
        {
            var today = DateTime.Now;
            var prefix = $"GL{today:yyyyMMdd}";
            
            var lastEntry = await _context.GeneralLedgerEntries
                .Where(e => e.EntryNumber.StartsWith(prefix))
                .OrderByDescending(e => e.EntryNumber)
                .FirstOrDefaultAsync();

            if (lastEntry == null)
                return $"{prefix}-001";

            var lastNumber = int.Parse(lastEntry.EntryNumber.Substring(prefix.Length + 1));
            return $"{prefix}-{(lastNumber + 1):D3}";
        }
    }
}
