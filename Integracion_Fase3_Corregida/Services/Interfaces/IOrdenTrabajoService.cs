using DAS_Proyecto.Models;
using DAS_Proyecto.ViewModels;

namespace DAS_Proyecto.Services.Interfaces;

public interface IOrdenTrabajoService
{
    Task<IReadOnlyList<OrdenTrabajo>> ObtenerListadoAsync(string? busqueda, EstadoOrden? estado, CancellationToken cancellationToken = default);
    Task<OrdenTrabajoDetalleViewModel?> ObtenerDetalleAsync(int id, CancellationToken cancellationToken = default);
    Task<CrearOrdenViewModel> ObtenerDatosParaCrearAsync(int idCotizacion, CancellationToken cancellationToken = default);
    Task CrearDesdeCotizacionAsync(CrearOrdenViewModel modelo, CancellationToken cancellationToken = default);
    Task AgregarMaterialAsync(int idOrden, int idMaterial, decimal cantidad, CancellationToken cancellationToken = default);
    Task ActualizarEstadoAsync(int idOrden, EstadoOrden nuevoEstado, CancellationToken cancellationToken = default);
}
