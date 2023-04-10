using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
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
        bool inserirSubcategoria = false)
    {
        var ent = await Context.Categorias.AddAsync(
            new Categoria
            {
                Tipo = tipo,
                IdUsuario = idUsuario,
                Nome = "Outros",

                Subcategorias = !inserirSubcategoria ? null! :
                new List<Subcategoria>
                {
                    new Subcategoria {
                        Nome = "Sub-outros",
                    }
                }
            });

        await Context.SaveChangesAsync();

        return ent.Entity;
    }

    public async Task<MeioPagamento> InserirMeioPagamento(
        decimal? saldo = 100,
        int idUsuario = TestConstants.USUARIO_COMUM_ID,
        TipoMeioPagamento tipo = TipoMeioPagamento.ContaCorrente,
        bool ativo = true)
    {
        var ent = await Context.MeiosPagamento.AddAsync(
            new MeioPagamento
            {
                Tipo = tipo,
                IdUsuario = idUsuario,
                Nome = "Conta",
                Ativo = ativo,
                Saldo = saldo,
                Cor = "000000"
            });

        await Context.SaveChangesAsync();

        return ent.Entity;
    }
}
