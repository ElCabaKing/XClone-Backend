using Microsoft.Extensions.DependencyInjection;
using X.Application.Modules.User.CreateUser;


namespace X.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // UseCases
        services.AddScoped<CreateUserHandler>();

        return services;
    }
}