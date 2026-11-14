using DAS_Proyecto.Data;
using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Repositories.Implementations
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioRepository(
            ApplicationDbContext context,
            IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync(string? busqueda = null)
        {
            IQueryable<Usuario> query = _context.Usuarios.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var termino = busqueda.Trim();

                var buscarPorRol = Enum.TryParse<RolUsuario>(
                    termino, true, out var rol
                ) && Enum.IsDefined(typeof(RolUsuario), rol);

                query = query.Where(u =>
                    u.Nombre.Contains(termino) ||
                    u.Correo.Contains(termino) ||
                    (buscarPorRol && u.Rol == rol)
                );
            }

            return await query.OrderBy(u => u.Nombre).ToListAsync();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(
                u => u.Correo == correo.Trim()
            );
        }

        public async Task AgregarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}