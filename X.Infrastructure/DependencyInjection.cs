using Microsoft.Extensions.DependencyInjection;
using X.Application.Interfaces;
using X.Domain.Interfaces.Repository;
using X.Infrastructure.Repository;
using X.Infrastructure.Services;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using X.Infrastructure.env;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace X.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        AddCache(services);

        ConfigureJwt(services, configuration);
        
        var account = new Account(
    configuration[ConfigurationConstants.CloudinaryCloudName], 
    configuration[ConfigurationConstants.CloudinaryApiKey], 
    configuration[ConfigurationConstants.CloudinaryApiSecret]
);

        var cloudinary = new Cloudinary(account);

        services.AddSingleton(cloudinary);
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHash, Password>();
        services.AddScoped<IStorage, ImageStorage>();
        services.AddScoped<IToken, Jwt>();

        return services;
    }

    public static void AddCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<ICacheService, CacheService>();
    }

    private static void ConfigureJwt(
    IServiceCollection services,
    IConfiguration configuration)
    {
    var key = configuration[ConfigurationConstants.JwtKey]
    ?? throw new InvalidOperationException("JWT Key is not configured.");
    var issuer = configuration[ConfigurationConstants.JwtIssuer]
    ?? throw new InvalidOperationException("JWT Issuer is not configured.");
    var audience = configuration[ConfigurationConstants.JwtAudience]
    ?? throw new InvalidOperationException("JWT Audience is not configured.");
    var expireMinutes = int.Parse(configuration[ConfigurationConstants.JwtExpireMinutes]
    ?? throw new InvalidOperationException("JWT Expire Minutes is not configured."));

    services.Configure<TokenConfiguration>(options =>
    {
        options.Key = key;
        options.Issuer = issuer;
        options.Audience = audience;
        options.Expiration = DateTime.UtcNow.AddMinutes(expireMinutes);
    });

    services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = issuer,
            ValidAudience = audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key!)
            )
        };
    });

    services.AddAuthorization();
}
}