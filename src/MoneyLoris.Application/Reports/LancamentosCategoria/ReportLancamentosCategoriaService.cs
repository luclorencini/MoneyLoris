using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
using MoneyLoris.Application.Shared;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Reports.LancamentosCategoria;
public class ReportLancamentosCategoriaService : ServiceBase, IReportLancamentosCategoriaService
{
    private readonly IReportLancamentosCategoriaRepository _reportRepo;
    private readonly IAuthenticationManager _authenticationManager;

    public ReportLancamentosCategoriaService(IReportLancamentosCategoriaRepository reportRepo, IAuthenticationManager authenticationManager)
    {
        _reportRepo = reportRepo;
        _authenticationManager = authenticationManager;
    }

    #region Consolidado

    public Result<ICollection<CategoriaReportItemDto>> LancamentosPorCategoriaConsolidado(ReportLancamentoFilterDto filtro)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var despesas = this.GetDadosRelatorioTipoLancamento(userInfo.Id, TipoLancamento.Despesa, filtro.Mes, filtro.Ano, filtro.Quantidade);
        var receitas = this.GetDadosRelatorioTipoLancamento(userInfo.Id, TipoLancamento.Receita, filtro.Mes, filtro.Ano, filtro.Quantidade);

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
                Valor01 = g.Sum(r => r.val01),
                Valor02 = g.Sum(r => r.val02),
                Valor03 = g.Sum(r => r.val03),
                Valor04 = g.Sum(r => r.val04),
                Valor05 = g.Sum(r => r.val05),
                Valor06 = g.Sum(r => r.val06),
                Valor07 = g.Sum(r => r.val07),
                Valor08 = g.Sum(r => r.val08),
                Valor09 = g.Sum(r => r.val09),
                Valor10 = g.Sum(r => r.val10),
                Valor11 = g.Sum(r => r.val11),
                Valor12 = g.Sum(r => r.val12),

                //agrupamento das subcategorias
                Items = g
                    .Select(c => new CategoriaReportItemDto
                    {
                        IdCategoria = c.catId,
                        IdSubcategoria = c.subId,
                        Descricao = c.subNome!,
                        Valor01 = c.val01,
                        Valor02 = c.val02,
                        Valor03 = c.val03,
                        Valor04 = c.val04,
                        Valor05 = c.val05,
                        Valor06 = c.val06,
                        Valor07 = c.val07,
                        Valor08 = c.val08,
                        Valor09 = c.val09,
                        Valor10 = c.val10,
                        Valor11 = c.val11,
                        Valor12 = c.val12,
                    }
                    ).ToList()
            })
            .ToList();

        //ajustes categoria - retira lista de subcategorias com apenas 1 elemento (a propria categoria criada no groupby)
        foreach (var c in catGroup)
        {
            if (c.Items != null && c.Items.Count == 1)
                c.Items = null;
        }

        //agrupamento superior do valor total
        var upper = new CategoriaReportItemDto
        {
            Descricao = $"{tipo.ObterDescricao()}s",
            Valor01 = catGroup.Sum(c => c.Valor01 != 0 ? c.Valor01 : null),
            Valor02 = catGroup.Sum(c => c.Valor02),
            Valor03 = catGroup.Sum(c => c.Valor03),
            Valor04 = catGroup.Sum(c => c.Valor04),
            Valor05 = catGroup.Sum(c => c.Valor05),
            Valor06 = catGroup.Sum(c => c.Valor06),
            Valor07 = catGroup.Sum(c => c.Valor07),
            Valor08 = catGroup.Sum(c => c.Valor08),
            Valor09 = catGroup.Sum(c => c.Valor09),
            Valor10 = catGroup.Sum(c => c.Valor10),
            Valor11 = catGroup.Sum(c => c.Valor11),
            Valor12 = catGroup.Sum(c => c.Valor12),
            Items = catGroup.ToList()
        };

        //ajustes finais
        ValorNullSeZero(upper);

        return upper;
    }

    private void ValorNullSeZero(CategoriaReportItemDto c)
    {
        if (c.Valor01 == 0) c.Valor01 = null;
        if (c.Valor02 == 0) c.Valor02 = null;
        if (c.Valor03 == 0) c.Valor03 = null;
        if (c.Valor04 == 0) c.Valor04 = null;
        if (c.Valor05 == 0) c.Valor05 = null;
        if (c.Valor06 == 0) c.Valor06 = null;
        if (c.Valor07 == 0) c.Valor07 = null;
        if (c.Valor08 == 0) c.Valor08 = null;
        if (c.Valor09 == 0) c.Valor09 = null;
        if (c.Valor10 == 0) c.Valor10 = null;
        if (c.Valor11 == 0) c.Valor11 = null;
        if (c.Valor12 == 0) c.Valor12 = null;

        if (c.Items != null)
        {
            foreach (var i in c.Items)
            {
                ValorNullSeZero(i);
            }
        }
    }

    #endregion

    public async Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> PesquisarDetalhe(ReportLancamentoDetalheFilterDto filtro)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        //pega o total
        var total = await _reportRepo.DetalheTotalRegistros(filtro, userInfo.Id);

        //faz a consulta paginada
        var lancamentos = await _reportRepo.DetalhePaginado(filtro, userInfo.Id);

        //transforma no tipo de retorno
        ICollection<LancamentoListItemDto> ret =
            lancamentos.Select(l => new LancamentoListItemDto(l)).ToList();

        return Pagination(pagedData: ret, total: total);
    }

    public async Task<Result<decimal>> ObterDetalheSomatorio(ReportLancamentoDetalheFilterDto filtro)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var somatorio = await _reportRepo.DetalheSomatorio(filtro, userInfo.Id);

        return somatorio;
    }

}
