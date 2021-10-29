using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Bgt.Ocean.Infrastructure.Helpers
{
    public static class SecureHelper
    {
        public static string GenerateRandomNumber(string keyComp = "0123456789", int randomLength = 3)
        {
            Random rnd = new Random();
            StringBuilder lastStr = new StringBuilder();
            string strModel = "0123456789";
            var charArra = strModel.ToCharArray();
            for (int i = 0; i < randomLength; i++)
            {
                int numRandom = rnd.Next(0, strModel.Length);
                lastStr.Append(charArra[numRandom].ToString());
            }

            return lastStr.ToString();
        }

    }
    public static class Sha256Encryption
    {
        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="PlainText">Text to be encrypted</param>
        /// <param name="CipherText">Text to be decrypted</param>
        /// <param name="Password">Password to encrypt with</param>
        /// <param name="Salt">Salt to encrypt with</param>
        /// <param name="HashAlgorithm">Can be either SHA1 or MD5</param>
        /// <param name="PasswordIterations">Number of iterations to do</param>
        /// <param name="InitialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="KeySize">Can be 128, 192, or 256</param>
        /// <returns>An encrypted string</returns>
        /// 

        public static readonly string HashAlgorithm = "SHA256";
        public static readonly int PasswordIterations = 2;
        public static readonly string InitialVector = "OFRna73m*aze01xY";
        public static readonly int KeySize = 256;

        public static string Encrypt(string PlainText, string Password, string Salt = "") // PIN(String), Password(Any but must map when decrypt), Password2(Optional)
        {
            try
            {
                if (string.IsNullOrEmpty(PlainText) || string.IsNullOrEmpty(Password))
                    return null;
                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
                byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
                byte[] PlainTextBytes = Encoding.UTF8.GetBytes(PlainText);
                PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
                byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
                RijndaelManaged SymmetricKey = new RijndaelManaged();
                SymmetricKey.Mode = CipherMode.CBC;
                byte[] CipherTextBytes = null;
                using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
                {
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
                        {
                            CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);
                            CryptoStream.FlushFinalBlock();
                            CipherTextBytes = MemStream.ToArray();
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                SymmetricKey.Clear();
                return Convert.ToBase64String(CipherTextBytes);
            }
            catch
            {
                return null;
            }
        }

        public static string Decrypt(string CipherText, string Password, string Salt = "") // PIN(Encrypted), Password(Any but must map when decrypt), Password2(Optional)
        {
            try
            {
                if (string.IsNullOrEmpty(CipherText) || string.IsNullOrEmpty(Password))
                    return null;
                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
                byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
                byte[] CipherTextBytes = Convert.FromBase64String(CipherText);
                PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
                byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
                RijndaelManaged SymmetricKey = new RijndaelManaged();
                SymmetricKey.Mode = CipherMode.CBC;
                byte[] PlainTextBytes = new byte[CipherTextBytes.Length];
                int ByteCount = 0;
                using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
                {
                    using (MemoryStream MemStream = new MemoryStream(CipherTextBytes))
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
                        {

                            ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                SymmetricKey.Clear();
                return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);
            }
            catch
            {
                return null;
            }
        }
    }
}
