using MoneyLoris.Application.Business.MeiosPagamento;
using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Stubs;
public class MeioPagamentoServiceStub : ServiceBase, IMeioPagamentoService
{
    //lista mock
    private readonly ICollection<MeioPagamentoCadastroListItemDto> _meios = new List<MeioPagamentoCadastroListItemDto>()
        {
            new MeioPagamentoCadastroListItemDto { Id = 301, Nome = "Carteira", Tipo = (byte)TipoMeioPagamento.Carteira, Ativo = true, Valor = 45 },
            new MeioPagamentoCadastroListItemDto { Id = 302, Nome = "Nubank", Tipo = (byte)TipoMeioPagamento.CartaoCredito, Cor = "820AD1", Ordem = 1, Ativo = true, Valor = 8000 },
            new MeioPagamentoCadastroListItemDto { Id = 303, Nome = "PicPay", Tipo = (byte)TipoMeioPagamento.CarteiraDigital, Cor = "11C56E", Ordem = 2, Ativo = true, Valor = 275.9 },
            new MeioPagamentoCadastroListItemDto { Id = 304, Nome = "Caixa", Tipo = (byte)TipoMeioPagamento.ContaCorrente, Cor = "0369B9", Ordem = 3, Ativo = true, Valor = 10873.75 },
            new MeioPagamentoCadastroListItemDto { Id = 305, Nome = "NuConta", Tipo = (byte)TipoMeioPagamento.ContaPagamento, Cor = "820AD1", Ordem = 4, Ativo = true, Valor = 2198.54 },
            new MeioPagamentoCadastroListItemDto { Id = 306, Nome = "Sodexo", Tipo = (byte)TipoMeioPagamento.ContaPagamento, Cor = "FF0000", Ordem = 5, Ativo = false, Valor = 137 },
            new MeioPagamentoCadastroListItemDto { Id = 307, Nome = "Banco do Brasil", Tipo = (byte)TipoMeioPagamento.Poupanca, Cor = "FCF800", Ordem = 6, Ativo = true, Valor = 3263.98 }
        };


    public async Task<Result<ICollection<MeioPagamentoCadastroListItemDto>>> Listar()
    {
        var ret = _meios;

        foreach (var c in ret)
            c.TipoDescricao = ((TipoMeioPagamento)c.Tipo).ObterDescricao();

        ret = ret.OrderByDescending(x => x.Ativo)
            .ThenByDescending(x => x.Ordem.HasValue)
            .ThenBy(x => x.Ordem)
            .ThenBy(x => x.Nome)
            .ToList();

        return await TaskSuccess(ret);
    }

    public async Task<Result<MeioPagamentoCadastroDto>> Obter(int id)
    {
        MeioPagamentoCadastroDto contaCadastro = null!;

        var conta = _meios.Where(c => c.Id == id).FirstOrDefault();

        if (conta != null)
        {
            contaCadastro = new MeioPagamentoCadastroDto()
            {
                Id = conta.Id,
                Nome = conta.Nome,
                Tipo = conta.Tipo,
                Cor = conta.Cor,
                Ordem = conta.Ordem,
                Ativo = conta.Ativo
            };

            if (conta.Tipo == (byte)TipoMeioPagamento.CartaoCredito)
            {
                contaCadastro.Limite = 12000.90M;
                contaCadastro.DiaFechamento = 1;
                contaCadastro.DiaVencimento = 10;
            }
        }

        return await TaskSuccess(contaCadastro);
    }

    public async Task<Result<int>> Inserir(MeioPagamentoCriacaoDto model)
    {
        return await TaskSuccess(123);
    }

    public async Task<Result<int>> Alterar(MeioPagamentoCadastroDto model)
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
