namespace Fanex.NCB.Core.Extensions
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class CryptographyExtend
    {
#pragma warning disable S2278 // Neither DES (Data Encryption Standard) nor DESede (3DES) should be used
        private static readonly DESCryptoServiceProvider CryptoServiceProvider = new DESCryptoServiceProvider();
#pragma warning restore S2278 // Neither DES (Data Encryption

        private static readonly byte[] RgbKey = new byte[] { 33, 99, 84, 90, 145, 172, 110, 204 };
        private static readonly byte[] RgbIv = Encoding.UTF8.GetBytes("&％#@?,:*!~^.");

        public static string DecryptPassword(string encryptedPassword)
        {
            if (encryptedPassword == null)
            {
                throw new ArgumentNullException(nameof(encryptedPassword));
            }

            string s = StandardizeEncryptedPasswordLength(encryptedPassword);
            byte[] buffer = Convert.FromBase64String(s);
            using MemoryStream stream = new MemoryStream(buffer);
            using ICryptoTransform transform = ((SymmetricAlgorithm)(object)CryptoServiceProvider).CreateDecryptor(RgbKey, RgbIv);
            using StreamReader streamReader = new StreamReader(new CryptoStream(stream, transform, CryptoStreamMode.Read));
            return streamReader.ReadToEnd().Trim();
        }

        private static string StandardizeEncryptedPasswordLength(string encryptedPassword)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(encryptedPassword))
            {
                result = encryptedPassword[..(encryptedPassword.Length >> 2 << 2)];
            }

            return result;
        }
    }
}