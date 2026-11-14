using System.Diagnostics;
using DAS_Proyecto.Models;
using DAS_Proyecto.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DAS_Proyecto.Controllers;

public class HomeController : Controller
{
    private readonly IDashboardService _dashboardService;
    public HomeController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
        => View(await _dashboardService.ObtenerResumenAsync(cancellationToken));

    // Compatibilidad con enlaces heredados de Fase 2.
    public IActionResult Inventario() => RedirectToAction("Index", "Inventario");
    public IActionResult Cotizaciones() => RedirectToAction("Cotizaciones", "Cotizaciones");
    public IActionResult AgregarCotizacion() => RedirectToAction("AgregarCotizacion", "Cotizaciones");

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
