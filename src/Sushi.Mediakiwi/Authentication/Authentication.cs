using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Authentication
{
    public class AuthenticationLogic : IDisposable
	{
        private Encryption.EncryptionHelper _encryptionHelper;

        public AuthenticationLogic()
        {
        }

        HttpContext m_Context;
        public AuthenticationLogic(HttpContext context)
        {
            m_Context = context;
        }

        #region Dispose
        private bool disposed = false;
        
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {
                    //  Components.Dispose();
                }
            }
            disposed = true;         
        }

        /// <summary>
        /// 
        /// </summary>
        ~AuthenticationLogic()      
        {
            Dispose(false);
        }
        #endregion Dispose

        /// <summary>
        /// Password which is used to encrypt / decrypt the ticket.
        /// </summary>
        public string Password
        {
            set
            {
                if (_encryptionHelper == null)
                {
                    _encryptionHelper = new Encryption.EncryptionHelper(value);
                }
            }
        }


        /// <summary>
        /// Gets the custom ticket value.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            if (m_Context != null && m_Context.Request != null)
            {
                if (m_Context.Request.Cookies[CommonConfiguration.AUTHENTICATION_COOKIE] == null)
                {
                    return null;
                }

                if (CookieSettings == null)
                {
                    try
                    {
                        CookieSettings = new NameValueCollection();
                        var decrypted = Decrypt(m_Context.Request.Cookies[CommonConfiguration.AUTHENTICATION_COOKIE]);
                        if (string.IsNullOrWhiteSpace(decrypted) == false)
                        {
                            foreach (var nv in decrypted.Split('&'))
                            {
                                CookieSettings.Add(nv.Split('=')[0], WebUtility.UrlDecode(nv.Split('=')[1]));
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                return CookieSettings[key];
            }
            return null;
        }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        /// <value>The domain.</value>
        public string Domain
        {
            get
            {
                if (string.IsNullOrWhiteSpace(m_Context.Request.Headers["X-Forwarded-Host"]))
                {
                    return m_Context.Request.Host.ToString();
                }

                return m_Context.Request.Headers["X-Forwarded-Host"];
            }
        }

        /// <summary>
        /// Gets or sets the additional cookie settings.
        /// </summary>
        /// <value>
        /// The cookie settings.
        /// </value>
        NameValueCollection CookieSettings { get; set; }

        /// <summary>
        /// Create the ticket.
        /// </summary>
        public void CreateTicket()
        {
            string cookie = string.Empty;
            foreach (var key in CookieSettings.AllKeys)
            {
                cookie += $"{(cookie.Length > 0 ? "&" : string.Empty)}{key}={WebUtility.UrlEncode(CookieSettings[key])}";
            }

            string encryption = Encrypt(cookie);
            if (m_Context != null && m_Context.Response != null)
            {
                try
                {
                    //To add Headers AFTER everything you need to do this
                    m_Context.Response.OnStarting(state =>
                    {
                        var httpContext = (HttpContext)state;
                        httpContext.Response.Cookies.Append(CommonConfiguration.AUTHENTICATION_COOKIE, encryption, new CookieOptions()
                        {
                            MaxAge = TimeSpan.FromMinutes(CommonConfiguration.AUTHENTICATION_TIMEOUT),
                            Domain = Domain.Split(':')[0],
                            HttpOnly = false,
                            Secure = true,
                            IsEssential = true
                        });

                        return Task.CompletedTask;
                    }, m_Context);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Add value to the ticket.
        /// </summary>
        /// <param name="Key">Key of the value</param>
        /// <param name="Value">Value belonging to the key</param>
        public void AddValue(string key, string Value)
        {
            if (CookieSettings == null)
            {
                CookieSettings = new NameValueCollection();
            }

            CookieSettings.Add(key, Value);
        }
     
        /// <summary>
        /// Encrypt a value and convert to Base64
        /// </summary>
        /// <param name="Value">String to be encrypted</param>
        /// <returns></returns>
        public string Encrypt(string value)
        {
            return _encryptionHelper.EncryptString(value);
        }

        /// <summary>
        /// Decrypt Base64 encoded value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Decrypt(string value)
        {
            return _encryptionHelper.DecryptString(value);
        }
    }
}
