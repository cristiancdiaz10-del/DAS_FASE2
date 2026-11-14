using Microsoft.AspNetCore.Mvc;
namespace DAS_Proyecto.Controllers;

public class VentasController : Controller
{
    // El informe mensual real está centralizado en InformesController y toma los datos de Pagos/Órdenes.
    [HttpGet]
    public IActionResult Index() => RedirectToAction("Mensual", "Informes");
}
