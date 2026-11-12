
using Microsoft.AspNetCore.Mvc;
using DAS_Proyecto.ViewModels;
using System;

namespace DAS_Proyecto.Controllers
{
    public class InformesController : Controller
    {
        [HttpGet]
        public IActionResult Mensual(int? mes, int? anio)
        {
            int mesSeleccionado = mes ?? DateTime.Today.Month;
            int anioSeleccionado = anio ?? DateTime.Today.Year;

            if (mesSeleccionado < 1 || mesSeleccionado > 12)
            {
                mesSeleccionado = DateTime.Today.Month;
            }

            if (anioSeleccionado < 2000 ||
                anioSeleccionado > DateTime.Today.Year + 1)
            {
                anioSeleccionado = DateTime.Today.Year;
            }

            var informe = new InformeMensualViewModel
            {
                Mes = mesSeleccionado,
                Anio = anioSeleccionado,
                TotalIngresos = 0,
                CantidadPedidos = 0
            };

            return View(informe);
        }
    }
}
