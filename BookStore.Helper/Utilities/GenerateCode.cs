using System.Security.Cryptography;
using System.Text;

namespace BookStore.Helper.Utilities
{
    public static class GenerateCode
    {
        private const string AlphanumericCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const string NumericCharacters = "0123456789";

        public static string GenerateAlphanumericCode(int length = 32)
        {
            return Generate(AlphanumericCharacters, length);
        }

        public static string GenerateNumericCode(int length = 6)
        {
            return Generate(NumericCharacters, length);
        }

        private static string Generate(string characters, int length)
        {
            var result = new StringBuilder(length);
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (result.Length < length)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    result.Append(characters[(int)(num % (uint)characters.Length)]);
                }
            }
            return result.ToString();
        }
    }
}
