using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Business.Contas;
using MoneyLoris.Application.Stubs;
using MoneyLoris.Infrastructure.Auth;

namespace MoneyLoris.Infrastructure.DI;
public static class DependencyInjectionExtensions
{
    public static void InjetarDependencias(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IAuthenticationManager, AuthenticationManager>();

        var ativarStubs = Convert.ToBoolean(config["AtivarStubs"]);

        if (ativarStubs)
        {
            InjetarDependenciasStubs(services);
        }
        else
        {
            InjetarDependenciasAplicacao(services);
        }

        return;
    }

    private static void InjetarDependenciasAplicacao(IServiceCollection services)
    {
        //DI da aplicação

        //services.AddScoped<BaseApplicationDbContext, ApplicationDbContext>();

        //services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        //services.AddScoped<ILoginService, LoginService>();

        //services.AddScoped<ICategoriaService, CategoriaService>();

        //services.AddScoped<IContaService, ContaService>();
    }

    private static void InjetarDependenciasStubs(IServiceCollection services)
    {
        //DI de serviços stub com dados fixos para demonstração e para facilitar a construção do front-end

        services.AddScoped<ILoginService, LoginServiceStub>();
        services.AddScoped<ICategoriaService, CategoriaServiceStub>();
        services.AddScoped<IContaService, ContaServiceStub>();
        return;
    }
}
