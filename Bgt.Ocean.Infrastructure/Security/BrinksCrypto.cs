using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Bgt.Ocean.Infrastructure.Security
{
    public static class BrinksCrypto
    {
        public static CypherResult AES_Encrypt(this string plainText, string publicKey)
        {
            try
            {
                // Generate salt
                byte[] key, iv;
                byte[] salt = new byte[8];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetNonZeroBytes(salt);
                DeriveKeyAndIV(publicKey, salt, out key, out iv);
                // Encrypt bytes
                byte[] encryptedBytes = EncryptStringToBytesAes(plainText, key, iv);
                // Add salt at first 8 bytes
                byte[] encryptedBytesWithSalt = new byte[salt.Length + encryptedBytes.Length + 8];
                Buffer.BlockCopy(Encoding.ASCII.GetBytes("Salted__"), 0, encryptedBytesWithSalt, 0, 8);
                Buffer.BlockCopy(salt, 0, encryptedBytesWithSalt, 8, salt.Length);
                Buffer.BlockCopy(encryptedBytes, 0, encryptedBytesWithSalt, salt.Length + 8, encryptedBytes.Length);
                // Base64 encode
                return new CypherResult() { IsSuccess = true, Message = Convert.ToBase64String(encryptedBytesWithSalt) };
            }
            catch (Exception ex)
            {
                return new CypherResult() { CustomMessage = ex.InnerException.Message };
            }
        }

        public static CypherResult AES_Decrypt(this string encrypted, string publicKey)
        {
            try
            {
                // Convert from base 64 decode to byte[]
                byte[] encryptedBytesWithSalt = Convert.FromBase64String(encrypted);
                // Extract salt (first 8 bytes of encrypted)
                byte[] salt = new byte[8];
                byte[] encryptedBytes = new byte[encryptedBytesWithSalt.Length - salt.Length - 8];
                Buffer.BlockCopy(encryptedBytesWithSalt, 8, salt, 0, salt.Length);
                Buffer.BlockCopy(encryptedBytesWithSalt, salt.Length + 8, encryptedBytes, 0, encryptedBytes.Length);
                // Get key and iv
                byte[] key, iv;
                DeriveKeyAndIV(publicKey, salt, out key, out iv);
                return new CypherResult() { IsSuccess = true, Message = DecryptStringFromBytesAes(encryptedBytes, key, iv) };
            }
            catch 
            {
                return new CypherResult() { CustomMessage = "Decrypting Process Failed. Please check if your key is correct." };
            }
        }

        private static void DeriveKeyAndIV(string publicKey, byte[] salt, out byte[] key, out byte[] iv)
        {
            List<byte> concatenatedHashes = new List<byte>(48);
            byte[] passphrase = Encoding.UTF8.GetBytes(publicKey);
            byte[] currentHash = new byte[0];

            // The hash size for the MD5 algorithm is 128 bits.
            MD5 md5 = MD5.Create();

            bool enoughBytesForKey = false;
            while (!enoughBytesForKey)
            {
                int preHashLength = currentHash.Length + passphrase.Length + salt.Length;
                byte[] preHash = new byte[preHashLength];

                Buffer.BlockCopy(currentHash, 0, preHash, 0, currentHash.Length);
                Buffer.BlockCopy(passphrase, 0, preHash, currentHash.Length, passphrase.Length);
                Buffer.BlockCopy(salt, 0, preHash, currentHash.Length + passphrase.Length, salt.Length);

                // The ComputeHash methods of the MD5 class return the hash as an array of 16 bytes
                currentHash = md5.ComputeHash(preHash);
                concatenatedHashes.AddRange(currentHash);

                if (concatenatedHashes.Count >= 48)
                    enoughBytesForKey = true;
            }

            // Generate key and iv
            key = new byte[32];
            iv = new byte[16];

            concatenatedHashes.CopyTo(0, key, 0, 32);
            concatenatedHashes.CopyTo(32, iv, 0, 16);

            md5.Clear();
        }

        private static byte[] EncryptStringToBytesAes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");

            MemoryStream msEncrypt;
            RijndaelManaged aesAlg = null;

            try
            {
                // Create a RijndaelManaged object with the specified key and IV.
                aesAlg = new RijndaelManaged { KeySize = 128, BlockSize = 128, Key = key, IV = iv, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                msEncrypt = new MemoryStream();
                CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    //Write all data to the stream.
                    swEncrypt.Write(plainText);
                    swEncrypt.Flush();
                    swEncrypt.Close();
                }
                
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return msEncrypt.ToArray();
        }

        private static string DecryptStringFromBytesAes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");

            RijndaelManaged aesAlg = null;
            string plaintext;

            try
            {
                // Create a RijndaelManaged object with the specified key and IV.
                aesAlg = new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                        srDecrypt.Close();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }
    }

    public class CypherResult
    {
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; }
        public string CustomMessage { get; set; }
    }
}
