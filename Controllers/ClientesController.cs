using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace DAS_Proyecto.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(string nombre, string telefono, string correo, string direccion, string observaciones)
        {
            TempData["MensajeExito"] = "¡Cliente agregado correctamente!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Detalle(string id)
        {
            var listaClientes = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "Codigo", "CL-001" },
                    { "Nombre", "Comercial ABC" },
                    { "Fecha", "10/10/2026" },
                    { "Telefono", "7000-1234" },
                    { "Correo", "contacto@ejemplo.com" },
                    { "Direccion", "Dirección registrada del cliente" },
                    { "Observaciones", "Información adicional registrada sobre el cliente." },
                    { "TotalOrdenes", "2" }
                },
                new Dictionary<string, string>
                {
                    { "Codigo", "CL-002" },
                    { "Nombre", "Cliente Ejemplo" },
                    { "Fecha", "13/10/2026" },
                    { "Telefono", "7100-5678" },
                    { "Correo", "ventas@ejemplo.com" },
                    { "Direccion", "Soyapango, El Salvador" },
                    { "Observaciones", "Cliente preferencial." },
                    { "TotalOrdenes", "0" }
                },
                new Dictionary<string, string>
                {
                    { "Codigo", "CL-003" },
                    { "Nombre", "Negocio XYZ" },
                    { "Fecha", "11/10/2026" },
                    { "Telefono", "7888-5555" },
                    { "Correo", "info@xyz.com" },
                    { "Direccion", "San Salvador, El Salvador" },
                    { "Observaciones", "Cliente preferencial." },
                    { "TotalOrdenes", "1" }
                }
            };

            var clienteSeleccionado = listaClientes.FirstOrDefault(c => c["Codigo"] == id);

            if (clienteSeleccionado == null)
            {
                return NotFound();
            }

            
            var todasLasOrdenes = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "Codigo", "OT-001" },
                    { "ClienteId", "CL-001" },
                    { "Trabajo", "Fabricación de rótulo luminoso" },
                    { "Total", "$350.00" }
                },
                new Dictionary<string, string>
                {
                    { "Codigo", "OT-002" },
                    { "ClienteId", "CL-003" },
                    { "Trabajo", "Cenefa impresas" },
                    { "Total", "$180.00" }
                },
                new Dictionary<string, string>
                {
                    { "Codigo", "OT-003" },
                    { "ClienteId", "CL-001" },
                    { "Trabajo", "Adhesivo troquelado" },
                    { "Total", "$95.00" }
                }
            };

        
            var ordenesAsociadas = todasLasOrdenes.Where(o => o["ClienteId"] == id).ToList();

            ViewBag.Cliente = clienteSeleccionado;
            ViewBag.Ordenes = ordenesAsociadas;

            return View();
        }
    }
}