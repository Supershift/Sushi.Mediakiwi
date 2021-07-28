using Sushi.Mediakiwi.Data.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;

namespace Sushi.Mediakiwi.Logic
{
    public class OAuth2Logic
    {
        public static System.Uri AuthenticationUrl(string state, string domain)
        {
            if (WimServerConfiguration.Instance.Authentication != null && WimServerConfiguration.Instance.Authentication.Aad != null && WimServerConfiguration.Instance.Authentication.Aad.Enabled)
            {
                var callback = WimServerConfiguration.Instance.Authentication.Aad.RedirectUrl;
                
                if (callback.OriginalString.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    callback = new Uri($"{domain}{callback}");
                }
                
                var ad_clientid = WimServerConfiguration.Instance.Authentication.Aad.Client;
                var ad_tenantid = WimServerConfiguration.Instance.Authentication.Aad.Tenant;

                string url = $"https://login.windows.net/{ad_tenantid}/oauth2/authorize?response_type=id_token&redirect_uri={callback}&client_id={ad_clientid}&scope=email&response_mode=form_post&nonce=default&state={state}";
                return new System.Uri(url);
            }
            return null;
        }

        public static string ExtractUpn(TokenValidation validation, string idtoken)
        {
            if (IsValidToken(validation, idtoken))
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(idtoken);
                var token = jsonToken as JwtSecurityToken;

                var email = token.Claims.First(claim => claim.Type == "upn").Value;
                return email;
            }
            return null;
        }

        public static bool IsValidToken(TokenValidation validation, string idtoken)
        {
            if (string.IsNullOrWhiteSpace(idtoken))
            {
                throw new ArgumentNullException(nameof(idtoken));
            }
            if (validation == null)
            {
                throw new ArgumentNullException(nameof(validation));
            }

            var exponent = validation.Exponent;
            var modulus = validation.Modulus;
            return VerifyToken(idtoken, exponent, modulus);
        }

        private static bool VerifyToken(string idToken, string exponent, string modulus)
        {
            try
            {
                var parts = idToken.Split('.');
                var header = parts[0];
                var payload = parts[1];
                string signedSignature = parts[2];
                //Extract user info from payload   
                string userInfo = Encoding.UTF8.GetString(Base64UrlDecode(payload));
                //Which will be Verified
                string originalMessage = string.Concat(header, ".", payload);
                byte[] keyBytes = Base64UrlDecode(modulus);
                string keyBase = Convert.ToBase64String(keyBytes);
                string key = @"<RSAKeyValue> <Modulus>" + keyBase + "</Modulus> <Exponent>" + exponent + "</Exponent> </RSAKeyValue>";
                bool result = VerifyData(originalMessage, signedSignature, key);
                if (result)
                    return true;
                else
                    return false;
            }
            catch (Exception) 
            {
                // to assure no exception being thrown
            }
            return false;
        }

        /// <summary>  
        /// Verifies encrypted signed message with public key encrypted original message.  
        /// </summary>  
        /// <param name="originalMessage">Original message as string. (Encrypted form)</param>  
        /// <param name="signedMessage">Signed message as string. (Encrypted form)</param>  
        /// <param name="publicKey">Public key as XML string.</param>  
        /// <returns>Boolean True if successful otherwise return false.</returns>  
        private static bool VerifyData(string originalMessage, string signedMessage, string publicKey)
        {
            bool success = false;
            using (var rsa = new RSACryptoServiceProvider())
            {
                var encoder = new UTF8Encoding();
                byte[] bytesToVerify = encoder.GetBytes(originalMessage);
                byte[] signedBytes = Base64UrlDecode(signedMessage);
                try
                {

                    rsa.FromXmlString(publicKey);
                    SHA256Managed Hash = new SHA256Managed();
                    byte[] hashedData = Hash.ComputeHash(signedBytes);
                    // Summary:
                    //     Verifies that a digital signature is valid by determining the hash value in the
                    //     signature using the provided public key and comparing it to the hash value of
                    //     the provided data.
                    success = rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA256"), signedBytes);
                }
                catch (CryptographicException e)
                {
                    success = false;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return success;
        }

        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding  
            output = output.Replace('_', '/'); // 63rd char of encoding  
            switch (output.Length % 4) // Pad with trailing '='s  
            {
                case 0: break; // No pad chars in this case  
                case 2: output += "=="; break; // Two pad chars  
                case 3: output += "="; break; // One pad char  
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder  
            return converted;
        }
    }
}
