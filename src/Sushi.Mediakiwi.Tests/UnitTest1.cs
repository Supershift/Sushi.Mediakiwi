using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Logic;
using Sushi.Mediakiwi.Utilities;
using Sushi.MicroORM;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace Sushi.Mediakiwi.Tests
{
    [TestClass]
    public class Data
    {
        [TestInitialize]
        public void Init()
        {
            string connectionString = "Server=testing-mediakiwi.database.windows.net,1433;Database=testing-mediakiwi;Uid=mediakiwi;Pwd=tMAX28KozCCXt0HUVDCg";
            DatabaseConfiguration.SetDefaultConnectionString(connectionString);
        }

        [TestMethod]
        public async Task Fetch_All_Countries()
        {
            var cc = await Country.SelectAllAsync();
            Assert.IsTrue(cc.Length > 0);
        }

        void ExtractText(List<string> collection, ContentItem item)
        {
            if (item == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(item.Text))
            {
                collection.Add(item.Text);
            }

            if (item.MultiFieldContent != null && item.MultiFieldContent.Any())
            {
                foreach (var nested in item.MultiFieldContent)
                {
                    ExtractText(collection, nested.Value);
                }
            }
        }


        [TestMethod]
        public async Task Fetch_User()
        {
            var visitor = Visitor.Select(new Guid("708150D2-9292-4C43-81C7-F92D25D0E954"));
            Assert.IsTrue(visitor.Data.Items.Length > 0);
        }



        [TestMethod]
        public async Task Create_Instance()
        {
            var instance = Utils.CreateInstance("Sushi.Mediakiwi.dll", "Sushi.Mediakiwi.AppCentre.Data.Implementation.Browsing");
            Assert.IsTrue(instance != null);
        }

        [TestMethod]
        public async Task Authentication()
        {
            var mockHttpContext = new Mock<HttpContext>();

            var mockRequest = new Mock<HttpRequest>();
            var mockResponse = new Mock<HttpResponse>();
            var mockHttpContextAccessor = new Mock<HttpContextAccessor>();

            mockHttpContext.SetupGet(x => x.Request).Returns(mockRequest.Object);
            mockHttpContext.SetupGet(x => x.Response).Returns(mockResponse.Object);

            //mockRequest.SetupGet(x => x.Headers).Returns(
            //    new HeaderDictionary  {
            //        { "x-session-id", SessionID },
            //        //{ "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8" }
            //    });

            // tell the mock to return "GET" when HttpMethod is called
            mockRequest.SetupGet(x => x.Method).Returns("GET");

            var value = "test";
            using (Authentication auth = new Authentication(mockHttpContext.Object))
            {
                auth.Password = "pwd";
                var encrypted = auth.Encrypt(value);
                Assert.IsTrue(auth.Decrypt(encrypted).Equals(value));
            }
        }

        [TestMethod]
        public async Task ValidateJwt()
        {
            var idtoken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IlM2a2ZZWmdJaFF5MDM2ZHZQelpib0tsUWZfczFvd2RpMDlfQzdOZEx3Z0EifQ.eyJleHAiOjE2MjUwNDE0NTMsIm5iZiI6MTYyNDk1NTA1MywidmVyIjoiMS4wIiwiaXNzIjoiaHR0cHM6Ly9sb2dpbi50aG9tYXNjb29rLmlkL2ViYWM5OWNhLTZmNWEtNGUzMy04MDc2LWZlYWQxNDM0YzlmMi92Mi4wLyIsInN1YiI6ImQ1MTNiZjg0LTk1N2EtNGM2Yy05OGRmLTRiYTk3ZDg2ODkyMCIsImF1ZCI6ImZjNTFlYjU5LWFhNmMtNDVlYS05ZTY2LTRmN2I2MDc3Nzg3ZSIsImFjciI6ImIyY18xYV9zaWduaW4iLCJpYXQiOjE2MjQ5NTUwNTMsImF1dGhfdGltZSI6MTYyNDk0NDk5OCwiYXV0aGVudGljYXRpb25Tb3VyY2UiOiJsb2NhbEFjY291bnRBdXRoZW50aWNhdGlvbiIsInVzZXJuYW1lIjoiQnVpbGQ3OCIsImF0X2hhc2giOiJKZGkwS0JFTmVTWEIxb2ptV0NYc2hBIn0.s8DDHF_opuf0R3ePV_WP2LzZas_3Te29jR0L18cQZazKPJ1vhmga4gqrNHCuqs0i4DS9jCAeLQB0sOPxKCw5a5OD9vcoBnMyQBSB7SF0Aig_4RBic1WrdSk-HXTmGbnFlhd9tSXXWePjaztFrJ9l91g3Mcq2_ZpJEjtQCQdOHEXHMR-RXKu7pmnNSqWMvHaxFWpR-u11O7Fmz28sIAIP2yc2ozdJtNLv81NujwC9G0zwEJQTTdH__Yauc8k3Sc85kuXp1Jrc9LY-ik8RFWfiOYRlaVtsJpxc36CzGeGIxixtlgdpXYDEk0BHFwLtpYrVEFROiX9s9MH-HEQWnehVPQ";

            var exponent = "AQAB";
            var modulus = "yTPJoKCQvfkH8lkUGH0s6mkLlpVdyNOCu7gfcG8niX6NNdayK9nHzrkoFxC4UNDvNi8E1oQumAvfb85HFHW60LuQXAtJlrifdeL8dPjYzohkVogP5Zvuv7_Hza1W1uS_019Syf9q8clAOW2sk81qPHaX0ZZNj3figLNsjryqUr0ZfeNQ2WZ7al5mrwJtAty4kJ7hOSjX4vMWJGVZ1-sFo-uWSMgvCjrkup0aRja7H-cTtAdQ8DkSc9R7HKP_9lrfR1zRBxh9Ls7AA0CHkYdXlvRITCnoY8wT_mn7zuPHTlhBZUpS7_nd_zlA0UX-Dfro20ux1Mm8fT8FbWDcOoHssQ";
            var result = VerifyTokenDetails(idtoken, exponent, modulus);
        }
        private static bool VerifyTokenDetails(string idToken, string exponent, string modulus)
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
            catch (Exception ex) { }
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
