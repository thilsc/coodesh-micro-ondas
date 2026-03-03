using System.Security.Cryptography;
using System.Text;

public static class HashHelper
{
    public static string CalcularSha256(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash).ToLowerInvariant();        
    }
}
