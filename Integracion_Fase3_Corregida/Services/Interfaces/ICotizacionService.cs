using DAS_Proyecto.Models;

namespace DAS_Proyecto.Services.Interfaces
{
    public interface ICotizacionService
    {
        Task<IReadOnlyList<Cotizacion>> ObtenerListadoAsync(string? busqueda, EstadoCotizacion? estado, CancellationToken cancellationToken = default);
        Task<Cotizacion?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task CrearAsync(Cotizacion cotizacion, CancellationToken cancellationToken = default);
        Task ActualizarAsync(int id, Cotizacion datos, CancellationToken cancellationToken = default);
        Task EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}
