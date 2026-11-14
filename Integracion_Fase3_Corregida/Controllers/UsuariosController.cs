using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using DAS_Proyecto.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DAS_Proyecto.Controllers;
[Authorize(Roles = "Administrador")]
public class UsuariosController : Controller
{
    private readonly IUsuarioRepository _repository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuariosController(IUsuarioRepository repository, IPasswordHasher<Usuario> passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? busqueda)
    {
        var usuarios = await _repository.ObtenerTodosAsync(busqueda);
        ViewBag.Busqueda = busqueda;
        return View(usuarios);
    }

    [HttpGet]
    public IActionResult Create() => View(new UsuarioViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioViewModel modelo)
    {
        if (string.IsNullOrWhiteSpace(modelo.Password))
            ModelState.AddModelError(nameof(modelo.Password), "La contraseña es obligatoria.");

        if (!Enum.IsDefined(typeof(RolUsuario), modelo.Rol))
            ModelState.AddModelError(nameof(modelo.Rol), "Selecciona un rol válido.");

        if (!ModelState.IsValid) return View(modelo);

        var usuarios = await _repository.ObtenerTodosAsync();
        if (usuarios.Any(u => u.Correo.Equals(modelo.Correo.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(modelo.Correo), "Ya existe un usuario con ese correo.");
            return View(modelo);
        }

        var usuario = new Usuario
        {
            Nombre = modelo.Nombre.Trim(),
            Correo = modelo.Correo.Trim(),
            Rol = modelo.Rol
        };
        usuario.PasswordHash = _passwordHasher.HashPassword(usuario, modelo.Password!);

        try
        {
            await _repository.AgregarAsync(usuario);
            TempData["MensajeExito"] = "Usuario creado correctamente. La contraseña quedó almacenada mediante hash seguro.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar el usuario.");
            return View(modelo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var usuario = await _repository.ObtenerPorIdAsync(id);
        if (usuario == null) return NotFound();
        return View(new UsuarioViewModel
        {
            IdUsuario = usuario.IdUsuario,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Rol = usuario.Rol,
            Password = string.Empty
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsuarioViewModel modelo)
    {
        if (id != modelo.IdUsuario) return BadRequest();
        if (!Enum.IsDefined(typeof(RolUsuario), modelo.Rol))
            ModelState.AddModelError(nameof(modelo.Rol), "Selecciona un rol válido.");
        if (!ModelState.IsValid) return View(modelo);

        var usuario = await _repository.ObtenerPorIdAsync(id);
        if (usuario == null) return NotFound();

        var usuarios = await _repository.ObtenerTodosAsync();
        if (usuarios.Any(u => u.IdUsuario != id && u.Correo.Equals(modelo.Correo.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(modelo.Correo), "Otro usuario ya utiliza ese correo.");
            return View(modelo);
        }

        usuario.Nombre = modelo.Nombre.Trim();
        usuario.Correo = modelo.Correo.Trim();
        usuario.Rol = modelo.Rol;
        if (!string.IsNullOrWhiteSpace(modelo.Password))
            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, modelo.Password);

        try
        {
            await _repository.ActualizarAsync(usuario);
            TempData["MensajeExito"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "No se pudo actualizar el usuario.");
            return View(modelo);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repository.EliminarAsync(id);
            TempData["MensajeExito"] = "Usuario eliminado correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["MensajeError"] = "No se puede eliminar el usuario porque tiene cotizaciones asociadas.";
        }
        return RedirectToAction(nameof(Index));
    }
}
