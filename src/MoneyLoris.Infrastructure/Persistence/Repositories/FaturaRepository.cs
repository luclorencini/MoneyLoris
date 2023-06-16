using Microsoft.EntityFrameworkCore;
using MoneyLoris.Application.Business.Faturas.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Infrastructure.Persistence.Context;
using MoneyLoris.Infrastructure.Persistence.Repositories.Base;

namespace MoneyLoris.Infrastructure.Persistence.Repositories;
public class FaturaRepository : RepositoryBase<Fatura>, IFaturaRepository
{
    public FaturaRepository(BaseApplicationDbContext context) : base(context) { }

    public async Task<Fatura> ObterPorMesAno(int idCartao, int mes, int ano)
    {
        var ent = await _dbset
            .Where(
                f => f.IdMeioPagamento == idCartao &&
                f.Ano == ano &&
                f.Mes == mes
            )
            .SingleOrDefaultAsync();

        return ent!;
    }
}
