namespace MoneyLoris.Application.Shared;

//referência: https://lostechies.com/jimmybogard/2008/11/09/systemtime-versus-isystemclock-dependencies-revisited/
public static class SystemTime
{
    public static Func<DateTime> Today = () => DateTime.Today;

    public static Func<DateTime> Now = () => DateTime.Now;
}
