using System;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Utilities
{
    public class Authentication : IDisposable
	{
        private Wim.Utilities.EncryptionByRijndael m_Encryption = null;

        public Authentication()
        {
        }

        HttpContext m_Context;
        public Authentication(HttpContext context)
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
            if(!this.disposed)
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
        ~Authentication()      
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
                SetEncryption(value);
            }
        }

        /// <summary>
        /// Lifetime of the new custom ticket.
        /// </summary>
        public DateTime LifeTime { get; set; }

        /// <summary>
        /// Name of the ticket.
        /// </summary>
        public string TicketName { get; set; } = "Mediakiwi";
        

        void SetEncryption(string password)
        {
            if (m_Encryption == null)
            {
                string vector = "@1B2c3D4e5v6g7H8";
                m_Encryption = new Wim.Utilities.EncryptionByRijndael(password, vector, 8, 16);
                m_Encryption.PasswordIterations = 3;
            }
        }


        /// <summary>
        /// Gets the custom ticket value.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            if(m_Context != null && m_Context.Request != null)
            {
                if (m_Context.Request.Cookies[TicketName] == null)
                    return null;

                if (CookieSettings == null)
                {
                    try
                    {
                        CookieSettings = new NameValueCollection();
                        var decrypted = Decrypt(m_Context.Request.Cookies[TicketName]);

                        foreach (var nv in decrypted.Split('&'))
                            CookieSettings.Add(nv.Split('=')[0], WebUtility.UrlDecode(nv.Split('=')[1]));
                    }
                    catch(Exception)
                    {
                        return null;
                    }
                }
                return CookieSettings[key];
            }
            return null;
        }

        /// <summary>
        /// Remove the ticket.
        /// </summary>
        public void RemoveCustomTicket()
        {
            RemoveTicket(TicketName);
        }

        /// <summary>
        /// Remove the ticket.
        /// </summary>
        /// <param name="TicketName">Name of the ticket</param>
        public void RemoveTicket(string TicketName)
        {
            //if (m_Context != null && m_Context.Request != null && m_Context.Request.Cookies[TicketName] != null)
            //{
            //    m_Context.Response.Cookies.Delete(TicketName);
            //}
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
                    return string.Concat(m_Context.Request.Host.ToString());

                return string.Concat(m_Context.Request.Headers["X-Forwarded-Host"]);
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
            RemoveCustomTicket();

            string cookie = string.Empty;
            foreach (var key in CookieSettings.AllKeys)
                cookie += $"{(cookie.Length > 0 ? "&" : string.Empty)}{key}={WebUtility.UrlEncode(CookieSettings[key])}";

            string encryption = Encrypt(cookie);
            if (m_Context != null && m_Context.Response != null)
            {
                try
                {
                    //To add Headers AFTER everything you need to do this
                    m_Context.Response.OnStarting(state => {
                        var httpContext = (HttpContext)state;
                        httpContext.Response.Cookies.Append(TicketName, encryption, new CookieOptions() {
                            Expires = LifeTime,
                            Domain = this.Domain.Split(':')[0],
                            HttpOnly = false,
                            Secure = true,
                            IsEssential = true
                        });

                        return Task.CompletedTask;
                    }, m_Context);
                }
                catch (Exception ex) {
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
            if (this.CookieSettings == null)
                this.CookieSettings = new NameValueCollection();

            this.CookieSettings.Add(key, Value);
        }
     
        /// <summary>
        /// Encrypt a value and convert to Base64
        /// </summary>
        /// <param name="Value">String to be encrypted</param>
        /// <returns></returns>
        public string Encrypt(string value)
        {
            return m_Encryption.Encrypt(value);
        }

        /// <summary>
        /// Decrypt Base64 encoded value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Decrypt(string value)
        {
            return m_Encryption.Decrypt(value);
        }
    }
}
