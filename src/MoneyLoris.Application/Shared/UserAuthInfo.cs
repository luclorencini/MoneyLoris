namespace MoneyLoris.Application.Shared;
public class UserAuthInfo
{
    public int Id { get; set; }
    public string UserName { get; set; } = default!;
    public bool IsAdmin { get; set; }
}
