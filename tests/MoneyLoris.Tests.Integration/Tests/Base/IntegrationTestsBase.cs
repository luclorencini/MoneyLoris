using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Infrastructure.Auth;
using MoneyLoris.Infrastructure.Persistence.Context;
using MoneyLoris.Tests.Integration.Setup.Auth;
using MoneyLoris.Tests.Integration.Setup.EF;
using MoneyLoris.Tests.Integration.Setup.Utils;

namespace MoneyLoris.Tests.Integration.Tests.Base;
public class IntegrationTestsBase : IDisposable
{
    protected HttpClient HttpClient = null!;
    protected BaseApplicationDbContext Context = null!;

    //referencia: https://gunnarpeipman.com/aspnet-core-integration-tests-appsettings/

    public void SubirAplicacao(bool logado = true, PerfilUsuario? perfil = null)
    {
        //faz alguns overrides no Program.cs do projeto Web:

        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.json");


        var webAppFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            //troca o appsettings para usar o de teste
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.AddJsonFile(configPath);
            });

            //troca o DbSession para usar o de teste
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<BaseApplicationDbContext, TestApplicationDbContext>();
            });

            if (logado)
            {
                var usuario = perfil!.Value == PerfilUsuario.Administrador ?
                                    TestConstants.UsuarioAdmin() :
                                    TestConstants.UsuarioComum();

                var _authManager = new AuthenticationManager();

                var testClaims = _authManager.GerarClaimsUsuario(usuario);

                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IAuthenticationSchemeProvider, MockSchemeProvider>();
                    services.AddSingleton<MockClaimSeed>(_ => new(testClaims));
                    services.AddSingleton<DatabaseSeeder, DatabaseSeeder>();
                });
            }

        });

        //cria o client da aplicação

        HttpClient = webAppFactory.CreateDefaultClient();


        //cria um banco de dados novo para o teste

        var scope = webAppFactory.Services.CreateScope();

        Context = scope.ServiceProvider.GetRequiredService<BaseApplicationDbContext>();

        Context.Database.EnsureCreated();

        
    }

    public async Task InserirUsuariosNaBase()
    {
        var dbSeed = new DatabaseSeeder(Context);
        await dbSeed.InserirUsuarios();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
    }
}
