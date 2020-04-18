using System;
using System.IO;
using System.Security.Cryptography;
using Convert = System.Convert;

namespace Infrastructure.Security
{
    public class AesCryptography
    {
        private const string _key = "PdE4NZvORBkS65VgMPBu+2HSQjpMVr20LYPGtIxt3xY=";
        private const string IvForGeneralPurpose = "kKidVuTzCS+KcHR9WpS7eA==";

        public static byte[] Encrypt(string plainText, string base64Iv)
        {
            return Encrypt(plainText, Convert.FromBase64String(base64Iv));
        }

        public static byte[] Encrypt(string plainText)
        {
            return Encrypt(plainText, Convert.FromBase64String(IvForGeneralPurpose));
        }

        public static string Decrypt(string base64CipherText, string base64Iv)
        {
            return Decrypt(Convert.FromBase64String(base64CipherText), Convert.FromBase64String(base64Iv));
        }

        public static string Decrypt(string base64CipherText)
        {
            return Decrypt(Convert.FromBase64String(base64CipherText), Convert.FromBase64String(IvForGeneralPurpose));
        }

        public static byte[] Encrypt(string plainText, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;

            // Create an Aes object with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(_key);
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string Decrypt(byte[] cipherText, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold the decrypted text.
            string plaintext = null;

            // Create an Aes object with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(_key);
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    // Read the decrypted bytes from the decrypting stream and place them in a string.
                    plaintext = srDecrypt.ReadToEnd();
                }

            }
            return plaintext;
        }
    }
}
