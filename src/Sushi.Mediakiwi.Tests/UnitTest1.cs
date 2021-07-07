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
using System.IdentityModel.Tokens.Jwt;
using Sushi.Mediakiwi.Authentication;

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
            using (var auth = new AuthenticationLogic(mockHttpContext.Object))
            {
                auth.Password = "pwd";
                var encrypted = auth.Encrypt(value);
                Assert.IsTrue(auth.Decrypt(encrypted).Equals(value));
            }
        }

        [TestMethod]
        public async Task Extract_ID()
        {
            var idtoken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyIsImtpZCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyJ9.eyJhdWQiOiI4NzI0OTRhNi0zMGVlLTQ0ZTktODNlZi00MTFhNDE2YTExZGEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85NWM5MGEwNC01ZWE4LTQ2NWUtYWMzZS0xYWFlM2IyOWIwYzkvIiwiaWF0IjoxNjI1NDUzNTQ5LCJuYmYiOjE2MjU0NTM1NDksImV4cCI6MTYyNTQ1NzQ0OSwiYWdlR3JvdXAiOiIzIiwiYWlvIjoiQWFRQVcvOFRBQUFBM051THBwQ0NIU2o0Tzh6WHRNaTZHTlluNjNac21Dd0Rla3F5SUNscDVFaEp1M2tXUUpNZm1UbjEyQW1NRFNRd2VBcmRMcWd4bzE1dW91eVdNNmNjYkF0VzJ2UnoxUE9PT0ptVEN4bXRlMFdSV1BJU3hLOStnY05WQnJZa3NRTnpZdkpqY2FlMTU0Uyt6TG4rNU1uMlY5NjRud3FpRnBkTmY1MzJOSnNwQWJGa082NTRDOTYxWEdxSnBQc0Q1T01mN2l1bVVPQWdFT0xZMFFIMEQ3eUtBZz09IiwiYW1yIjpbInB3ZCIsInJzYSIsIm1mYSJdLCJmYW1pbHlfbmFtZSI6Ik1vbGVud2lqayIsImdpdmVuX25hbWUiOiJNYXJjIiwiaXBhZGRyIjoiODkuMjM4LjE3Ny4xMDMiLCJuYW1lIjoiTWFyYyBNb2xlbndpamsiLCJub25jZSI6ImRlZmF1bHQiLCJvaWQiOiI4OTQ3ODZiMS1kZTA0LTQzOTUtYTU0MS1mOTY2ZjY3MTgxODkiLCJyaCI6IjAuQVFVQUJBckpsYWhlWGthc1BocXVPeW13eWFhVUpJZnVNT2xFZy05QkdrRnFFZG9GQVBjLiIsInN1YiI6IkQwWkJGWks5WXZYdWJKZVVWaXZRT2pscXhaOV81X2NDT3lIRW9NQnJPd2siLCJ0aWQiOiI5NWM5MGEwNC01ZWE4LTQ2NWUtYWMzZS0xYWFlM2IyOWIwYzkiLCJ1bmlxdWVfbmFtZSI6Im1hcmMubW9sZW53aWprQHN1cGVyc2hpZnQubmwiLCJ1cG4iOiJtYXJjLm1vbGVud2lqa0BzdXBlcnNoaWZ0Lm5sIiwidXRpIjoiMG5vUXBjZUd2RXk5ZEhYM1MxQnRBQSIsInZlciI6IjEuMCJ9.QeHuthopOQJfdKyKj4r4e55eLy8WoJHEJbQu3d8qVCXPDRqmHNDT1VZ8YgNSgcmVpHy1XO8cxa0gn-_f__TcSrmbDSMN_kw-EOp8XWMGMQWxvVL_NhcsRDCIU4WQAQ3z3V7_-4fHo5XMy4pd_xYnwW8c4XoXv4eaJ0su27RK9KkKhWgNyxR6_qmcgNG6KAK7R6ScPtH3DqYq4K3cccMV9bmHJNVrY51DQ4YNySpwaAkywGkipM3kgRwAEYDM1pMMvsGfL_DMWJOuK7M13Pi9DTSFygYWTxmIvO_IoLvR7j-XYSdIXZOt6CGYXXY_T1d0l7ZFi9yY36ClXcVaUXi4-Q";

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(idtoken);
            var tokenS = jsonToken as JwtSecurityToken;

            var email = tokenS.Claims.First(claim => claim.Type == "upn").Value;
            Console.WriteLine(email);
        }

        [TestMethod]
        public async Task ValidateJwt()
        {
            var idtoken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyIsImtpZCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyJ9.eyJhdWQiOiI4NzI0OTRhNi0zMGVlLTQ0ZTktODNlZi00MTFhNDE2YTExZGEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85NWM5MGEwNC01ZWE4LTQ2NWUtYWMzZS0xYWFlM2IyOWIwYzkvIiwiaWF0IjoxNjI1NDUzNTQ5LCJuYmYiOjE2MjU0NTM1NDksImV4cCI6MTYyNTQ1NzQ0OSwiYWdlR3JvdXAiOiIzIiwiYWlvIjoiQWFRQVcvOFRBQUFBM051THBwQ0NIU2o0Tzh6WHRNaTZHTlluNjNac21Dd0Rla3F5SUNscDVFaEp1M2tXUUpNZm1UbjEyQW1NRFNRd2VBcmRMcWd4bzE1dW91eVdNNmNjYkF0VzJ2UnoxUE9PT0ptVEN4bXRlMFdSV1BJU3hLOStnY05WQnJZa3NRTnpZdkpqY2FlMTU0Uyt6TG4rNU1uMlY5NjRud3FpRnBkTmY1MzJOSnNwQWJGa082NTRDOTYxWEdxSnBQc0Q1T01mN2l1bVVPQWdFT0xZMFFIMEQ3eUtBZz09IiwiYW1yIjpbInB3ZCIsInJzYSIsIm1mYSJdLCJmYW1pbHlfbmFtZSI6Ik1vbGVud2lqayIsImdpdmVuX25hbWUiOiJNYXJjIiwiaXBhZGRyIjoiODkuMjM4LjE3Ny4xMDMiLCJuYW1lIjoiTWFyYyBNb2xlbndpamsiLCJub25jZSI6ImRlZmF1bHQiLCJvaWQiOiI4OTQ3ODZiMS1kZTA0LTQzOTUtYTU0MS1mOTY2ZjY3MTgxODkiLCJyaCI6IjAuQVFVQUJBckpsYWhlWGthc1BocXVPeW13eWFhVUpJZnVNT2xFZy05QkdrRnFFZG9GQVBjLiIsInN1YiI6IkQwWkJGWks5WXZYdWJKZVVWaXZRT2pscXhaOV81X2NDT3lIRW9NQnJPd2siLCJ0aWQiOiI5NWM5MGEwNC01ZWE4LTQ2NWUtYWMzZS0xYWFlM2IyOWIwYzkiLCJ1bmlxdWVfbmFtZSI6Im1hcmMubW9sZW53aWprQHN1cGVyc2hpZnQubmwiLCJ1cG4iOiJtYXJjLm1vbGVud2lqa0BzdXBlcnNoaWZ0Lm5sIiwidXRpIjoiMG5vUXBjZUd2RXk5ZEhYM1MxQnRBQSIsInZlciI6IjEuMCJ9.QeHuthopOQJfdKyKj4r4e55eLy8WoJHEJbQu3d8qVCXPDRqmHNDT1VZ8YgNSgcmVpHy1XO8cxa0gn-_f__TcSrmbDSMN_kw-EOp8XWMGMQWxvVL_NhcsRDCIU4WQAQ3z3V7_-4fHo5XMy4pd_xYnwW8c4XoXv4eaJ0su27RK9KkKhWgNyxR6_qmcgNG6KAK7R6ScPtH3DqYq4K3cccMV9bmHJNVrY51DQ4YNySpwaAkywGkipM3kgRwAEYDM1pMMvsGfL_DMWJOuK7M13Pi9DTSFygYWTxmIvO_IoLvR7j-XYSdIXZOt6CGYXXY_T1d0l7ZFi9yY36ClXcVaUXi4-Q";

            var exponent = "AQAB";
            var modulus = "oaLLT9hkcSj2tGfZsjbu7Xz1Krs0qEicXPmEsJKOBQHauZ_kRM1HdEkgOJbUznUspE6xOuOSXjlzErqBxXAu4SCvcvVOCYG2v9G3-uIrLF5dstD0sYHBo1VomtKxzF90Vslrkn6rNQgUGIWgvuQTxm1uRklYFPEcTIRw0LnYknzJ06GC9ljKR617wABVrZNkBuDgQKj37qcyxoaxIGdxEcmVFZXJyrxDgdXh9owRmZn6LIJlGjZ9m59emfuwnBnsIQG7DirJwe9SXrLXnexRQWqyzCdkYaOqkpKrsjuxUj2-MHX31FqsdpJJsOAvYXGOYBKJRjhGrGdONVrZdUdTBQ";
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
