using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;

namespace MoneyLoris.Application.Reports.LancamentosCategoria;
public class ReportLancamentosCategoriaService : IReportLancamentosCategoriaService
{
    private readonly IReportLancamentosCategoriaRepository _reportRepo;

    public ReportLancamentosCategoriaService(IReportLancamentosCategoriaRepository reportRepo)
    {
        _reportRepo = reportRepo;
    }

    public ICollection<CategoriaGroupItemtoDto> RelatorioLancamentosPorCategoria(int mes, int ano, int quantidade)
    {
        var ret = new List<CategoriaGroupItemtoDto>();

        var list = _reportRepo.RelatorioLancamentosPorCategoria(mes, ano, quantidade);

        //agrupamento de categorias
        var catGroup = list
            .GroupBy(r => r.catNome)
            .Select(g => new CategoriaGroupItemtoDto { 
                catId = g.First().catId,
                catNome = g.First().catNome,
                catOrdem = g.First().catOrdem,
                jan = g.Sum(r => r.jan),
                fev = g.Sum(r => r.fev),
                mar = g.Sum(r => r.mar),
                Items = g.ToList()
            } )

            .ToList();

        //agrupamento superior do valor total
        var upper = new CategoriaGroupItemtoDto
        {
            catNome = "Despesas",
            jan = catGroup.Sum(c => c.jan),
            fev = catGroup.Sum(c => c.fev),
            mar = catGroup.Sum(c => c.mar),
            Items = catGroup.ToList()
        };

        ret.Add(upper);

        return ret;
    }
}
