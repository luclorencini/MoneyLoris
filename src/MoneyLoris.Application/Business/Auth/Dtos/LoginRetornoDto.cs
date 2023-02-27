namespace MoneyLoris.Application.Business.Auth.Dtos;
public class LoginRetornoDto
{
    public string? UrlRedir { get; set; }
    public bool? AlterarSenha { get; set; }
    public string? PwaToken { get; set; }
}
