using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using DAS_Proyecto.Services.Interfaces;
using DAS_Proyecto.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DAS_Proyecto.Controllers
{
    public class CotizacionesController : Controller
    {
        private readonly ICotizacionService _cotizacionService;
        private readonly IClienteRepository _clienteRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IServicioRepository _servicioRepository;

        public CotizacionesController(
            ICotizacionService cotizacionService,
            IClienteRepository clienteRepository,
            IUsuarioRepository usuarioRepository,
            IServicioRepository servicioRepository)
        {
            _cotizacionService = cotizacionService;
            _clienteRepository = clienteRepository;
            _usuarioRepository = usuarioRepository;
            _servicioRepository = servicioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Cotizaciones(string? busqueda, string? estado, CancellationToken cancellationToken)
        {
            EstadoCotizacion? filtroEstado = null;
            if (!string.IsNullOrWhiteSpace(estado))
            {
                if (Enum.TryParse<EstadoCotizacion>(estado, true, out var estadoParseado) &&
                    Enum.IsDefined(typeof(EstadoCotizacion), estadoParseado))
                {
                    filtroEstado = estadoParseado;
                }
                else
                {
                    ModelState.AddModelError("estado", "El filtro de estado seleccionado no es válido.");
                }
            }

            var modelo = new CotizacionesListadoViewModel
            {
                Busqueda = busqueda,
                EstadoSeleccionado = estado,
                Cotizaciones = await _cotizacionService.ObtenerListadoAsync(busqueda, filtroEstado, cancellationToken)
            };

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> AgregarCotizacion(CancellationToken cancellationToken)
        {
            var modelo = new CotizacionFormViewModel { FechaCotizacion = DateTime.Today };
            await CargarOpcionesAsync(modelo, cancellationToken);
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarCotizacion(CotizacionFormViewModel modelo, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await CargarOpcionesAsync(modelo, cancellationToken);
                return View(modelo);
            }

            var cotizacion = new Cotizacion
            {
                IdCliente = modelo.IdCliente,
                IdUsuario = modelo.IdUsuario,
                IdServicio = modelo.IdServicio,
                FechaCotizacion = modelo.FechaCotizacion,
                DescripcionTrabajo = modelo.DescripcionTrabajo.Trim(),
                CostoMateriales = modelo.CostoMateriales,
                CostoManoObra = modelo.CostoManoObra
            };

            try
            {
                await _cotizacionService.CrearAsync(cotizacion, cancellationToken);
                TempData["MensajeExito"] = $"La cotización {cotizacion.CodigoCotizacion} se guardó correctamente.";
                return RedirectToAction(nameof(Cotizaciones));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "No se pudo guardar la cotización. Verifica las relaciones y vuelve a intentarlo.");
            }

            await CargarOpcionesAsync(modelo, cancellationToken);
            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> EditarCotizacion(int id, CancellationToken cancellationToken)
        {
            var cotizacion = await _cotizacionService.ObtenerPorIdAsync(id, cancellationToken);
            if (cotizacion == null) return NotFound();

            var modelo = new CotizacionFormViewModel
            {
                IdCotizacion = cotizacion.IdCotizacion,
                CodigoCotizacion = cotizacion.CodigoCotizacion,
                IdCliente = cotizacion.IdCliente,
                IdUsuario = cotizacion.IdUsuario,
                IdServicio = cotizacion.IdServicio,
                FechaCotizacion = cotizacion.FechaCotizacion,
                DescripcionTrabajo = cotizacion.DescripcionTrabajo,
                CostoMateriales = cotizacion.CostoMateriales,
                CostoManoObra = cotizacion.CostoManoObra,
                Estado = cotizacion.Estado
            };

            await CargarOpcionesAsync(modelo, cancellationToken);
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCotizacion(int id, CotizacionFormViewModel modelo, CancellationToken cancellationToken)
        {
            if (id != modelo.IdCotizacion) return BadRequest();

            if (!ModelState.IsValid)
            {
                await CargarOpcionesAsync(modelo, cancellationToken);
                return View(modelo);
            }

            var datos = new Cotizacion
            {
                IdCliente = modelo.IdCliente,
                IdUsuario = modelo.IdUsuario,
                IdServicio = modelo.IdServicio,
                FechaCotizacion = modelo.FechaCotizacion,
                DescripcionTrabajo = modelo.DescripcionTrabajo.Trim(),
                CostoMateriales = modelo.CostoMateriales,
                CostoManoObra = modelo.CostoManoObra,
                Estado = modelo.Estado
            };

            try
            {
                await _cotizacionService.ActualizarAsync(id, datos, cancellationToken);
                TempData["MensajeExito"] = $"La cotización {modelo.CodigoCotizacion} se actualizó correctamente.";
                return RedirectToAction(nameof(Cotizaciones));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "No se pudieron guardar los cambios de la cotización.");
            }

            modelo.CodigoCotizacion = (await _cotizacionService.ObtenerPorIdAsync(id, cancellationToken))?.CodigoCotizacion;
            await CargarOpcionesAsync(modelo, cancellationToken);
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCotizacion(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _cotizacionService.EliminarAsync(id, cancellationToken);
                TempData["MensajeExito"] = "La cotización se eliminó correctamente.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["MensajeError"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["MensajeError"] = ex.Message;
            }
            catch (DbUpdateException)
            {
                TempData["MensajeError"] = "No se pudo eliminar la cotización porque tiene información relacionada.";
            }

            return RedirectToAction(nameof(Cotizaciones));
        }

        private async Task CargarOpcionesAsync(CotizacionFormViewModel modelo, CancellationToken cancellationToken)
        {
            // El mismo DbContext scoped se comparte entre repositorios: ejecutar estas consultas
            // en paralelo provocaria operaciones concurrentes no soportadas por EF Core.
            var clientes = await _clienteRepository.ObtenerTodosAsync();
            var usuarios = await _usuarioRepository.ObtenerTodosAsync();
            var servicios = await _servicioRepository.ObtenerTodosAsync();

            modelo.Clientes = clientes
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem(c.Empresa == null ? c.Nombre : $"{c.Nombre} - {c.Empresa}", c.IdCliente.ToString(), c.IdCliente == modelo.IdCliente))
                .ToList();

            modelo.Usuarios = usuarios
                .OrderBy(u => u.Nombre)
                .Select(u => new SelectListItem($"{u.Nombre} ({u.Rol})", u.IdUsuario.ToString(), u.IdUsuario == modelo.IdUsuario))
                .ToList();

            modelo.Servicios = servicios
                .OrderBy(s => s.NombreServicio)
                .Select(s => new SelectListItem($"{s.NombreServicio} - ${s.PrecioBase:N2}", s.IdServicio.ToString(), s.IdServicio == modelo.IdServicio))
                .ToList();
        }
    }
}
