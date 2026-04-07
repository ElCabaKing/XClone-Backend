using Microsoft.Extensions.DependencyInjection;
using X.Application.Modules.Auth.LogIn;
using X.Application.Modules.User.CreateUser;


namespace X.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // UseCases
        services.AddScoped<CreateUserHandler>();
        services.AddScoped<UserLogInHandler>();

        return services;
    }
}