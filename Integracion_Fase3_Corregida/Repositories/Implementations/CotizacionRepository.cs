using DAS_Proyecto.Data;
using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Repositories.Implementations
{
    public class CotizacionRepository : ICotizacionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public CotizacionRepository(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<Cotizacion>> ObtenerListadoAsync(
            string? busqueda,
            EstadoCotizacion? estado,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Cotizacion> query = _context.Cotizaciones
                .AsNoTracking()
                .Include(c => c.Cliente)
                .Include(c => c.Usuario)
                .Include(c => c.Servicio);

            if (estado.HasValue)
            {
                query = query.Where(c => c.Estado == estado.Value);
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var termino = busqueda.Trim();
                query = query.Where(c =>
                    c.CodigoCotizacion.Contains(termino) ||
                    c.Cliente.Nombre.Contains(termino) ||
                    (c.Cliente.Empresa != null && c.Cliente.Empresa.Contains(termino)) ||
                    c.DescripcionTrabajo.Contains(termino));
            }

            return await query
                .OrderByDescending(c => c.FechaCotizacion)
                .ThenByDescending(c => c.IdCotizacion)
                .ToListAsync(cancellationToken);
        }

        public Task<Cotizacion?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _context.Cotizaciones
                .Include(c => c.Cliente)
                .Include(c => c.Usuario)
                .Include(c => c.Servicio)
                .FirstOrDefaultAsync(c => c.IdCotizacion == id, cancellationToken);
        }

        public async Task<bool> ReferenciasExistenAsync(
            int idCliente,
            int idUsuario,
            int idServicio,
            CancellationToken cancellationToken = default)
        {
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.IdCliente == idCliente, cancellationToken);
            if (!clienteExiste) return false;

            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == idUsuario, cancellationToken);
            if (!usuarioExiste) return false;

            return await _context.Servicios.AnyAsync(s => s.IdServicio == idServicio, cancellationToken);
        }

        public Task<bool> ExisteCodigoAsync(string codigo, CancellationToken cancellationToken = default)
        {
            return _context.Cotizaciones.AnyAsync(c => c.CodigoCotizacion == codigo, cancellationToken);
        }

        public Task<bool> TieneOrdenAsociadaAsync(int idCotizacion, CancellationToken cancellationToken = default)
        {
            return _context.OrdenesTrabajo.AnyAsync(o => o.IdCotizacion == idCotizacion, cancellationToken);
        }

        public async Task AgregarAsync(Cotizacion cotizacion, CancellationToken cancellationToken = default)
        {
            await _context.Cotizaciones.AddAsync(cotizacion, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ActualizarAsync(Cotizacion cotizacion, CancellationToken cancellationToken = default)
        {
            // La entidad fue cargada y está siendo rastreada por este mismo DbContext.
            // No se usa Update() para evitar marcar Cliente, Usuario y Servicio como modificados.
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task EliminarAsync(Cotizacion cotizacion, CancellationToken cancellationToken = default)
        {
            _context.Cotizaciones.Remove(cotizacion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
