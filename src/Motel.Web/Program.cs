using System.Globalization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Motel.Application.Interfaces;
using Motel.Infrastructure.Data;
using Motel.Infrastructure.Notifications;
using Motel.Infrastructure.Services;
using Motel.Web.Branding;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container - Standard MVC
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization()
    .AddViewLocalization();

builder.Services.AddRazorPages();

// Add localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configure branding
builder.Services.Configure<BrandingOptions>(
    builder.Configuration.GetSection("Branding"));

// Configure DbContext - SQL Server or SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("Default");

if (connectionString?.Contains("Data Source=") == true && !connectionString.Contains("Server="))
{
    // SQLite for development
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    // SQL Server for production
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Motel.Application.DTOs.RoomDto>();

// Register application services
builder.Services.AddScoped<IRoomsService, RoomsService>();
builder.Services.AddScoped<IClientsService, ClientsService>();
builder.Services.AddScoped<IReservationsService, ReservationsService>();
builder.Services.AddScoped<IInvoicesService, InvoicesService>();
builder.Services.AddScoped<INotificationsService, NotificationsService>();
builder.Services.AddScoped<IReportsService, ReportsService>();

// Register notification providers
builder.Services.AddScoped<IEmailProvider, MailKitEmailProvider>();
builder.Services.AddScoped<ISmsProvider, FakeSmsProvider>();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? builder.Configuration.GetConnectionString("Default");

        if (connStr?.Contains("Data Source=") == true && !connStr.Contains("Server="))
        {
            // SQLite for development - ensure directory exists
            var dbPath = connStr.Replace("Data Source=", "").Trim();
            var directory = Path.GetDirectoryName(dbPath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                logger.LogInformation("Created AppData directory at {Directory}", directory);
            }

            // Use EnsureCreated for SQLite (no migrations needed)
            dbContext.Database.EnsureCreated();
            logger.LogInformation("SQLite database initialized successfully");
        }
        else
        {
            // SQL Server - apply migrations
            logger.LogInformation("Applying SQL Server migrations...");
            dbContext.Database.Migrate();
            logger.LogInformation("SQL Server database migrations applied successfully");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
        // Don't throw - allow app to start but log the error
    }
}

// Configure localization
var supportedCultures = new[] { new CultureInfo("ar-EG") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ar-EG"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// CRITICAL FIX: Map Area routes FIRST, then default routes
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
