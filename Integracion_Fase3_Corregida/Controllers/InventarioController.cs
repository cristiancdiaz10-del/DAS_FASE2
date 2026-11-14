using DAS_Proyecto.Services.Interfaces;
using DAS_Proyecto.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Controllers;

public class InventarioController : Controller
{
    private readonly IMaterialService _materialService;
    public InventarioController(IMaterialService materialService) => _materialService = materialService;

    [HttpGet]
    public async Task<IActionResult> Index(string? busqueda, CancellationToken cancellationToken)
    {
        ViewBag.Busqueda = busqueda;
        var materiales = await _materialService.ObtenerListadoAsync(busqueda, cancellationToken);
        return View(materiales);
    }

    [HttpGet]
    public IActionResult Nuevo() => View(new MaterialViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddInven(MaterialViewModel modelo, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View("Nuevo", modelo);
        try
        {
            await _materialService.CrearAsync(modelo, cancellationToken);
            TempData["MensajeExito"] = "Material guardado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Nuevo", modelo);
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar el material. Verifica el código y vuelve a intentarlo.");
            return View("Nuevo", modelo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditarMaterial(int id, CancellationToken cancellationToken)
    {
        var material = await _materialService.ObtenerPorIdAsync(id, cancellationToken);
        if (material == null) return NotFound();
        return View(new MaterialViewModel
        {
            IdMaterial = material.IdMaterial,
            CodigoMaterial = material.CodigoMaterial,
            NombreMaterial = material.NombreMaterial,
            CantidadDisponible = material.CantidadDisponible,
            UnidadMedida = material.UnidadMedida,
            CostoUnitario = material.CostoUnitario,
            Descripcion = material.Descripcion
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarMaterial(MaterialViewModel modelo, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View("EditarMaterial", modelo);
        try
        {
            await _materialService.ActualizarAsync(modelo, cancellationToken);
            TempData["MensajeExito"] = "Material actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("EditarMaterial", modelo);
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "No se pudieron guardar los cambios del material.");
            return View("EditarMaterial", modelo);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarMaterial(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _materialService.EliminarAsync(id, cancellationToken);
            TempData["MensajeExito"] = "Material eliminado correctamente.";
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException or DbUpdateException)
        {
            TempData["MensajeError"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
