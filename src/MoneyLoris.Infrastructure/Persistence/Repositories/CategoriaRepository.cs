using Microsoft.EntityFrameworkCore;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Infrastructure.Persistence.Context;
using MoneyLoris.Infrastructure.Persistence.Repositories.Base;

namespace MoneyLoris.Infrastructure.Persistence.Repositories;
public class CategoriaRepository : RepositoryBase<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(BaseApplicationDbContext context) : base(context) { }

    public async Task<ICollection<Categoria>> ListarCategoriasUsuario(TipoLancamento tipo, int idUsuario)
    {
        var list = await _dbset
            .Where(c =>
                c.Tipo == tipo && c.IdUsuario == idUsuario)
            .OrderBy(c => c.Ordem).ThenBy(c => c.Nome)
            .AsNoTracking()
            .ToListAsync();

        return list;
    }
}
