using DAS_FASE2.Models;

namespace DAS_FASE2.Repositories.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> ObtenerTodosAsync();
        Task<Cliente?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Cliente cliente);
        Task ActualizarAsync(Cliente cliente);
        Task EliminarAsync(int id);
    }
}
