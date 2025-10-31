using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Linq;
using SyncVerseStudio.Models;
using SyncVerseStudio.Services;

namespace SyncVerseStudio.Views.CashierDashboard
{
    /// <summary>
    /// Professional receipt/invoice printer for thermal and A4 paper
    /// Supports dual currency (USD/KHR) display
    /// </summary>
    public class ReceiptPrintView : Form
    {
        private PrintDocument printDocument;
        private PrintPreviewDialog printPreview;
        private Sale saleData;
        private bool isThermalPrint = true; // true for 80mm thermal, false for A4

        // Company Information
        private const string BRAND_NAME = "SYNCVERSE";
        private const string COMPANY_NAME = "SYNCVERSE STUDIO POS SYSTEM";
        private const string COMPANY_ADDRESS = "123 Main Street, Sangkat Boeung Keng Kang 1, Khan Chamkarmon";
        private const string COMPANY_CITY = "Phnom Penh, Cambodia 12302";
        private const string COMPANY_PHONE = "+855 12 345 678";
        private const string COMPANY_EMAIL = "******@rpi***r.edu.kh";
        private const string COMPANY_WEBSITE = "eirsvi.github.io";
        private const string TAX_ID = "K001-123456789";
        private const string SHIPPING_COST = "0.00"; // Free shipping default

        public ReceiptPrintView(Sale sale, bool thermalPrint = true)
        {
            saleData = sale;
            isThermalPrint = thermalPrint;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Receipt Preview";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            // Set paper size based on print type
            if (isThermalPrint)
            {
                // 80mm thermal paper (3.15 inches)
                printDocument.DefaultPageSettings.PaperSize = new PaperSize("Thermal", 315, 1200);
            }
            else
            {
                // A4 paper
                printDocument.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
            }

            printPreview = new PrintPreviewDialog
            {
                Document = printDocument,
                Width = 800,
                Height = 600,
                StartPosition = FormStartPosition.CenterParent
            };

            ShowPrintPreview();
        }

        private void ShowPrintPreview()
        {
            printPreview.ShowDialog();
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (isThermalPrint)
            {
                DrawThermalReceipt(e.Graphics, e.MarginBounds);
            }
            else
            {
                DrawA4Invoice(e.Graphics, e.MarginBounds);
            }
        }

        private void DrawThermalReceipt(Graphics g, Rectangle bounds)
        {
            int yPos = 8;
            int leftMargin = 8;
            int receiptWidth = 279; // 80mm = 295 pixels, minus margins

            // Professional Fonts
            var brandFont = new Font("Arial", 16, FontStyle.Bold);
            var titleFont = new Font("Arial", 12, FontStyle.Bold);
            var headerFont = new Font("Arial", 9, FontStyle.Bold);
            var normalFont = new Font("Arial", 8, FontStyle.Regular);
            var smallFont = new Font("Arial", 7, FontStyle.Regular);
            var tinyFont = new Font("Arial", 6, FontStyle.Italic);

            // ===== PROFESSIONAL HEADER =====
            // Brand Name (Centered, Large)
            using (var brandBrush = new SolidBrush(Color.Black))
            {
                DrawCenteredText(g, BRAND_NAME, brandFont, leftMargin, receiptWidth, ref yPos);
            }
            yPos += 2;
            
            // Company Name
            DrawCenteredText(g, COMPANY_NAME, normalFont, leftMargin, receiptWidth, ref yPos);
            yPos += 3;
            
            // Address (Multi-line)
            DrawCenteredText(g, COMPANY_ADDRESS, smallFont, leftMargin, receiptWidth, ref yPos);
            DrawCenteredText(g, COMPANY_CITY, smallFont, leftMargin, receiptWidth, ref yPos);
            yPos += 2;
            
            // Contact Info (Compact)
            DrawCenteredText(g, $"Tel: {COMPANY_PHONE}", smallFont, leftMargin, receiptWidth, ref yPos);
            DrawCenteredText(g, COMPANY_EMAIL, smallFont, leftMargin, receiptWidth, ref yPos);
            DrawCenteredText(g, COMPANY_WEBSITE, smallFont, leftMargin, receiptWidth, ref yPos);
            DrawCenteredText(g, $"Tax ID: {TAX_ID}", tinyFont, leftMargin, receiptWidth, ref yPos);
            
            yPos += 8;
            DrawLine(g, leftMargin, receiptWidth, ref yPos, 2);
            yPos += 5;

            // ===== INVOICE TITLE =====
            DrawCenteredText(g, "INVOICE", titleFont, leftMargin, receiptWidth, ref yPos);
            yPos += 8;
            DrawLine(g, leftMargin, receiptWidth, ref yPos, 1);
            yPos += 5;

            // ===== TRANSACTION DETAILS =====
            DrawText(g, $"Invoice #: {saleData.InvoiceNumber}", headerFont, leftMargin, ref yPos);
            DrawText(g, $"Date: {saleData.SaleDate:MMM dd, yyyy}", normalFont, leftMargin, ref yPos);
            DrawText(g, $"Time: {saleData.SaleDate:hh:mm:ss tt}", normalFont, leftMargin, ref yPos);
            DrawText(g, $"Cashier: {saleData.Cashier?.Username ?? "N/A"}", normalFont, leftMargin, ref yPos);
            DrawText(g, $"Payment: {saleData.PaymentMethod}", normalFont, leftMargin, ref yPos);
            yPos += 3;
            
            // ===== CUSTOMER INFO =====
            DrawText(g, "BILL TO:", headerFont, leftMargin, ref yPos);
            DrawText(g, saleData.Customer?.FullName ?? "Walk-in Customer", normalFont, leftMargin, ref yPos);
            if (saleData.Customer != null && !string.IsNullOrEmpty(saleData.Customer.Phone))
            {
                DrawText(g, saleData.Customer.Phone, smallFont, leftMargin, ref yPos);
            }
            
            yPos += 8;
            DrawLine(g, leftMargin, receiptWidth, ref yPos, 1);
            yPos += 3;

            // ===== ITEMS TABLE HEADER =====
            DrawText(g, "ITEM", headerFont, leftMargin, ref yPos, false);
            DrawRightText(g, "QTY", headerFont, leftMargin + 160, ref yPos, false);
            DrawRightText(g, "PRICE", headerFont, leftMargin + 210, ref yPos, false);
            DrawRightText(g, "TOTAL", headerFont, leftMargin + 270, ref yPos);
            
            DrawLine(g, leftMargin, receiptWidth, ref yPos, 1);
            yPos += 2;

            // ===== ITEMS LIST =====
            foreach (var item in saleData.SaleItems)
            {
                // Product name (smart truncation)
                var productName = item.Product.Name;
                if (productName.Length > 18)
                {
                    productName = productName.Substring(0, 15) + "...";
                }
                
                DrawText(g, productName, normalFont, leftMargin, ref yPos, false);
                DrawRightText(g, item.Quantity.ToString(), normalFont, leftMargin + 160, ref yPos, false);
                DrawRightText(g, CurrencyService.FormatAuto(item.UnitPrice), normalFont, leftMargin + 210, ref yPos, false);
                DrawRightText(g, CurrencyService.FormatAuto(item.TotalPrice), normalFont, leftMargin + 270, ref yPos);
                yPos += 1;
            }

            yPos += 5;
            DrawLine(g, leftMargin, receiptWidth, ref yPos, 1);
            yPos += 3;

            // ===== SUMMARY SECTION =====
            var subtotal = saleData.SaleItems.Sum(i => i.TotalPrice);
            var shippingCost = 0m; // Free shipping
            
            DrawRightAlignedRow(g, "Subtotal:", CurrencyService.FormatAuto(subtotal), normalFont, leftMargin, receiptWidth, ref yPos);
            DrawRightAlignedRow(g, "Shipping:", CurrencyService.FormatAuto(shippingCost), normalFont, leftMargin, receiptWidth, ref yPos);
            
            if (saleData.DiscountAmount > 0)
            {
                DrawRightAlignedRow(g, "Discount:", $"-{CurrencyService.FormatAuto(saleData.DiscountAmount)}", normalFont, leftMargin, receiptWidth, ref yPos);
            }
            
            if (saleData.TaxAmount > 0)
            {
                var taxPercent = subtotal > 0 ? (saleData.TaxAmount / subtotal * 100) : 0;
                DrawRightAlignedRow(g, $"Tax ({taxPercent:F1}%):", CurrencyService.FormatAuto(saleData.TaxAmount), normalFont, leftMargin, receiptWidth, ref yPos);
            }

            yPos += 5;
            DrawLine(g, leftMargin, receiptWidth, ref yPos, 2);
            yPos += 3;
            
            // ===== GRAND TOTAL (Highlighted) =====
            DrawRightAlignedRow(g, "GRAND TOTAL:", CurrencyService.FormatAuto(saleData.TotalAmount), titleFont, leftMargin, receiptWidth, ref yPos);
            
            // Dual Currency Display
            yPos += 2;
            DrawCenteredText(g, $"({CurrencyService.FormatDual(saleData.TotalAmount)})", smallFont, leftMargin, receiptWidth, ref yPos);
            
            yPos += 8;
            DrawLine(g, leftMargin, receiptWidth, ref yPos, 2);
            yPos += 5;

            // ===== NOTES SECTION =====
            DrawCenteredText(g, "NOTES:", headerFont, leftMargin, receiptWidth, ref yPos);
            yPos += 2;
            DrawCenteredText(g, "Thank you for your purchase.", smallFont, leftMargin, receiptWidth, ref yPos);
            DrawCenteredText(g, "All sales are final unless stated.", smallFont, leftMargin, receiptWidth, ref yPos);
            DrawCenteredText(g, "For returns, contact us within 7 days.", tinyFont, leftMargin, receiptWidth, ref yPos);
            
            yPos += 8;

            // ===== PROFESSIONAL FOOTER =====
            DrawCenteredText(g, "Thank you for your business!", headerFont, leftMargin, receiptWidth, ref yPos);
            yPos += 3;
            DrawCenteredText(g, "Please come again!", normalFont, leftMargin, receiptWidth, ref yPos);
            yPos += 5;
            
            DrawLine(g, leftMargin, receiptWidth, ref yPos, 1);
            yPos += 3;
            
            DrawCenteredText(g, "For questions, contact us above.", tinyFont, leftMargin, receiptWidth, ref yPos);
            DrawCenteredText(g, $"Powered by {BRAND_NAME}", tinyFont, leftMargin, receiptWidth, ref yPos);
        }

        private void DrawA4Invoice(Graphics g, Rectangle bounds)
        {
            int yPos = 30;
            int leftMargin = 50;
            int rightMargin = bounds.Width - 50;
            int contentWidth = rightMargin - leftMargin;

            // Professional Fonts
            var brandFont = new Font("Arial", 24, FontStyle.Bold);
            var titleFont = new Font("Arial", 16, FontStyle.Bold);
            var sectionFont = new Font("Arial", 11, FontStyle.Bold);
            var headerFont = new Font("Arial", 10, FontStyle.Bold);
            var normalFont = new Font("Arial", 9, FontStyle.Regular);
            var smallFont = new Font("Arial", 8, FontStyle.Regular);
            var tinyFont = new Font("Arial", 7, FontStyle.Regular);

            // ===== PROFESSIONAL HEADER SECTION =====
            // Left Side: Company Information
            int headerStartY = yPos;
            
            // Brand Name
            using (var brandBrush = new SolidBrush(Color.FromArgb(30, 58, 138)))
            {
                g.DrawString(BRAND_NAME, brandFont, brandBrush, leftMargin, yPos);
            }
            yPos += 32;
            
            // Company Details
            DrawText(g, COMPANY_NAME, normalFont, leftMargin, ref yPos);
            DrawText(g, COMPANY_ADDRESS, smallFont, leftMargin, ref yPos);
            DrawText(g, COMPANY_CITY, smallFont, leftMargin, ref yPos);
            yPos += 5;
            DrawText(g, $"Phone: {COMPANY_PHONE}", smallFont, leftMargin, ref yPos);
            DrawText(g, $"Email: {COMPANY_EMAIL}", smallFont, leftMargin, ref yPos);
            DrawText(g, $"Website: {COMPANY_WEBSITE}", smallFont, leftMargin, ref yPos);
            DrawText(g, $"Tax ID: {TAX_ID}", tinyFont, leftMargin, ref yPos);

            // Right Side: INVOICE Title and Number
            int invoiceBoxX = leftMargin + contentWidth - 250;
            int invoiceBoxY = headerStartY;
            int invoiceBoxWidth = 250;
            int invoiceBoxHeight = 90;
            
            // Invoice Box Background
            using (var invoiceBg = new SolidBrush(Color.FromArgb(239, 246, 255)))
            {
                g.FillRectangle(invoiceBg, invoiceBoxX, invoiceBoxY, invoiceBoxWidth, invoiceBoxHeight);
            }
            
            // Invoice Box Border
            using (var borderPen = new Pen(Color.FromArgb(30, 58, 138), 2))
            {
                g.DrawRectangle(borderPen, invoiceBoxX, invoiceBoxY, invoiceBoxWidth, invoiceBoxHeight);
            }
            
            // INVOICE Title
            int invoiceTextY = invoiceBoxY + 15;
            using (var invoiceTitleBrush = new SolidBrush(Color.FromArgb(30, 58, 138)))
            {
                var invoiceTitle = "INVOICE";
                var titleSize = g.MeasureString(invoiceTitle, titleFont);
                g.DrawString(invoiceTitle, titleFont, invoiceTitleBrush, 
                    invoiceBoxX + (invoiceBoxWidth - titleSize.Width) / 2, invoiceTextY);
            }
            
            invoiceTextY += 35;
            
            // Invoice Number
            var invNumLabel = "Invoice #:";
            var invNumSize = g.MeasureString(invNumLabel, smallFont);
            g.DrawString(invNumLabel, smallFont, Brushes.Black, invoiceBoxX + 15, invoiceTextY);
            g.DrawString(saleData.InvoiceNumber, headerFont, Brushes.Black, 
                invoiceBoxX + 15 + invNumSize.Width + 5, invoiceTextY - 2);

            yPos = Math.Max(yPos, headerStartY + invoiceBoxHeight + 20);
            
            // Horizontal Separator
            using (var pen = new Pen(Color.FromArgb(203, 213, 225), 2))
            {
                g.DrawLine(pen, leftMargin, yPos, rightMargin, yPos);
            }
            yPos += 20;

            // ===== TRANSACTION DETAILS SECTION =====
            int detailsBoxY = yPos;
            int detailsBoxHeight = 85;
            
            // Background for details section
            using (var detailsBg = new SolidBrush(Color.FromArgb(249, 250, 251)))
            {
                g.FillRectangle(detailsBg, leftMargin, detailsBoxY, contentWidth / 2 - 10, detailsBoxHeight);
            }
            
            yPos += 12;
            int detailsX = leftMargin + 15;
            
            // Date & Time
            DrawText(g, "Date:", headerFont, detailsX, ref yPos, false);
            yPos -= 16;
            DrawText(g, saleData.SaleDate.ToString("MMMM dd, yyyy"), normalFont, detailsX + 120, ref yPos);
            
            DrawText(g, "Time:", headerFont, detailsX, ref yPos, false);
            yPos -= 16;
            DrawText(g, saleData.SaleDate.ToString("hh:mm:ss tt"), normalFont, detailsX + 120, ref yPos);
            
            // Payment Method
            DrawText(g, "Payment Method:", headerFont, detailsX, ref yPos, false);
            yPos -= 16;
            DrawText(g, saleData.PaymentMethod.ToString(), normalFont, detailsX + 120, ref yPos);
            
            // Cashier
            DrawText(g, "Cashier:", headerFont, detailsX, ref yPos, false);
            yPos -= 16;
            DrawText(g, saleData.Cashier?.Username ?? "N/A", normalFont, detailsX + 120, ref yPos);
            
            // Status
            DrawText(g, "Status:", headerFont, detailsX, ref yPos, false);
            yPos -= 16;
            using (var statusBrush = new SolidBrush(Color.FromArgb(34, 197, 94)))
            {
                g.DrawString(saleData.Status.ToString(), normalFont, statusBrush, detailsX + 120, yPos);
            }
            yPos += 16;

            yPos = detailsBoxY + detailsBoxHeight + 20;

            // ===== CUSTOMER INFORMATION =====
            DrawText(g, "BILL TO:", sectionFont, leftMargin, ref yPos);
            yPos += 5;
            
            // Customer Box
            using (var customerBorder = new Pen(Color.FromArgb(203, 213, 225), 1))
            {
                g.DrawRectangle(customerBorder, leftMargin, yPos, contentWidth / 2 - 10, 65);
            }
            
            yPos += 12;
            int customerX = leftMargin + 15;
            
            DrawText(g, saleData.Customer?.FullName ?? "Walk-in Customer", headerFont, customerX, ref yPos);
            
            if (saleData.Customer != null)
            {
                if (!string.IsNullOrEmpty(saleData.Customer.Phone))
                {
                    DrawText(g, $"Phone: {saleData.Customer.Phone}", normalFont, customerX, ref yPos);
                }
                if (!string.IsNullOrEmpty(saleData.Customer.Email))
                {
                    DrawText(g, $"Email: {saleData.Customer.Email}", normalFont, customerX, ref yPos);
                }
            }
            
            yPos += 25;

            // ===== ITEMS TABLE =====
            DrawText(g, "ITEMS:", sectionFont, leftMargin, ref yPos);
            yPos += 8;
            
            // Table Border
            int tableStartY = yPos;
            using (var tableBorder = new Pen(Color.FromArgb(203, 213, 225), 1))
            {
                g.DrawRectangle(tableBorder, leftMargin, yPos, contentWidth, 0); // Will adjust height later
            }
            
            // Table Header
            using (var headerBg = new SolidBrush(Color.FromArgb(241, 245, 249)))
            {
                g.FillRectangle(headerBg, leftMargin + 1, yPos + 1, contentWidth - 2, 32);
            }

            int itemNoX = leftMargin + 15;
            int itemDescX = leftMargin + 50;
            int qtyX = leftMargin + contentWidth - 320;
            int priceX = leftMargin + contentWidth - 220;
            int totalX = leftMargin + contentWidth - 100;

            yPos += 10;
            g.DrawString("#", headerFont, Brushes.Black, itemNoX, yPos);
            g.DrawString("DESCRIPTION", headerFont, Brushes.Black, itemDescX, yPos);
            g.DrawString("QTY", headerFont, Brushes.Black, qtyX, yPos);
            g.DrawString("UNIT PRICE", headerFont, Brushes.Black, priceX, yPos);
            g.DrawString("AMOUNT", headerFont, Brushes.Black, totalX, yPos);
            yPos += 25;

            // Horizontal line after header
            using (var linePen = new Pen(Color.FromArgb(203, 213, 225), 1))
            {
                g.DrawLine(linePen, leftMargin, yPos, rightMargin, yPos);
            }
            yPos += 12;

            // Table Rows
            int rowNumber = 1;
            foreach (var item in saleData.SaleItems)
            {
                g.DrawString(rowNumber.ToString(), normalFont, Brushes.Black, itemNoX, yPos);
                g.DrawString(item.Product.Name, normalFont, Brushes.Black, itemDescX, yPos);
                g.DrawString(item.Quantity.ToString(), normalFont, Brushes.Black, qtyX, yPos);
                g.DrawString(CurrencyService.FormatAuto(item.UnitPrice), normalFont, Brushes.Black, priceX, yPos);
                g.DrawString(CurrencyService.FormatAuto(item.TotalPrice), normalFont, Brushes.Black, totalX, yPos);
                
                yPos += 28;
                rowNumber++;
            }

            yPos += 8;
            
            // Bottom border of table
            using (var tableBorder = new Pen(Color.FromArgb(203, 213, 225), 1))
            {
                g.DrawRectangle(tableBorder, leftMargin, tableStartY, contentWidth, yPos - tableStartY);
            }
            
            yPos += 15;

            // ===== SUMMARY SECTION =====
            int summaryBoxX = leftMargin + contentWidth - 300;
            int summaryBoxWidth = 300;
            int summaryLabelX = summaryBoxX + 15;
            int summaryValueX = summaryBoxX + summaryBoxWidth - 15;

            var subtotal = saleData.SaleItems.Sum(i => i.TotalPrice);
            var shippingCost = 0m; // Free shipping

            // Summary Box Background
            using (var summaryBg = new SolidBrush(Color.FromArgb(249, 250, 251)))
            {
                g.FillRectangle(summaryBg, summaryBoxX, yPos, summaryBoxWidth, 110);
            }
            
            yPos += 12;

            // Subtotal
            DrawText(g, "Subtotal:", normalFont, summaryLabelX, ref yPos, false);
            DrawRightText(g, CurrencyService.FormatAuto(subtotal), normalFont, summaryValueX, ref yPos);

            // Shipping
            DrawText(g, "Shipping:", normalFont, summaryLabelX, ref yPos, false);
            DrawRightText(g, "FREE", normalFont, summaryValueX, ref yPos);

            // Discount
            if (saleData.DiscountAmount > 0)
            {
                DrawText(g, "Discount:", normalFont, summaryLabelX, ref yPos, false);
                using (var discountBrush = new SolidBrush(Color.FromArgb(220, 38, 38)))
                {
                    var discountText = $"-{CurrencyService.FormatAuto(saleData.DiscountAmount)}";
                    var discountSize = g.MeasureString(discountText, normalFont);
                    g.DrawString(discountText, normalFont, discountBrush, summaryValueX - discountSize.Width, yPos);
                }
                yPos += (int)g.MeasureString("X", normalFont).Height + 2;
            }

            // Tax
            if (saleData.TaxAmount > 0)
            {
                var taxPercent = subtotal > 0 ? (saleData.TaxAmount / subtotal * 100) : 0;
                DrawText(g, $"Tax ({taxPercent:F1}%):", normalFont, summaryLabelX, ref yPos, false);
                DrawRightText(g, CurrencyService.FormatAuto(saleData.TaxAmount), normalFont, summaryValueX, ref yPos);
            }

            yPos += 8;
            
            // Separator line
            using (var pen = new Pen(Color.FromArgb(203, 213, 225), 1))
            {
                g.DrawLine(pen, summaryLabelX, yPos, summaryValueX, yPos);
            }
            yPos += 12;

            // Grand Total
            var grandTotalFont = new Font("Arial", 13, FontStyle.Bold);
            DrawText(g, "GRAND TOTAL:", grandTotalFont, summaryLabelX, ref yPos, false);
            using (var totalBrush = new SolidBrush(Color.FromArgb(30, 58, 138)))
            {
                var totalText = CurrencyService.FormatAuto(saleData.TotalAmount);
                var totalSize = g.MeasureString(totalText, grandTotalFont);
                g.DrawString(totalText, grandTotalFont, totalBrush, summaryValueX - totalSize.Width, yPos);
            }
            yPos += (int)g.MeasureString("X", grandTotalFont).Height + 2;
            
            // Dual Currency
            yPos += 5;
            DrawRightText(g, $"({CurrencyService.FormatDual(saleData.TotalAmount)})", smallFont, summaryValueX, ref yPos);

            yPos += 20;

            // ===== PAYMENT TERMS & NOTES =====
            DrawText(g, "PAYMENT TERMS & NOTES:", sectionFont, leftMargin, ref yPos);
            yPos += 5;
            
            using (var notesBorder = new Pen(Color.FromArgb(203, 213, 225), 1))
            {
                g.DrawRectangle(notesBorder, leftMargin, yPos, contentWidth, 60);
            }
            
            yPos += 12;
            int notesX = leftMargin + 15;
            
            DrawText(g, "Payment is due upon receipt. Thank you for your business.", normalFont, notesX, ref yPos);
            DrawText(g, "All sales are final unless otherwise stated in writing.", smallFont, notesX, ref yPos);
            DrawText(g, "For returns or exchanges, please contact us within 7 days with this invoice.", smallFont, notesX, ref yPos);
            
            yPos += 25;

            // ===== FOOTER =====
            yPos = bounds.Height - 70;
            
            using (var footerLine = new Pen(Color.FromArgb(203, 213, 225), 1))
            {
                g.DrawLine(footerLine, leftMargin, yPos, rightMargin, yPos);
            }
            yPos += 15;

            // Thank you message
            using (var footerBrush = new SolidBrush(Color.FromArgb(71, 85, 105)))
            {
                var thankYouText = "Thank you for your business!";
                var thankYouSize = g.MeasureString(thankYouText, headerFont);
                g.DrawString(thankYouText, headerFont, footerBrush, 
                    leftMargin + (contentWidth - thankYouSize.Width) / 2, yPos);
            }
            yPos += 22;

            // Footer info
            DrawCenteredText(g, "For questions or concerns, please contact us using the information above.", tinyFont, leftMargin, contentWidth, ref yPos);
            DrawCenteredText(g, $"This is a computer-generated invoice. No signature required.", tinyFont, leftMargin, contentWidth, ref yPos);
        }

        // Helper Methods
        private void DrawText(Graphics g, string text, Font font, int x, ref int y, bool incrementY = true)
        {
            g.DrawString(text, font, Brushes.Black, x, y);
            if (incrementY)
                y += (int)g.MeasureString(text, font).Height + 2;
        }

        private void DrawRightText(Graphics g, string text, Font font, int x, ref int y, bool incrementY = true)
        {
            var size = g.MeasureString(text, font);
            g.DrawString(text, font, Brushes.Black, x - size.Width, y);
            if (incrementY)
                y += (int)size.Height + 2;
        }

        private void DrawCenteredText(Graphics g, string text, Font font, int leftMargin, int width, ref int y)
        {
            var size = g.MeasureString(text, font);
            var x = leftMargin + (width - size.Width) / 2;
            g.DrawString(text, font, Brushes.Black, x, y);
            y += (int)size.Height + 2;
        }

        private void DrawLine(Graphics g, int x, int width, ref int y, int thickness = 1)
        {
            using (var pen = new Pen(Color.Black, thickness))
            {
                g.DrawLine(pen, x, y, x + width, y);
            }
            y += thickness + 5;
        }

        private void DrawRightAlignedRow(Graphics g, string label, string value, Font font, int leftMargin, int width, ref int y)
        {
            g.DrawString(label, font, Brushes.Black, leftMargin, y);
            var valueSize = g.MeasureString(value, font);
            g.DrawString(value, font, Brushes.Black, leftMargin + width - valueSize.Width, y);
            y += (int)valueSize.Height + 2;
        }

        public void Print()
        {
            printDocument.Print();
        }
    }
}
