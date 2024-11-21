using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using Sushi.MailTemplate.Logic;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SG = SendGrid;

namespace Sushi.MailTemplate.SendGrid
{
    /// <summary>
    /// SendGrid Mailer class to be used in conjunction with Wim.Module.MailTemplate
    /// </summary>
    public class Mailer : ISendPreviewEmailEventHandler
    {
        private readonly MailTemplateHelper _mailTemplateHelper;
        private readonly BlobPersister _blobPersister;
        private readonly QueuePersister _queuePersister;
        private readonly SendGridMailerOptions _sendGridMailerOptions;

        /// <summary>
        /// Mailer Constructor
        /// </summary>
        /// <param name="emailStorageAccount"></param>
        /// <param name="emailBlobContainer"></param>
        /// <param name="emailQueueName"></param>
        /// <param name="sendGridAPIKey"></param>
        public Mailer(MailTemplateHelper mailTemplateHelper, IOptions<SendGridMailerOptions> sendGridMailerOptions, BlobPersister blobPersister, QueuePersister queuePersister)
        {
            _mailTemplateHelper = mailTemplateHelper;
            _blobPersister = blobPersister;
            _queuePersister = queuePersister;
            _sendGridMailerOptions = sendGridMailerOptions.Value;
        }

        

        /// <summary>
        /// QueueMail inserts a queue message and saves the Data.MailTemplate mail in blob storage.
        /// </summary>
        /// <param name="mail">Data.MailTemplate with its properties filled</param>
        /// <param name="emailTo">E-mail address to send to</param>
        /// <param name="customerGUID">Optional customer GUID, which can be set to null</param>
        /// <returns></returns>
        public async Task<bool> QueueMailAsync(Data.MailTemplate mail, string emailTo, Guid? customerGUID)
        {
            // new guid to use as identifier for blob and queue
            var id = Guid.NewGuid();

            (BlobClient blob, QueueClient queue, string email) = await GetBlobQueueAndEmailAsync(mail, emailTo, id, customerGUID);

            // upload to blob
            using (var ms = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(ms);
                writer.Write(email);
                writer.Flush();
                ms.Position = 0;
                await blob.UploadAsync(ms);
            }

            // queue message only has id as reference to blob
            var queueMessage = id.ToString();

            // add queue message
            await queue.SendMessageAsync(queueMessage);

            return true;
        }


        private async Task<(BlobClient blob, QueueClient queue, string email)> GetBlobQueueAndEmailAsync(Data.MailTemplate mail, string emailTo, Guid id, Guid? customerGUID)
        {
            string emailFromName = mail.DefaultSenderName;
            string emailFrom = mail.DefaultSenderEmail;
            string subject = mail.Subject;
            string body = mail.Body;
            string templateName = mail.Identifier;
            string bccs = mail.BCCReceivers;

            // get blob information
            var blob = await _blobPersister.GetBlobClientAsync(_sendGridMailerOptions.BlobContainer, id.ToString());

            // create email message to save to blob
            var email = new Email
            {
                ID = id,
                FromName = emailFromName,
                From = emailFrom,
                To = emailTo,
                Subject = subject,
                Body = body,
                Bccs = bccs,
                TemplateName = templateName,
                CustomerGUID = customerGUID
            };
            var jsonEmail = Newtonsoft.Json.JsonConvert.SerializeObject(email);

            // create queue information            
            var queue = await _queuePersister.GetQueueAsync(_sendGridMailerOptions.QueueName);

            return (blob, queue, jsonEmail);
        }

        /// <summary>
        /// Sends the e-mail from the provided email. Does not clean up blob like SendMailFromBlobAsync, you should do this yourself.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> SendMailAsync(Email email)
        {
            if (string.IsNullOrEmpty(_sendGridMailerOptions.ApiKey))
            {
                throw new ApplicationException("SendGridAPIKey is empty");
            }

            string emailFrom = email.From?.Trim();
            string emailFromName = HttpUtility.HtmlDecode(email.FromName);
            string emailTo = email.To?.Trim();
            string subject = email.Subject; // already encoded in apply placeholders
            string body = email.Body; // already encoded in apply placeholders
            string bccs = email.Bccs;
            
            Guid? customerGuid = email.CustomerGUID;

            var client = new SG.SendGridClient(_sendGridMailerOptions.ApiKey);
            
            var message = SG.Helpers.Mail.MailHelper.CreateSingleEmail(
                new SG.Helpers.Mail.EmailAddress(emailFrom, emailFromName),
                new SG.Helpers.Mail.EmailAddress(emailTo),
                subject,
                string.Empty,
                body
                );

            if (!string.IsNullOrWhiteSpace(bccs))
            {
                var bccAddresses = bccs.Split(';', StringSplitOptions.RemoveEmptyEntries).Distinct();
                
                foreach (var bcc in bccAddresses)
                {
                    // remove whitespace at start/end
                    string bccAddress = bcc.Trim();

                    // make sure it is not same as 'email to', because than it will be rejected by sendgrid
                    if (bccAddress.Equals(emailTo, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    
                    message.AddBcc(new SG.Helpers.Mail.EmailAddress(bccAddress));
                }
            }

            // we are only interested in mail events from mails to customers. If we don't have a customer guid, 
            // then we also do not need to send the message id
            if (customerGuid.HasValue)
            {
                message.CustomArgs = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "CustomerGUID", customerGuid.ToString() },
                    { "MessageGUID", email.ID.ToString() }
                };
            }

            var result = await client.SendEmailAsync(message).ConfigureAwait(false);

            if (result.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return true;
            }
            else
            {
                // reading response https://github.com/sendgrid/sendgrid-csharp/blob/master/USAGE.md#post-mailsend
                var returnBody = string.Empty;
                if (result.Body != null)
                {
                    returnBody = await result.Body.ReadAsStringAsync();
                }
                throw new ApplicationException($"Sendgrid returned status code {result.StatusCode}, for {returnBody} with headers {result.Headers?.ToString()}");
            }

        }

        /// <summary>
        /// Use SendMailAsync in an Azure Function that is triggered by a queue message. It expects an id to a blob.
        /// </summary>
        /// <param name="id">id of blob</param>
        public async Task<bool> SendMailFromBlobAsync(string id)
        {
            if (string.IsNullOrEmpty(_sendGridMailerOptions.ApiKey))
            {
                throw new ApplicationException("SendGridAPIKey is empty");
            }
            
            // get blob information
            var blob = await _blobPersister.GetBlobClientAsync(_sendGridMailerOptions.BlobContainer, id.ToString());

            // check if the blob exists
            if (await blob.ExistsAsync())
            {
                var text = await blob.DownloadContentAsync();

                var email = Newtonsoft.Json.JsonConvert.DeserializeObject<Email>(text.Value.Content.ToString());

                string emailFrom = email.From;
                string emailFromName = HttpUtility.HtmlDecode(email.FromName);
                string emailTo = email.To;
                string subject = email.Subject;
                string body = email.Body;
                string bccs = email.Bccs;
                string templateName = email.TemplateName;
                Guid? customerGuid = email.CustomerGUID;

                var client = new SG.SendGridClient(_sendGridMailerOptions.ApiKey);

                var message = SG.Helpers.Mail.MailHelper.CreateSingleEmail(
                    new SG.Helpers.Mail.EmailAddress(emailFrom, emailFromName),
                    new SG.Helpers.Mail.EmailAddress(emailTo),
                    subject,
                    string.Empty,
                    body
                    );

                if (!string.IsNullOrWhiteSpace(bccs))
                {
                    foreach (var bcc in bccs.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        message.AddBcc(new SG.Helpers.Mail.EmailAddress(bcc));
                    }
                }

                // we are only interested in mail events from mails to customers. If we don't have a customer guid, 
                // then we also do not need to send the message id
                if (customerGuid.HasValue)
                {
                    message.CustomArgs = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "CustomerGUID", customerGuid.ToString() },
                        { "MessageGUID", id }
                    };
                }

                var result = await client.SendEmailAsync(message).ConfigureAwait(false);

                if (result.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    // clean up the blob
                    await blob.DeleteAsync();
                    return true;
                }
                else
                {
                    // reading response https://github.com/sendgrid/sendgrid-csharp/blob/master/USAGE.md#post-mailsend
                    var returnBody = string.Empty;
                    if (result.Body != null)
                    {
                        try
                        {
                            returnBody = await result.Body.ReadAsStringAsync();
                        }
                        catch (Exception)
                        {
                            // leave the returnBody empty
                        }
                    }

                    throw new ApplicationException($"Sendgrid returned status code {result.StatusCode}, for {returnBody} with headers {result.Headers?.ToString()}");
                }
            }
            else
            {
                // blob doesn't exist
                throw new ApplicationException($"Blob {id} not found");
            }
        }

        public async Task SendPreviewEmailAsync(SendPreviewEmailEventArgs e)
        {
            var emailFrom = e.EmailFrom;
            var emailTo = e.EmailTo;
            

            var emailToSend = new Email
            {
                From = emailFrom,
                FromName = e.MailTemplate.DefaultSenderName,
                TemplateName = e.MailTemplate.Name,
                Body = e.MailTemplate.Body,
                Subject = e.MailTemplate.Subject,
                To = emailTo
            };

            e.IsSuccess = await SendMailAsync(emailToSend);
        }
    }
}
