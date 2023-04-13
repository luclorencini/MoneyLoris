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
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly ISubcategoriaRepository _subcategoriaRepo;
    private readonly IAuthenticationManager _authenticationManager;

    public CategoriaService(
        ICategoriaRepository categoriaRepo,
        ISubcategoriaRepository subcategoriaRepo,
        IAuthenticationManager authenticationManager)
    {
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
        var categoria = await obterCategoria(id);

        var dto = new CategoriaCadastroDto(categoria);

        return dto;
    }

    public async Task<Result<int>> InserirCategoria(CategoriaCadastroDto dto)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        if (userInfo.IsAdmin)
            throw new BusinessException(
                code: ErrorCodes.Categoria_AdminNaoPode,
                message: "Administradores não possuem categorias.");

        var categoria = new Categoria
        {
            Tipo = dto.Tipo,
            Nome = dto.Nome,
            Ordem = dto.Ordem,
            IdUsuario = userInfo.Id
        };

        categoria = await _categoriaRepo.Insert(categoria);

        return (categoria.Id, "Categoria criada com sucesso.");
    }

    public async Task<Result<int>> AlterarCategoria(CategoriaCadastroDto dto)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        if (userInfo.IsAdmin)
            throw new BusinessException(
                code: ErrorCodes.Categoria_AdminNaoPode,
                message: "Administradores não possuem categorias.");

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

        await _categoriaRepo.Update(categoria);

        return (categoria.Id, "Categoria alterada com sucesso.");
    }

    public async Task<Result<int>> ExcluirCategoria(int id)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        if (userInfo.IsAdmin)
            throw new BusinessException(
                code: ErrorCodes.Categoria_AdminNaoPode,
                message: "Administradores não possuem categorias.");

        var categoria = await obterCategoria(id);

        if (categoria.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Categoria_NaoPertenceAoUsuario,
                message: "Categoria não pertence ao usuário.");

        await _categoriaRepo.Delete(id);

        return (id, "Categoria excluída com sucesso.");
    }

    private async Task<Categoria> obterCategoria(int id)
    {
        var categoria = await _categoriaRepo.GetById(id);

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
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        if (userInfo.IsAdmin)
            throw new BusinessException(
                code: ErrorCodes.Categoria_AdminNaoPode,
                message: "Administradores não possuem categorias.");

        var categoria = await obterCategoria(dto.IdCategoria);

        if (categoria.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Categoria_NaoPertenceAoUsuario,
                message: "Categoria não pertence ao usuário.");


        var subcat = new Subcategoria
        {
            IdCategoria = dto.IdCategoria,
            Nome = dto.Nome,
            Ordem = dto.Ordem
        };

        subcat = await _subcategoriaRepo.Insert(subcat);

        return (subcat.Id, "Subcategoria criada com sucesso.");
    }

    public async Task<Result<int>> AlterarSubcategoria(SubcategoriaCadastroDto dto)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        if (userInfo.IsAdmin)
            throw new BusinessException(
                code: ErrorCodes.Categoria_AdminNaoPode,
                message: "Administradores não possuem categorias.");

        var categoria = await obterCategoria(dto.IdCategoria);

        if (categoria.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Categoria_NaoPertenceAoUsuario,
                message: "Categoria não pertence ao usuário.");

        var subcat = await obterSubcategoria(dto.Id);

        if (subcat.IdCategoria != categoria.Id)
            throw new BusinessException(
                code: ErrorCodes.Subcategoria_NaoPertenceACategoria,
                message: "Subcategoria não pertence à categoria informada");

        subcat.Nome = dto.Nome;
        subcat.Ordem = dto.Ordem;

        await _subcategoriaRepo.Update(subcat);

        return (subcat.Id, "Subcategoria alterada com sucesso.");
    }

    public async Task<Result<int>> ExcluirSubcategoria(int id)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        if (userInfo.IsAdmin)
            throw new BusinessException(
                code: ErrorCodes.Categoria_AdminNaoPode,
                message: "Administradores não possuem categorias.");

        var subcat = await _subcategoriaRepo.GetById(id);

        if (subcat == null)
            throw new BusinessException(
                code: ErrorCodes.Subcategoria_NaoEncontrada,
                message: "Subcategoria não encontrada");

        var categoria = await _categoriaRepo.GetById(subcat.IdCategoria);

        if (categoria.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Categoria_NaoPertenceAoUsuario,
                message: "Categoria não pertence ao usuário.");


        await _subcategoriaRepo.Delete(id);

        return (id, "Subcategoria excluída com sucesso.");
    }

    private async Task<Subcategoria> obterSubcategoria(int id)
    {
        var subcat = await _subcategoriaRepo.GetById(id);

        if (subcat == null)
            throw new BusinessException(
                code: ErrorCodes.Subcategoria_NaoEncontrada,
                message: "Subcategoria não encontrada");

        return subcat;
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
