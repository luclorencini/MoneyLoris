using MoneyLoris.Application.Business.Faturas.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Faturas;
public class FaturaValidator : IFaturaValidator
{
    public void Existe(Fatura fatura)
    {
        if (fatura == null)
            throw new BusinessException(
                code: ErrorCodes.Fatura_NaoEncontrada,
                message: "Fatura não encontrada");
    }
}
