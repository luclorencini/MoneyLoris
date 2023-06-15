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

    //campos para cartão de crédito
    public short? Parcelas { get; set; }
    public int? FaturaMes { get; set; }
    public int? FaturaAno { get; set; }
    
    public LancamentoCadastroDto()
    {
    }
}
