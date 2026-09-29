using OrderService.Protos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var orderServiceAddress =
    builder.Configuration["GrpcServices:OrderService"]
    ?? throw new InvalidOperationException(
        "GrpcServices:OrderService is not config*red.");

builder.Services.AddGrpcClient<Order.OrderClient>(options =>
{
    options.Address = new Uri(orderServiceAddress);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Order}/{action=Index}/{id?}");

app.Run();