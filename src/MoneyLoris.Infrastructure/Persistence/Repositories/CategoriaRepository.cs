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

            .Where(c => c.Tipo == tipo && c.IdUsuario == idUsuario)

            //ordena categoria por ordem crescente, com nulls ao final
            .OrderBy(c => c.Ordem == null)
            .ThenBy(c => c.Ordem)
            .ThenBy(c => c.Nome)

            .Include(c => c.Subcategorias
                //ordena subcategoria por ordem crescente, com nulls ao final
                .OrderBy(s => s.Ordem == null)
                .ThenBy(s => s.Ordem)
                .ThenBy(s => s.Nome)
            )

            .AsNoTracking()
            .ToListAsync();

        return list;
    }
}
