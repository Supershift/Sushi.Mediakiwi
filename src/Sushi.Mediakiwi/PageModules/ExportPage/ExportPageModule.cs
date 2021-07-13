using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.PageModules.ExportPage
{
    public class ExportPageModule : IPageModule
    {

        public ExportPageModule()
        {
            IconClass = "icon-export";
            Tooltip = "Export this page";
            //IconURL = "https://cdn0.iconfinder.com/data/icons/typicons-2/24/export-outline-32.png";
            //ConfirmationNeeded = true;
            //ConfirmationQuestion = "Hiermee wordt de opgeslagen pagina geëxporteerd, dit kan enige ogenblikken duren.";
            //ConfirmationTitle = "Weet u het zeker ?";
        }

        public string IconClass { get; set; }
        public string IconURL { get; set; }
        public string Tooltip { get; set; }
        public bool ConfirmationNeeded { get; set; }
        public string ConfirmationTitle { get; set; }
        public string ConfirmationQuestion { get; set; }

        public async Task<ModuleExecutionResult> ExecuteAsync(Page inPage, IApplicationUser inUser, HttpContext context)
        {
            PageTransferExporter exporter = new PageTransferExporter();

            var exportedPage = await exporter.ExportPageAsync(inPage);
            string base64String;

            exportedPage.ExportedOn = DateTime.UtcNow.Ticks;
            exportedPage.ExportedBy = inUser.Displayname;

            try
            {
                base64String = EncodeTo64(Newtonsoft.Json.JsonConvert.SerializeObject(exportedPage, Newtonsoft.Json.Formatting.Indented));
            }
            catch (Exception ex)
            {
                return new ModuleExecutionResult()
                {
                    IsSuccess = false,
                    WimNotificationOutput = $"Something went wrong exporting the page.<br/>{ex.Message}"
                };
            }

            string fileName = $"{Utility.CleanUrl(inPage.Name)}#{inPage.ID}.json";

            ModuleExecutionResult temp = new ModuleExecutionResult()
            {
                IsSuccess = true,
                WimNotificationOutput = $"Download the exported page : <strong><a href=\"data:application/json;base64,{base64String}\" download=\"{fileName}\">{fileName}</a></strong>"
            };

            // This does not work in netcore, the page output will be added to the json output.
            //context.Response.ContentType = "application/json";
            //context.Response.Headers.Add($"content-disposition", $"attachment; filename={Utility.CleanUrl(inPage.Name)}#{inPage.ID}.json");
            //using (var fs = await GenerateStreamFromStringAsync(Newtonsoft.Json.JsonConvert.SerializeObject(exportedPage, Newtonsoft.Json.Formatting.Indented)).ConfigureAwait(false))
            //{
            //    await fs.CopyToAsync(context.Response.Body).ConfigureAwait(false);
            //}
            
            //await context.Response.Body.FlushAsync().ConfigureAwait(false);

            return temp;
        }

        public bool ShowOnPage(Page inPage, IApplicationUser inUser)
        {
            if (inPage?.MasterID.GetValueOrDefault(0) > 0)
                return false;
            return true;
        }

        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }
    }
}
