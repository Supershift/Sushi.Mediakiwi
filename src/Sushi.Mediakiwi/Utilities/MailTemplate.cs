using System;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Collections.Generic;
using System.Text;

namespace Wim.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class MailTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MailTemplate"/> class.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="displayname">The displayname.</param>
        public MailTemplate(string email, string displayname)
        {
            m_FromAddress = new MailAddress(email, displayname);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailTemplate"/> class.
        /// </summary>
        /// <param name="email">The email.</param>
        public MailTemplate(string email)
        {
            m_FromAddress = new MailAddress(email);
        }

        protected MailAddress m_FromAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailTemplate"/> class.
        /// </summary>
        public MailTemplate()
        {
            m_FromAddress = new MailAddress(Sushi.Mediakiwi.Data.Environment.Current.DefaultMailAddress);
        }

        ///// <summary>
        ///// Selects the one.
        ///// </summary>
        ///// <returns></returns>
        //public MailTemplate SelectOne(int TypeID)
        //{
        //    MailTemplate template = new MailTemplate();
        //    return template;
        //}

        public List<MailAddress> mailList;
        List<MailAddress> mailListBcc;
        List<MailAddress> mailListCC;
        List<Attachment> mailAttachmentList;

        /// <summary>
        /// Adds to.
        /// </summary>
        /// <param name="displayname">The displayname.</param>
        /// <param name="email">The email.</param>
        public void AddRecipient(string email, string displayname)
        {
            if (string.IsNullOrEmpty(email)) return;
            if (mailList == null) mailList = new List<MailAddress>();
            mailList.Add(new MailAddress(email, displayname));
        }

        /// <summary>
        /// Adds the replacement.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="isTextData">If set to true the value will be added to the textversion of the email. All HTML tags will be removed from the text and the HTML Breaks will be replaced by normal.</param>
        public void AddReplacement(object key, object value, bool isTextData)
        {
            if (m_Replacements == null) m_Replacements = new System.Collections.Hashtable();
            m_Replacements.Add(key, value);

            if (isTextData)
            {
                if (m_TextData == null)
                    m_TextData = new StringBuilder();

                //m_TextData.Append(Wim.Utility.CleanFormatting(Wim.Utility.CleanLineFeed(value.ToString(), false)));
                m_TextData.Append(value.ToString());
            }
        }

        StringBuilder m_TextData;

        public void AddReplacement(object key, object value)
        {
            AddReplacement(key, value, false);
        }

        /// <summary>
        /// Adds the recepient.
        /// </summary>
        /// <param name="email">The email.</param>
        public void AddRecipient(string email)
        {
            if (mailList == null) mailList = new List<MailAddress>();
            mailList.Add(new MailAddress(email));
        }

        /// <summary>
        /// Adds an email address to this message it's BCC address list.
        /// </summary>
        /// <param name="email">The email.</param>
        public void AddRecipientBCC(string email)
        {
            if (string.IsNullOrEmpty(email)) return;
            if (mailListBcc == null) mailListBcc = new List<MailAddress>();
            mailListBcc.Add(new MailAddress(email));
        }

        /// <summary>
        /// Adds an email address to this message it's CC address list.
        /// </summary>
        /// <param name="email">The email.</param>
        public void AddRecipientCC(string email)
        {
            if (string.IsNullOrEmpty(email)) return;
            if (mailListCC == null) mailListCC = new List<MailAddress>();
            mailListCC.Add(new MailAddress(email));
        }

        /// <summary>a
        /// Adds to.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void AddAttachment(string filename)
        {
            if (mailAttachmentList == null) mailAttachmentList = new List<Attachment>();
            mailAttachmentList.Add(new Attachment(filename));
        }

        internal class Link
        {
            internal bool NestImages;
            internal string BaseFolder;
            internal string BaseUrl;
            internal int Counter = 1;
            internal System.Collections.Hashtable BinaryTable;

            /// <summary>
            /// Spanses the specified m.
            /// </summary>
            /// <param name="m">The m.</param>
            /// <returns></returns>
            internal string Links(Match m)
            {
                string path = m.Value.Replace("src=", string.Empty).Replace("\"", string.Empty);
                
                if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                    return string.Format("src=\"{0}\"", path);

                if (path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    return string.Format("src=\"{0}\"", path);


                if (NestImages)
                {
                    string candidate = string.Concat(this.BaseFolder, "/", path);

                    if (BinaryTable == null)
                        BinaryTable = new System.Collections.Hashtable();

                    if (!BinaryTable.ContainsKey(candidate))
                    {
                        BinaryTable.Add(candidate, Counter);
                        Counter++;
                        return string.Format("src=\"cid:img{0}\"", Counter - 1);
                    }
                    else
                        return string.Format("src=\"cid:img{0}\"", BinaryTable[candidate]);
                }
                else
                {
                    string candidate = string.Concat(this.BaseUrl, this.BaseFolder, "/", path);
                    return string.Format("src=\"{0}\"", candidate);
                }
            }

            internal string Hrefs(Match m)
            {
                string path = m.Value.Replace("href=", string.Empty).Replace("\"", string.Empty);

                if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                    return string.Format("href=\"{0}\"", path);

                if (path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    return string.Format("href=\"{0}\"", path);

                if (path.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
                    return string.Format("href=\"{0}\"", path);

                string candidate = string.Concat(this.BaseUrl, this.BaseFolder, "/", path);
                return string.Format("href=\"{0}\"", candidate);
            }
        }

        System.Collections.Hashtable m_Replacements;

        /// <summary>
        /// Submits the specified subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public bool SubmitPage(string subject, int pageID)
        {
            return SubmitPage(subject, pageID, null);
        }

        /// <summary>
        /// Submits the specified subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="postData">The post data.</param>
        /// <returns></returns>
        public bool SubmitPage(string subject, int pageID, Dictionary<string, string> postData)
        {
            Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(pageID);
            string output = WebScrape.GetUrlResponse(page.HRefFull, 60, null, postData);

            return SubmitTemplate(subject, output, false);
        }

        /// <summary>
        /// Submits the URL.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public bool SubmitUrl(string subject, string url)
        {
            return SubmitUrl(subject, url, null);
        }

        /// <summary>
        /// Prepares the URL.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public bool PrepareUrl(string subject, string url)
        {
            string output = WebScrape.GetUrlResponse(url, 60, null, null);
            return SubmitTemplate(subject, output, false, false);
        }

        /// <summary>
        /// Submits the URL.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="url">The URL.</param>
        /// <param name="postData">The post data.</param>
        /// <returns></returns>
        public bool SubmitUrl(string subject, string url, Dictionary<string, string> postData)
        {
            string output = WebScrape.GetUrlResponse(url, 60, null, postData);
            return SubmitTemplate(subject, output, false);
        }

        /// <summary>
        /// Submits the specified subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="template">The template (filename) originated from /Repository/wim/Config/Mailtemplates/</param>
        /// <returns></returns>
        public bool Submit(string subject, string template)
        {
            string configurationFile =
                string.Concat(Wim.CommonConfiguration.LocalConfigurationFolder, string.Format("/mailtemplates/{0}", template));

            string output = System.IO.File.ReadAllText(configurationFile, System.Text.Encoding.UTF8);

            return SubmitTemplate(subject, output, true);
        }

        /// <summary>
        /// Submits the specified subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="template">The template (filename).</param>
        /// <param name="templatePath">The template relative path.</param>
        /// <returns></returns>
        public bool Submit(string subject, string template, string templatePath)
        {
            string configurationFile = string.Concat(templatePath, template);
            string output = System.IO.File.ReadAllText(configurationFile, System.Text.Encoding.UTF8);

            return SubmitTemplate(subject, output, true);
        }

        /// <summary>
        /// Submits the specified subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="template">The template (filename).</param>
        /// <param name="templatePath">The template relative path.</param>
        /// <param name="SendMail">Actually send the mail.</param>
        /// <returns></returns>
        public bool Submit(string subject, string template, bool SendMail)
        {
            string configurationFile = string.Concat(Wim.CommonConfiguration.LocalConfigurationFolder, string.Format("/mailtemplates/{0}", template)); 
            string output = System.IO.File.ReadAllText(configurationFile, System.Text.Encoding.UTF8);

            return SubmitTemplate(subject, output, true, SendMail);
        }

        bool SubmitTemplate(string subject, string body, bool isLocalConfigurationRelativeFolder)
        {
            return SubmitTemplate(subject, body, isLocalConfigurationRelativeFolder, true);
        }

        protected bool NestImages = true;

        public bool SubmitData(string body, string subject)
        {
            return SubmitTemplate(subject, body, false, true);
        }
        
        public string Subject;
        public string Body;
        /// <summary>
        /// Submits the specified title.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isLocalConfigurationRelativeFolder">if set to <c>true</c> [is local configuration relative folder].</param>
        /// <param name="sendMail">if set to <c>true</c> [send mail] else just prepare it for SendPreparedMail</param>
        /// <returns></returns>
        bool SubmitTemplate(string subject, string body, bool isLocalConfigurationRelativeFolder, bool sendMail)
        {
            Subject = subject;
            Body = body;

            if (m_Replacements != null)
            {
                System.Collections.IDictionaryEnumerator idict = m_Replacements.GetEnumerator();

                while (idict.MoveNext())
                    body = body.Replace(idict.Key.ToString(), idict.Value == null ? null : idict.Value.ToString());
            }

            Link link = new Link();
            link.BaseUrl =  Wim.Utility.GetCurrentHost();
            link.NestImages = NestImages;

            if (isLocalConfigurationRelativeFolder)
                link.BaseFolder = string.Concat(Wim.CommonConfiguration.LocalConfigurationRelativeFolder, "/mailtemplates/");

            Regex rex = new Regex(@"(src="".*?"")", RegexOptions.IgnoreCase);
            body = rex.Replace(body, link.Links);

            Regex rex2 = new Regex(@"(href="".*?"")", RegexOptions.IgnoreCase);
            body = rex2.Replace(body, link.Hrefs);

            MatchCollection collection = rex.Matches(body);

            MailMessage message = new MailMessage();

            if (m_TextData != null)
            {
                AlternateView view3 = AlternateView.CreateAlternateViewFromString(Wim.Utility.CleanFormatting(m_TextData.ToString()), System.Text.Encoding.UTF8, System.Net.Mime.MediaTypeNames.Text.Plain);
                message.AlternateViews.Add(view3);

                AlternateView view2 = AlternateView.CreateAlternateViewFromString(m_TextData.ToString(), System.Text.Encoding.UTF8, System.Net.Mime.MediaTypeNames.Text.RichText);
                message.AlternateViews.Add(view2);
            }

            AlternateView view1 = AlternateView.CreateAlternateViewFromString(body, System.Text.Encoding.UTF8, System.Net.Mime.MediaTypeNames.Text.Html);
            message.AlternateViews.Add(view1);

            if (NestImages)
            {
                if (link.BinaryTable != null)
                {
                    System.Collections.IDictionaryEnumerator idict2 = link.BinaryTable.GetEnumerator();
                    while (idict2.MoveNext())
                    {
                        setResource(idict2.Key.ToString(), string.Concat("img", idict2.Value.ToString()), view1);
                    }
                }
            }

            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = subject;
            message.From = this.m_FromAddress;

            if (mailList == null && mailListBcc == null && mailListCC == null)
            {
                Sushi.Mediakiwi.Data.Notification.InsertOne("Send Formatted Mail", "No recipients added!");
                return false;
            }

            if (mailList != null)
            {
                foreach (MailAddress address in this.mailList)
                    message.To.Add(address);
            }
            if (mailListCC != null)
            {
                foreach (MailAddress address in this.mailListCC)
                    message.CC.Add(address);
            }

            if (mailListBcc != null)
            {
                foreach (MailAddress address in this.mailListBcc)
                    message.Bcc.Add(address);
            }

            if (this.mailAttachmentList != null)
            {
                foreach (Attachment attachment in this.mailAttachmentList)
                    message.Attachments.Add(attachment);
            }

            try
            {
                m_message = message;

                if (sendMail)
                {
                    if (string.IsNullOrEmpty(OverrideMailUrl))
                        Sushi.Mediakiwi.Data.Environment.Current.SmtpClient().Send(m_message);
                    else
                    {
                        var client = Sushi.Mediakiwi.Data.Environment.Current.SmtpClient();
                        client.Host = this.OverrideMailUrl;
                        client.Send(m_message);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Sushi.Mediakiwi.Data.Notification.InsertOne("Send Formatted Mail", ex);
                return false;
            }
        }

        public string OverrideMailUrl { get; set; }

        SmtpClient m_client;
        protected  MailMessage m_message;

        /// <summary>
        /// Resends the mail.
        /// </summary>
        /// <param name="toName">To name.</param>
        /// <param name="toEmail">To email.</param>
        /// <returns></returns>
        public bool SendPreparedMail(string toName, string toEmail)
        {
            if (m_client == null || m_message == null) 
                return false;

            try
            {
                m_message.To.Clear();
                m_message.To.Add(new MailAddress(toEmail, toName));
                m_client.Send(m_message);
                return true;
            }
            catch (Exception ex)
            {
                Sushi.Mediakiwi.Data.Notification.InsertOne("ReSend Formatted Mail", ex);
                return false;
            }
        }

        /// <summary>
        /// Sets the resource.
        /// </summary>
        /// <param name="imagePath">The image path.</param>
        /// <param name="ContId">The cont id.</param>
        /// <param name="htmlView">The HTML view.</param>
        void setResource(string imagePath, string ContId, AlternateView htmlView)
        {
            //CB; fixje voor NLE's mail probleem met wim:images
            imagePath = imagePath.Replace("//", "/");
            imagePath = Wim.Utility.RemApplicationPath(imagePath);

            try
            {

                string file = System.Web.HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Server.UrlDecode(Wim.Utility.AddApplicationPath(imagePath.Replace("/..", string.Empty))));
                if (System.IO.File.Exists(file))
                {
                    LinkedResource resource = new LinkedResource(file);

                    resource.ContentId = ContId;

                    if (imagePath.ToLower().EndsWith(".png"))
                        resource.ContentType = new System.Net.Mime.ContentType("image/png");
                    else if (imagePath.ToLower().EndsWith(".jpg") || imagePath.ToLower().EndsWith(".jpeg"))
                        resource.ContentType = new System.Net.Mime.ContentType("image/jpeg");
                    else if (imagePath.ToLower().EndsWith(".gif"))
                        resource.ContentType = new System.Net.Mime.ContentType("image/gif");

                    htmlView.LinkedResources.Add(resource);
                }
                else
                    Sushi.Mediakiwi.Data.Notification.InsertOne("Mail template", Sushi.Mediakiwi.Data.NotificationType.Warning, string.Format("setResource file not found: '{0}'", file));
            }
            catch (Exception exc)
            {
                Sushi.Mediakiwi.Data.Notification.InsertOne("Mail template", exc);
            }
        }

        string m_HtmlTemplate;

        /// <summary>
        /// Gets or sets the HTML template.
        /// </summary>
        /// <value>The HTML template.</value>
        public string HtmlTemplate
        {
            get { return m_HtmlTemplate; }
            set { m_HtmlTemplate = value; }
        }
    }
}
