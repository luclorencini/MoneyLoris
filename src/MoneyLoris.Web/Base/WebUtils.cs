using System.Security.Claims;

namespace MoneyLoris.Web.Base;

public static class WebUtils
{
    public static string UsuarioNome(this ClaimsPrincipal user)
    {
        var nomeUsuario = user.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();

        return nomeUsuario!;
    }

    public static string PrimeiroNome(this string nome)
    {
        if (nome is null)
            return "";
        else
        {
            var primeiroEspaco = nome.IndexOf(" ");

            //se tem mais de uma palavra, pega a primeira
            if (primeiroEspaco > -1)
            {
                return nome.Substring(0, primeiroEspaco);
            }
            else
            {
                //se só tem 1, retorna ela
                return nome;
            }
        }
    }
}