using System;
using System.Security.Cryptography;
using System.Text;

namespace OPFService.Utilities
{
    public static class StringUtilities
    {
        public static bool PercentageMatch(string value, string subvalue, ushort percent)
        {
            int matchingCharacters = 0;

            for (int i = 0; i < Math.Min(value.Length, subvalue.Length); i++)
            {
                if (value[i] == subvalue[i])
                {
                    matchingCharacters++;
                }
            }

            return (double)matchingCharacters / value.Length * 100 >= Math.Min(percent, 100U);
        }

        public static string GetPasswordHash(string password)
        {
            using SHA1Managed sh1 = new();
            var passwordHashBytes = sh1.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(passwordHashBytes).Replace("-", string.Empty);
        }
    }
}
