using MoneyLoris.Application.Business.Usuarios;
using MoneyLoris.Application.Business.Usuarios.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Stubs;
public class UsuarioServiceStub : ServiceBase, IUsuarioService
{
    public Task<Result<Pagination<ICollection<UsuarioListItemDto>>>> Pesquisar(UsuarioPesquisaDto filtro)
    {
        var usuarios = MockLista();

        if (!String.IsNullOrWhiteSpace(filtro.Nome))
            usuarios = usuarios.Where(c => c.Nome.ToUpper().Contains(filtro.Nome.ToUpper())).ToList();

        if (filtro.IdPerfil.HasValue)
            usuarios = usuarios.Where(c => c.IdPerfil == filtro.IdPerfil.Value).ToList();

        if (filtro.Ativo.HasValue)
            usuarios = usuarios.Where(c => c.Ativo).ToList();

        var totalFiltrado = usuarios.Count();

        usuarios = usuarios.Skip(filtro.ResultsPerPage * (filtro.CurrentPage - 1)).Take(filtro.ResultsPerPage).ToList();

        ICollection<UsuarioListItemDto> ret = usuarios.Select(c => new UsuarioListItemDto(c)).ToList();

        return TaskSuccess(dataPage: ret, total: totalFiltrado);
    }

    public Task<Result<int>> CriarUsuario(UsuarioCriacaoInputDto dto)
    {
        return TaskSuccess((123, "Usuário criado com sucesso."));
    }

    public Task<Result<UsuarioInfoDto>> ObterUsuario(int idUsuario)
    {
        var info = new UsuarioInfoDto
        {
            Id = 123,
            Nome = "Mariana Rangel Lorencini",
            IdPerfil = (byte)PerfilUsuario.Usuario,
            Perfil = PerfilUsuario.Usuario.ObterDescricao(),
            Login = "mari.rangel",
            UltimoLogin = DateTime.Today.AddDays(-5),
            DataInativacao = DateTime.Today.AddDays(-2)
        };

        return TaskSuccess(info);
    }

    public Task<Result<int>> AlterarUsuario(UsuarioAlteracaoDto dto)
    {
        return TaskSuccess((123, "Usuário alterado com sucesso."));
    }

    public Task<Result<int>> MarcarParaRedefinirSenha(int idUsuario)
    {
        return TaskSuccess((123, "Senha redefinida para a padrão."));
    }

    public Task<Result<int>> InativarUsuario(int idUsuario)
    {
        return TaskSuccess((123, "Usuário inativado com sucesso."));
    }

    public Task<Result<int>> ReativarUsuario(int idUsuario)
    {
        return TaskSuccess((123, "Usuário reativado com sucesso."));
    }

    public Task<Result<int>> ExcluirUsuario(int idUsuario)
    {
        return TaskSuccess((123, "Usuário excluído com sucesso."));
    }


    private ICollection<Usuario> MockLista()
    {
        var usuarios = new List<Usuario>() {
            new Usuario { Id = 10, Ativo = true, UltimoLogin = DateTime.Today.AddDays(-1), IdPerfil = PerfilUsuario.Administrador, Nome = "Luciano Silva Lorencini" },
            new Usuario { Id = 11, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Grace Rangel Felizardo Lorencini" },
            new Usuario { Id = 13, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Mariana Rangel Lorencini" },
            new Usuario { Id = 14, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Carlos Eduardo Freitas" },
            new Usuario { Id = 15, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Sophia Duarte Lima" },
            new Usuario { Id = 16, Ativo = false, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Maria Vitória Monteiro" },
            new Usuario { Id = 17, Ativo = false, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Pietro Duarte Ramos Cavalcante de Menezes" },
            new Usuario { Id = 18, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Enzo Gabriel Jesus" },
            new Usuario { Id = 19, Ativo = true, IdPerfil = PerfilUsuario.Usuario, Nome = "Caroline Barbosa Schwarzkopf" },
            new Usuario { Id = 20, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Vinicius Novaes de Oliveira" },
            new Usuario { Id = 21, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Daniela da Rocha Couto" },
            new Usuario { Id = 22, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Caio Campos" },
            new Usuario { Id = 23, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Administrador, Nome = "Nicolas Gonçalves" },
            new Usuario { Id = 24, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Leonardo Rodrigues" },
            new Usuario { Id = 25, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Esther Araújo" },
            new Usuario { Id = 26, Ativo = false, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Rebeca Mendes" },
            new Usuario { Id = 27, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "João Lucas Lima" },
            new Usuario { Id = 28, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Ana Carolina Lima" },
            new Usuario { Id = 29, Ativo = true, IdPerfil = PerfilUsuario.Usuario, Nome = "Felipe Moura Martinelli" },
            new Usuario { Id = 30, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Diogo Costa do Nascimento" },
            new Usuario { Id = 31, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Ana Lívia Azevedo" },
            new Usuario { Id = 32, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "João Felipe Cavalcanti" },
            new Usuario { Id = 33, Ativo = false, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Marcos Vinicius Jesus" },
            new Usuario { Id = 34, Ativo = false, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Igor Sales Ricoccelli" },
            new Usuario { Id = 35, Ativo = false, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Marcelo da Cunha" },
            new Usuario { Id = 36, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Enrico da Paz Miraflores" },
            new Usuario { Id = 37, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Larissa Cândido Cunha" },
            new Usuario { Id = 38, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Anthony de Aragão e Castela" },
            new Usuario { Id = 39, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Ana Luiza Gomes" },
            new Usuario { Id = 40, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Pedro Henrique Souza" },
            new Usuario { Id = 41, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Administrador, Nome = "Ana Sophia Pires" },
            new Usuario { Id = 42, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Juan da Mata dos Anjos" },
            new Usuario { Id = 43, Ativo = false, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Luiza Pires Souto Maior" },
            new Usuario { Id = 44, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Mariana Carvalho" },
            new Usuario { Id = 45, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Theo da Cunha Vervloet" },
            new Usuario { Id = 46, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Pietro Ferreira" },
            new Usuario { Id = 47, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Sarah Campos" },
            new Usuario { Id = 48, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Valentina Gomes Bruck" },
            new Usuario { Id = 49, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Sofia Novaes" },
            new Usuario { Id = 50, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Clara Dias" },
            new Usuario { Id = 51, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Kamilly Rangel Borges" },
            new Usuario { Id = 52, Ativo = false, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Igor Gomes" },
            new Usuario { Id = 53, Ativo = false, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Maria Sophia Caldeira" },
            new Usuario { Id = 54, Ativo = false, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Maria Eduarda Araújo" },
            new Usuario { Id = 55, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Thiago da Paz" },
            new Usuario { Id = 56, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Lara da Mota" },
            new Usuario { Id = 57, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "João Oliveira" },
            new Usuario { Id = 58, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Administrador, Nome = "Luigi Novaes" },
            new Usuario { Id = 59, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Leandro da Cunha" },
            new Usuario { Id = 60, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Clarice Cavalcanti" },
            new Usuario { Id = 61, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Enzo Constantino Macchiatti" },
            new Usuario { Id = 62, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Vitória Nunes" },
            new Usuario { Id = 63, Ativo = true, UltimoLogin = DateTime.Today, IdPerfil = PerfilUsuario.Usuario, Nome = "Ana Luiza Schneck da Paz" }
        };

        return usuarios;
    }
}
