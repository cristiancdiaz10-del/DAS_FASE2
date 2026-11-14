using DAS_Proyecto.Data;
using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Repositories.Implementations
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public ClienteRepository(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Cliente>> ObtenerTodosAsync(string? busqueda = null)
        {
            IQueryable<Cliente> query = _context.Clientes.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var termino = busqueda.Trim();
                query = query.Where(c => c.Nombre.Contains(termino) ||
                    c.Telefono.Contains(termino) ||
                    (c.Correo != null && c.Correo.Contains(termino)) ||
                    (c.Empresa != null && c.Empresa.Contains(termino)));
            }
            return await query.OrderBy(c => c.Nombre).ToListAsync();
        }

        public async Task<IReadOnlyList<OrdenTrabajo>> ObtenerOrdenesAsync(int idCliente)
        {
            return await _context.OrdenesTrabajo
                .AsNoTracking()
                .Include(o => o.Cotizacion)
                .Where(o => o.Cotizacion.IdCliente == idCliente)
                .OrderByDescending(o => o.FechaOrden)
                .ToListAsync();
        }

        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        public async Task AgregarAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}