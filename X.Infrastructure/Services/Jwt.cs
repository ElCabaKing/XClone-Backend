using X.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using X.Infrastructure.env;
using X.Shared.Helpers;
using X.Infrastructure.Helpers;
using X.Application.Models;

namespace X.Infrastructure.Services;

public class Jwt(IOptions<TokenConfiguration> jwtOptions) : IToken
{
    private readonly TokenConfiguration _jwtOptions = jwtOptions.Value;

    public string GenerateToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.Expiration.Minute),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public string CreateRefresh(Guid collaboratorId, ICacheService cacheService)
    {
        var token = RnText.GenerateRandomText(100);
        var cacheKey = CacheHelper.AuthRefreshTokenCreation(token, jwtOptions);

        cacheService.Create(cacheKey.Key, cacheKey.Expiration, new RefreshToken   
        {
            CollaboratorId = collaboratorId,
            ExpirationInDays = cacheKey.Expiration
        });

        return token;
    }

}