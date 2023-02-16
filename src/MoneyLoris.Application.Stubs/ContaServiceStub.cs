using MoneyLoris.Application.Business.Contas;
using MoneyLoris.Application.Business.Contas.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Stubs;
public class ContaServiceStub : ServiceBase, IContaService
{
    //lista mock
    private ICollection<ContaCadastroListItemDto> _contas = new List<ContaCadastroListItemDto>()
        {
            new ContaCadastroListItemDto { Id = 301, Nome = "Carteira", Tipo = (byte)TipoConta.Carteira, Ativo = true, Valor = 45 },
            new ContaCadastroListItemDto { Id = 302, Nome = "Nubank", Tipo = (byte)TipoConta.CartaoCredito, Cor = "820AD1", Ordem = 1, Ativo = true, Valor = 8000 },
            new ContaCadastroListItemDto { Id = 303, Nome = "PicPay", Tipo = (byte)TipoConta.CarteiraDigital, Cor = "11C56E", Ordem = 2, Ativo = true, Valor = 275.9 },
            new ContaCadastroListItemDto { Id = 304, Nome = "Caixa", Tipo = (byte)TipoConta.ContaCorrente, Cor = "0369B9", Ordem = 3, Ativo = true, Valor = 10873.75 },
            new ContaCadastroListItemDto { Id = 305, Nome = "NuConta", Tipo = (byte)TipoConta.ContaPagamento, Cor = "820AD1", Ordem = 4, Ativo = true, Valor = 2198.54 },
            new ContaCadastroListItemDto { Id = 306, Nome = "Sodexo", Tipo = (byte)TipoConta.ContaPagamento, Cor = "FF0000", Ordem = 5, Ativo = false, Valor = 137 },
            new ContaCadastroListItemDto { Id = 307, Nome = "Banco do Brasil", Tipo = (byte)TipoConta.Poupanca, Cor = "FCF800", Ordem = 6, Ativo = true, Valor = 3263.98 }
        };


    public async Task<Result<ICollection<ContaCadastroListItemDto>>> Listar()
    {
        //ICollection<ContaCadastroListItemDto> ret = new List<ContaCadastroListItemDto>()
        //{
        //    new ContaCadastroListItemDto { Id = 301, Nome = "Carteira", Tipo = (byte)TipoConta.Carteira, Ativo = true, Valor = 45 },
        //    new ContaCadastroListItemDto { Id = 302, Nome = "Nubank", Tipo = (byte)TipoConta.CartaoCredito, Cor = "820AD1", Ordem = 1, Ativo = true, Valor = 8000 },
        //    new ContaCadastroListItemDto { Id = 303, Nome = "PicPay", Tipo = (byte)TipoConta.CarteiraDigital, Cor = "11C56E", Ordem = 2, Ativo = true, Valor = 275.90 },
        //    new ContaCadastroListItemDto { Id = 304, Nome = "Caixa", Tipo = (byte)TipoConta.ContaCorrente, Cor = "0369B9", Ordem = 3, Ativo = true, Valor = 10873.75 },
        //    new ContaCadastroListItemDto { Id = 305, Nome = "NuConta", Tipo = (byte)TipoConta.ContaPagamento, Cor = "820AD1", Ordem = 4, Ativo = true, Valor = 2198.54 },
        //    new ContaCadastroListItemDto { Id = 306, Nome = "Sodexo", Tipo = (byte)TipoConta.ContaPagamento, Cor = "FF0000", Ordem = 5, Ativo = false, Valor = 137 },
        //    new ContaCadastroListItemDto { Id = 307, Nome = "Banco do Brasil", Tipo = (byte)TipoConta.Poupanca, Cor = "FCF800", Ordem = 6, Ativo = true, Valor = 3263.98 }
        //};

        var ret = _contas;

        foreach (var c in ret)
            c.TipoDescricao = ((TipoConta)c.Tipo).ObterDescricao();

        ret = ret.OrderByDescending(x => x.Ativo)
            .ThenByDescending(x => x.Ordem.HasValue)
            .ThenBy(x => x.Ordem)
            .ThenBy(x => x.Nome)
            .ToList();

        return await TaskSuccess(ret);
    }

    public async Task<Result<ContaCadastroDto>> Obter(int id)
    {
        ContaCadastroDto contaCadastro = null!;

        var conta = _contas.Where(c => c.Id == id).FirstOrDefault();

        if (conta != null)
        {
            contaCadastro = new ContaCadastroDto()
            {
                Id = conta.Id,
                Nome = conta.Nome,
                Tipo = conta.Tipo,
                Cor = conta.Cor,
                Ordem = conta.Ordem,
                Ativo = conta.Ativo
            };

            if (conta.Tipo == (byte)TipoConta.CartaoCredito)
            {
                contaCadastro.Limite = 12000.90M;
                contaCadastro.DiaFechamento = 1;
                contaCadastro.DiaVencimento = 10;
            }
        }

        return await TaskSuccess(contaCadastro);
    }

    public async Task<Result<int>> Inserir(ContaCriacaoDto model)
    {
        return await TaskSuccess(123);
    }

    public async Task<Result<int>> Alterar(ContaCadastroDto model)
    {
        return await TaskSuccess(123);
    }

    public async Task<Result<int>> Excluir(int id)
    {
        return await TaskSuccess(123);
    }

    public async Task<Result> Inativar(int id)
    {
        return await TaskSuccess();
    }

    public async Task<Result> Reativar(int id)
    {
        return await TaskSuccess();
    }
}
