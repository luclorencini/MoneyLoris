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
    private readonly ICategoriaValidator _validator;
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly ISubcategoriaRepository _subcategoriaRepo;
    private readonly IAuthenticationManager _authenticationManager;

    public CategoriaService(
        ICategoriaValidator validator,
        ICategoriaRepository categoriaRepo,
        ISubcategoriaRepository subcategoriaRepo,
        IAuthenticationManager authenticationManager)
    {
        _validator = validator;
        _categoriaRepo = categoriaRepo;
        _subcategoriaRepo = subcategoriaRepo;
        _authenticationManager = authenticationManager;
    }

    public async Task<Result<ICollection<CategoriaCadastroListItemDto>>> ListarCategoriasUsuario(TipoLancamento tipo)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var categorias = await _categoriaRepo.ListarCategoriasUsuario(tipo, userInfo.Id);

        ICollection<CategoriaCadastroListItemDto> ret =
            categorias.Select(c => new CategoriaCadastroListItemDto(c))
            .ToList();

        return new Result<ICollection<CategoriaCadastroListItemDto>>(ret);
    }


    public async Task<Result<CategoriaCadastroDto>> ObterCategoria(int id)
    {
        var categoria = await _categoriaRepo.GetById(id);

        _validator.Existe(categoria);
        _validator.PertenceAoUsuario(categoria);

        var dto = new CategoriaCadastroDto(categoria);

        return dto;
    }

    public async Task<Result<int>> InserirCategoria(CategoriaCadastroDto dto)
    {
        _validator.NaoEhAdmin();

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var categoria = new Categoria
        {
            Tipo = dto.Tipo,
            Nome = dto.Nome,
            Ordem = dto.Ordem,
            IdUsuario = userInfo.Id
        };

        _validator.EstaConsistente(categoria);

        categoria = await _categoriaRepo.Insert(categoria);

        return (categoria.Id, "Categoria criada com sucesso.");
    }

    public async Task<Result<int>> AlterarCategoria(CategoriaCadastroDto dto)
    {
        _validator.NaoEhAdmin();

        var categoria = await _categoriaRepo.GetById(dto.Id);

        _validator.Existe(categoria);
        _validator.PertenceAoUsuario(categoria);
        _validator.NaoPodeAlterarTipo(categoria, dto.Tipo);

        categoria.Nome = dto.Nome;
        categoria.Ordem = dto.Ordem;

        _validator.EstaConsistente(categoria);

        await _categoriaRepo.Update(categoria);

        return (categoria.Id, "Categoria alterada com sucesso.");
    }

    public async Task<Result<int>> ExcluirCategoria(int id)
    {
        _validator.NaoEhAdmin();

        var categoria = await _categoriaRepo.GetById(id);

        _validator.Existe(categoria);
        _validator.PertenceAoUsuario(categoria);

        await _categoriaRepo.Delete(id);

        return (id, "Categoria excluída com sucesso.");
    }


    public async Task<Result<SubcategoriaCadastroDto>> ObterSubcategoria(int id)
    {
        var subcat = await _subcategoriaRepo.GetById(id);

        _validator.Existe(subcat);

        var dto = new SubcategoriaCadastroDto(subcat);

        return dto;
    }



    public async Task<Result<int>> InserirSubcategoria(SubcategoriaCadastroDto dto)
    {
        _validator.NaoEhAdmin();

        var categoria = await _categoriaRepo.GetById(dto.IdCategoria);

        _validator.Existe(categoria);
        _validator.PertenceAoUsuario(categoria);

        var subcat = new Subcategoria
        {
            IdCategoria = dto.IdCategoria,
            Nome = dto.Nome,
            Ordem = dto.Ordem
        };

        _validator.EstaConsistente(subcat);

        subcat = await _subcategoriaRepo.Insert(subcat);

        return (subcat.Id, "Subcategoria criada com sucesso.");
    }

    public async Task<Result<int>> AlterarSubcategoria(SubcategoriaCadastroDto dto)
    {
        _validator.NaoEhAdmin();

        var categoria = await _categoriaRepo.GetById(dto.IdCategoria);

        _validator.Existe(categoria);
        _validator.PertenceAoUsuario(categoria);

        var subcat = await _subcategoriaRepo.GetById(dto.Id);

        _validator.Existe(subcat);
        _validator.PertenceACategoria(subcat, categoria);

        subcat.Nome = dto.Nome;
        subcat.Ordem = dto.Ordem;

        _validator.EstaConsistente(subcat);

        await _subcategoriaRepo.Update(subcat);

        return (subcat.Id, "Subcategoria alterada com sucesso.");
    }

    public async Task<Result<int>> ExcluirSubcategoria(int id)
    {
        _validator.NaoEhAdmin();

        var subcat = await _subcategoriaRepo.GetById(id);

        _validator.Existe(subcat);

        var categoria = await _categoriaRepo.GetById(subcat.IdCategoria);

        _validator.Existe(categoria);
        _validator.PertenceAoUsuario(categoria);

        _validator.PertenceACategoria(subcat, categoria);

        await _subcategoriaRepo.Delete(id);

        return (id, "Subcategoria excluída com sucesso.");
    }

    public async Task<Result<ICollection<CategoriaListItemDto>>> ObterCategoriasUsuario(TipoLancamento tipo)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var categorias = await _categoriaRepo.ListarCategoriasUsuario(tipo, userInfo.Id);

        ICollection<CategoriaListItemDto> ret = new List<CategoriaListItemDto>();

        foreach (var c in categorias)
        {
            ret.Add(new CategoriaListItemDto { CategoriaId = c.Id, CategoriaNome = c.Nome });

            foreach (var s in c.Subcategorias)
            {
                ret.Add(new CategoriaListItemDto { CategoriaId = c.Id, CategoriaNome = c.Nome, SubcategoriaId = s.Id, SubcategoriaNome = s.Nome });
            }

        }

        return new Result<ICollection<CategoriaListItemDto>>(ret);
    }
}
