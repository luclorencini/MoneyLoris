using MoneyLoris.Application.Business.Contas;
using MoneyLoris.Application.Business.Contas.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Stubs;
public class ContaServiceStub : ServiceBase, IContaService
{
    public async Task<Result<ICollection<ContaCadastroListItemDto>>> Listar()
    {
        ICollection<ContaCadastroListItemDto> ret = new List<ContaCadastroListItemDto>()
        {
            new ContaCadastroListItemDto { Id = 301, Nome = "Carteira", Tipo = TipoConta.Carteira, Ativo = true, Saldo = 45 },
            new ContaCadastroListItemDto { Id = 302, Nome = "Nubank", Tipo = TipoConta.CartaoCredito, Cor = "820AD1", Ordem = 1, Ativo = true, Saldo = -7846.33 },
            new ContaCadastroListItemDto { Id = 303, Nome = "PicPay", Tipo = TipoConta.CarteiraDigital, Cor = "11C56E", Ordem = 2, Ativo = true, Saldo = 275.90 },
            new ContaCadastroListItemDto { Id = 304, Nome = "Caixa", Tipo = TipoConta.ContaCorrente, Cor = "0369B9", Ordem = 3, Ativo = true, Saldo = 10873.75 },
            new ContaCadastroListItemDto { Id = 305, Nome = "NuConta", Tipo = TipoConta.ContaPagamento, Cor = "820AD1", Ordem = 4, Ativo = true, Saldo = 2198.54 },
            new ContaCadastroListItemDto { Id = 306, Nome = "Sodexo", Tipo = TipoConta.ContaPagamento, Cor = "FF0000", Ordem = 5, Ativo = false, Saldo = 137 },
            new ContaCadastroListItemDto { Id = 307, Nome = "Banco do Brasil", Tipo = TipoConta.Poupanca, Cor = "FCF800", Ordem = 6, Ativo = true, Saldo = 3263.98 }
        };

        ret = ret.OrderBy(x => x.Ativo).ThenBy(x => x.Ordem).ToList();

        return await TaskSuccess(ret);
    }

    public async Task<Result<ContaCadastroDto>> Obter(int id)
    {
        var conta = new ContaCadastroDto()
        {
            Id = 301,
            Nome = "Nubank",
            Tipo = TipoConta.CartaoCredito,
            Cor = "820AD1",
            Ordem = 1,
            Ativo = true,

            Limite = 12000,
            DiaFechamento = 1,
            DiaVencimento = 10
        };


        return await TaskSuccess(conta);
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
