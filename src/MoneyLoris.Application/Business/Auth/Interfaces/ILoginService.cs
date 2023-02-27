using MoneyLoris.Application.Business.Auth.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Auth.Interfaces;
public interface ILoginService
{
    Task<Result<LoginRetornoDto>> Login(LoginInputDto login);
    Task<Result> AlterarSenha(AlteracaoSenhaDto dto);
    Task<Result> LogOut();
}
