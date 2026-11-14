using DAS_Proyecto.Services.Interfaces;
using DAS_Proyecto.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DAS_Proyecto.Controllers
{
    public class PagosController : Controller
    {
        private readonly IPagoService _pagoService;

        public PagosController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var modelo = await _pagoService.ObtenerListadoAsync();
            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> RegistrarPago()
        {
            var modelo = new RegistrarPagoViewModel
            {
                FechaPago = DateTime.Today,
                OrdenesDisponibles = await _pagoService.ObtenerOrdenesDisponiblesAsync()
            };

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarPago(RegistrarPagoViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.OrdenesDisponibles = await _pagoService.ObtenerOrdenesDisponiblesAsync();
                return View("RegistrarPago", modelo);
            }

            var resultado = await _pagoService.RegistrarPagoAsync(
                modelo.IdOrdenTrabajo,
                modelo.MontoPago,
                modelo.TipoPago,
                modelo.FechaPago);

            if (!resultado.Exito)
            {
                ModelState.AddModelError(string.Empty, resultado.Mensaje);
                modelo.OrdenesDisponibles = await _pagoService.ObtenerOrdenesDisponiblesAsync();
                return View("RegistrarPago", modelo);
            }

            TempData["MensajeExito"] = $"Pago {resultado.CodigoPago} registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DetallesPago(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var modelo = await _pagoService.ObtenerDetallePorCodigoAsync(id);
            if (modelo == null)
                return NotFound();

            return View(modelo);
        }
    }
}
