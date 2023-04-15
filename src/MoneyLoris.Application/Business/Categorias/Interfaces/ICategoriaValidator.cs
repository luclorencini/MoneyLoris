using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Categorias.Interfaces;
public interface ICategoriaValidator
{
    void NaoEhAdmin();
    void Existe(Categoria categoria);
    void PertenceAoUsuario(Categoria categoria);
    void EstaConsistente(Categoria categoria);
    void NaoPodeAlterarTipo(Categoria categoria, TipoLancamento tipoSugerido);


    void Existe(Subcategoria subcat);
    void PertenceACategoria(Subcategoria subcat, Categoria categoria);
    void EstaConsistente(Subcategoria subcat);
}
