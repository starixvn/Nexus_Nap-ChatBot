namespace Fanex.NCB.AutoJobs.Core.Helpers
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class CryptographyExtend
    {
        private static readonly DESCryptoServiceProvider CryptoServiceProvider = new();
        private static readonly byte[] RgbKey = new byte[] { 33, 99, 84, 90, 145, 172, 110, 204 };
        private static readonly byte[] RgbIv = Encoding.UTF8.GetBytes("&％#@?,:*!~^.");

        public static string DecryptPassword(string encryptedPassword)
        {
            if (encryptedPassword == null)
            {
                throw new ArgumentNullException(nameof(encryptedPassword));
            }

            using MemoryStream stream = new(Convert.FromBase64String(StandardizeEncryptedPasswordLength(encryptedPassword)));
            using ICryptoTransform transform = ((SymmetricAlgorithm)(object)CryptoServiceProvider).CreateDecryptor(RgbKey, RgbIv);
            using StreamReader streamReader = new(new CryptoStream(stream, transform, CryptoStreamMode.Read));
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