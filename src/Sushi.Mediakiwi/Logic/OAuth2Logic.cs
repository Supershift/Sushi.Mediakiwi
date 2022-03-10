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
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

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
        public static async Task<string> ExtractUpnAsync(AuthenticationConfiguration authenticationConfiguration, string idtoken, HttpContext context)
        {
            try
            {
                var handler = new JsonWebTokenHandler();
                var jsonToken = handler.ReadToken(idtoken);
                var token = jsonToken as JsonWebToken;

                if (token == null)
                {
                    if (authenticationConfiguration.Aad?.LogUpnExtractionErrors == true)
                    {
                        await Data.Notification.InsertOneAsync(nameof(ExtractUpnAsync), Data.NotificationType.Error, $"No token encountered: {idtoken}").ConfigureAwait(false);
                    }
                    return null;
                }

                // todo: make this an instance method and provide configurationmanager through DI                
                var openIdConfigurationManager = context.RequestServices.GetRequiredService<ConfigurationManager<OpenIdConnectConfiguration>>();
                var openIdConnectConfigData = await openIdConfigurationManager.GetConfigurationAsync();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    RequireSignedTokens = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = authenticationConfiguration.Aad.Client,
                    IssuerSigningKeys = openIdConnectConfigData.SigningKeys,
                    ValidIssuer = openIdConnectConfigData.Issuer,
                };


                var isValidToken = handler.ValidateToken(idtoken, validationParameters);

                if (isValidToken.IsValid)
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
                        if (authenticationConfiguration.Aad?.LogUpnExtractionErrors == true)
                        {
                            await Data.Notification.InsertOneAsync(nameof(ExtractUpnAsync), ex).ConfigureAwait(false);
                        }
                    }

                    // BD 2021-08-05: Loop through all the claims and look for the email
                    Claim claimOfDefaultType = null;
                    Claim claimWithEmailValue = null;
                    foreach (var claim in token.Claims)
                    {
                        if (claimtypes.Any(x => x.Equals(claim.Type, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            // If we find the claim with the configured type, return it immediately
                            return await LogAndReturnAsync(authenticationConfiguration, claim.Value).ConfigureAwait(false);
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
                        return await LogAndReturnAsync(authenticationConfiguration, claimOfDefaultType.Value).ConfigureAwait(false);
                    }

                    // Otherwise, if we found a claim that has the form of an email address, return that
                    if (claimWithEmailValue != null)
                    {
                        return await LogAndReturnAsync(authenticationConfiguration, claimWithEmailValue.Value).ConfigureAwait(false);
                    }

                    // If we could not find a claim of an expected type, but we only have one claim, return that
                    if (token.Claims.Count() == 1)
                    {
                        return await LogAndReturnAsync(authenticationConfiguration, token.Claims.First().Value).ConfigureAwait(false);
                    }

                    // We could not find a claim. Log the token for examination.
                    if (authenticationConfiguration.Aad?.LogUpnExtractionErrors == true)
                    {
                        await Data.Notification.InsertOneAsync(nameof(ExtractUpnAsync), Data.NotificationType.Error, $"No email claim found: {idtoken}").ConfigureAwait(false);
                    }
                }
                else if (authenticationConfiguration.Aad?.LogUpnExtractionErrors == true)
                {
                    await Data.Notification.InsertOneAsync(nameof(ExtractUpnAsync), Data.NotificationType.Error, $"Token not valid: {idtoken}, {isValidToken.Exception}").ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (authenticationConfiguration.Aad?.LogUpnExtractionErrors == true)
                {
                    await Data.Notification.InsertOneAsync(nameof(ExtractUpnAsync), ex).ConfigureAwait(false);
                }
                return null;
            }
            return null;
        }

        private static async Task<string> LogAndReturnAsync(AuthenticationConfiguration authenticationConfiguration, string email)
        {
            if (authenticationConfiguration.Aad?.LogUpnExtraction == true)
            {
                await Data.Notification.InsertOneAsync(nameof(ExtractUpnAsync), Data.NotificationType.Information, $"Extracted: {email}").ConfigureAwait(false);
            }
            return email;
        }
    }
}
