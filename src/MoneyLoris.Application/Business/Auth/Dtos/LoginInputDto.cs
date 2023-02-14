namespace MoneyLoris.Application.Business.Auth.Dtos;
public class LoginInputDto
{
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public bool ManterConectado { get; set; }
}
