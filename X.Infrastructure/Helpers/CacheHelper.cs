using Microsoft.Extensions.Options;
using X.Shared.Constants;
using X.Infrastructure.env;

namespace X.Infrastructure.Helpers;

public static class CacheHelper
{
	public static readonly Random rnd = new();

	/// <summary>
	/// Genera la clave para almacenar tokens de autenticación
	/// </summary>
	public static string AuthTokenKey(string tokenValue)
	{
		return $"auth:tokens:{tokenValue}";
	}

	/// <summary>
	/// Crea una clave de caché con expiración para tokens
	/// </summary>
	public static CacheKey AuthTokenCreation(string tokenValue, TimeSpan expiration)
	{
		return new CacheKey
		{
			Key = AuthTokenKey(tokenValue),
			Expiration = expiration
		};
	}

	/// <summary>
	/// Genera la clave para almacenar refresh tokens
	/// </summary>
	public static string AuthRefreshTokenKey(string tokenValue)
	{
		return $"auth:refresh_tokens:{tokenValue}";
	}

	/// <summary>
	/// Crea una clave de caché con expiración para refresh tokens
	/// </summary>
	public static CacheKey AuthRefreshTokenCreation(string tokenValue, IOptions<TokenConfiguration> configuration)
	{
		return new CacheKey
		{
			Key = AuthRefreshTokenKey(tokenValue),
			Expiration = TimeSpan.FromDays(Convert.ToInt32(configuration.Value.Expiration)) // Usa valor por defecto o ajusta según necesites
		};
	}
}

public class CacheKey
{
	public required string Key { get; set; }
	public required TimeSpan Expiration { get; set; }
}
