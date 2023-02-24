using MoneyLoris.Application.Business.Usuarios.Dtos;
using MoneyLoris.Application.Common.Interfaces;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Usuarios.Interfaces;
public interface IUsuarioRepository : IRepositoryBase<Usuario>
{
    Task<Usuario> GetByLoginSenha(string login, string senhaHash);
    Task<ICollection<Usuario>> PesquisaPaginada(UsuarioPesquisaDto filtro);
    Task<int> PesquisaTotalRegistros(UsuarioPesquisaDto filtro);
}
