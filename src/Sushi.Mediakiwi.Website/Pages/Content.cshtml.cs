using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sushi.Mediakiwi.Headless;
using Sushi.Mediakiwi.Website.Models;

namespace Sushi.Mediakiwi.Website.Pages
{
    public class ContentModel : BaseMKRazorPageModel
    {
        public ContentModel(IMediaKiwiContentService service) : base(service)
        {

        }

        public void OnGet()
        {

        }
    }
}
