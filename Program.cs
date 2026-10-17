var builder = WebApplication.CreateBuilder(args);

// Agregar servicios de MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Habilitar archivos estáticos (wwwroot)
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Ruta por defecto para iniciar en el Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();