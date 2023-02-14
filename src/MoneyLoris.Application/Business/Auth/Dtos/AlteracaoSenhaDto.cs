namespace MoneyLoris.Application.Business.Auth.Dtos;
public class AlteracaoSenhaDto
{
    public string Login { get; set; } = default!;
    public string SenhaAtual { get; set; } = default!;
    public string NovaSenha { get; set; } = default!;
}
