using DAS_Proyecto.Models;
using DAS_Proyecto.Services.Interfaces;
using DAS_Proyecto.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DAS_Proyecto.Controllers;

public class OrdenesController : Controller
{
    private readonly IOrdenTrabajoService _ordenService;
    public OrdenesController(IOrdenTrabajoService ordenService) => _ordenService = ordenService;

    [HttpGet]
    public async Task<IActionResult> Index(string? busqueda, string? estado, CancellationToken cancellationToken)
    {
        EstadoOrden? filtro = null;
        if (!string.IsNullOrWhiteSpace(estado) && Enum.TryParse<EstadoOrden>(estado, true, out var estadoParseado) && Enum.IsDefined(typeof(EstadoOrden), estadoParseado))
            filtro = estadoParseado;
        else if (!string.IsNullOrWhiteSpace(estado))
            ModelState.AddModelError("estado", "El estado seleccionado no es válido.");

        return View(new OrdenTrabajoListadoViewModel
        {
            Busqueda = busqueda,
            EstadoSeleccionado = estado,
            Ordenes = await _ordenService.ObtenerListadoAsync(busqueda, filtro, cancellationToken)
        });
    }

    [HttpGet]
    public async Task<IActionResult> CrearDesdeCotizacion(int id, CancellationToken cancellationToken)
    {
        try { return View(await _ordenService.ObtenerDatosParaCrearAsync(id, cancellationToken)); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { TempData["MensajeError"] = ex.Message; return RedirectToAction("Cotizaciones", "Cotizaciones"); }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearDesdeCotizacion(CrearOrdenViewModel modelo, CancellationToken cancellationToken)
    {
        try
        {
            await _ordenService.CrearDesdeCotizacionAsync(modelo, cancellationToken);
            TempData["MensajeExito"] = "La orden de trabajo se generó correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            try
            {
                var datos = await _ordenService.ObtenerDatosParaCrearAsync(modelo.IdCotizacion, cancellationToken);
                datos.FechaEntregaEstimada = modelo.FechaEntregaEstimada;
                return View(datos);
            }
            catch { return RedirectToAction("Cotizaciones", "Cotizaciones"); }
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken)
    {
        var modelo = await _ordenService.ObtenerDetalleAsync(id, cancellationToken);
        return modelo == null ? NotFound() : View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarMaterial(int idOrden, int idMaterial, decimal cantidadUtilizada, CancellationToken cancellationToken)
    {
        try
        {
            await _ordenService.AgregarMaterialAsync(idOrden, idMaterial, cantidadUtilizada, cancellationToken);
            TempData["MensajeExito"] = "Material agregado a la orden.";
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            TempData["MensajeError"] = ex.Message;
        }
        return RedirectToAction(nameof(Detalle), new { id = idOrden });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarEstado(int id, string nuevoEstado, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<EstadoOrden>(nuevoEstado, true, out var estado) || !Enum.IsDefined(typeof(EstadoOrden), estado))
        {
            TempData["MensajeError"] = "El estado seleccionado no es válido.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        try
        {
            await _ordenService.ActualizarEstadoAsync(id, estado, cancellationToken);
            TempData["MensajeExito"] = "El estado de la orden se actualizó correctamente.";
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            TempData["MensajeError"] = ex.Message;
        }
        return RedirectToAction(nameof(Detalle), new { id });
    }
}
