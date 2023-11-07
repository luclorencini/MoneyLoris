using System.Data.Common;

namespace MoneyLoris.Infrastructure.Persistence.Repositories;
public static class AdoNetExtensions
{
    public static T ReadColumn<T>(this DbDataReader result, string field)
    {
        if (result == null) return default!;
        if (field == null) return default!;

        return Convert.IsDBNull(result[field]) ? default! : (T)result[field];
    }
}
