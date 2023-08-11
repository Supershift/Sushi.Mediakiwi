using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OwaspHeaders.Core.Extensions;
using Sushi.Mediakiwi.Headless;
using Sushi.Mediakiwi.Headless.Config;
using Sushi.Mediakiwi.Headless.SectionHelper;
using Sushi.Mediakiwi.Website.Models;
using Sushi.Mediakiwi.Website.RazorAdditions;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
// public IConfiguration Configuration { get; }

var appSettings = new ApplicationSettings();

builder.Configuration.Bind(appSettings);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddAreaPageRoute("Internals", "/SitemapXML", "sitemap.xml");
    // You can also configure [AllowAnonymous] for pages/folders/areas
    //options.Conventions.AllowAnonymousToAreaPage(".auth", "me");
});

builder.Services.Configure<ApplicationSettings>(builder.Configuration);

if (appSettings.EnableResponseCaching)
{
    builder.Services.AddResponseCaching(options =>
    {
        options.MaximumBodySize = 1024 * 100;
        options.UseCaseSensitivePaths = false;
    });
}

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
    options.AutomaticAuthentication = true;
});

if (appSettings.EnableResponseCaching)
{
    builder.Services.AddMvc(options =>
    {
        options.CacheProfiles.Add("Default", new Microsoft.AspNetCore.Mvc.CacheProfile()
        {
            Duration = 120,
            Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.Any,
            VaryByHeader = "User-Agent,Accept-Encoding,x-mediakiwi-fullrequesturl",
        });
    });
}
else
{
    builder.Services.AddMvc(options =>
    {
        options.CacheProfiles.Add("Default", new Microsoft.AspNetCore.Mvc.CacheProfile()
        {
            NoStore = true,
            Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.None,
        });
    });
}

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ApplicationSettings>(appSettings);
builder.Services.AddSingleton<ISushiApplicationSettings>(appSettings);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddClients();
builder.Services.AddMediaKiwiContentService();

builder.Services.AddTransient<ITagHelperComponent, ScriptTagHelperComponent>();
builder.Services.AddTransient<ITagHelperComponent, MetaTagHelperComponent>();

builder.Services.AddSectionHelper();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseDeveloperExceptionPage();
}

// use security headers
app.UseSecureHeadersMiddleware(SecureHeadersMiddlewareBuilder
                .CreateBuilder()
                .UseHsts()
                .UseXFrameOptions(OwaspHeaders.Core.Enums.XFrameOptions.Sameorigin)
                .UseXSSProtection()
                .UseContentTypeOptions()
                //.UseContentDefaultSecurityPolicy()
                .UsePermittedCrossDomainPolicies()
                .UseReferrerPolicy()
                .RemovePoweredByHeader()
                .Build()
);

// When static file caching is enabled, use it
// Responses would be cached for 4 hours
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // using Microsoft.AspNetCore.Http;
        ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={TimeSpan.FromHours(4).Seconds}");
    }
});

// URL Rewrite rules from XML
var options = new RewriteOptions();
if (File.Exists("URLRewrites.xml"))
{
    using (StreamReader iisUrlRewriteStreamReader = File.OpenText("URLRewrites.xml"))
    {
        options.AddIISUrlRewrite(iisUrlRewriteStreamReader);
    }
}

// URL Rewrite for MediaKiwi
options.Add(new MediaKiwiRewriteRule(app.Services));

app.UseRewriter(options);


if (appSettings.EnableResponseCaching)
    app.UseResponseCaching();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
    endpoints.MapDefaultControllerRoute();
});

app.Run();

// Configure Compression level
// services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
// services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

// services.AddResponseCompression(options =>
// {
//     List<string> MimeTypes = new List<string>()
//     {
//         // General
//         "text/plain",
//         "text/css",
//         "font/woff2",
//         "font/woff",
//         "application/javascript",
//         "image/x-icon",
//         "image/png",
//         "image/jpeg",
//         "image/webp",
//         "text/html"
//         };

//     options.EnableForHttps = true;
//     options.MimeTypes = MimeTypes;
//     options.Providers.Add<GzipCompressionProvider>();
//     options.Providers.Add<BrotliCompressionProvider>();
// });
