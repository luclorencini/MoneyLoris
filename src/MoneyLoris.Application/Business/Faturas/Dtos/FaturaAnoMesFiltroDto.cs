using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Faturas.Dtos;
public class FaturaAnoMesFiltroDto
{
    public int IdCartao { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
}
