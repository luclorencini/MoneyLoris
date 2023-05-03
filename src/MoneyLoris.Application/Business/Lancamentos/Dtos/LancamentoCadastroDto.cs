﻿using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Lancamentos.Dtos;
public class LancamentoCadastroDto
{
    public int Id { get; set; }
    public DateTime Data { get; set; }

    public int IdMeioPagamento { get; set; }
    public int IdCategoria { get; set; }
    public int? IdSubcategoria { get; set; }
    public string Descricao { get; set; } = default!;
    public decimal Valor { get; set; }
    public short? Parcelas { get; set; }

    public LancamentoCadastroDto()
    {
    }

    public LancamentoCadastroDto(Lancamento lancamento)
    {
        Id = lancamento.Id;
        Data = lancamento.Data;
        IdMeioPagamento = lancamento.IdMeioPagamento;
        IdCategoria = lancamento.IdCategoria!.Value;
        IdSubcategoria = lancamento.IdSubcategoria;
        Descricao = lancamento.Descricao;
        Valor = lancamento.Valor;
    }
}
