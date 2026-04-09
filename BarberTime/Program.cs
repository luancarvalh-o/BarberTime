using BarberTime.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BarberTime.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BarberTimeContext>(Options =>
Options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BarberTimeContext>();

    context.Database.Migrate();

    if (!context.Servicos.Any())
    {
        context.Servicos.AddRange(
            new Servico { Nome = "Corte", Preco = 30, DuracaoEmMinutos = 30, Ativo = true },
            new Servico { Nome = "Barba", Preco = 20, DuracaoEmMinutos = 20, Ativo = true },
            new Servico { Nome = "Corte + Barba", Preco = 45, DuracaoEmMinutos = 50, Ativo = true }
        );

        context.SaveChanges();
    }
}


app.Run();
