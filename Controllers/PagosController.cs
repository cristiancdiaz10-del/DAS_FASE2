using Microsoft.AspNetCore.Mvc;
using DAS_Proyecto.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAS_Proyecto.Controllers
{
    public class PagosController : Controller
    {
        // ==========================================
        // 1. INDEX: Listado de pagos
        // ==========================================
        [HttpGet]
        public IActionResult Index()
        {
            var modelo = new PagoListadoViewModel
            {
                TotalPagado = 355.00m,
                TotalAnticipos = 175.00m,
                TotalSaldosPendientes = 180.00m,
                Pagos = new List<PagoItemViewModel>()
            };

            return View(modelo);
        }

        // ==========================================
        // 2. REGISTRAR PAGO (GET): Muestra el formulario
        // ==========================================
        [HttpGet]
        public IActionResult RegistrarPago()
        {
            var modelo = new RegistrarPagoViewModel
            {
                FechaPago = DateTime.Today
            };

            return View(modelo);
        }

        // ==========================================
        // 3. GUARDAR PAGO (POST): Procesa el registro
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarPago(RegistrarPagoViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View("RegistrarPago", modelo);
            }

            TempData["MensajeExito"] = "¡Pago registrado correctamente!";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 4. DETALLES DE PAGO: Muestra la información detallada
        // ==========================================
        [HttpGet]
        public IActionResult DetallesPago(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

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