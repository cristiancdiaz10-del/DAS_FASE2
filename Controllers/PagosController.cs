using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RotulosRanibal.Controllers
{
    public class PagosController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpGet]
        public IActionResult RegistrarPago()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarPago(string orden, string tipoPago, decimal monto, DateTime fecha)
        {
            
            TempData["MensajeExito"] = "¡Pago registrado correctamente!";

            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult DetallesPago(string id)
        {
            var listaPagos = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> { { "Codigo", "PAG-001" }, { "Orden", "OT-001" }, { "Cliente", "Comercial ABC" }, { "Monto", "$175.00" }, { "Tipo", "Anticipo" }, { "Fecha", "10/10/2026" } },
                new Dictionary<string, string> { { "Codigo", "PAG-002" }, { "Orden", "OT-002" }, { "Cliente", "Negocio XYZ" }, { "Monto", "$180.00" }, { "Tipo", "Saldo final" }, { "Fecha", "11/10/2026" } },
                new Dictionary<string, string> { { "Codigo", "PAG-003" }, { "Orden", "OT-003" }, { "Cliente", "Cliente de prueba" }, { "Monto", "$95.00" }, { "Tipo", "Saldo final" }, { "Fecha", "12/09/2026" } }
            };

            var pagoSeleccionado = listaPagos.FirstOrDefault(p => p["Codigo"] == id);

            if (pagoSeleccionado == null)
            {
                return NotFound();
            }

            ViewBag.Pago = pagoSeleccionado;
            return View();
        }
    }
}