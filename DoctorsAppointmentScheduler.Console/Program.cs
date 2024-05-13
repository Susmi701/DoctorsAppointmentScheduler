using System;

namespace DoctorsAppointmentScheduler.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Enter password to hash:");
            string password = Console.ReadLine();

            string hashedPassword = HashPassword(password);

            Console.WriteLine($"Hashed password: {hashedPassword}");
        }
        public static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashedPassword;
            }
        }
    }
}