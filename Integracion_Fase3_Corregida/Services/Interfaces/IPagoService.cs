using DAS_Proyecto.Models;
using DAS_Proyecto.ViewModels;

namespace DAS_Proyecto.Services.Interfaces
{
    public interface IPagoService
    {
        Task<IEnumerable<Pago>> ObtenerTodosAsync();
        Task<Pago?> ObtenerPorIdAsync(int idPago);
        Task<IEnumerable<Pago>> ObtenerPorOrdenAsync(int idOrdenTrabajo);
        Task<decimal> ObtenerTotalPagadoAsync(int idOrdenTrabajo);

        Task<PagoListadoViewModel> ObtenerListadoAsync();
        Task<List<OrdenPagoOpcionViewModel>> ObtenerOrdenesDisponiblesAsync();
        Task<DetallePagoViewModel?> ObtenerDetallePorCodigoAsync(string codigoPago);
        Task<InformeMensualViewModel> ObtenerInformeMensualAsync(int mes, int anio);

        Task<ResultadoPago> RegistrarPagoAsync(
            int idOrdenTrabajo,
            decimal monto,
            TipoPago tipoPago,
            DateTime fechaPago);
    }

    public sealed record ResultadoPago(
        bool Exito,
        string Mensaje,
        string? CodigoPago = null);
}
