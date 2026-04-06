using Microsoft.Extensions.DependencyInjection;
using X.Application.Interfaces;
using X.Domain.Interfaces.Repository;
using X.Infrastructure.Repository;
using X.Infrastructure.Services;
using CloudinaryDotNet;

namespace X.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var account = new Account(
    "dg8an29xj","633787576718813","f-iV8nCp2WIN_K5BYfuzVjSeyf0"
);

        var cloudinary = new Cloudinary(account);

        services.AddSingleton(cloudinary);
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHash, Password>();
        services.AddScoped<IStorage, ImageStorage>();

        return services;
    }
}