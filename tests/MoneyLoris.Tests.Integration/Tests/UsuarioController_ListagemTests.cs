using System.Net.Http.Json;
using MoneyLoris.Application.Business.Usuarios.Dtos;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests;
public class UsuarioController_ListagemTests : IntegrationTestsBase
{
    private void ArrangeDadosListagem()
    {
        //cria 40 usuários na base, do 'Fulano 01' a 'Fulano 40'

        var usuarios = new List<Usuario>();

        for (int i = 0; i < 40; i++)
        {
            usuarios.Add(
                new Usuario
                {
                    Nome = $"Fulano {(i + 1).ToString("D2")}",
                    Login = $"login.{(i + 1).ToString("D2")}",
                    Senha = "abc",
                    DataCriacao = DateTime.Now,
                    Ativo = (i % 5 != 4), // para cada 4 ativos, 1 inativo (05, 10, 15)
                    IdPerfil = (i % 3 == 0 ? PerfilUsuario.Administrador : PerfilUsuario.Usuario), // para cada 1 admin, 2 usuarios comuns (01, 04, 07)
                });
        }

        //invertendo a ordem da lista para testar a ordenação por nome
        usuarios = usuarios.OrderByDescending(c => c.Nome).ToList();

        Context.Usuarios.AddRange(usuarios);
        Context.SaveChanges();
    }

    [Fact]
    public async Task Pesquisar_SemFiltro_TrazPrimeiraPagina()
    {
        await ExecutarConsulta(new UsuarioPesquisaDto { },
            40, 25, "Fulano 01", "Fulano 25");
    }

    [Fact]
    public async Task Pesquisar_SemFiltro_TrazSegundaPagina()
    {
        await ExecutarConsulta(new UsuarioPesquisaDto { CurrentPage = 2 },
            40, 15, "Fulano 26", "Fulano 40");
    }

    [Fact]
    public async Task Pesquisar_FiltroNome_Retorna()
    {
        await ExecutarConsulta(new UsuarioPesquisaDto { Nome = "ulano 3" },
            10, 10, "Fulano 30", "Fulano 39");
    }

    [Fact]
    public async Task Pesquisar_FiltroAtivo_Retorna()
    {
        await ExecutarConsulta(new UsuarioPesquisaDto { Ativo = true },
            32, 25, "Fulano 01", "Fulano 31");
    }

    [Fact]
    public async Task Pesquisar_FiltroInativo_Retorna()
    {
        await ExecutarConsulta(new UsuarioPesquisaDto { Ativo = false },
            8, 8, "Fulano 05", "Fulano 40");
    }

    [Fact]
    public async Task Pesquisar_FiltroAdministrador_Retorna()
    {
        await ExecutarConsulta(new UsuarioPesquisaDto { IdPerfil = PerfilUsuario.Administrador },
            14, 14, "Fulano 01", "Fulano 40");
    }

    [Fact]
    public async Task Pesquisar_FiltroPsicologo_Retorna()
    {
        await ExecutarConsulta(new UsuarioPesquisaDto { IdPerfil = PerfilUsuario.Usuario },
            26, 25, "Fulano 02", "Fulano 38");
    }

    [Fact]
    public async Task Pesquisar_FiltroComposto_Retorna()
    {
        await ExecutarConsulta(
            new UsuarioPesquisaDto
            {
                Nome = "ulano 1",
                Ativo = true,
                IdPerfil = PerfilUsuario.Administrador
            },
            3, 3, "Fulano 13", "Fulano 19");
    }


    private async Task ExecutarConsulta(
        UsuarioPesquisaDto filtro,
        long totalRegistros, long paginaRegistros,
        string primeiroNome, string ultimoNome
    )
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        ArrangeDadosListagem();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/pesquisar", filtro);

        //Assert
        var pag = await response.AssertResultOk<Pagination<ICollection<UsuarioListItemDto>>>();

        var list = pag.DataPage;

        Assert.Equal(totalRegistros, pag.Total);
        Assert.Equal(paginaRegistros, list.Count);
        Assert.Equal(primeiroNome, list.First().Nome);
        Assert.Equal(ultimoNome, list.Last().Nome);
    }

}
