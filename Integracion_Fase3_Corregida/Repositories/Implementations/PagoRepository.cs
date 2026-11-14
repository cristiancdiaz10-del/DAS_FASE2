using DAS_Proyecto.Data;
using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Repositories.Implementations
{
    public class PagoRepository : IPagoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public PagoRepository(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        private IQueryable<Pago> PagosConRelaciones()
        {
            return _context.Pagos
                .AsNoTracking()
                .Include(p => p.OrdenTrabajo)
                    .ThenInclude(o => o.Cotizacion)
                        .ThenInclude(c => c.Cliente);
        }

        public async Task<IEnumerable<Pago>> ObtenerTodosAsync()
        {
            return await PagosConRelaciones()
                .OrderByDescending(p => p.FechaPago)
                .ThenByDescending(p => p.IdPago)
                .ToListAsync();
        }

        public async Task<Pago?> ObtenerPorIdAsync(int idPago)
        {
            return await PagosConRelaciones()
                .FirstOrDefaultAsync(p => p.IdPago == idPago);
        }

        public async Task<Pago?> ObtenerPorCodigoAsync(string codigoPago)
        {
            if (string.IsNullOrWhiteSpace(codigoPago))
                return null;

            return await PagosConRelaciones()
                .FirstOrDefaultAsync(p => p.CodigoPago == codigoPago);
        }

        public async Task<IEnumerable<Pago>> ObtenerPorOrdenAsync(int idOrdenTrabajo)
        {
            return await PagosConRelaciones()
                .Where(p => p.IdOrdenTrabajo == idOrdenTrabajo)
                .OrderBy(p => p.FechaPago)
                .ToListAsync();
        }

        public async Task AgregarAsync(Pago pago)
        {
            await _context.Pagos.AddAsync(pago);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<decimal> ObtenerTotalPagadoAsync(int idOrdenTrabajo)
        {
            return await _context.Pagos
                .Where(p => p.IdOrdenTrabajo == idOrdenTrabajo)
                .SumAsync(p => (decimal?)p.MontoPago) ?? 0m;
        }

        public async Task<decimal?> ObtenerTotalOrdenAsync(int idOrdenTrabajo)
        {
            return await _context.OrdenesTrabajo
                .Where(o => o.IdOrdenTrabajo == idOrdenTrabajo)
                .Select(o => (decimal?)o.Cotizacion.TotalCotizacion)
                .FirstOrDefaultAsync();
        }

        public Task<bool> ExisteAnticipoAsync(int idOrdenTrabajo)
        {
            return _context.Pagos.AnyAsync(p =>
                p.IdOrdenTrabajo == idOrdenTrabajo &&
                p.TipoPago == TipoPago.Anticipo);
        }

        public Task<bool> ExisteCodigoAsync(string codigoPago)
        {
            return _context.Pagos.AnyAsync(p => p.CodigoPago == codigoPago);
        }

        public async Task<IEnumerable<OrdenTrabajo>> ObtenerOrdenesParaPagoAsync()
        {
            return await _context.OrdenesTrabajo
                .AsNoTracking()
                .Include(o => o.Cotizacion)
                    .ThenInclude(c => c.Cliente)
                .Include(o => o.Pagos)
                .OrderBy(o => o.FechaOrden)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pago>> ObtenerPorPeriodoAsync(
            DateTime inicio,
            DateTime finExclusivo)
        {
            return await PagosConRelaciones()
                .Where(p => p.FechaPago >= inicio && p.FechaPago < finExclusivo)
                .OrderBy(p => p.FechaPago)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrdenTrabajo>> ObtenerOrdenesPorPeriodoAsync(
            DateTime inicio,
            DateTime finExclusivo)
        {
            return await _context.OrdenesTrabajo
                .AsNoTracking()
                .Include(o => o.Cotizacion)
                .Where(o => o.FechaOrden >= inicio && o.FechaOrden < finExclusivo)
                .OrderBy(o => o.FechaOrden)
                .ToListAsync();
        }
    }
}
