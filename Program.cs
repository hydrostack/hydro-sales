using System.IO.Compression;
using Hydro.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using HydroSales;
using HydroSales.Authorization;
using HydroSales.Database;
using HydroSales.Pages.Start;
using HydroSales.Utils;
using HydroSales.Utils.Validation;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration.Get<AppSettings>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.Conditional(_ => builder.Environment.IsProduction(), sink => sink.File(appSettings.LogFile, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10))
    .CreateLogger();

builder.Host.UseSerilog();

var services = builder.Services;

services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.Duration;
    logging.CombineLogs = true;
});

services.AddRazorPages();
services.AddHttpContextAccessor();
services.AddAntiforgery(options => options.Cookie.Name = "antiforgery");
services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(appSettings.DataProtectionKeysLocation));

services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.LoginPath = "/";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(365);
    });

services.AddSingleton(appSettings);

services.AddHydro(options => { options.AntiforgeryTokenEnabled = true; });

services.AddResponseCompression(options => options.EnableForHttps = true);

builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

services.AddScoped<IAuthorizationService, WebAuthorizationService>();
services.AddScoped<IMembershipService, MembershipService>();
services.AddDbContext<DatabaseContext>(context => context.UseSqlite(appSettings.DatabaseConnectionString));
services.AddScoped<IDatabase>(provider => provider.GetService<DatabaseContext>()!);
services.AddScoped<IDatabaseMigrator>(provider => provider.GetService<DatabaseContext>()!);

services.Configure<RequestLocalizationOptions>(options =>
{
    const string defaultCulture = "en-US";
    options.DefaultRequestCulture = new(defaultCulture);
    options.SupportedCultures = [new(defaultCulture)];
    options.SupportedUICultures = [new(defaultCulture)];
});

services.Configure<RouteOptions>(option =>
{
    option.LowercaseUrls = true;
    option.LowercaseQueryStrings = true;
});

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseResponseCompression();
}

app.UseRequestLocalization();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseExceptionHandler(b => b.UseMiddleware<HydroExceptionMiddleware>());
app.UseStaticFiles();
app.UseHttpLogging();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.UseHydro(builder.Environment);

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>().Migrate();
}

app.Run();