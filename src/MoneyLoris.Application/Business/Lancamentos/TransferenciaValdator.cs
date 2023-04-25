using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos;
public class TransferenciaValdator : ITransferenciaValdator
{
    private readonly IAuthenticationManager _authenticationManager;

    public TransferenciaValdator(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    private UserAuthInfo userInfo => _authenticationManager.ObterInfoUsuarioLogado();

    public void NaoEhAdmin()
    {
        if (userInfo.IsAdmin)
            throw new BusinessException(
                code: ErrorCodes.Transferencia_AdminNaoPode,
                message: "Administradores não realizam transferências");
    }
}
