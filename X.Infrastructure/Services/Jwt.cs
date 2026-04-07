using X.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace X.Infrastructure.Services;

public class Jwt : IToken
{
    public string GenerateToken(string userId, string email)
    {
        throw new NotImplementedException();
    }
}