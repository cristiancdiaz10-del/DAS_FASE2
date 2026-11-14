using DAS_Proyecto.Models;

namespace DAS_Proyecto.Repositories.Interfaces
{
    public interface IPagoRepository
    {
        Task<IEnumerable<Pago>> ObtenerTodosAsync();
        Task<Pago?> ObtenerPorIdAsync(int idPago);
        Task<Pago?> ObtenerPorCodigoAsync(string codigoPago);
        Task<IEnumerable<Pago>> ObtenerPorOrdenAsync(int idOrdenTrabajo);
        Task AgregarAsync(Pago pago);
        Task<decimal> ObtenerTotalPagadoAsync(int idOrdenTrabajo);
        Task<decimal?> ObtenerTotalOrdenAsync(int idOrdenTrabajo);
        Task<bool> ExisteAnticipoAsync(int idOrdenTrabajo);
        Task<bool> ExisteCodigoAsync(string codigoPago);

        // Consultas para el formulario y los informes.
        Task<IEnumerable<OrdenTrabajo>> ObtenerOrdenesParaPagoAsync();
        Task<IEnumerable<Pago>> ObtenerPorPeriodoAsync(DateTime inicio, DateTime finExclusivo);
        Task<IEnumerable<OrdenTrabajo>> ObtenerOrdenesPorPeriodoAsync(DateTime inicio, DateTime finExclusivo);
    }
}
