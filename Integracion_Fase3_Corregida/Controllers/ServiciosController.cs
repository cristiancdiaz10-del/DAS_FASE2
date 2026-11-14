using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using DAS_Proyecto.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Controllers;

public class ServiciosController : Controller
{
    private readonly IServicioRepository _repository;
    public ServiciosController(IServicioRepository repository) => _repository = repository;

    [HttpGet]
    public async Task<IActionResult> Index(string? busqueda)
    {
        ViewBag.Busqueda = busqueda;
        return View(await _repository.ObtenerTodosAsync(busqueda));
    }

    [HttpGet]
    public IActionResult Create() => View(new ServicioViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServicioViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);
        try
        {
            await _repository.AgregarAsync(new Servicio { NombreServicio = modelo.NombreServicio.Trim(), Descripcion = modelo.Descripcion?.Trim(), PrecioBase = modelo.PrecioBase });
            TempData["MensajeExito"] = "Servicio agregado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar el servicio.");
            return View(modelo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var servicio = await _repository.ObtenerPorIdAsync(id);
        if (servicio == null) return NotFound();
        return View(new ServicioViewModel { IdServicio = servicio.IdServicio, NombreServicio = servicio.NombreServicio, Descripcion = servicio.Descripcion, PrecioBase = servicio.PrecioBase });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServicioViewModel modelo)
    {
        if (id != modelo.IdServicio) return BadRequest();
        if (!ModelState.IsValid) return View(modelo);
        var servicio = await _repository.ObtenerPorIdAsync(id);
        if (servicio == null) return NotFound();
        servicio.NombreServicio = modelo.NombreServicio.Trim();
        servicio.Descripcion = modelo.Descripcion?.Trim();
        servicio.PrecioBase = modelo.PrecioBase;
        try
        {
            await _repository.ActualizarAsync(servicio);
            TempData["MensajeExito"] = "Servicio actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "No se pudo actualizar el servicio.");
            return View(modelo);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repository.EliminarAsync(id);
            TempData["MensajeExito"] = "Servicio eliminado correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["MensajeError"] = "No se puede eliminar el servicio porque está asociado a cotizaciones.";
        }
        return RedirectToAction(nameof(Index));
    }
}
