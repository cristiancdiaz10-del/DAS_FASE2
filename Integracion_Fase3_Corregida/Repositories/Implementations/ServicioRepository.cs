using DAS_Proyecto.Data;
using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Repositories.Implementations
{
    public class ServicioRepository : IServicioRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public ServicioRepository(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Servicio>> ObtenerTodosAsync(string? busqueda = null)
        {
            IQueryable<Servicio> query = _context.Servicios.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var termino = busqueda.Trim();
                query = query.Where(s => s.NombreServicio.Contains(termino) ||
                    (s.Descripcion != null && s.Descripcion.Contains(termino)));
            }
            return await query.OrderBy(s => s.NombreServicio).ToListAsync();
        }

        public async Task<Servicio?> ObtenerPorIdAsync(int id)
        {
            return await _context.Servicios.FindAsync(id);
        }

        public async Task AgregarAsync(Servicio servicio)
        {
            await _context.Servicios.AddAsync(servicio);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Servicio servicio)
        {
            _context.Servicios.Update(servicio);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio != null)
            {
                _context.Servicios.Remove(servicio);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}