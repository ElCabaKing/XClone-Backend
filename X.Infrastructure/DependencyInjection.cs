using Microsoft.Extensions.DependencyInjection;
using X.Application.Interfaces;
using X.Domain.Interfaces.Repository;
using X.Infrastructure.Repository;
using X.Infrastructure.Services;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using X.Infrastructure.env;

namespace X.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        AddCache(services);
        
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
}