using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using DAS_Proyecto.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Controllers;

public class ClientesController : Controller
{
    private readonly IClienteRepository _clienteRepository;
    public ClientesController(IClienteRepository clienteRepository) => _clienteRepository = clienteRepository;

    [HttpGet]
    public async Task<IActionResult> Index(string? busqueda)
    {
        ViewBag.Busqueda = busqueda;
        return View(await _clienteRepository.ObtenerTodosAsync(busqueda));
    }

    [HttpGet]
    public IActionResult Create() => View(new ClienteViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClienteViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);
        try
        {
            await _clienteRepository.AgregarAsync(new Cliente
            {
                Nombre = modelo.Nombre.Trim(), Telefono = modelo.Telefono.Trim(), Correo = modelo.Correo?.Trim(),
                Empresa = modelo.Empresa?.Trim(), Direccion = modelo.Direccion?.Trim(), Observaciones = modelo.Observaciones?.Trim()
            });
            TempData["MensajeExito"] = "Cliente agregado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar el cliente. Verifica los datos e inténtalo nuevamente.");
            return View(modelo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var cliente = await _clienteRepository.ObtenerPorIdAsync(id);
        if (cliente == null) return NotFound();
        return View(new ClienteViewModel
        {
            IdCliente = cliente.IdCliente, Nombre = cliente.Nombre, Telefono = cliente.Telefono,
            Correo = cliente.Correo, Empresa = cliente.Empresa, Direccion = cliente.Direccion, Observaciones = cliente.Observaciones
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClienteViewModel modelo)
    {
        if (id != modelo.IdCliente) return BadRequest();
        if (!ModelState.IsValid) return View(modelo);
        var existente = await _clienteRepository.ObtenerPorIdAsync(id);
        if (existente == null) return NotFound();
        existente.Nombre = modelo.Nombre.Trim();
        existente.Telefono = modelo.Telefono.Trim();
        existente.Correo = modelo.Correo?.Trim();
        existente.Empresa = modelo.Empresa?.Trim();
        existente.Direccion = modelo.Direccion?.Trim();
        existente.Observaciones = modelo.Observaciones?.Trim();
        try
        {
            await _clienteRepository.ActualizarAsync(existente);
            TempData["MensajeExito"] = "Cliente actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "No se pudo actualizar el cliente porque tiene información relacionada o los datos no son válidos.");
            return View(modelo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var cliente = await _clienteRepository.ObtenerPorIdAsync(id);
        if (cliente == null) return NotFound();
        var ordenes = await _clienteRepository.ObtenerOrdenesAsync(id);
        return View(new ClienteDetalleViewModel { Cliente = cliente, Ordenes = ordenes });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _clienteRepository.EliminarAsync(id);
            TempData["MensajeExito"] = "Cliente eliminado correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["MensajeError"] = "No se puede eliminar el cliente porque tiene cotizaciones u órdenes asociadas.";
        }
        return RedirectToAction(nameof(Index));
    }
}
