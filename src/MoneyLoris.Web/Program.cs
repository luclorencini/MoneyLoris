using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;
using MoneyLoris.Infrastructure.Auth;
using MoneyLoris.Infrastructure.DI;
using MoneyLoris.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

#region Data Protection

//IMPORTANTE: no deploy, � preciso que a aplica��o .net tenha permiss�o de escrita no diret�rio, para poder criar a pasta e arquivo do DataProtection

var keysDirectoryName = "dataProtectionKeys";
var keysDirectoryPath = Path.Combine(builder.Environment.ContentRootPath, keysDirectoryName);
if (!Directory.Exists(keysDirectoryPath))
{
    Directory.CreateDirectory(keysDirectoryPath);
}
services.AddDataProtection()
      .SetApplicationName("MoneyLoris")
      .PersistKeysToFileSystem(new DirectoryInfo(keysDirectoryPath));

#endregion

#region Autentica��o e Autoriza��o

var authSection = builder.Configuration.GetSection("Auth");

builder.Services.AddOptions<AuthConfig>().Bind(authSection);

var authConfig = new AuthConfig();
authSection.Bind(authConfig);

services.AddAuthentication(authConfig.Scheme)
    .AddCookie(authConfig.Scheme, config =>
    {
        config.Cookie.Name = authConfig.Cookie;
        config.Cookie.Domain = authConfig.Domain;
        config.LoginPath = "/Login";
        //config.SlidingExpiration = true;
        config.ExpireTimeSpan = TimeSpan.FromDays(14); //for�ando aqui para testes
    });

services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador"));
    options.AddPolicy("Usuario", policy => policy.RequireClaim(ClaimTypes.Role, "Usuario"));

});

#endregion

services.AddTransient<ILogger>(s => s.GetRequiredService<ILogger<Program>>());
services.AddHttpContextAccessor();

//todo - fazer o m�todo pegar configuration por conta propria, e n�o passar via parametro
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