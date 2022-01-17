using Sushi.Mediakiwi.Data.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.JsonWebTokens;

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

        private static Regex EmailRegex { get; set; } = new Regex(Data.Utility.GlobalRegularExpression.EmailAddress, RegexOptions.IgnoreCase);
        public static async Task<string> ExtractUpnAsync(AuthenticationConfiguration authenticationConfiguration, string idtoken)
        {
            try
            {
                var handler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
                var jsonToken = handler.ReadToken(idtoken);
                var token = jsonToken as Microsoft.IdentityModel.JsonWebTokens.JsonWebToken;

                if (token == null)
                {
                    await Data.Notification.InsertOneAsync(nameof(ExtractUpnAsync), Data.NotificationType.Error, $"No token encountered: {idtoken}").ConfigureAwait(false);
                    return null;
                }

                var isValidToken = await IsValidTokenAsync(authenticationConfiguration, idtoken, token);
                if (isValidToken)
                {

                    const string DEFAULT_CLAIM_TYPE = "email";
                    List<string> claimtypes = new List<string>();
                    try
                    {
                        var claimtypesSetting = WimServerConfiguration.Instance.Authentication.Aad.EmailClaim;
                        if (!string.IsNullOrWhiteSpace(claimtypesSetting))
                        {
                            claimtypes.AddRange(claimtypesSetting.Split(new char[] { ',' }));
                        }
                    }
                    catch (Exception ex)
                    {
                        await Data.Notification.InsertOneAsync(nameof(ExtractUpnAsync), ex).ConfigureAwait(false);
                    }

                    // BD 2021-08-05: Loop through all the claims and look for the email
                    Claim claimOfDefaultType = null;
                    Claim claimWithEmailValue = null;
                    foreach (var claim in token.Claims)
                    {
                        if (claimtypes.Any(x => x.Equals(claim.Type, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            // If we find the claim with the configured type, return it immediately
                            return await LogAndReturnAsync(claim.Value).ConfigureAwait(false);
                        }
                        else if (claim.Type.Equals(DEFAULT_CLAIM_TYPE, StringComparison.CurrentCultureIgnoreCase))
                        {
                            // Remember we have encountered a claim of the default type
                            claimOfDefaultType = claim;
                        }
                        else if (EmailRegex.Match(claim.Value).Success)
                        {
                            // Remember we have encountered a claim that looks like an email address
                            claimWithEmailValue = claim;
                        }
                    }

                    // If we have not found the claim of the configured type, return the claim of the default type if we found it
                    if (claimOfDefaultType != null)
                    {
                        return await LogAndReturnAsync(claimOfDefaultType.Value).ConfigureAwait(false);
                    }

                    // Otherwise, if we found a claim that has the form of an email address, return that
                    if (claimWithEmailValue != null)
                    {
                        return await LogAndReturnAsync(claimWithEmailValue.Value).ConfigureAwait(false);
                    }

                    // If we could not find a claim of an expected type, but we only have one claim, return that
                    if (token.Claims.Count() == 1)
                    {
                        return await LogAndReturnAsync(token.Claims.First().Value).ConfigureAwait(false);
                    }

                    // We could not find a claim. Log the token for examination.
                    await Data.Notification.InsertOneAsync(nameof(ExtractUpnAsync), Data.NotificationType.Error, $"No email claim found: {idtoken}").ConfigureAwait(false);
                }
                else
                {
                    await Data.Notification.InsertOneAsync(nameof(ExtractUpnAsync), Data.NotificationType.Error, $"Token not valid: {idtoken}").ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                await Data.Notification.InsertOneAsync(nameof(ExtractUpnAsync), ex).ConfigureAwait(false);
                return null;
            }
            return null;
        }

        private static async Task<string> LogAndReturnAsync(string email)
        {
            await Data.Notification.InsertOneAsync(nameof(ExtractUpnAsync), Data.NotificationType.Information, $"Extracted: {email}").ConfigureAwait(false);
            return email;
        }

        public static async Task<bool> IsValidTokenAsync(AuthenticationConfiguration authenticationConfiguration, string idtoken, JsonWebToken token)
        {
            if (string.IsNullOrWhiteSpace(idtoken))
            {
                throw new ArgumentNullException(nameof(idtoken));
            }
            if (authenticationConfiguration == null)
            {
                throw new ArgumentNullException(nameof(authenticationConfiguration));
            }

            var exponent = authenticationConfiguration.Token.Exponent;
            return await VerifyTokenAsync(idtoken, exponent, authenticationConfiguration, token);
        }

        private static async Task<bool> VerifyTokenAsync(string idToken, string exponent, AuthenticationConfiguration authenticationConfiguration, JsonWebToken token)
        {
            try
            {
                // Get modulus
                string modulus = await DiscoveryLogic.GetModulusAsync(authenticationConfiguration.Aad.Tenant, authenticationConfiguration.Token.KeyType, token);

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
                return result;
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
