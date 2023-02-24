using MoneyLoris.Application.Common.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Categorias.Interfaces;
public interface ICategoriaRepository : IRepositoryBase<Categoria>
{
    Task<ICollection<Categoria>> ListarCategoriasUsuario(TipoLancamento tipo, int idUsuario);
}
