namespace MoneyLoris.Application.Utils;
public static class DateExtensions
{
    public static DateTime UltimoDiaMes(this DateTime date)
    {
        var pd = new DateTime(date.Year, date.Month, 1);
        pd = pd.AddMonths(1).AddDays(-1);
        return pd;
    }

    public static string ToSqlStringYMD(this DateTime date)
    {
        var s = date.ToString("yyyy-MM-dd");
        return s;
    }
}
