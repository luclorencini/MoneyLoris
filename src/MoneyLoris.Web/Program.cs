
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MoneyLoris.Infrastructure.Auth;
using MoneyLoris.Infrastructure.DI;
using MoneyLoris.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

//#region Data Protection

//var keysDirectoryName = "dataProtectionKeys";
//var keysDirectoryPath = Path.Combine(builder.Environment.ContentRootPath, keysDirectoryName);
//if (!Directory.Exists(keysDirectoryPath))
//{
//    Directory.CreateDirectory(keysDirectoryPath);
//}
//services.AddDataProtection()
//      .SetApplicationName("MoneyLoris")
//      .PersistKeysToFileSystem(new DirectoryInfo(keysDirectoryPath));

//#endregion

#region Autenticação e Autorização

var authSection = builder.Configuration.GetSection("Auth");

builder.Services.AddOptions<AuthConfig>().Bind(authSection);

var authConfig = new AuthConfig();
authSection.Bind(authConfig);

services.AddAuthentication(authConfig.Scheme)
    .AddCookie(authConfig.Scheme, config =>
    {
        config.Cookie.Name = authConfig.Cookie;
        //config.Cookie.Domain = "moneyloris.com.br";
        config.Cookie.Domain = authConfig.Domain;
        config.LoginPath = "/Login";
        //config.SlidingExpiration = true;
        config.ExpireTimeSpan = TimeSpan.FromDays(14); //forçando aqui para testes
    });

//services.Configure<SecurityStampValidatorOptions>(options =>
//    options.ValidationInterval = TimeSpan.FromDays(28)
//);

//services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.Cookie.Name = ".AspNetCore.Cookies";
//        options.Cookie.Domain = "moneyloris.com.br";

//        options.ExpireTimeSpan = TimeSpan.FromDays(14);
//        //options.SlidingExpiration = true;
//        options.LoginPath = "/Login";
//    });

services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador"));
    options.AddPolicy("Usuario", policy => policy.RequireClaim(ClaimTypes.Role, "Usuario"));

});

#endregion

services.AddTransient<ILogger>(s => s.GetRequiredService<ILogger<Program>>());
services.AddHttpContextAccessor();

//todo - fazer o método pegar configuration por conta propria, e não passar via parametro
services.InjetarDependencias(builder.Configuration);




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }