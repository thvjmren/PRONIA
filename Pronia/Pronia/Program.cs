using Microsoft.EntityFrameworkCore;
using Pronia.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer("server=DESKTOP-M0SCP4I\\SQLEXPRESS;database=Pronia;trusted_connection=true;integrated security=true;TrustServerCertificate=true;");
});

var app = builder.Build();
app.UseStaticFiles();
app.MapControllerRoute(
    "default",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();
