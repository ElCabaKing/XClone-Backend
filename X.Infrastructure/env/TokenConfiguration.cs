namespace X.Infrastructure.env;

public class TokenConfiguration
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public required string TokenKey { get; set; }
    public required DateTime Expiration { get; set; } = DateTime.UtcNow.AddMinutes(
        Random.Shared.Next(1, 5)
    );
}
