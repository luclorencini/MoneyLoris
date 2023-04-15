using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Categorias;
public class CategoriaValidator : ICategoriaValidator
{
    private readonly IAuthenticationManager _authenticationManager;

    public CategoriaValidator(
        IAuthenticationManager authenticationManager
    )
    {
        _authenticationManager = authenticationManager;
    }

    private UserAuthInfo userInfo => _authenticationManager.ObterInfoUsuarioLogado();

    public void NaoEhAdmin()
    {
        if (userInfo.IsAdmin)
            throw new BusinessException(
                code: ErrorCodes.Categoria_AdminNaoPode,
                message: "Administradores não possuem categorias.");

    }

    #region Categorias

    public void Existe(Categoria categoria)
    {
        if (categoria == null)
            throw new BusinessException(
                code: ErrorCodes.Categoria_NaoEncontrada,
                message: "Categoria não encontrada");
    }

    public void PertenceAoUsuario(Categoria categoria)
    {
        if (categoria.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Categoria_NaoPertenceAoUsuario,
                message: "Categoria não pertence ao usuário.");
    }

    public void EstaConsistente(Categoria categoria)
    {
        if (categoria is null)
            throw new BusinessException(
                code: ErrorCodes.Categoria_CamposObrigatorios,
                message: "Categoria nao informada");

        if (categoria.IdUsuario <= 0)
            throw new BusinessException(
                code: ErrorCodes.Categoria_CamposObrigatorios,
                message: "Usuário nao informado");

        if (String.IsNullOrWhiteSpace(categoria.Nome))
            throw new BusinessException(
                code: ErrorCodes.Categoria_CamposObrigatorios,
                message: "Nome nao informado");
    }

    public void NaoPodeAlterarTipo(Categoria categoria, TipoLancamento tipoSugerido)
    {
        if (categoria.Tipo != tipoSugerido)
            throw new BusinessException(
                code: ErrorCodes.Categoria_NaoPodeAlterarTipo,
                message: "Categoria não pode ter seu tipo alterado.");
    }

    #endregion

    #region Subcategorias

    public void Existe(Subcategoria subcat)
    {
        if (subcat == null)
            throw new BusinessException(
                code: ErrorCodes.Subcategoria_NaoEncontrada,
                message: "Subcategoria não encontrada");
    }

    public void PertenceACategoria(Subcategoria subcat, Categoria categoria)
    {
        if (subcat.IdCategoria != categoria.Id)
            throw new BusinessException(
                code: ErrorCodes.Subcategoria_NaoPertenceACategoria,
                message: "Subcategoria não pertence à categoria informada");
    }

    public void EstaConsistente(Subcategoria subcat)
    {
        if (subcat is null)
            throw new BusinessException(
                code: ErrorCodes.Subcategoria_CamposObrigatorios,
                message: "Subcategoria nao informada");

        if (subcat.IdCategoria <= 0)
            throw new BusinessException(
                code: ErrorCodes.Subcategoria_CamposObrigatorios,
                message: "Categoria nao informada");

        if (String.IsNullOrWhiteSpace(subcat.Nome))
            throw new BusinessException(
                code: ErrorCodes.Subcategoria_CamposObrigatorios,
                message: "Nome nao informado");
    }

    #endregion
}
