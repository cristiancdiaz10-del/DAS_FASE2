using DAS_Proyecto.Models;

namespace DAS_Proyecto.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> ObtenerTodosAsync(string? busqueda = null);
        Task<Usuario?> ObtenerPorIdAsync(int id);

        Task<Usuario?> ObtenerPorCorreoAsync(string correo);

        Task AgregarAsync(Usuario usuario);

        Task ActualizarAsync(Usuario usuario);

        Task EliminarAsync(int id);
    }
}