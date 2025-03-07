using erpv0._1.Controllers;
using erpv0._1.Data;
using erpv0._1.Models;
using erpv0._1.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging() // Logs parameter values
           .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));

builder.Services.AddScoped<ISystemSettingsService, SystemSettingsService>();

// Add Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
    // Add other providers as needed

    // Configure minimum log level
    loggingBuilder.SetMinimumLevel(LogLevel.Information);
});

// Configure supported cultures
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ar")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});


var app = builder.Build();
//var port = Environment.GetEnvironmentVariable("PORT") ?? "80";


// Configure the HTTP request pipeline.

// Configure error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

// Add Localization middleware
var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

app.UseRouting();

app.UseAuthorization();

// Enhanced routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// API routes for AJAX operations
app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller}/{action}/{id?}");

app.Run();