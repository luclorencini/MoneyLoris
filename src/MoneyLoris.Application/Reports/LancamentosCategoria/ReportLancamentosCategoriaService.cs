using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Reports.LancamentosCategoria;
public class ReportLancamentosCategoriaService : IReportLancamentosCategoriaService
{
    private readonly IReportLancamentosCategoriaRepository _reportRepo;
    private readonly IAuthenticationManager _authenticationManager;

    public ReportLancamentosCategoriaService(IReportLancamentosCategoriaRepository reportRepo, IAuthenticationManager authenticationManager)
    {
        _reportRepo = reportRepo;
        _authenticationManager = authenticationManager;
    }

    public ICollection<CategoriaReportItemDto> RelatorioLancamentosPorCategoria(int mes, int ano, int quantidade)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var despesas = this.GetDadosRelatorioTipoLancamento(userInfo.Id, TipoLancamento.Despesa, mes, ano, quantidade);
        var receitas = this.GetDadosRelatorioTipoLancamento(userInfo.Id, TipoLancamento.Receita, mes, ano, quantidade);

        var ret = new List<CategoriaReportItemDto>
        {
            receitas,
            despesas
        };

        return ret;
    }

    private CategoriaReportItemDto GetDadosRelatorioTipoLancamento(int idUsuario, TipoLancamento tipo, int mes, int ano, int quantidade)
    {

        var ret = new List<CategoriaReportItemDto>();

        var list = _reportRepo.RelatorioLancamentosPorCategoria(idUsuario, tipo, mes, ano, quantidade);

        //agrupamento de categorias
        var catGroup = list
            .GroupBy(r => r.catNome)
            .Select(g => new CategoriaReportItemDto
            {
                IdCategoria = g.First().catId,
                IdSubcategoria = g.First().subId,
                Descricao = g.First().catNome!,
                Valor01 = g.Sum(r => r.jan),
                Valor02 = g.Sum(r => r.fev),
                Valor03 = g.Sum(r => r.mar),

                //agrupamento das subcategorias
                Items = g.ToList().Select(
                    c => new CategoriaReportItemDto
                    {
                        IdCategoria = c.catId,
                        IdSubcategoria = c.subId,
                        Descricao = c.subNome!,
                        Valor01 = c.jan,
                        Valor02 = c.fev,
                        Valor03 = c.mar,
                    }
                    ).ToList()
            })
            .ToList();

        //agrupamento superior do valor total
        var upper = new CategoriaReportItemDto
        {
            Descricao = $"{tipo.ObterDescricao()}s",
            Valor01 = catGroup.Sum(c => c.Valor01),
            Valor02 = catGroup.Sum(c => c.Valor02),
            Valor03 = catGroup.Sum(c => c.Valor03),
            Items = catGroup.ToList()
        };

        return upper;
    }
}
