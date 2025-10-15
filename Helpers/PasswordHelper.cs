using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace hcp.Helpers
{
    public static class PasswordHelper
    {
        /// Hash mật khẩu bằng PBKDF2 (chuẩn OWASP)
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) {
                throw new ArgumentException("Password cannot be empty or null", nameof(password));
            }

            // Tạo salt ngẫu nhiên (16 byte)
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            // Hash bằng PBKDF2 với 100.000 vòng lặp
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32);

            // Kết hợp salt + hash (dạng base64) để lưu vào DB
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string? storedHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash)) {
                return false;
            }

            // Tách salt và hash
            var parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            try {
                var salt = Convert.FromBase64String(parts[0]);
                var stored = Convert.FromBase64String(parts[1]);

                // Hash lại mật khẩu nhập vào với cùng salt
                var hash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 32);

                // So sánh byte một cách an toàn (tránh timing attack)
                return CryptographicOperations.FixedTimeEquals(hash, stored);
            }
            catch {
                // Nếu storedHash bị lỗi format base64
                return false;
            }
        }
    }
}
