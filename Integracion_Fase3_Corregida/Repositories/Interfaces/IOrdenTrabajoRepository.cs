using DAS_Proyecto.Models;

namespace DAS_Proyecto.Repositories.Interfaces;

public interface IOrdenTrabajoRepository
{
    Task<IReadOnlyList<OrdenTrabajo>> ObtenerListadoAsync(string? busqueda, EstadoOrden? estado, CancellationToken cancellationToken = default);
    Task<OrdenTrabajo?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Cotizacion?> ObtenerCotizacionAsync(int idCotizacion, CancellationToken cancellationToken = default);
    Task<bool> ExistePorCotizacionAsync(int idCotizacion, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DetalleOrdenMaterial>> ObtenerDetallesAsync(int idOrden, CancellationToken cancellationToken = default);
    Task<bool> ExisteCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task AgregarAsync(OrdenTrabajo orden, CancellationToken cancellationToken = default);
    Task AgregarDetalleAsync(DetalleOrdenMaterial detalle, CancellationToken cancellationToken = default);
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
