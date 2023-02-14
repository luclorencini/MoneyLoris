using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyLoris.Application.Business.Auth.Interfaces;
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
            //DI de serviços stub com dados fixos para demonstração e para facilitar a construção do front-end
            services.AddScoped<ILoginService, LoginServiceStub>();
            //services.AddScoped<IContaService, DemoContaService>();
            return;
        }

        //DI da aplicação
        //services.AddScoped<BaseApplicationDbContext, ApplicationDbContext>();

        //services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        //services.AddScoped<ILoginService, LoginService>();
        //services.AddScoped<IContaService, ContaService>();
    }
}
