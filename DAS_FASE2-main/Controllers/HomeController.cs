using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DAS_Proyecto.Models;

namespace DAS_Proyecto.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    #region Módulo de Inventario

    public IActionResult Inventario()
    {
        return View();
    }

    [HttpGet]
    public IActionResult AddInven()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddInven(string codigo, string material, int existencias, string unidad)
    {
     

        TempData["MensajeExito"] = "¡Material guardado correctamente en el inventario!";

        return RedirectToAction(nameof(Inventario));
    }

    #endregion

    #region Módulo de Cotizaciones


    public IActionResult Cotizaciones()
    {
        return View("~/Views/Cotizaciones/Cotizaciones.cshtml");
    }

  
    [HttpGet]
    public IActionResult AgregarCotizacion()
    {
        return View("~/Views/Cotizaciones/AgregarCotizacion.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AgregarCotizacion(string cliente, DateTime fecha, decimal total, string estado)
    {
     

        TempData["MensajeExito"] = "¡Cotización registrada exitosamente!";

        return RedirectToAction(nameof(Cotizaciones));
    }

    #endregion

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}