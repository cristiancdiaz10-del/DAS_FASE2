using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAS_Proyecto.Controllers
{
    public class CotizacionesController : Controller
    {
        public IActionResult Cotizaciones()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AgregarCotizacion()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarCotizacion(string cliente, DateTime fecha, decimal total, string estado)
        {
            TempData["MensajeExito"] = "Cotización agregada correctamente";
            return RedirectToAction(nameof(Cotizaciones));
        }

        [HttpGet]
        public IActionResult EditarCotizacion(string id)
        {
            var listaCotizaciones = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "Codigo", "COT-001" },
                    { "OrdenId", "OT-001" },
                    { "Cliente", "Comercial ABC" },
                    { "Fecha", "2026-10-10" },
                    { "Descripcion", "Fabricación de rótulo luminoso para fachada principal." },
                    { "Materiales", "120.00" },
                    { "ManoObra", "80.00" },
                    { "Total", "200.00" }
                },
                new Dictionary<string, string>
                {
                    { "Codigo", "COT-002" },
                    { "OrdenId", "OT-002" },
                    { "Cliente", "Negocio XYZ" },
                    { "Fecha", "2026-10-11" },
                    { "Descripcion", "Impresión de cenefas publicitarias." },
                    { "Materiales", "100.00" },
                    { "ManoObra", "80.00" },
                    { "Total", "180.00" }
                },
                new Dictionary<string, string>
                {
                    { "Codigo", "COT-003" },
                    { "OrdenId", "OT-003" },
                    { "Cliente", "Cliente de prueba" },
                    { "Fecha", "2026-10-12" },
                    { "Descripcion", "Adhesivos troquelados en vinil." },
                    { "Materiales", "50.00" },
                    { "ManoObra", "45.00" },
                    { "Total", "95.00" }
                }
            };

            var cotizacion = listaCotizaciones.FirstOrDefault(c => c["Codigo"] == id);

            if (cotizacion == null)
            {
                return NotFound();
            }

            ViewBag.Cotizacion = cotizacion;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCotizacion(string codigo, string cliente, DateTime fecha, string descripcion, decimal materiales, decimal manoObra, decimal total)
        {
            TempData["MensajeExito"] = $"¡Cotización {codigo} actualizada correctamente!";
            return RedirectToAction(nameof(Cotizaciones));
        }
    }
}