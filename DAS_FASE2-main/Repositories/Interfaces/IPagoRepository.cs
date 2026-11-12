
using DAS_Proyecto.Models;

namespace DAS_Proyecto.Repositories.Interfaces
{
    public interface IPagoRepository
    {
        Task<IEnumerable<Pago>> ObtenerTodosAsync();

        Task<Pago?> ObtenerPorIdAsync(int idPago);

        Task<IEnumerable<Pago>> ObtenerPorOrdenAsync(int idOrdenTrabajo);

        Task AgregarAsync(Pago pago);

        Task<decimal> ObtenerTotalPagadoAsync(int idOrdenTrabajo);

        Task<bool> ExisteAnticipoAsync(int idOrdenTrabajo);

        Task<bool> ExisteCodigoAsync(string codigoPago);
    }
}
