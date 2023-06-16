using MoneyLoris.Application.Business.Faturas.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Faturas;
internal class FaturaValidator : IFaturaValidator
{
    public void Existe(Fatura fatura)
    {
        if (fatura == null)
            throw new BusinessException(
                code: ErrorCodes.Fatura_NaoEncontrado,
                message: "Fatura não encontrada");
    }
}
