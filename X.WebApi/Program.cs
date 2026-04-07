
using X.Application;
using X.Infrastructure;
using X.WebApi.Middlewares;
using Serilog;
using X.WebApi;
using DotNetEnv;


Env.Load(".env");
Env.Load("../X.Infrastructure/.env");

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog();

builder.Services.AddOpenApi();

builder.Services.AddWebApi(builder.Configuration).AddInfrastructure(builder.Configuration).AddApplication();

builder.Services.AddControllers();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();

