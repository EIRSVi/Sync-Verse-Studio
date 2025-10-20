# SyncVerse Studio - Point of Sale System

A comprehensive Point of Sale (POS) system built with .NET 8 and Windows Forms, featuring role-based authentication, real-time dashboard analytics, and complete inventory management.

## ?? Latest Updates

### ? **Version 2.0 Improvements**
- **Resizable Login Form**: Modern, professional login interface with maximize/minimize controls
- **Real-Time Dashboard**: Live data updates with proper async/await implementation
- **Enhanced UI/UX**: Improved Material Design implementation with better responsiveness
- **Thread-Safe Operations**: Fixed database threading issues for stable performance
- **Better Error Handling**: Comprehensive error management and user feedback

## ?? Key Features

### ?? **Advanced Authentication System**
- **Secure Login**: BCrypt password hashing with role-based access control
- **Professional UI**: Resizable login form with modern Material Design
- **Multi-Role Support**: Administrator, Cashier, and Inventory Clerk roles
- **Session Management**: Secure user sessions with audit logging

### ?? **Real-Time Dashboard Analytics**
- **Live Data Updates**: Real-time metrics updated asynchronously
- **Role-Specific Views**: Customized dashboards for each user role
- **Performance Metrics**: Sales, inventory, and user activity tracking
- **Recent Activities**: Live feed of system activities and changes

### ?? **Complete POS Functionality**
- **Product Management**: Full CRUD operations with categories and suppliers
- **Inventory Tracking**: Real-time stock management with low stock alerts
- **Sales Processing**: Transaction management with receipt generation
- **Customer Database**: Customer management with loyalty points system

### ?? **Modern UI/UX Design**
- **Material Design**: Clean, professional interface following Material Design principles
- **Responsive Layout**: Adaptive design that works on different screen sizes
- **FontAwesome Icons**: Professional iconography throughout the application
- **Color-Coded System**: Intuitive color scheme for easy navigation

## ??? **Database Schema**

The system includes comprehensive database entities:

```sql
Users              -- Authentication and role management
Products           -- Inventory items with full details
Categories         -- Product organization and grouping
Suppliers          -- Vendor and supplier management
Customers          -- Customer database with loyalty system
Sales & SaleItems  -- Complete transaction management
InventoryMovements -- Real-time stock tracking
AuditLogs          -- Security and compliance logging
```

## ?? **System Requirements**

### **Minimum Requirements**
- Windows 10/11 (64-bit)
- .NET 8 Runtime
- 4GB RAM
- 500MB available storage
- SQL Server (LocalDB, Express, or Full)
- 1920x1080 minimum resolution

### **Recommended Requirements**
- Windows 11 (64-bit)
- .NET 8 SDK
- 8GB RAM or more
- 2GB available storage
- SQL Server Express or Full
- 1920x1080 or higher resolution

## ??? **Quick Installation**

### **Method 1: Using the Batch File**
1. Download and extract the project
2. Double-click `RunApplication.bat`
3. Follow the on-screen instructions

### **Method 2: Manual Installation**
```bash
# Clone the repository
git clone https://github.com/eirsvi/syncversestudio.git
cd syncversestudio

# Restore packages and build
dotnet restore
dotnet build

# Run the application
dotnet run --project syncversestudio
```

### **Method 3: Visual Studio**
1. Open `syncversestudio.sln` in Visual Studio 2022
2. Set `syncversestudio` as the startup project
3. Press F5 to run

## ?? **Default Login Credentials**

**Administrator Account:**
- **Username**: `vi`
- **Password**: `vi`
- **Email**: `vi@syncverse.com`
- **Permissions**: Full system access

## ?? **Dashboard Features by Role**

### ?? **Administrator Dashboard**
- **Total Users**: Active user count across all roles
- **Total Products**: Complete inventory overview
- **Today's Sales**: Daily revenue tracking
- **Low Stock Items**: Inventory alerts and notifications
- **System Activities**: Complete audit trail access
- **Full CRUD Access**: All system management capabilities

### ?? **Cashier Dashboard**
- **Personal Sales**: Individual daily performance tracking
- **Transaction Count**: Number of processed transactions
- **Customers Served**: Daily customer interaction metrics
- **POS Access**: Complete point-of-sale functionality
- **Receipt Generation**: Automated receipt printing/export

### ?? **Inventory Clerk Dashboard**
- **Product Management**: Complete product lifecycle management
- **Stock Levels**: Real-time inventory tracking
- **Categories Overview**: Product organization metrics
- **Supplier Management**: Vendor relationship tracking
- **Stock Reports**: Detailed inventory analytics

## ?? **Security Features**

- **BCrypt Encryption**: Military-grade password hashing
- **Role-Based Access**: Granular permission system
- **Audit Logging**: Complete activity tracking
- **Session Security**: Secure user session management
- **SQL Injection Protection**: Parameterized queries and EF Core
- **Input Validation**: Comprehensive data validation

## ?? **Design System**

### **Color Palette**
- **Primary Black**: `#000000` - Main text and headers
- **Primary White**: `#FFFFFF` - Background and contrast
- **Accent Red**: `#FF0050` - Alerts and important actions
- **Accent Blue**: `#187712` - Success states and confirmations
- **Accent Green**: `#256366` - Information and secondary actions

### **Typography**
- **Primary Font**: Segoe UI (Windows standard)
- **Fallback Font**: Khmer OS Font (for Khmer language support)
- **Icon Library**: FontAwesome Sharp for professional iconography

### **Interface Elements**
- **Border Radius**: 4px for consistent rounded corners
- **Button Styles**: Flat design with hover effects
- **Form Controls**: Material Design inspired inputs
- **Grid System**: Responsive layout with proper spacing

## ?? **Technology Stack**

```xml
<!-- Core Framework -->
.NET 8 Windows Forms Application

<!-- UI & Design -->
FontAwesome.Sharp 6.3.0      <!-- Professional icons -->
System.Drawing.Common 8.0.0  <!-- Graphics and UI -->

<!-- Database & ORM -->
EntityFramework Core 8.0.0   <!-- Modern ORM -->
SQL Server                   <!-- Reliable database -->

<!-- Security & Utils -->
BCrypt.Net-Next 4.0.3       <!-- Password security -->
ZXing.Net 0.16.9           <!-- Barcode support -->
QuestPDF 2023.12.6         <!-- PDF generation -->
```

## ?? **Getting Started Guide**

### **First Time Setup**
1. **Launch Application**: Run using any of the installation methods
2. **Database Initialization**: The app will automatically create the database and sample data
3. **Login**: Use the default credentials (vi/vi)
4. **Explore Features**: Navigate through different modules based on your role
5. **Add Data**: Start adding your products, categories, and suppliers
6. **Process Sales**: Begin using the POS system for transactions

### **Daily Operations**
1. **Login**: Use your assigned credentials
2. **Check Dashboard**: Review daily metrics and alerts
3. **Process Transactions**: Use role-specific features
4. **Monitor Inventory**: Keep track of stock levels
5. **Generate Reports**: Access analytics and reports
6. **Logout**: Always logout securely when finished

## ?? **Performance Optimizations**

- **Async Operations**: Non-blocking database operations
- **Connection Pooling**: Efficient database connection management
- **Lazy Loading**: On-demand data loading for better performance
- **Indexing**: Optimized database queries with proper indexing
- **Memory Management**: Proper disposal of resources and contexts

## ?? **Configuration**

### **Database Connection**
Update the connection string in `Data/ApplicationDbContext.cs`:
```csharp
"Data Source=YOUR_SERVER;Initial Catalog=khmerdatabase;Integrated Security=True;..."
```

### **Application Settings**
- **Minimum Stock Levels**: Configurable per product
- **User Permissions**: Role-based access control
- **Audit Retention**: Configurable audit log retention
- **UI Themes**: Material Design color customization

## ?? **Troubleshooting**

### **Common Issues**
1. **Database Connection Error**: Check SQL Server service and connection string
2. **Login Issues**: Verify credentials and database initialization
3. **Performance Issues**: Ensure adequate system resources
4. **UI Issues**: Check display scaling and resolution settings

### **Error Resolution**
- Check the application logs for detailed error information
- Verify all dependencies are properly installed
- Ensure SQL Server is running and accessible
- Contact support for persistent issues

## ?? **Support & Community**

- **GitHub Issues**: Report bugs and feature requests
- **Documentation**: Comprehensive guides and API reference
- **Community**: Join discussions and share experiences
- **Professional Support**: Available for enterprise implementations

## ?? **License**

This project is licensed under the MIT License. See the LICENSE file for complete details.

---

**SyncVerse Studio** - *Empowering businesses with modern point-of-sale technology*

**Built with ?? using .NET 8 and Material Design principles**