
using X.WebApi.Middlewares;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddScoped<ErrorHandlerMiddleware>();
        return services;
    }
}