using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // whats this used for?
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderServiceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("db")));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();