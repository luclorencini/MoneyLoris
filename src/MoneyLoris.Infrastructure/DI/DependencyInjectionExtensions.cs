﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyLoris.Application.Business.Auth;
using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Categorias;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Business.Faturas;
using MoneyLoris.Application.Business.Faturas.Interfaces;
using MoneyLoris.Application.Business.Lancamentos;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Application.Business.MeiosPagamento;
using MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
using MoneyLoris.Application.Business.Usuarios;
using MoneyLoris.Application.Business.Usuarios.Interfaces;
using MoneyLoris.Application.Reports.LancamentosCategoria;
using MoneyLoris.Application.Stubs;
using MoneyLoris.Infrastructure.Auth;
using MoneyLoris.Infrastructure.Persistence.Context;
using MoneyLoris.Infrastructure.Persistence.Repositories;
using MoneyLoris.Infrastructure.Persistence.Repositories.Reports;

namespace MoneyLoris.Infrastructure.DI;
public static class DependencyInjectionExtensions
{
    public static void InjetarDependencias(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IAuthenticationManager, AuthenticationManager>();

        var ativarStubs = Convert.ToBoolean(config["AtivarStubs"]);

        if (ativarStubs)
            InjetarDependenciasStubs(services);
        else
            InjetarDependenciasAplicacao(services);
    }

    private static void InjetarDependenciasAplicacao(IServiceCollection services)
    {
        //DI da aplicação

        services.AddScoped<BaseApplicationDbContext, ApplicationDbContext>();

        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<ICategoriaValidator, CategoriaValidator>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<ISubcategoriaRepository, SubcategoriaRepository>();

        services.AddScoped<IMeioPagamentoService, MeioPagamentoService>();
        services.AddScoped<IMeioPagamentoValidator, MeioPagamentoValidator>();
        services.AddScoped<IMeioPagamentoRepository, MeioPagamentoRepository>();

        services.AddScoped<ILancamentoService, LancamentoService>();
        services.AddScoped<ILancamentoConsultaService, LancamentoConsultaService>();
        services.AddScoped<ILancamentoValidator, LancamentoValidator>();
        services.AddScoped<ILancamentoConverter, LancamentoConverter>();
        services.AddScoped<IParcelaCalculator, ParcelaCalculator>();
        services.AddScoped<ILancamentoRepository, LancamentoRepository>();

        services.AddScoped<ITransferenciaService, TransferenciaService>();
        services.AddScoped<ITransferenciaValidator, TransferenciaValidator>();

        services.AddScoped<IFaturaService, FaturaService>();
        services.AddScoped<IFaturaHelper, FaturaHelper>();
        services.AddScoped<IFaturaFactory, FaturaFactory>();
        services.AddScoped<IFaturaValidator, FaturaValidator>();
        services.AddScoped<IFaturaRepository, FaturaRepository>();

        services.AddScoped<IReportLancamentosCategoriaService, ReportLancamentosCategoriaService>();
        services.AddScoped<IReportLancamentosCategoriaRegimeCompetenciaRepository, ReportLancamentosCategoriaRegimeCompetenciaRepository>();
        services.AddScoped<IReportLancamentosCategoriaRegimeCaixaRepository, ReportLancamentosCategoriaRegimeCaixaRepository>();
    }

    private static void InjetarDependenciasStubs(IServiceCollection services)
    {
        //DI de serviços stub com dados fixos para demonstração e para facilitar a construção do front-end

        services.AddScoped<ILoginService, LoginServiceStub>();
        services.AddScoped<IUsuarioService, UsuarioServiceStub>();
        services.AddScoped<ICategoriaService, CategoriaServiceStub>();
        services.AddScoped<IMeioPagamentoService, MeioPagamentoServiceStub>();
        services.AddScoped<ILancamentoService, LancamentoServiceStub>();
        services.AddScoped<ILancamentoConsultaService, LancamentoConsultaServiceStub>();
        services.AddScoped<ITransferenciaService, TransferenciaServiceStub>();
        services.AddScoped<IFaturaService, FaturaServiceStub>();

        services.AddScoped<IReportLancamentosCategoriaService, ReportLancamentosCategoriaServiceStub>();
    }
}
