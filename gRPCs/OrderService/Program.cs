using Microsoft.EntityFrameworkCore;
using OrderService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrdersDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("db")
        ?? throw new InvalidOperationException(
            "Connection string 'ConStr' was not found.");

    options.UseSqlServer(connectionString);
});

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<OrderService.Services.OrderService>();

app.MapGet("/", () =>
    "This application hosts the Order gRPC service.");

app.Run();