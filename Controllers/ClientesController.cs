using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAS_Proyecto.Data;
using DAS_Proyecto.Models;
using DAS_Proyecto.ViewModels;
using DAS_Proyecto.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS_Proyecto.Controllers
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
                TempData["MensajeExito"] = "¡Cliente agregado correctamente!";
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
        // 6. DETALLE (GET): Muestra la información y órdenes asociadas
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var cliente = await _clienteRepository.ObtenerPorIdAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            // Simulación de órdenes asociadas para la vista de detalles
            var todasLasOrdenes = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "Codigo", "OT-001" },
                    { "ClienteId", cliente.IdCliente.ToString() },
                    { "Trabajo", "Fabricación de rótulo luminoso" },
                    { "Total", "$350.00" }
                },
                new Dictionary<string, string>
                {
                    { "Codigo", "OT-002" },
                    { "ClienteId", "999" }, // Otro ID de prueba
                    { "Trabajo", "Cenefa impresas" },
                    { "Total", "$180.00" }
                },
                new Dictionary<string, string>
                {
                    { "Codigo", "OT-003" },
                    { "ClienteId", cliente.IdCliente.ToString() },
                    { "Trabajo", "Adhesivo troquelado" },
                    { "Total", "$95.00" }
                }
            };

            var ordenesAsociadas = todasLasOrdenes.Where(o => o["ClienteId"] == cliente.IdCliente.ToString()).ToList();

            ViewBag.Cliente = cliente;
            ViewBag.Ordenes = ordenesAsociadas;

            return View();
        }

        // ==========================================
        // 7. DELETE: Elimina el registro por ID
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