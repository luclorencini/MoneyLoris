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
        decimal? saldo = 100,
        int idUsuario = TestConstants.USUARIO_COMUM_ID,
        TipoMeioPagamento tipo = TipoMeioPagamento.ContaCorrente,
        bool ativo = true,
        string nome = "Conta"
    )
    {
        var ent = await Context.MeiosPagamento.AddAsync(
            new MeioPagamento
            {
                Tipo = tipo,
                IdUsuario = idUsuario,
                Nome = nome,
                Ativo = ativo,
                Saldo = saldo,
                Cor = "000000",

                Limite = (tipo == TipoMeioPagamento.CartaoCredito ? 5000 : null),
                DiaFechamento = (tipo == TipoMeioPagamento.CartaoCredito ? 1 : null),
                DiaVencimento = (tipo == TipoMeioPagamento.CartaoCredito ? 10 : null)
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
        TipoLancamento tipo = TipoLancamento.Despesa
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
                Descricao = "Compras",
                Valor = valor,
                Data = SystemTime.Today(),
                Operacao = OperacaoLancamento.LancamentoSimples,
                Realizado = true,
            });

        await Context.SaveChangesAsync();

        return ent.Entity;
    }
}
