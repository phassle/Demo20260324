using Serilog;
using PortCom.Demo.Middleware;
using PortCom.Demo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IFoodOrderService, FoodOrderService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<ICustomerReviewService, CustomerReviewService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

app.UseMiddleware<AuthMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
