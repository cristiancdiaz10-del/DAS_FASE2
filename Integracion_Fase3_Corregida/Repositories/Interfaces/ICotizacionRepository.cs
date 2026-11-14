using DAS_Proyecto.Models;

namespace DAS_Proyecto.Repositories.Interfaces
{
    public interface ICotizacionRepository
    {
        Task<IReadOnlyList<Cotizacion>> ObtenerListadoAsync(
            string? busqueda,
            EstadoCotizacion? estado,
            CancellationToken cancellationToken = default);

        Task<Cotizacion?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ReferenciasExistenAsync(int idCliente, int idUsuario, int idServicio, CancellationToken cancellationToken = default);
        Task<bool> ExisteCodigoAsync(string codigo, CancellationToken cancellationToken = default);
        Task<bool> TieneOrdenAsociadaAsync(int idCotizacion, CancellationToken cancellationToken = default);
        Task AgregarAsync(Cotizacion cotizacion, CancellationToken cancellationToken = default);
        Task ActualizarAsync(Cotizacion cotizacion, CancellationToken cancellationToken = default);
        Task EliminarAsync(Cotizacion cotizacion, CancellationToken cancellationToken = default);
    }
}
