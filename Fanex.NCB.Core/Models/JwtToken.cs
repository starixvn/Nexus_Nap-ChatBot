namespace Fanex.NCB.Core.Models;

public class JwtToken
{
    public string TokenType { get; set; }

    public int ExpiresIn { get; set; }

    public string AccessToken { get; set; }
}