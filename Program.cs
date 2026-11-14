using DAS_Proyecto.Data;
using DAS_Proyecto.Repositories.Implementations;
using DAS_Proyecto.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ==========================================
            // 1. REGISTRAR EL DbContext (Base de datos)
            // ==========================================
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ==========================================
            // 2. REGISTRAR REPOSITORIOS
            // ==========================================
            builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
            builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

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
        }
    }
}