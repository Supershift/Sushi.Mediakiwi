using System.Collections.Generic;
using System.Linq;
using Sushi.MailTemplate.Entities;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Framework;
using Sushi.MailTemplate.Data;
using Sushi.MailTemplate.Logic;

namespace Sushi.MailTemplate.MediaKiwi.UI
{
    /// <summary>
    /// SendTestMail_List Component List
    /// </summary>
    public class SendTestMail_List : ComponentListTemplate
    {
        private readonly MailTemplateRepository _mailTemplateRepository;
        private readonly PlaceholderLogic _placeholderLogic;
        private readonly ISendPreviewEmailEventHandler _sendPreviewEmailEventHandler;

        /// <summary>
        /// 
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// SendTestMail_List ctor
        /// </summary>
        public SendTestMail_List(MailTemplateRepository mailTemplateRepository, PlaceholderLogic placeholderLogic,  ISendPreviewEmailEventHandler sendPreviewEmailEventHandler = null)
        {
            wim.OpenInEditMode = true;
            wim.CanContainSingleInstancePerDefinedList = true;
            if (wim.CurrentList != null) wim.CurrentList.Label_Save = "Send";

            ListLoad += SendTestMail_List_ListLoad;
            ListSave += SendTestMail_List_ListSave;
            ListPreRender += SendTestMail_List_ListPreRender;
            _mailTemplateRepository = mailTemplateRepository;
            _placeholderLogic = placeholderLogic;
            _sendPreviewEmailEventHandler = sendPreviewEmailEventHandler;
        }

        private async Task SendTestMail_List_ListPreRender(ComponentListEventArgs e)
        {
            var idQueryString = Request.Query["item"].ToString();

            int.TryParse(idQueryString, out int id);
            var mailTemplate = await _mailTemplateRepository.FetchSingleAsync(id);
            EmailFrom ??= mailTemplate.DefaultSenderEmail;
            EmailTo ??= wim.CurrentApplicationUser.Email;

            if (string.IsNullOrWhiteSpace(mailTemplate.Subject))
            {
                wim.Notification.AddError("A subject is mandatory, please supply a subject before sending a test e-mail. E-mail can't be sent.");
            }
        }

        private async Task SendTestMail_List_ListSave(ComponentListEventArgs e)
        {
            var idQueryString = Request.Query["item"].ToString();

            int.TryParse(idQueryString, out int id);
            await SendTestMailAsync(id);
        }

        /// <summary>
        /// Send the current mail template with the provided values to the handler
        /// </summary>
        /// <param name="id"></param>
        public async Task SendTestMailAsync(int id)
        {
            if (_sendPreviewEmailEventHandler == null)
            {
                wim.CurrentVisitor.Data.Apply("wim.note", $"No implementation for {nameof(ISendPreviewEmailEventHandler)} supplied, cannot send test mail");
                await wim.CurrentVisitor.SaveAsync();
                return;
            }

            var mailTemplate = await _mailTemplateRepository.FetchSingleAsync(id);

            var placeholderSubject = CreatePlaceholderObject(mailTemplate.Subject);
            var placeholderBody = CreatePlaceholderObject(mailTemplate.Body);
            var placeholderGroupSubject = CreatePlaceholderGroupsObject(mailTemplate.Subject);
            var placeholderGroupBody = CreatePlaceholderGroupsObject(mailTemplate.Body);

            var sectionBody = CreateSectionObject(mailTemplate.Body);

            mailTemplate = await _placeholderLogic.ApplyPlaceholdersAsync(mailTemplate, placeholderGroupBody, placeholderBody, sectionBody);

            var body = mailTemplate.Body;
            var subject = mailTemplate.Subject;

            if (string.IsNullOrWhiteSpace(subject))
            {
                // not possible to send an e-mail without a subject
                wim.CurrentVisitor.Data.Apply("wim.note", "A subject is mandatory, please supply a subject before sending a test e-mail. E-mail wasn't sent.");
                await wim.CurrentVisitor.SaveAsync();
                return;
            }

            
            var e = new SendPreviewEmailEventArgs { EmailFrom = EmailFrom, EmailTo = EmailTo, MailTemplate = mailTemplate, EmailFromName = mailTemplate.DefaultSenderName };
            
            await _sendPreviewEmailEventHandler.SendPreviewEmailAsync(e);

            if (e.IsSuccess)
            {
                // send successfull
                wim.CurrentVisitor.Data.Apply("wim.note", "E-mail has been sent successfully");
                await wim.CurrentVisitor.SaveAsync();
            }
            else
            {
                wim.CurrentVisitor.Data.Apply("wim.note", $"E-mail wasn't sent properly, see '{nameof(SendTestMail_List)}' notifications for more details");
                
                await Mediakiwi.Data.Notification.InsertOneAsync(nameof(SendTestMail_List), e.ErrorMessage);
                await wim.CurrentVisitor.SaveAsync();
            }
        }

        private async Task SendTestMail_List_ListLoad(ComponentListEventArgs e)
        {
            var idQueryString = Request.Query["item"].ToString();

            int.TryParse(idQueryString, out int id);

            var mailTemplate = await _mailTemplateRepository.FetchSingleAsync(id);

            var placeholdersSubject = PlaceholderLogic.GetPlaceholderTags(mailTemplate.Subject);
            foreach (var placeholder in placeholdersSubject)
            {
                wim.Form.AddElement(this, placeholder, true, true,
                    new Mediakiwi.Framework.ContentListItem.TextFieldAttribute(placeholder, 4000, true));
            }

            var placeholdersBody = PlaceholderLogic.GetPlaceholderTags(mailTemplate.Body);
            foreach (var placeholder in placeholdersBody)
            {
                wim.Form.AddElement(this, placeholder, true, true,
                    new Mediakiwi.Framework.ContentListItem.TextFieldAttribute(placeholder, 4000, true));
            }

            var sections = PlaceholderLogic.GetSectionTags(mailTemplate.Body);
            foreach (var section in sections)
            {
                wim.Form.AddElement(this, section, true, true,
                    new Mediakiwi.Framework.ContentListItem.Choice_CheckboxAttribute(section, false));
            }
        }

        private List<Placeholder> CreatePlaceholderObject(string textWithTags)
        {
            var result = new List<Placeholder>();
            var placeholderTags = PlaceholderLogic.GetPlaceholderTags(textWithTags);

            foreach (var placeholderTag in placeholderTags)
            {
                var placeholders = GetPlaceholdersFromRequest(placeholderTag);
                result.AddRange(placeholders);
            }

            return result;
        }

        private List<string> CreateSectionObject(string textWithTags)
        {
            var result = new List<string>();
            var sectionTags = PlaceholderLogic.GetSectionTags(textWithTags);

            foreach (var sectionTag in sectionTags)
            {
                var sections = GetSectionsFromRequest(sectionTag);
                result.AddRange(sections);
            }

            return result;
        }

        private List<PlaceholderGroup> CreatePlaceholderGroupsObject(string textWithTags)
        {
            var placeholderGroups = PlaceholderLogic.GetPlaceholderGroupsWithPlaceholders(textWithTags);

            foreach (var placeholderGroup in placeholderGroups)
            {
                // repeat 3 times so the test e-mail has some content
                for (int i = 0; i < 3; i++)
                {
                    placeholderGroup.AddNewRow();

                    foreach (var placeholderTag in placeholderGroup.PlaceholderTags)
                    {
                        var dict = GetPlaceholdersFromRequest(placeholderTag);

                        foreach (var item in dict)
                        {
                            placeholderGroup.AddNewRowItem(item.Name, $"{item.Value}");
                        }
                    }
                }
            }

            return placeholderGroups;
        }

        private List<Placeholder> GetPlaceholdersFromRequest(string fieldIdentifier)
        {
            var placeholders = new List<Placeholder>();
            if (Request != null && Request.Form != null && Request.Form.Keys != null && !string.IsNullOrEmpty(fieldIdentifier))
            {
                foreach (var formKey in Request.Form.Keys.Where(x => x.EndsWith(fieldIdentifier)))
                {
                    string identifier = formKey.Split(['_'], 2).LastOrDefault();
                    if (!string.IsNullOrEmpty(identifier))
                    {
                        string value = Request.Form[formKey];
                        // for test purposes, if supplied with "null", make it a real null so testing with defaults is possible
                        if (value.Equals("null", System.StringComparison.OrdinalIgnoreCase))
                        {
                            value = null;
                        }

                        placeholders.Add(new Placeholder(identifier, value));
                    }
                }
            }

            return placeholders;
        }

        private List<string> GetSectionsFromRequest(string fieldIdentifier)
        {
            var sections = new List<string>();
            if (Request != null && Request.Form != null && Request.Form.Keys != null && !string.IsNullOrEmpty(fieldIdentifier))
            {
                foreach (var formKey in Request.Form.Keys.Where(x => x.EndsWith(fieldIdentifier)))
                {
                    string identifier = formKey.Split(['_'], 2).LastOrDefault();
                    if (!string.IsNullOrEmpty(identifier))
                    {
                        string value = Request.Form[formKey];

                        // a checkbox returns on
                        if (value.Equals("1", System.StringComparison.OrdinalIgnoreCase))
                        {
                            sections.Add(identifier);
                        }
                    }
                }
            }

            return sections;
        }

        /// <summary>
        /// Explanation of this window
        /// </summary>
        [Mediakiwi.Framework.ContentListItem.TextLine("")]
        public string Title
        {
            get
            {
                return @"Use this page to send a test e-mail to the e-mail address provided. Fill in the placeholders below.
Leave empty to fill with default values. Fill in with ""null"" to remove from the template.";
            }
        }

        /// <summary>
        /// E-mail textbox
        /// </summary>
        [Mediakiwi.Framework.ContentListItem.TextField("Email To", 255, IsRequired = true, Mandatory = true)]
        public string EmailTo { get; set; }
        /// <summary>
        /// E-mail textbox
        /// </summary>
        [Mediakiwi.Framework.ContentListItem.TextField("Email From", 255, IsRequired = true, Mandatory = true)]
        public string EmailFrom { get; set; }

    }
}
