using DAS_Proyecto.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DAS_Proyecto.Controllers
{
    public class InformesController : Controller
    {
        private readonly IPagoService _pagoService;

        public InformesController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        [HttpGet]
        public async Task<IActionResult> Mensual(int? mes, int? anio)
        {
            int mesSeleccionado = mes ?? DateTime.Today.Month;
            int anioSeleccionado = anio ?? DateTime.Today.Year;

            if (mesSeleccionado < 1 || mesSeleccionado > 12)
                mesSeleccionado = DateTime.Today.Month;

            if (anioSeleccionado < 2000 || anioSeleccionado > DateTime.Today.Year + 1)
                anioSeleccionado = DateTime.Today.Year;

            var informe = await _pagoService.ObtenerInformeMensualAsync(
                mesSeleccionado, anioSeleccionado);

            return View(informe);
        }
    }
}
