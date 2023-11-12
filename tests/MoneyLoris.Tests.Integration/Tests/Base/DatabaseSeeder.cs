using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Infrastructure.Persistence.Context;
using MoneyLoris.Tests.Integration.Setup.Utils;

namespace MoneyLoris.Tests.Integration.Tests.Base;
public class DatabaseSeeder
{
    private readonly BaseApplicationDbContext Context = null!;

    public DatabaseSeeder(BaseApplicationDbContext context)
    {
        Context = context;
    }

    public async Task InserirUsuarios()
    {
        // inserindo na ordem, para poder usar os ids com segurança

        var admin = TestConstants.UsuarioAdmin();
        await Context.Usuarios.AddAsync(admin);

        var comum = TestConstants.UsuarioComum();
        await Context.Usuarios.AddAsync(comum);

        var comub = TestConstants.UsuarioComumB();
        await Context.Usuarios.AddAsync(comub);

        await Context.SaveChangesAsync();
    }

    public async Task<Categoria> InserirCategoria(
        TipoLancamento tipo = TipoLancamento.Despesa,
        int idUsuario = TestConstants.USUARIO_COMUM_ID,
        string nome = "Outros",
        byte? ordem = null
    )
    {
        var ent = await Context.Categorias.AddAsync(
            new Categoria
            {
                Tipo = tipo,
                IdUsuario = idUsuario,
                Nome = nome,
                Ordem = ordem
            });

        await Context.SaveChangesAsync();

        return ent.Entity;
    }

    public async Task<Subcategoria> InserirSubcategoria(
        int idCategoria,
        string nome = "Sub-outros",
        byte? ordem = null
    )
    {
        var ent = await Context.Subcategorias.AddAsync(
            new Subcategoria
            {
                IdCategoria = idCategoria,
                Nome = nome,
                Ordem = ordem
            });

        await Context.SaveChangesAsync();

        return ent.Entity;
    }

    public async Task<MeioPagamento> InserirMeioPagamento(
        int idUsuario = TestConstants.USUARIO_COMUM_ID,
        TipoMeioPagamento tipo = TipoMeioPagamento.ContaCorrente,
        string nome = "Conta",
        byte? ordem = null,
        bool ativo = true,
        byte? fecha = 1,
        byte? vence = 10
    )
    {
        var ent = await Context.MeiosPagamento.AddAsync(
            new MeioPagamento
            {
                Tipo = tipo,
                IdUsuario = idUsuario,
                Nome = nome,
                Ativo = ativo,
                Cor = "000000",
                Ordem = ordem,

                Limite = (tipo == TipoMeioPagamento.CartaoCredito ? 5000 : null),
                DiaFechamento = (tipo == TipoMeioPagamento.CartaoCredito ? fecha : null),
                DiaVencimento = (tipo == TipoMeioPagamento.CartaoCredito ? vence : null)
            });

        await Context.SaveChangesAsync();

        return ent.Entity;
    }

    public async Task<Fatura> InserirFatura(
        int idCartao,
        int mes = 6,
        int ano = 2023,
        DateTime? dataIni = null,
        DateTime? dataFim = null,
        DateTime? dataVen = null,
        decimal? valorPago = null
    )
    {
        if (dataIni is null) dataIni = new DateTime(2023, 5, 3);
        if (dataFim is null) dataFim = new DateTime(2023, 6, 2);
        if (dataVen is null) dataVen = new DateTime(2023, 6, 10);

        var ent = await Context.Faturas.AddAsync(
            new Fatura
            {
                IdMeioPagamento = idCartao,
                Mes = mes,
                Ano = ano,
                DataInicio = dataIni.Value,
                DataFim = dataFim.Value,
                DataVencimento = dataVen.Value,
                ValorPago = valorPago
            });

        await Context.SaveChangesAsync();

        return ent.Entity;
    }


    public async Task<Lancamento> InserirLancamentoSimples(
        int idMeioPagamento,
        int idCategoria,
        int? idSubcategoria = null,
        decimal valor = 100,
        int idUsuario = TestConstants.USUARIO_COMUM_ID,
        TipoLancamento tipo = TipoLancamento.Despesa,
        string descricao = "Compras",
        DateTime? data = null,
        int? idFatura = null
    )
    {
        return await _inserirLancamento(
            OperacaoLancamento.LancamentoSimples,
            idMeioPagamento,
            idCategoria,
            idSubcategoria,
            valor,
            idUsuario,
            tipo,
            descricao,
            data,
            tipoTransferencia: null,
            idFatura
        );
    }

    public async Task<Lancamento> InserirLancamentoTransferencia(
        int idMeioPagamento,
        TipoTransferencia tipoTransferencia = TipoTransferencia.TransferenciaEntreContas,
        decimal valor = 100,
        int idUsuario = TestConstants.USUARIO_COMUM_ID,
        TipoLancamento tipo = TipoLancamento.Despesa,
        string descricao = "Compras",
        DateTime? data = null,
        int? idFatura = null
    )
    {
        return await _inserirLancamento(
            OperacaoLancamento.Transferencia,
            idMeioPagamento,
            idCategoria: null,
            idSubcategoria: null,
            valor,
            idUsuario,
            tipo,
            descricao,
            data,
            tipoTransferencia,
            idFatura
        );
    }

    private async Task<Lancamento> _inserirLancamento(
        OperacaoLancamento operacao,
        int idMeioPagamento,
        int? idCategoria,
        int? idSubcategoria,
        decimal valor,
        int idUsuario,
        TipoLancamento tipo,
        string descricao,
        DateTime? data,
        TipoTransferencia? tipoTransferencia,
        int? idFatura = null
    )
    {
        var ent = await Context.Lancamentos.AddAsync(
            new Lancamento
            {
                IdUsuario = idUsuario,
                IdCategoria = idCategoria,
                IdSubcategoria = idSubcategoria,
                IdMeioPagamento = idMeioPagamento,
                Tipo = tipo,
                Descricao = descricao,
                Valor = valor,
                Data = (data.HasValue ? data.Value : SystemTime.Today()),
                Operacao = operacao,
                Realizado = true,
                TipoTransferencia = tipoTransferencia,
                IdFatura = idFatura
            });

        await Context.SaveChangesAsync();

        return ent.Entity;
    }

    public async Task AssociarLancamentosTransferencia(Lancamento origem, Lancamento destino)
    {
        origem.IdLancamentoTransferencia = destino.Id;
        destino.IdLancamentoTransferencia = origem.Id;

        await Context.SaveChangesAsync();
    }
}
