using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.PageModules.ExportPage
{
    public class ExportPageModule : IPageModule
    {

        public ExportPageModule()
        {
            IconClass = "icon-export";
            Tooltip = "Exporteer deze pagina";
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
            string base64String = "";

            try
            {
                base64String = EncodeTo64(Newtonsoft.Json.JsonConvert.SerializeObject(exportedPage, Newtonsoft.Json.Formatting.Indented));
            }
            catch (Exception ex)
            {
                return new ModuleExecutionResult()
                {
                    IsSuccess = false,
                    WimNotificationOutput = $"Er ging iets mis bij het exporteren.<br/>{ex.Message}"
                };
            }

            ModuleExecutionResult temp = new ModuleExecutionResult()
            {
                IsSuccess = true,
                WimNotificationOutput = $"Download hier de geëxporteerde pagina : <strong><a href=\"data:application/json;base64,{base64String}\" download=\"{inPage.GUID}.json\">Download {inPage.GUID}.json</a></strong>"
            };

            exportedPage.ExportedOn = DateTime.UtcNow.Ticks;
            exportedPage.ExportedBy = inUser.Displayname;

            context.Response.Headers.Add($"content-disposition", $"attachment; filename={Utility.CleanUrl(inPage.Name)}#{inPage.ID}.json");
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(exportedPage, Newtonsoft.Json.Formatting.Indented));

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
