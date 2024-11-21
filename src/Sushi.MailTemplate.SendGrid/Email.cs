using System;

namespace Sushi.MailTemplate.SendGrid
{
    /// <summary>
    /// E-mail to send
    /// </summary>
    public class Email
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// The name to display
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// The from e-mail address
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// The to e-mail address
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// The subject of the e-mail
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The body of the e-mail
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The list of bcc receivers, ";"-separated
        /// </summary>
        public string Bccs { get; set; }

        /// <summary>
        /// The template name
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// The customer guid
        /// </summary>
        public Guid? CustomerGUID { get; set; }
    }
}