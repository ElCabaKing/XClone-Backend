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

        ConfigureJwt(services, configuration);

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