using BCrypt.Net;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Services
{
    public class AuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private User? _currentUser;

        public AuthenticationService()
        {
            _context = new ApplicationDbContext();
        }

        public User? CurrentUser => _currentUser;

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

                if (user != null)
                {
                    // Check if user has no password (empty string)
                    bool passwordMatch = false;
                    if (string.IsNullOrEmpty(user.Password))
                    {
                        // No password required
                        passwordMatch = true;
                    }
                    else
                    {
                        // Verify password
                        passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
                    }

                    if (passwordMatch)
                    {
                        _currentUser = user;
                        
                        // Enhanced audit logging with role and status information
                        await LogAuditAsync("LOGIN_SUCCESS", "Users", user.Id, null, 
                            $"User '{username}' (Role: {user.Role}) successfully logged in - Status: ACTIVE");
                        
                        return true;
                    }
                }

                // Enhanced failed login logging
                if (user != null)
                {
                    await LogAuditAsync("LOGIN_FAILED", "Users", user.Id, null, 
                        $"Failed login attempt for user '{username}' (Role: {user.Role}) - Status: INVALID_PASSWORD");
                }
                else
                {
                    await LogAuditAsync("LOGIN_FAILED", "Users", null, null, 
                        $"Failed login attempt for unknown user '{username}' - Status: USER_NOT_FOUND");
                }

                return false;
            }
            catch (Exception ex)
            {
                // Enhanced error logging
                await LogAuditAsync("LOGIN_ERROR", "Users", null, null, 
                    $"Login system error for user '{username}' - Status: SYSTEM_ERROR - Details: {ex.Message}");
                Console.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            if (_currentUser != null)
            {
                var username = _currentUser.Username;
                var role = _currentUser.Role;
                var userId = _currentUser.Id;
                
                // Enhanced logout logging
                await LogAuditAsync("LOGOUT", "Users", userId, null, 
                    $"User '{username}' (Role: {role}) logged out - Status: SESSION_ENDED");
                
                _currentUser = null;
            }
        }

        public void Logout()
        {
            if (_currentUser != null)
            {
                var username = _currentUser.Username;
                var role = _currentUser.Role;
                var userId = _currentUser.Id;
                
                // Fire and forget logout logging
                Task.Run(async () => await LogAuditAsync("LOGOUT", "Users", userId, null, 
                    $"User '{username}' (Role: {role}) logged out - Status: SESSION_ENDED"));
                
                _currentUser = null;
            }
        }

        public bool HasPermission(string action)
        {
            if (_currentUser == null) return false;

            return _currentUser.Role switch
            {
                UserRole.Administrator => true, // Admin has all permissions
                UserRole.Cashier => action switch
                {
                    // Product permissions
                    "VIEW_PRODUCTS" => true,
                    
                    // Sale permissions
                    "PROCESS_SALE" => true,
                    "VIEW_SALES" => true,
                    "ADD_SALE" => true,
                    "GENERATE_RECEIPT" => true,
                    "PROCESS_RETURN" => true,
                    
                    // Customer permissions
                    "VIEW_CUSTOMERS" => true,
                    "ADD_CUSTOMER" => true,
                    "EDIT_CUSTOMER" => true,
                    
                    // Dashboard permissions
                    "VIEW_DASHBOARD" => true,
                    "VIEW_CASHIER_REPORTS" => true,
                    
                    _ => false
                },
                UserRole.InventoryClerk => action switch
                {
                    // Product permissions
                    "VIEW_PRODUCTS" => true,
                    "ADD_PRODUCT" => true,
                    "EDIT_PRODUCT" => true,
                    "DELETE_PRODUCT" => true,
                    
                    // Category permissions
                    "VIEW_CATEGORIES" => true,
                    "ADD_CATEGORY" => true,
                    "EDIT_CATEGORY" => true,
                    "DELETE_CATEGORY" => true,
                    
                    // Supplier permissions
                    "VIEW_SUPPLIERS" => true,
                    "ADD_SUPPLIER" => true,
                    "EDIT_SUPPLIER" => true,
                    "DELETE_SUPPLIER" => true,
                    
                    // Inventory permissions
                    "INVENTORY_ADJUSTMENT" => true,
                    "VIEW_INVENTORY" => true,
                    "ADD_INVENTORY_MOVEMENT" => true,
                    "EDIT_INVENTORY_MOVEMENT" => true,
                    
                    // Reports permissions
                    "VIEW_INVENTORY_REPORTS" => true,
                    "VIEW_DASHBOARD" => true,
                    
                    _ => false
                },
                _ => false
            };
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task LogAuditAsync(string action, string tableName, int? recordId, string? oldValues, string newValues)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = _currentUser?.Id ?? 0,
                    Action = action,
                    TableName = tableName,
                    RecordId = recordId,
                    OldValues = oldValues,
                    NewValues = newValues,
                    Timestamp = DateTime.Now,
                    IpAddress = "127.0.0.1" // For desktop app
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Audit log error: {ex.Message}");
            }
        }

        public ApplicationDbContext GetDbContext()
        {
            return _context;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
