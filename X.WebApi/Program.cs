using Microsoft.EntityFrameworkCore;
using X.Application;
using X.Infrastructure;
using X.Infrastructure.Database.SqlServer.Context;
using X.WebApi.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.MongoDB(builder.Configuration.GetConnectionString("MongoDbConnection")!, collectionName: "Logs")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOpenApi();



builder.Services.AddDbContext<XDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddInfrastructure().AddApplication().AddWebApi();

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();

