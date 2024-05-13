using DoctorsAppointmentScheduler.DTO;
using System.Security.Cryptography;
using System.Text;

namespace DoctorsAppointmentScheduler.Data.Services
{
    internal class Helper
    {
        public static string GenerateRandomUsername()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            string randomUsername;

            Random random = new Random();
           // int attempts = 0;
            string randomPart = new string(Enumerable.Repeat(chars, 6) 
                .Select(s => s[random.Next(s.Length)]).ToArray());

            randomUsername = $"{randomPart}{random.Next(100, 1000)}";
            return randomUsername; 

        }
        public static string GenerateRandomPassword()
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int passwordLength = 12;

            char[] password = new char[passwordLength];
            Random random = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                password[i] = allowedChars[random.Next(allowedChars.Length)];
            }

            return new string(password);
        }
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashedPassword;
            }
        }
      
    }
}
