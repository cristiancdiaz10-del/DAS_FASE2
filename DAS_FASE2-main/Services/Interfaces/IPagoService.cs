
using DAS_Proyecto.Models;

namespace DAS_Proyecto.Services.Interfaces
{
    public interface IPagoService
    {
        Task<IEnumerable<Pago>> ObtenerTodosAsync();

        Task<Pago?> ObtenerPorIdAsync(int idPago);

        Task<IEnumerable<Pago>> ObtenerPorOrdenAsync(int idOrdenTrabajo);

        Task<decimal> ObtenerTotalPagadoAsync(int idOrdenTrabajo);

        Task<bool> RegistrarPagoAsync(
            int idOrdenTrabajo,
            decimal monto,
            TipoPago tipoPago);
    }
}
