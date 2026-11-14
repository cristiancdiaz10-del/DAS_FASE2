using Microsoft.AspNetCore.Mvc;
using DAS_FASE2.Repositories.Interfaces;
using DAS_FASE2.Models;
using DAS_FASE2.ViewModels;

namespace DAS_FASE2.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly IServicioRepository _servicioRepository;

        public ServiciosController(IServicioRepository servicioRepository)
        {
            _servicioRepository = servicioRepository;
        }

        // ==========================================
        // 1. READ (Listar): Muestra todos los servicios
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listaServicios = await _servicioRepository.ObtenerTodosAsync();
            return View(listaServicios);
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
        public async Task<IActionResult> Create(ServicioViewModel servicioViewModel)
        {
            if (ModelState.IsValid)
            {
                var servicio = new Servicio
                {
                    NombreServicio = servicioViewModel.NombreServicio,
                    Descripcion = servicioViewModel.Descripcion,
                    PrecioBase = servicioViewModel.PrecioBase
                };

                await _servicioRepository.AgregarAsync(servicio);
                return RedirectToAction(nameof(Index));
            }
            return View(servicioViewModel);
        }

        // ==========================================
        // 4. EDIT (GET): Muestra el formulario con los datos a editar
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var servicio = await _servicioRepository.ObtenerPorIdAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }

            var servicioViewModel = new ServicioViewModel
            {
                IdServicio = servicio.IdServicio,
                NombreServicio = servicio.NombreServicio,
                Descripcion = servicio.Descripcion,
                PrecioBase = servicio.PrecioBase
            };

            return View(servicioViewModel);
        }

        // ==========================================
        // 5. EDIT (POST): Guarda los cambios actualizados
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServicioViewModel servicioViewModel)
        {
            if (ModelState.IsValid)
            {
                var servicio = new Servicio
                {
                    IdServicio = id, // Vital para identificar qué servicio actualizar
                    NombreServicio = servicioViewModel.NombreServicio,
                    Descripcion = servicioViewModel.Descripcion,
                    PrecioBase = servicioViewModel.PrecioBase
                };

                await _servicioRepository.ActualizarAsync(servicio);
                return RedirectToAction(nameof(Index));
            }
            return View(servicioViewModel);
        }

        // ==========================================
        // 6. DELETE: Elimina el registro por ID
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _servicioRepository.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
