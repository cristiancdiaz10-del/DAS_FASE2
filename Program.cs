using Microsoft.EntityFrameworkCore;
using DAS_Proyecto.Data; // Asegúrate de que coincida con el namespace de tu proyecto

var builder = WebApplication.CreateBuilder(args);

// --- 1. REGISTRAR EL DBCONTEXT CON SQL SERVER ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();