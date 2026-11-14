using DAS_Proyecto.Data;
using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using DAS_Proyecto.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Repositories.Implementations;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;
    public DashboardRepository(ApplicationDbContext context) => _context = context;

    public async Task<DashboardViewModel> ObtenerResumenAsync(CancellationToken cancellationToken = default)
    {
        var hoy = DateTime.Today;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
        var inicioMesSiguiente = inicioMes.AddMonths(1);

        var ingresos = await _context.Pagos
            .Where(p => p.FechaPago >= inicioMes && p.FechaPago < inicioMesSiguiente)
            .Select(p => (decimal?)p.MontoPago)
            .SumAsync(cancellationToken) ?? 0m;

        var recientes = await _context.OrdenesTrabajo
            .AsNoTracking()
            .Include(o => o.Cotizacion).ThenInclude(c => c.Cliente)
            .OrderByDescending(o => o.FechaOrden)
            .ThenByDescending(o => o.IdOrdenTrabajo)
            .Take(5)
            .ToListAsync(cancellationToken);

        return new DashboardViewModel
        {
            TotalOrdenes = await _context.OrdenesTrabajo.CountAsync(cancellationToken),
            OrdenesEnProceso = await _context.OrdenesTrabajo.CountAsync(o => o.Estado == EstadoOrden.EnProceso, cancellationToken),
            CotizacionesPendientes = await _context.Cotizaciones.CountAsync(c => c.Estado == EstadoCotizacion.Pendiente, cancellationToken),
            IngresosDelMes = ingresos,
            OrdenesRecientes = recientes
        };
    }
}
