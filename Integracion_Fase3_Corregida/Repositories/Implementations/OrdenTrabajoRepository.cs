using DAS_Proyecto.Data;
using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Repositories.Implementations;

public class OrdenTrabajoRepository : IOrdenTrabajoRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public OrdenTrabajoRepository(ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<OrdenTrabajo>> ObtenerListadoAsync(string? busqueda, EstadoOrden? estado, CancellationToken cancellationToken = default)
    {
        IQueryable<OrdenTrabajo> query = _context.OrdenesTrabajo
            .AsNoTracking()
            .Include(o => o.Cotizacion).ThenInclude(c => c.Cliente)
            .Include(o => o.Cotizacion).ThenInclude(c => c.Servicio)
            .Include(o => o.Pagos);

        if (estado.HasValue)
            query = query.Where(o => o.Estado == estado.Value);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();
            query = query.Where(o => o.CodigoOrden.Contains(termino) ||
                                     o.Cotizacion.CodigoCotizacion.Contains(termino) ||
                                     o.Cotizacion.Cliente.Nombre.Contains(termino) ||
                                     (o.Cotizacion.Cliente.Empresa != null && o.Cotizacion.Cliente.Empresa.Contains(termino)) ||
                                     o.Cotizacion.DescripcionTrabajo.Contains(termino));
        }

        return await query.OrderByDescending(o => o.FechaOrden).ThenByDescending(o => o.IdOrdenTrabajo).ToListAsync(cancellationToken);
    }

    public Task<OrdenTrabajo?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.OrdenesTrabajo
            .Include(o => o.Cotizacion).ThenInclude(c => c.Cliente)
            .Include(o => o.Cotizacion).ThenInclude(c => c.Servicio)
            .Include(o => o.Pagos)
            .FirstOrDefaultAsync(o => o.IdOrdenTrabajo == id, cancellationToken);

    public Task<Cotizacion?> ObtenerCotizacionAsync(int idCotizacion, CancellationToken cancellationToken = default)
        => _context.Cotizaciones
            .Include(c => c.Cliente)
            .Include(c => c.Servicio)
            .FirstOrDefaultAsync(c => c.IdCotizacion == idCotizacion, cancellationToken);

    public Task<bool> ExistePorCotizacionAsync(int idCotizacion, CancellationToken cancellationToken = default)
        => _context.OrdenesTrabajo.AnyAsync(o => o.IdCotizacion == idCotizacion, cancellationToken);

    public async Task<IReadOnlyList<DetalleOrdenMaterial>> ObtenerDetallesAsync(int idOrden, CancellationToken cancellationToken = default)
        => await _context.DetalleOrdenMateriales
            .AsNoTracking()
            .Include(d => d.Material)
            .Where(d => d.IdOrdenTrabajo == idOrden)
            .OrderBy(d => d.IdDetalleOrdenMaterial)
            .ToListAsync(cancellationToken);

    public Task<bool> ExisteCodigoAsync(string codigo, CancellationToken cancellationToken = default)
        => _context.OrdenesTrabajo.AnyAsync(o => o.CodigoOrden == codigo, cancellationToken);

    public async Task AgregarAsync(OrdenTrabajo orden, CancellationToken cancellationToken = default)
    {
        await _context.OrdenesTrabajo.AddAsync(orden, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AgregarDetalleAsync(DetalleOrdenMaterial detalle, CancellationToken cancellationToken = default)
    {
        await _context.DetalleOrdenMateriales.AddAsync(detalle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
        => _unitOfWork.SaveChangesAsync(cancellationToken);
}
