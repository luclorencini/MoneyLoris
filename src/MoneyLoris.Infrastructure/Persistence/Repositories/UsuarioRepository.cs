using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MoneyLoris.Application.Business.Usuarios.Dtos;
using MoneyLoris.Application.Business.Usuarios.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Infrastructure.Persistence.Context;
using MoneyLoris.Infrastructure.Persistence.Repositories.Base;

namespace MoneyLoris.Infrastructure.Persistence.Repositories;
public class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(BaseApplicationDbContext context) : base(context) { }

    public async Task<Usuario> GetByLoginSenha(string login, string senhaHash)
    {
        var usuario = await _dbset
                .Where(
                    u => u.Login == login && u.Senha == senhaHash)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        return usuario!;
    }

    public async Task<ICollection<Usuario>> PesquisaPaginada(UsuarioPesquisaDto filtro)
    {
        var list = await _dbset
            .Where(whereQueryListagem(filtro))
            .OrderBy(u => u.Nome)
            .IncluiPaginacao(filtro)
            .AsNoTracking()
            .ToListAsync();

        return list;
    }

    public async Task<int> PesquisaTotalRegistros(UsuarioPesquisaDto filtro)
    {
        var total = await _dbset
            .Where(whereQueryListagem(filtro))
            .AsNoTracking()
            .CountAsync();

        return total;
    }

    private Expression<Func<Usuario, bool>> whereQueryListagem(UsuarioPesquisaDto filtro)
    {
        Expression<Func<Usuario, bool>> query =
            c => (filtro.Nome == null || c.Nome.Contains(filtro.Nome))
            && (filtro.Ativo == null || c.Ativo == filtro.Ativo)
            && (filtro.IdPerfil == null || c.IdPerfil == filtro.IdPerfil)
            ;

        return query;
    }
}
