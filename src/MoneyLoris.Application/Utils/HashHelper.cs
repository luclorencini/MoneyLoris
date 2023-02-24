using System.Security.Cryptography;
using System.Text;

namespace MoneyLoris.Application.Utils;
public class HashHelper
{
    public static string ComputeHash(string str)
    {
        string ret = null!;

        if (str != null)
        {
            var hash = SHA256.Create();

            var bytes = Encoding.Default.GetBytes(str);

            var hashStr = hash.ComputeHash(bytes);

            ret = Convert.ToHexString(hashStr);
        }

        return ret;
    }
}
