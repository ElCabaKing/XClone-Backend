using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using X.WebApi.Middlewares;
using Serilog;
using X.Infrastructure.Database.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using X.Infrastructure.env;
namespace X.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();

        services.AddScoped<ErrorHandlerMiddleware>();

        services.Configure<TokenConfiguration>(configuration.GetSection("JWT"));


        ConfigureLogger(services, configuration);

        ConfigureDB(services, configuration);

        return services;
    }

    private static void ConfigureLogger(IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.MongoDB(configuration[ConfigurationConstants.MongoConnectionString]??
    throw new InvalidOperationException("MongoDB Connection String is not configured."), collectionName: "Logs")
    .CreateLogger();
    }

    private static void ConfigureDB(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<XDbContext>(options =>
    options.UseSqlServer(
        configuration[ConfigurationConstants.ConnectionString] ?? throw new InvalidOperationException("SQL Server Connection String is not configured.")
    ));
    }

}