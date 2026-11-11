
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PedidosConnection")));

builder.Services.AddHttpClient("CatalogoApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7222/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient("InventarioApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7160/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
