using DAS_Proyecto.Models;

namespace DAS_Proyecto.ViewModels;

public class DashboardViewModel
{
    public int TotalOrdenes { get; set; }
    public int OrdenesEnProceso { get; set; }
    public int CotizacionesPendientes { get; set; }
    public decimal IngresosDelMes { get; set; }
    public IReadOnlyList<OrdenTrabajo> OrdenesRecientes { get; set; } = Array.Empty<OrdenTrabajo>();
}
