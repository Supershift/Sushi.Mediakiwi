using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sushi.Mediakiwi.Framework2
{
    public class MultifileUploadHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        
        public void ProcessRequest(HttpContext context)
        {
            var wim = new Sushi.Mediakiwi.Framework.WimRoot();
            try
            {
                if (wim.CurrentApplicationUser != null && wim.CurrentApplicationUser.IsLoggedIn())
                {
                    string source = context.Request["source"];
                    if (!String.IsNullOrEmpty(source))
                    {
                        string galleryGUID = context.Request["gallery"];
                        if (!String.IsNullOrEmpty(galleryGUID))
                        {
                            var gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(new Guid(galleryGUID));
                            if (gallery != null && !gallery.IsNewInstance)
                            {
                                var httpUploadedFile = context.Request.Files[0];
                                if (httpUploadedFile != null && httpUploadedFile.ContentLength > 0)
                                {
                                    var asset = new Sushi.Mediakiwi.Data.Asset();
                                    asset.SaveStream(httpUploadedFile, gallery);

                                    // Save asset and return to client the asset ID
                                    asset.Save();

                                    //update gallery count                                    
                                    gallery.UpdateCount();

                                    context.Response.StatusCode = 200;
                                    context.Response.Write(asset.ID);
                                }
                                else
                                {
                                    context.Response.StatusCode = 404;
                                    context.Response.Write($"httpfile file not found or no bytes");
                                }
                            }
                            else
                            {
                                context.Response.StatusCode = 404;
                                context.Response.Write($"Gallery {galleryGUID} not found");
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            context.Response.Write("Gallery param not supplied");
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Write("Source param not supplied");
                    }
                }
                else
                {
                    context.Response.StatusCode = 401;
                    context.Response.Write("Not Authorized");
                }
            }
            catch (Exception exc)
            {
                context.Response.StatusCode = 500;
                context.Response.Write("Exception: "+exc.Message);
            }
            finally
            {
                context.Response.ContentType = "text/plain";
                context.Response.End();
            }
        }
    }
}
