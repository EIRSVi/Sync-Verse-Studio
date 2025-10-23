using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SyncVerseStudio.Helpers
{
    public static class EncryptionHelper
    {
        // Use a secure key - In production, store this in a secure configuration
        private static readonly string EncryptionKey = "SyncVerse2024Key!@#$%^&*()_+";
        
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            try
            {
                byte[] key = Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 32));
                
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.GenerateIV();
                    
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        // Prepend IV to encrypted data
                        msEncrypt.Write(aes.IV, 0, aes.IV.Length);
                        
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encryption error: {ex.Message}");
                return plainText;
            }
        }
        
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                byte[] key = Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 32));
                byte[] buffer = Convert.FromBase64String(cipherText);
                
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    
                    // Extract IV from the beginning
                    byte[] iv = new byte[aes.IV.Length];
                    Array.Copy(buffer, 0, iv, 0, iv.Length);
                    aes.IV = iv;
                    
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    
                    using (MemoryStream msDecrypt = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption error: {ex.Message}");
                return cipherText;
            }
        }
        
        public static string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return email;
                
            var parts = email.Split('@');
            var username = parts[0];
            var domain = parts[1];
            
            if (username.Length <= 2)
                return $"{username[0]}***@{domain}";
                
            return $"{username.Substring(0, 2)}***@{domain}";
        }
        
        public static string MaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length < 4)
                return phone;
                
            return $"***{phone.Substring(phone.Length - 4)}";
        }
    }
}
