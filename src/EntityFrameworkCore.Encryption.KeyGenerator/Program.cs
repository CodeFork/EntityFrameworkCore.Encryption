using System;
using System.Security.Cryptography;

namespace EntityFrameworkCore.Encryption.KeyGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The KeyGenerator can be used to initialize the encryption keys.");
            
            Console.WriteLine($"Initialization Vector (IV): {GetRandomData(128)}");
            Console.WriteLine($"Key: {GetRandomData(256)}");
            
            Console.WriteLine("Please paste this keys into your application configuration!");
        }
        
        /// Acknowledgements: Taken from: https://stackoverflow.com/a/41344959 (THANKS)
        private static string GetRandomData(int bits)
        {
            var result = new byte[bits / 8];
            RandomNumberGenerator.Create().GetBytes(result);
            return Convert.ToBase64String(result);
        }
    }
}