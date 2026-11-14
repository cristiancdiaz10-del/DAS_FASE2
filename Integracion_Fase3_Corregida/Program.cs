using DAS_Proyecto.Data;
using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Implementations;
using DAS_Proyecto.Repositories.Interfaces;
using DAS_Proyecto.Services;
using DAS_Proyecto.Services.Implementations;
using DAS_Proyecto.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DAS_Proyecto;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("No se configuró ConnectionStrings:DefaultConnection.");

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

        builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
        builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
        builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        builder.Services.AddScoped<IPagoRepository, PagoRepository>();
        builder.Services.AddScoped<ICotizacionRepository, CotizacionRepository>();
        builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
        builder.Services.AddScoped<IOrdenTrabajoRepository, OrdenTrabajoRepository>();
        builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

        builder.Services.AddScoped<PagoValidacionService>();
        builder.Services.AddScoped<InformeMensualService>();
        builder.Services.AddScoped<IPagoService, PagoService>();
        builder.Services.AddScoped<ICotizacionService, CotizacionService>();
        builder.Services.AddScoped<IMaterialService, MaterialService>();
        builder.Services.AddScoped<IOrdenTrabajoService, OrdenTrabajoService>();
        builder.Services.AddScoped<IDashboardService, DashboardService>();

        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "Ranibal.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";

                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
        builder.Services.AddControllersWithViews();



        var app = builder.Build();

        var supportedCultures = new[] { new CultureInfo("en-US") };
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("en-US"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var passwordHasher = services.GetRequiredService<IPasswordHasher<Usuario>>();

               
                context.Database.Migrate();

             
                if (!context.Usuarios.Any())
                {
                    var usuarioAdmin = new Usuario
                    {
                        Nombre = "Administrador",
                        Correo = "admin@ranibal.com",
                        PasswordHash = passwordHasher.HashPassword(null, "123456"),
                        Rol = RolUsuario.Administrador
                    };

                    context.Usuarios.Add(usuarioAdmin);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Ocurrió un error al sembrar la base de datos.");
            }
        }


        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute(name: "default", pattern: "{controller=Account}/{action=Login}/{id?}");
        app.Run();
    }
}