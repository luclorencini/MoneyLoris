using System.ComponentModel;

namespace MoneyLoris.Application.Utils;
public static class EnumsExtensions
{
    public static bool IsInEnum<T>(this int valor) where T : Enum
    {
        var v = Enum.IsDefined(typeof(T), valor);
        return v;
    }

    public static ICollection<dynamic> ItemsFromEnum<T>() where T : Enum
    {
        var items = new List<dynamic>();

        foreach (T en in Enum.GetValues(typeof(T)))
        {
            var val = (int)Convert.ChangeType(en, en.GetTypeCode());

            var item = new { Id = val, Descricao = en.ObterDescricao() };
            items.Add(item);
        }

        return items;
    }

    public static string ObterDescricao(this Enum value)
    {
        try
        {
            DescriptionAttribute[] da = (DescriptionAttribute[])value.GetType().GetField(value.ToString())!.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }
        catch (Exception)
        {
            // se der NullReferenceException, é porque não há o valor informado no enum. Retorna string vazia.
            return string.Empty;
        }
    }

    public static int? ObterValor(this Enum value)
    {
        object val = Convert.ChangeType(value, value.GetTypeCode());
        return (int?)val;
    }
}
