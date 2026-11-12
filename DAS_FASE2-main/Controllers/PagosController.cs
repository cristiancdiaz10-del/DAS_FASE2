
using Microsoft.AspNetCore.Mvc;
using DAS_Proyecto.ViewModels;
using System;
using System.Collections.Generic;

namespace DAS_Proyecto.Controllers
{
    public class PagosController : Controller
    {
        // Listado de pagos.
        // Pendiente de conectar al repositorio.
        [HttpGet]
        public IActionResult Index()
        {
            var modelo = new PagoListadoViewModel
            {
                TotalPagado = 0,
                TotalAnticipos = 0,
                TotalSaldosPendientes = 0,
                Pagos = new List<PagoItemViewModel>()
            };

            return View(modelo);
        }

        // Formulario para registrar pagos.
        [HttpGet]
        public IActionResult RegistrarPago()
        {
            var modelo = new RegistrarPagoViewModel
            {
                FechaPago = DateTime.Today
            };

            return View(modelo);
        }

        // Registro de pagos.
        // Pendiente de conectar con órdenes y persistencia.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarPago(
            RegistrarPagoViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View("RegistrarPago", modelo);
            }

            ModelState.AddModelError(
                string.Empty,
                "El registro de pagos todavía no está disponible. " +
                "Se requiere conectar las órdenes de trabajo " +
                "y la base de datos."
            );

            return View("RegistrarPago", modelo);
        }

        // Detalle de un pago.
        // Pendiente de consultar información real del repositorio.
        [HttpGet]
        public IActionResult DetallesPago(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            // No se muestran datos ficticios.
            // Se implementará cuando exista conexión con el repositorio.
            return NotFound();
        }
    }
}
