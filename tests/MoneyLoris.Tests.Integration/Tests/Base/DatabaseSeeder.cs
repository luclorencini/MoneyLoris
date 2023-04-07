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
}
