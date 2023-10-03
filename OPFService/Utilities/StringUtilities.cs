using System.Security.Cryptography;
using System.Text;

namespace OPFService.Utilities
{
    public class StringUtilities
    {
        public static string GetPasswordHash(string password)
        {
            var encodedPasswordBytes = Encoding.UTF8.GetBytes(password);

            var sha1 = SHA1.Create();
            var passwordHashBytes = sha1.ComputeHash(encodedPasswordBytes);

            var passwordHash = new StringBuilder();
            foreach (var b in passwordHashBytes)
            {
                var hex = b.ToString("x2");
                passwordHash.Append(hex);
            }

            return passwordHash.ToString();
        }
    }
}
