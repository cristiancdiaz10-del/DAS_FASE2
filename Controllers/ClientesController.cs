using Microsoft.AspNetCore.Mvc;
using DAS_FASE2.Repositories.Interfaces;
using DAS_FASE2.Models;
using DAS_FASE2.ViewModels;

namespace DAS_FASE2.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClienteRepository _clienteRepository;

        public ClientesController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        // ==========================================
        // 1. READ (Listar): Muestra todos los clientes
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listaClientes = await _clienteRepository.ObtenerTodosAsync();
            return View(listaClientes);
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
        public async Task<IActionResult> Create(ClienteViewModel clienteViewModel)
        {
            if (ModelState.IsValid)
            {
                var cliente = new Cliente
                {
                    Nombre = clienteViewModel.Nombre,
                    Telefono = clienteViewModel.Telefono,
                    Correo = clienteViewModel.Correo,
                    Empresa = clienteViewModel.Empresa,
                    Direccion = clienteViewModel.Direccion,
                    Observaciones = clienteViewModel.Observaciones
                };

                await _clienteRepository.AgregarAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(clienteViewModel);
        }

        // ==========================================
        // 4. EDIT (GET): Muestra el formulario con los datos a editar
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteRepository.ObtenerPorIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            var clienteViewModel = new ClienteViewModel
            {
                Nombre = cliente.Nombre,
                Telefono = cliente.Telefono,
                Correo = cliente.Correo,
                Empresa = cliente.Empresa,
                Direccion = cliente.Direccion,
                Observaciones = cliente.Observaciones
            };

            return View(clienteViewModel);
        }

        // ==========================================
        // 5. EDIT (POST): Guarda los cambios actualizados
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel clienteViewModel)
        {
            if (ModelState.IsValid)
            {
                var cliente = new Cliente
                {
                    IdCliente = id, // Vital para identificar qué registro actualizar
                    Nombre = clienteViewModel.Nombre,
                    Telefono = clienteViewModel.Telefono,
                    Correo = clienteViewModel.Correo,
                    Empresa = clienteViewModel.Empresa,
                    Direccion = clienteViewModel.Direccion,
                    Observaciones = clienteViewModel.Observaciones
                };

                await _clienteRepository.ActualizarAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(clienteViewModel);
        }

        // ==========================================
        // 6. DELETE: Elimina el registro por ID
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _clienteRepository.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}