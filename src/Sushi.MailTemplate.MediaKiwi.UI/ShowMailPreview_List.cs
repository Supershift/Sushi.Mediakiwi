using Sushi.MailTemplate.Data;
using Sushi.Mediakiwi.Framework;
using System.Threading.Tasks;
using System.Web;

namespace Sushi.MailTemplate.MediaKiwi.UI
{
    /// <summary>
    /// ShowMailPreview_List Component List
    /// </summary>
    public class ShowMailPreview_List : ComponentListTemplate
    {
        private readonly MailTemplateRepository _mailTemplateRepository;

        /// <summary>
        /// ShowMailPreview_List ctor
        /// </summary>
        public ShowMailPreview_List(MailTemplateRepository mailTemplateRepository)
        {
            wim.CanContainSingleInstancePerDefinedList = true;
            wim.OpenInEditMode = true;
            ListLoad += ShowMailPreview_ListLoad;
            _mailTemplateRepository = mailTemplateRepository;
        }

        private async Task ShowMailPreview_ListLoad(ComponentListEventArgs e)
        {
            var mailTemplateKey = Context.Request.Query["MailTemplateID"].ToString();
            if (!string.IsNullOrEmpty(mailTemplateKey))
            {
                var mailTemplate = await _mailTemplateRepository.FetchSingleAsync(int.Parse(mailTemplateKey));

                var body = HttpUtility.HtmlDecode(mailTemplate.Body);

                //with the mail charset an Â shows instead of &nbsp: http://stackoverflow.com/questions/1461907/html-encoding-issues-%C3%82-character-showing-up-instead-of-nbsp
                body = body?.Replace(@"<meta http-equiv=""Content-Type"" content=""text/html; charset=ISO-8859-1"" />", @"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"" />");

                var subject = mailTemplate.Subject;

                var text = $"<div style='margin-left:10px'><h3 style='margin-left: 50px'>{subject}</h3><p>{body}</p></div>";
                var data = System.Text.Encoding.UTF8.GetBytes(text);
                await Response.Body.WriteAsync(data, 0, data.Length);
            }
        }
    }
}
