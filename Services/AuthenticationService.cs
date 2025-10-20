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

                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    _currentUser = user;
                    
                    // Log successful login
                    await LogAuditAsync("LOGIN", "Users", user.Id, null, $"User {username} logged in successfully");
                    
                    return true;
                }

                // Log failed login attempt
                if (user != null)
                {
                    await LogAuditAsync("LOGIN_FAILED", "Users", user.Id, null, $"Failed login attempt for user {username}");
                }

                return false;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            if (_currentUser != null)
            {
                // Log logout
                Task.Run(async () => await LogAuditAsync("LOGOUT", "Users", _currentUser.Id, null, $"User {_currentUser.Username} logged out"));
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
                    "VIEW_PRODUCTS" => true,
                    "PROCESS_SALE" => true,
                    "VIEW_CUSTOMERS" => true,
                    "ADD_CUSTOMER" => true,
                    "VIEW_SALES" => true,
                    "GENERATE_RECEIPT" => true,
                    "PROCESS_RETURN" => true,
                    _ => false
                },
                UserRole.InventoryClerk => action switch
                {
                    "VIEW_PRODUCTS" => true,
                    "ADD_PRODUCT" => true,
                    "EDIT_PRODUCT" => true,
                    "DELETE_PRODUCT" => true,
                    "VIEW_CATEGORIES" => true,
                    "ADD_CATEGORY" => true,
                    "EDIT_CATEGORY" => true,
                    "DELETE_CATEGORY" => true,
                    "VIEW_SUPPLIERS" => true,
                    "ADD_SUPPLIER" => true,
                    "EDIT_SUPPLIER" => true,
                    "DELETE_SUPPLIER" => true,
                    "INVENTORY_ADJUSTMENT" => true,
                    "VIEW_INVENTORY_REPORTS" => true,
                    _ => false
                },
                _ => false
            };
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private async Task LogAuditAsync(string action, string tableName, int? recordId, string? oldValues, string newValues)
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

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}