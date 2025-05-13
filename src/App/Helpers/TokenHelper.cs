using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace App.Helpers;

public static class TokenHelper
{
    public static string? GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}