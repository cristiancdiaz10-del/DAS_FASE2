using Microsoft.AspNetCore.Mvc;
using DAS_FASE2.Repositories.Interfaces;
using DAS_FASE2.Models;
using DAS_FASE2.ViewModels;

namespace DAS_FASE2.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuariosController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // ==========================================
        // 1. READ (Listar): Muestra todos los usuarios
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listaUsuarios = await _usuarioRepository.ObtenerTodosAsync();
            return View(listaUsuarios);
        }

        // ==========================================
        // 2. CREATE (GET): Muestra el formulario vacío
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ==========================================
        // 3. CREATE (POST): Recibe los datos y los guarda
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel usuarioViewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = new Usuario
                {
                    Nombre = usuarioViewModel.Nombre,
                    Correo = usuarioViewModel.Correo,
                    PasswordHash = usuarioViewModel.Password, // Mapeamos la contraseña al campo hash
                    Rol = usuarioViewModel.Rol
                };

                await _usuarioRepository.AgregarAsync(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuarioViewModel);
        }

        // ==========================================
        // 4. EDIT (GET): Muestra el formulario con los datos a editar
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var usuarioViewModel = new UsuarioViewModel
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = usuario.Rol,
                Password = "" // Por seguridad no devolvemos el hash a la vista; si deseas cambiarla, el usuario la ingresará
            };

            return View(usuarioViewModel);
        }

        // ==========================================
        // 5. EDIT (POST): Guarda los cambios actualizados
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioViewModel usuarioViewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = new Usuario
                {
                    IdUsuario = id, // Vital para identificar qué usuario actualizar
                    Nombre = usuarioViewModel.Nombre,
                    Correo = usuarioViewModel.Correo,
                    PasswordHash = usuarioViewModel.Password, // Actualiza con la nueva contraseña ingresada
                    Rol = usuarioViewModel.Rol
                };

                await _usuarioRepository.ActualizarAsync(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuarioViewModel);
        }

        // ==========================================
        // 6. DELETE: Elimina el registro por ID
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _usuarioRepository.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
