using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Assets.Scripts.Common
{
    /// <summary>
    /// AES (Advanced Encryption Standard) implementation with 128-bit key (default)
    /// - 128-bit AES is approved  by NIST, but not the 256-bit AES
    /// - 256-bit AES is slower than the 128-bit AES (by about 40%)
    /// - Use it for secure data protection
    /// - Do NOT use it for data protection in RAM (in most common scenarios)
    /// </summary>
    public static class AES
    {
        public static int KeyLength = 128;
        private const string SaltKey = "Vh7G8jLZy93~Gu@5";
        private const string VIKey = "~9YW@4Mh8x}A|LZs"; // TODO: Generate random VI each encryption and store it with encrypted value

        public static string Encrypt(byte[] value, string password)
        {
            var keyBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(SaltKey)).GetBytes(KeyLength / 8);
            var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.UTF8.GetBytes(VIKey));

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(value, 0, value.Length);
                    cryptoStream.FlushFinalBlock();
                    cryptoStream.Close();
                    memoryStream.Close();

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public static string Encrypt(string value, string password)
        {
            return Encrypt(Encoding.UTF8.GetBytes(value), password);
        }

        public static string Decrypt(string value, string password)
        {
            var cipherTextBytes = Convert.FromBase64String(value);
            var keyBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(SaltKey)).GetBytes(KeyLength / 8);
            var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.None };
            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.UTF8.GetBytes(VIKey));

            using (var memoryStream = new MemoryStream(cipherTextBytes))
            {
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    var plainTextBytes = new byte[cipherTextBytes.Length];
                    var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                    memoryStream.Close();
                    cryptoStream.Close();

                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
                }
            }
        }
    }
}