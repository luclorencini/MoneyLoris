using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Infrastructure.Persistence.Context;
using MoneyLoris.Infrastructure.Persistence.Repositories.Base;

namespace MoneyLoris.Infrastructure.Persistence.Repositories;
public class SubcategoriaRepository : RepositoryBase<Subcategoria>, ISubcategoriaRepository
{
    public SubcategoriaRepository(BaseApplicationDbContext context) : base(context) { }
}
