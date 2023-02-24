using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Business.Usuarios.Dtos;
using MoneyLoris.Application.Business.Usuarios.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Categorias;
public class CategoriaService : ServiceBase, ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IAuthenticationManager _authenticationManager;

    public CategoriaService(ICategoriaRepository categoriaRepository, IAuthenticationManager authenticationManager)
    {
        _categoriaRepository = categoriaRepository;
        _authenticationManager = authenticationManager;
    }

    public async Task<Result<ICollection<CategoriaCadastroListItemDto>>> ListarCategoriasUsuario(TipoLancamento tipo)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var categorias = await _categoriaRepository.ListarCategoriasUsuario(tipo, userInfo.Id);

        ICollection<CategoriaCadastroListItemDto> ret =
            categorias.Select(c => new CategoriaCadastroListItemDto(c)).ToList();

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

        var categoria = new Categoria {
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
        //TODO
        throw new NotImplementedException();
    }

    public async Task<Result<int>> ExcluirCategoria(int id)
    {
        //TODO
        throw new NotImplementedException();
    }

    private async Task<Categoria> obterCategoria(int id)
    {
        var usuario = await _categoriaRepository.GetById(id);

        if (usuario == null)
            throw new BusinessException(
                code: ErrorCodes.Categoria_NaoEncontrada,
                message: "Categoria não encontrada");

        return usuario;
    }



    public Task<Result<SubcategoriaCadastroDto>> ObterSubcategoria(int id)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> InserirSubcategoria(SubcategoriaCadastroDto dto)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> AlterarSubcategoria(SubcategoriaCadastroDto dto)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> ExcluirSubcategoria(int id)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<ICollection<CategoriaListItemDto>>> ObterCategoriasUsuario(TipoLancamento tipo)
    {
        //TODO
        throw new NotImplementedException();
    }
}
