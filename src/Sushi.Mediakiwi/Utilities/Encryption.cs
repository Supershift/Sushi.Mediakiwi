using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Wim.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class Encryption
    {
        static TripleDESCryptoServiceProvider GetCryptoService(string password)
        {
            if (password == null) password = "***";
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] key = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = key;
            des.Mode = CipherMode.ECB;
            return des;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Encrypt(string value)
        {
            return Encrypt(value, null);
        }

        /// <summary>
        /// Encrypts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static string Encrypt(string value, string password)
        {
            return Encrypt(value, password, false);
        }

        /// <summary>
        /// Encrypts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="password">The password.</param>
        /// <param name="returnHex">if set to <c>true</c> [return hex].</param>
        /// <returns></returns>
        public static string Encrypt(string value, string password, bool returnHex)
        {
            byte[] buff = ASCIIEncoding.ASCII.GetBytes(value);

            TripleDESCryptoServiceProvider des = GetCryptoService(password);
            if (returnHex)
                return BitConverter.ToString(des.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length)).Replace("-", string.Empty);
            return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length));
        }

        /// <summary>
        /// Decrypts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Decrypt(string value)
        {
            return Decrypt(value, null);
        }

        /// <summary>
        /// Decrypts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static string Decrypt(string value, string password)
        {
            return Decrypt(value, password, false);
        }

        /// <summary>
        /// Decrypts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="password">The password.</param>
        /// <param name="valueIsHex">if set to <c>true</c> [value is hex].</param>
        /// <returns></returns>
        public static string Decrypt(string value, string password, bool valueIsHex)
        {
            byte[] buff = null;
            if (valueIsHex)
            {
                buff = FromHexString(value);
            }
            else
                buff = Convert.FromBase64String(value);

            TripleDESCryptoServiceProvider des = GetCryptoService(password);
            return ASCIIEncoding.ASCII.GetString(des.CreateDecryptor().TransformFinalBlock(buff, 0, buff.Length));
        }

        static byte[] FromHexString(string encryptedToken)
        {
            List<byte> bytes = new List<byte>();

            for (int i = 0; i <= encryptedToken.Length; i += 2)
            {
                try
                {
                    bytes.Add((byte)Int32.Parse(encryptedToken.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
                }
                catch //whatever exception
                {
                    //handle
                }

            }

            return bytes.ToArray();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetSalt()
        {
            return CreateSalt(5);
        }
        
        static string CreateSalt(int intSize)
        {
            byte[] bytBuff = new byte[intSize];

            RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
            rngCrypto.GetBytes(bytBuff);
            return Convert.ToBase64String(bytBuff);
        }
    }
}
