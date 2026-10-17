using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace RotulosRanibal.Controllers
{
    public class OrdenesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detalle(string id)
        {
            var listaOrdenes = new List<Dictionary<string, string>>
    {
        new Dictionary<string, string> { { "Codigo", "OT-001" }, { "ClienteId", "CL-001" }, { "Cliente", "Comercial ABC" }, { "Fecha", "10/10/2026" }, { "Trabajo", "Rótulo luminoso" }, { "Total", "$350.00" }, { "Estado", "En proceso" } },
        new Dictionary<string, string> { { "Codigo", "OT-002" }, { "ClienteId", "CL-003" }, { "Cliente", "Negocio XYZ" }, { "Fecha", "11/10/2026" }, { "Trabajo", "Cenefa impresas" }, { "Total", "$180.00" }, { "Estado", "Listo" } },
        new Dictionary<string, string> { { "Codigo", "OT-003" }, { "ClienteId", "CL-001" }, { "Cliente", "Cliente de prueba" }, { "Fecha", "12/10/2026" }, { "Trabajo", "Adhesivo troquelado" }, { "Total", "$95.00" }, { "Estado", "Pendiente" } }
    };

            var ordenSeleccionada = listaOrdenes.FirstOrDefault(o => o["Codigo"] == id);

            if (ordenSeleccionada == null)
            {
                return NotFound();
            }

            ViewBag.Orden = ordenSeleccionada;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarEstado(string id, string nuevoEstado)
        {
            TempData["MensajeExito"] = $"¡Estado de la orden {id} actualizado a '{nuevoEstado}' correctamente!";

            return RedirectToAction(nameof(Detalle), new { id = id });
        }
    }
}