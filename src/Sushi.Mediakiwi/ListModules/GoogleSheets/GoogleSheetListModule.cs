using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Framework.Interfaces;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.ListModules.GoogleSheets
{
    public class GoogleSheetListModule : IListModule
    {
        public GoogleSheetListModule()
        {
            ShowInSearchMode = true;
            ShowInEditMode = false;
            Tooltip = "View this list in Google Sheets";
            IconURL = "data:image/png;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iaXNvLTg4NTktMSI/Pg0KPCEtLSBHZW5lcmF0b3I6IEFkb2JlIElsbHVzdHJhdG9yIDE4LjAuMCwgU1ZHIEV4cG9ydCBQbHVnLUluIC4gU1ZHIFZlcnNpb246IDYuMDAgQnVpbGQgMCkgIC0tPg0KPCFET0NUWVBFIHN2ZyBQVUJMSUMgIi0vL1czQy8vRFREIFNWRyAxLjEvL0VOIiAiaHR0cDovL3d3dy53My5vcmcvR3JhcGhpY3MvU1ZHLzEuMS9EVEQvc3ZnMTEuZHRkIj4NCjxzdmcgdmVyc2lvbj0iMS4xIiBpZD0iQ2FwYV8xIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHhtbG5zOnhsaW5rPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5L3hsaW5rIiB4PSIwcHgiIHk9IjBweCINCgkgdmlld0JveD0iMCAwIDIwNC4zNzYgMjA0LjM3NiIgc3R5bGU9ImVuYWJsZS1iYWNrZ3JvdW5kOm5ldyAwIDAgMjA0LjM3NiAyMDQuMzc2OyIgeG1sOnNwYWNlPSJwcmVzZXJ2ZSI+DQo8cGF0aCBkPSJNMTcxLjI0NywyMDQuMzc2YzIuNDg1LDAsNC41LTIuMDE1LDQuNS00LjVWNjEuMzVoLTUxLjc0NGMtNy41MDIsMC0xMy42MDUtNi4xMDctMTMuNjA1LTEzLjYxNFYwSDMzLjEzDQoJYy0yLjQ4NSwwLTQuNSwyLjAxNS00LjUsNC41djE5NS4zNzZjMCwyLjQ4NSwyLjAxNSw0LjUsNC41LDQuNUgxNzEuMjQ3eiBNNTIuODkxLDg3LjYyN2g5OS43MTd2ODBINTIuODkxVjg3LjYyN3ogTTEwNi43NDksMTQzLjk2DQoJaDM3Ljg1OHYxNS42NjdoLTM3Ljg1OFYxNDMuOTZ6IE02MC44OTEsMTE5LjI5NGgzNy44NTh2MTYuNjY2SDYwLjg5MVYxMTkuMjk0eiBNNjAuODkxLDE0My45NmgzNy44NTh2MTUuNjY3SDYwLjg5MVYxNDMuOTZ6DQoJIE0xMDYuNzQ5LDk1LjYyN2gzNy44NTh2MTUuNjY3aC0zNy44NThWOTUuNjI3eiBNMTA2Ljc0OSwxMTkuMjk0aDM3Ljg1OHYxNi42NjZoLTM3Ljg1OFYxMTkuMjk0eiBNNjAuODkxLDk1LjYyN2gzNy44NTh2MTUuNjY3DQoJSDYwLjg5MVY5NS42Mjd6IE0xMjAuMzk3LDQ3LjczNnYtMzcuMzRMMTY0LjIsNTEuMzVoLTQwLjE5N0MxMjIuMDE0LDUxLjM1LDEyMC4zOTcsNDkuNzI5LDEyMC4zOTcsNDcuNzM2eiIvPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPC9zdmc+DQo=";
            //ConfirmationNeeded = true;
            //ConfirmationQuestion = "Hiermee wordt de opgeslagen pagina geëxporteerd, dit kan enige ogenblikken duren.";
            //ConfirmationTitle = "Weet u het zeker ?";
        }

        public bool ShowInSearchMode { get; set; } 

        public bool ShowInEditMode { get; set; } 

        public string IconClass { get; set; }

        public string IconURL { get; set; }

        public string Tooltip { get; set; }

        public bool ConfirmationNeeded { get; set; }

        public string ConfirmationTitle { get; set; }

        public string ConfirmationQuestion { get; set; }

        public async Task<ModuleExecutionResult> ExecuteAsync(IComponentListTemplate inList, IApplicationUser inUser, HttpContext context)
        {
            var exec = new ModuleExecutionResult()
            {
                IsSuccess = true,
                WimNotificationOutput = "Successfully opened"
            };

            return exec;
        }

        public bool ShowOnList(IComponentListTemplate inList, IApplicationUser inUser)
        {
            return true;
        }
    }
}
