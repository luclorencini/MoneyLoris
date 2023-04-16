using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos;
public class LancamentoConsultaService : ServiceBase, ILancamentoConsultaService
{
    private readonly ILancamentoRepository _lancamentoRepo;
    private readonly IAuthenticationManager _authenticationManager;

    public LancamentoConsultaService(
        ILancamentoRepository lancamentoRepo,
        IAuthenticationManager authenticationManager)
    {
        _lancamentoRepo = lancamentoRepo;
        _authenticationManager = authenticationManager;
    }

    public async Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(LancamentoFiltroDto filtro)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        //pega o total
        var total = await _lancamentoRepo.PesquisaTotalRegistros(filtro, userInfo.Id);

        //faz a consulta paginada
        var lancamentos = await _lancamentoRepo.PesquisaPaginada(filtro, userInfo.Id);

        //transforma no tipo de retorno
        ICollection<LancamentoListItemDto> ret =
            lancamentos.Select(l => new LancamentoListItemDto(l)).ToList();

        return Pagination(pagedData: ret, total: total);
    }

    public async Task<Result<LancamentoBalancoDto>> ObterBalanco(LancamentoFiltroDto filtro)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var receita = await _lancamentoRepo.SomatorioReceitasFiltro(filtro, userInfo.Id);
        var despesa = await _lancamentoRepo.SomatorioDespesasFiltro(filtro, userInfo.Id);
        var balanco = receita + despesa;

        var ret = new LancamentoBalancoDto
        {
            Receitas = receita,
            Despesas = despesa,
            Balanco = balanco
        };

        return ret;
    }

    public async Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesDespesas(string termoBusca)
    {
        var list = await ObterSugestoes(TipoLancamento.Despesa, termoBusca);
        return new Result<ICollection<LancamentoSugestaoDto>>(list);
    }

    public async Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesReceitas(string termoBusca)
    {
        var list = await ObterSugestoes(TipoLancamento.Receita, termoBusca);
        return new Result<ICollection<LancamentoSugestaoDto>>(list);
    }

    private async Task<ICollection<LancamentoSugestaoDto>> ObterSugestoes(TipoLancamento tipo, string termoBusca)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var lancs = await _lancamentoRepo.ObterLancamentosRecentes(userInfo.Id, tipo, termoBusca);

        //distinct - fazendo group by e pegando o primeiro de cada grupo
        lancs = lancs
          .GroupBy(p => new { p.Descricao, p.IdCategoria, p.IdSubcategoria })
          .Select(g => g.First())
          .ToList();


        var list = lancs.Select(l => new LancamentoSugestaoDto
        {
            Descricao = l.Descricao,
            Categoria = new CategoriaListItemDto
            {
                CategoriaId = l.Categoria != null ? l.Categoria.Id : null,
                CategoriaNome = l.Categoria != null ? l.Categoria.Nome : null,
                SubcategoriaId = l.Subcategoria != null ? l.Subcategoria.Id : null,
                SubcategoriaNome = l.Subcategoria != null ? l.Subcategoria.Nome : null
            }
        }).ToList();

        return list;
    }
}
