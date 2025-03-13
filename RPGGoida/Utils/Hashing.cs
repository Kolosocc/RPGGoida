using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace RPGGoida.Utils
{
    public class Hashing
    {
        public static string HashPassword(string password)
        {

            string envFilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, ".env");
            DotEnv.Load(envFilePath);


            string salt = Environment.GetEnvironmentVariable("PASSWORD_SALT");

            string saltedPassword = password + salt;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
