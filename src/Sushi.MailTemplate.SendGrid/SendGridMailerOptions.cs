namespace Sushi.MailTemplate.SendGrid
{
    public class SendGridMailerOptions
    {
        /// <summary>
        /// Name of the blob container used for sending mails. Defaults to 'sendgrid'.
        /// </summary>
        public string BlobContainer { get; set; } = "sendgrid";
        
        /// <summary>
        /// Name of the queue used for triggering mails. Defaults to 'sendgrid'.
        /// </summary>
        public string QueueName { get; set; } = "sendgrid";

        public string AzureStorageAccount { get; set; }

        public string ApiKey { get; set; }
    }
}
