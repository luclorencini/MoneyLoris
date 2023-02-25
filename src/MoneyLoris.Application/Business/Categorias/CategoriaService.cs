using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Categorias;
public class CategoriaService : ServiceBase, ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly ISubcategoriaRepository _subcategoriaRepository;
    private readonly IAuthenticationManager _authenticationManager;

    public CategoriaService(
        ICategoriaRepository categoriaRepository,
        ISubcategoriaRepository subcategoriaRepository,
        IAuthenticationManager authenticationManager)
    {
        _categoriaRepository = categoriaRepository;
        _subcategoriaRepository = subcategoriaRepository;
        _authenticationManager = authenticationManager;
    }

    public async Task<Result<ICollection<CategoriaCadastroListItemDto>>> ListarCategoriasUsuario(TipoLancamento tipo)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var categorias = await _categoriaRepository.ListarCategoriasUsuario(tipo, userInfo.Id);

        ICollection<CategoriaCadastroListItemDto> ret =
            categorias.Select(c => new CategoriaCadastroListItemDto(c))
            .ToList();

        return new Result<ICollection<CategoriaCadastroListItemDto>>(ret);
    }


    public async Task<Result<CategoriaCadastroDto>> ObterCategoria(int id)
    {
        var categoria = await obterCategoria(id);

        var dto = new CategoriaCadastroDto(categoria);

        return dto;
    }

    public async Task<Result<int>> InserirCategoria(CategoriaCadastroDto dto)
    {
        //TODO - regras de validação

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var categoria = new Categoria
        {
            Tipo = dto.Tipo,
            Nome = dto.Nome,
            Ordem = dto.Ordem,
            IdUsuario = userInfo.Id
        };

        categoria = await _categoriaRepository.Insert(categoria);

        return (categoria.Id, "Categoria criada com sucesso.");

    }
    public async Task<Result<int>> AlterarCategoria(CategoriaCadastroDto dto)
    {
        //TODO - regras de validação

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var categoria = await obterCategoria(dto.Id);

        if (categoria.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Categoria_NaoPertenceAoUsuario,
                message: "Categoria não pertence ao usuário.");

        if (categoria.Tipo != dto.Tipo)
            throw new BusinessException(
                code: ErrorCodes.Categoria_NaoPodeAlterarTipo,
                message: "Categoria não pode ter seu tipo alterado.");

        categoria.Nome = dto.Nome;
        categoria.Ordem = dto.Ordem;

        await _categoriaRepository.Update(categoria);

        return (categoria.Id, "Categoria alterada com sucesso.");

    }

    public async Task<Result<int>> ExcluirCategoria(int id)
    {
        //TODO - regras de validação

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var categoria = await obterCategoria(id);

        if (categoria.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Categoria_NaoPertenceAoUsuario,
                message: "Categoria não pertence ao usuário.");

        await _categoriaRepository.Delete(id);

        return (id, "Categoria excluída com sucesso.");
    }

    private async Task<Categoria> obterCategoria(int id)
    {
        var categoria = await _categoriaRepository.GetById(id);

        if (categoria == null)
            throw new BusinessException(
                code: ErrorCodes.Categoria_NaoEncontrada,
                message: "Categoria não encontrada");

        return categoria;
    }



    public async Task<Result<SubcategoriaCadastroDto>> ObterSubcategoria(int id)
    {
        var subcat = await obterSubcategoria(id);

        var dto = new SubcategoriaCadastroDto(subcat);

        return dto;
    }



    public async Task<Result<int>> InserirSubcategoria(SubcategoriaCadastroDto dto)
    {
        //TODO - regras de validação

        var subcat = new Subcategoria
        {
            IdCategoria = dto.IdCategoria,
            Nome = dto.Nome,
            Ordem = dto.Ordem
        };

        subcat = await _subcategoriaRepository.Insert(subcat);

        return (subcat.Id, "Subcategoria criada com sucesso.");
    }

    public async Task<Result<int>> AlterarSubcategoria(SubcategoriaCadastroDto dto)
    {
        //TODO - regras de validação

        var subcat = await obterSubcategoria(dto.Id);

        subcat.Nome = dto.Nome;
        subcat.Ordem = dto.Ordem;

        await _subcategoriaRepository.Update(subcat);

        return (subcat.Id, "Subcategoria alterada com sucesso.");
    }

    public async Task<Result<int>> ExcluirSubcategoria(int id)
    {
        //TODO - regras de validação

        await _subcategoriaRepository.Delete(id);

        return (id, "Subcategoria excluída com sucesso.");
    }

    private async Task<Subcategoria> obterSubcategoria(int id)
    {
        var subcat = await _subcategoriaRepository.GetById(id);

        if (subcat == null)
            throw new BusinessException(
                code: ErrorCodes.Subcategoria_NaoEncontrada,
                message: "Subcategoria não encontrada");

        return subcat;
    }

    public Task<Result<ICollection<CategoriaListItemDto>>> ObterCategoriasUsuario(TipoLancamento tipo)
    {
        //TODO
        throw new NotImplementedException();
    }
}
