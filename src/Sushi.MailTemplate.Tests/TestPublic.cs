using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.MailTemplate.Extensions;
using Sushi.MailTemplate.SendGrid;
using Sushi.Mediakiwi.MicroORM;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.MailTemplate.Tests
{
    [TestClass]
    public class TestPublic
    {
        static IConfigurationRoot Configuration;
        static IServiceProvider ServiceProvider;

        private static string TEST_SENDER_EMAIL = "[YOUR_TEST_EMAIL_ADDRESS]";
        private static string TEST_SENDER_NAME = "[YOUR_TEST_NAME]";
        private static string TEST_RECEIVER_EMAIL = "[YOUR_TEST_EMAIL_ADDRESS]";
        private static string TEST_USER_NAME = "[YOUR_TEST_NAME]";
        private static string TEST_USER_EMAIL = "[YOUR_TEST_EMAIL_ADDRESS]";
        private static string TEST_TEMPLATE_ID = "INTERNAL-TEST";
        private static string TEST_TEMPLATE_ID_GROUPS = "INTERNAL-TEST-GROUPS";
        private static string TEST_TEMPLATE_ID_SECTIONS = "INTERNAL-TEST-SECTIONS";
        private static string TEST_QUEUED_MAIL_ID = "[YOUR_QUEUED_EMAIL_IDENTIFIER]";

        private static string[] TEST_TEMPLATES = new string[] 
        { 
            TEST_TEMPLATE_ID, 
            TEST_TEMPLATE_ID_GROUPS, 
            TEST_TEMPLATE_ID_SECTIONS 
        };
        private readonly MailTemplateHelper _mailTemplateHelper;
        private readonly Data.MailTemplateRepository _mailTemplateRepository;
        private readonly Data.MailTemplateListRepository _mailTemplateListRepository;
        private readonly Mailer _mailer;

        [AssemblyInitialize]
        public static async Task AssemblyInitialize(TestContext context)
        {
            Configuration = new ConfigurationBuilder()                                                                                          
                                .AddUserSecrets<TestPublic>()
                                .AddEnvironmentVariables()
                                .Build();


            IServiceCollection services = new ServiceCollection();

            string defaultConnectionString = Configuration.GetConnectionString("datastore");
            services.AddMicroORM(defaultConnectionString);
            
            services.AddSushiMailTemplate();
            services.AddSushiMailTemplateSendgrid(options=>
            {
                options.AzureStorageAccount = Configuration.GetConnectionString("azurestore");                
                options.ApiKey = Configuration["SendGrid:ApiKey"];
            });
            ServiceProvider = services.BuildServiceProvider();
        }

        public TestPublic()
        {
            _mailTemplateHelper = ActivatorUtilities.CreateInstance<MailTemplateHelper>(ServiceProvider);
            _mailTemplateRepository = ActivatorUtilities.CreateInstance<Data.MailTemplateRepository>(ServiceProvider);
            _mailTemplateListRepository = ActivatorUtilities.CreateInstance<Data.MailTemplateListRepository>(ServiceProvider);
            _mailer = ActivatorUtilities.CreateInstance<Mailer>(ServiceProvider);
        }

        [TestMethod]
        public async Task QueueMail()
        {   
            var mail = new Data.MailTemplate
            {
                DefaultSenderEmail = TEST_SENDER_EMAIL,
                DefaultSenderName = TEST_SENDER_NAME,
                Subject = "Test",
                Body = "body",
                Identifier = TEST_TEMPLATE_ID
            };

            var result = await _mailer.QueueMailAsync(mail, emailTo: TEST_RECEIVER_EMAIL, customerGUID: null);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task PublishAllTestTemplates()
        {
            foreach (var item in TEST_TEMPLATES)
            {
                var mailTemplate = await _mailTemplateHelper.FetchAsync(item, false);
                if (mailTemplate?.ID > 0)
                {
                    await _mailTemplateRepository.SaveAsync(mailTemplate, mailTemplate.UserID.Value, TEST_USER_NAME, TEST_USER_EMAIL);
                    await _mailTemplateRepository.PublishAsync(mailTemplate, mailTemplate.UserID.Value, TEST_USER_NAME, TEST_USER_EMAIL);
                }
                else 
                {
                    Assert.Fail($"Mailtemplate '{item}' wasn't found");
                }
            }
            
        }

        [TestMethod]
        public async Task SendMailFromBlob()
        {
            try
            {
                var id = TEST_QUEUED_MAIL_ID;
                
                var result = await _mailer.SendMailFromBlobAsync(id);

                Assert.IsTrue(result);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [TestMethod]
        public async Task SendMailAsync()
        {
            try
            {
                var template = await _mailTemplateHelper.FetchAsync(TEST_TEMPLATE_ID);

                template.PlaceholderList.Add("PREVIEWTEXT", "TEST: This is the previewtext");
                template.PlaceholderList.Add("INTRO", "TEST: This is the intro text");
                template.PlaceholderList.Add("BUTTONLINK", "https://www.google.nl");
                template.PlaceholderList.Add("BUTTONLABEL", "TEST: This is the Button label");
                template.PlaceholderList.Add("OUTRO", "TEST: This is the outro text");
                template.PlaceholderList.Add("FOOTERTITLE", "TEST: This is the footer title");

                template.PlaceholderGroupList.Add("REPEATER");
                template.PlaceholderGroupList.AddNewRow();
                template.PlaceholderGroupList.AddNewRowItem("REPEATEDPLACEHOLDER", "TEST: This is a repeated placeholder 1");
                template.PlaceholderGroupList.AddNewRow();
                template.PlaceholderGroupList.AddNewRowItem("REPEATEDPLACEHOLDER", "TEST: This is a repeated placeholder 2");
                template.PlaceholderGroupList.AddNewRow();
                template.PlaceholderGroupList.AddNewRowItem("REPEATEDPLACEHOLDER", "TEST: This is a repeated placeholder 3");

                template = await _mailTemplateHelper.ApplyPlaceholdersAsync(template, Console.Out);

                var email = new Email
                {
                    Body = template.Body,
                    From = template.DefaultSenderEmail,
                    FromName = template.DefaultSenderName,
                    To = TEST_RECEIVER_EMAIL,
                    ID = Guid.NewGuid(),
                    TemplateName = template.Identifier,
                    Subject = template.Subject
                };

                var result = await _mailer.SendMailAsync(email);

                Assert.IsTrue(result);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [TestMethod]
        public async Task GetTemplateAndApplyPlaceholders()
        {
            var template = await _mailTemplateHelper.FetchAsync(TEST_TEMPLATE_ID);

            if (template?.ID > 0)
            {
                template.PlaceholderList.Add("PREVIEWTEXT", "TEST: This is the previewtext");
                template.PlaceholderList.Add("INTRO", "TEST: This is the intro text");
                template.PlaceholderList.Add("BUTTONLINK", "https://www.google.nl");
                template.PlaceholderList.Add("BUTTONLABEL", "TEST: This is the Button label");
                template.PlaceholderList.Add("OUTRO", "TEST: This is the outro text");
                template.PlaceholderList.Add("FOOTERTITLE", "TEST: This is the footer title");

                var mail = await _mailTemplateHelper.ApplyPlaceholdersAsync(template, Console.Out);

                Console.WriteLine(mail);

                Assert.IsFalse(string.IsNullOrWhiteSpace(mail.Body));
            }
            else
            {
                Assert.Fail($"Mailtemplate '{TEST_TEMPLATE_ID}' wasn't found");
            }
        }

        [TestMethod]
        public async Task GetTemplateAndApplyPlaceholdersWithGroups()
        {
            var template = await _mailTemplateHelper.FetchAsync(TEST_TEMPLATE_ID_GROUPS);

            if (template?.ID > 0)
            {
                template.PlaceholderList.Add("PREVIEWTEXT", "TEST: This is the previewtext");
                template.PlaceholderList.Add("INTRO", "TEST: This is the intro text");
                template.PlaceholderList.Add("BUTTONLINK", "https://www.google.nl");
                template.PlaceholderList.Add("BUTTONLABEL", "TEST: This is the Button label");
                template.PlaceholderList.Add("OUTRO", "TEST: This is the outro text");
                template.PlaceholderList.Add("FOOTERTITLE", "TEST: This is the footer title");

                template.PlaceholderGroupList.Add("REPEATER");
                template.PlaceholderGroupList.AddNewRow();
                template.PlaceholderGroupList.AddNewRowItem("REPEATEDPLACEHOLDER", "TEST: This is a repeated placeholder 1");
                template.PlaceholderGroupList.AddNewRow();
                template.PlaceholderGroupList.AddNewRowItem("REPEATEDPLACEHOLDER", "TEST: This is a repeated placeholder 2");
                template.PlaceholderGroupList.AddNewRow();
                template.PlaceholderGroupList.AddNewRowItem("REPEATEDPLACEHOLDER", "TEST: This is a repeated placeholder 3");

                var mail = await _mailTemplateHelper.ApplyPlaceholdersAsync(template, Console.Out);

                Console.WriteLine(mail.Body);

                Assert.IsFalse(string.IsNullOrWhiteSpace(mail.Body));
            }
            else
            {
                Assert.Fail($"Mailtemplate '{TEST_TEMPLATE_ID_GROUPS}' wasn't found");
            }
        }

        [TestMethod]
        public async Task GetTemplateAndApplyPlaceholdersWithSections()
        {
            var template = await _mailTemplateHelper.FetchAsync(TEST_TEMPLATE_ID_SECTIONS);

            if (template?.ID > 0)
            {
                template.PlaceholderList.Add("PREVIEWTEXT", "TEST: This is the previewtext");
                template.PlaceholderList.Add("INTRO", "TEST: This is the intro text");
                template.PlaceholderList.Add("BUTTONLINK", "https://www.google.nl");
                template.PlaceholderList.Add("BUTTONLABEL", "TEST: This is the Button label");
                template.PlaceholderList.Add("OUTRO", "TEST: This is the outro text");
                template.PlaceholderList.Add("FOOTERTITLE", "TEST: This is the footer title");
                template.PlaceholderList.Add("SECTIONNAME", "TEST: This is the optional section name");
                
                template.OptionalSections.Add("OPTIONALSECTION");

                var mail = await _mailTemplateHelper.ApplyPlaceholdersAsync(template, Console.Out);

                Console.WriteLine(mail.Body);

                Assert.IsFalse(string.IsNullOrWhiteSpace(mail.Body));
            }
            else
            {
                Assert.Fail($"Mailtemplate '{TEST_TEMPLATE_ID_SECTIONS}' wasn't found");
            }
        }

        [TestMethod]
        public async Task GetSectionTags()
        {
            var mailTemplate = await _mailTemplateHelper.FetchAsync(TEST_TEMPLATE_ID_SECTIONS);
            var sections = Logic.PlaceholderLogic.GetSectionTags(mailTemplate.Body, logger: Console.Out);
            
            Assert.IsTrue(sections.Any());
        }

        [TestMethod]
        public async Task DeleteMailTemplate()
        {
            var template = await _mailTemplateHelper.FetchAsync("TEMPLATE1");
            var deleted = await _mailTemplateRepository.DeleteAsync([template.ID]);

            Assert.IsTrue(deleted);
        }

        [TestMethod]
        public async Task MailTemplateListFetchAll()
        {
            var items = await _mailTemplateListRepository.FetchAllAsync();

            Assert.IsTrue(items.Any());
        }


        [TestMethod]
        public async Task FetchAllByIdentifiers()
        {
            var mailTemplates = await _mailTemplateRepository.FetchAllByIdentifiersAsync(TEST_TEMPLATES, false);

            bool containsFirst = mailTemplates.Any(x => x.Identifier == TEST_TEMPLATE_ID);
            bool containsSecond = mailTemplates.Any(x => x.Identifier == TEST_TEMPLATE_ID_GROUPS);
            bool containsThird = mailTemplates.Any(x => x.Identifier == TEST_TEMPLATE_ID_SECTIONS);

            Assert.IsTrue(containsFirst && containsSecond && containsThird);
        }
    }
}