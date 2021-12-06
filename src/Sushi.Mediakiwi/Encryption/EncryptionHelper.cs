using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Sushi.Mediakiwi.Encryption
{
    internal class EncryptionHelper
    {
        private readonly string _encryptionKey;
        private byte[] _salt = Encoding.ASCII.GetBytes("o6806642kbM7c5");

        public EncryptionHelper(string key)
        {
            _encryptionKey = key;
            if (string.IsNullOrWhiteSpace(CommonConfiguration.ENCRYPTION_SALT) == false)
            {
                _salt = Encoding.ASCII.GetBytes(CommonConfiguration.ENCRYPTION_SALT);
            }
        }

        private string ConvertToValidKeyLength(string input)
        {
            const int keyLength = 32;
            var temp = input.ToLowerInvariant();
            temp = temp.Replace(" ", String.Empty, StringComparison.InvariantCultureIgnoreCase);

            if (temp.Length < keyLength)
            {
                temp = temp.PadLeft(keyLength, '0');
            }
            else if (temp.Length > keyLength)
            {
                temp = temp.Substring(0, keyLength);
            }

            return temp;
        }

        public string EncryptString(string plainText)
        {
            string result = null;

            try
            {
                // generate the key from the shared secret and the salt  
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_encryptionKey, _salt);

                // Create a RijndaelManaged object  
                using (RijndaelManaged aesAlg = new RijndaelManaged())
                {
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                    // Create a decryptor to perform the stream transform.  
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.  
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        // prepend the IV  
                        msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                        msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.  
                                swEncrypt.Write(plainText);
                            }
                        }
                        result = Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public string DecryptString(string cipherText)
        {
            // Declare the string used to hold  
            // the decrypted text.  
            string result = null;

            try
            {
                // generate the key from the shared secret and the salt  
                using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_encryptionKey, _salt))
                {
                    // Create the streams used for decryption.  
                    byte[] bytes = Convert.FromBase64String(cipherText);
                    using (MemoryStream msDecrypt = new MemoryStream(bytes))
                    {
                        // Create a RijndaelManaged object  
                        // with the specified key and IV.  
                        using (RijndaelManaged aesAlg = new RijndaelManaged())
                        {
                            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                            // Get the initialization vector from the encrypted stream  
                            aesAlg.IV = ReadByteArray(msDecrypt);
                            // Create a decrytor to perform the stream transform.  
                            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                {
                                    // Read the decrypted bytes from the decrypting stream  
                                    // and place them in a string.  
                                    result = srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}