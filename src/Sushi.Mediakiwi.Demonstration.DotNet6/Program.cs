using Microsoft.AspNetCore.Mvc.Infrastructure;
using Sushi.Mediakiwi;
using Sushi.Mediakiwi.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddHttpContextAccessor();
//builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddMediakiwi();
builder.Services.AddMediakiwiApi();

builder.Services.AddControllersWithViews(options =>
{
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseAuthentication();
app.UseAuthorization();

string[] excludePaths = new string[] { "/api/custom", "/myfiles", "/mkapi" };
app.UseMediakiwi(excludePaths);
app.UseMediakiwiApi();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
