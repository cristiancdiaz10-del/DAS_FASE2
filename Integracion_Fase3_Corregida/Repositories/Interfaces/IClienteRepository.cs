using DAS_Proyecto.Models;

namespace DAS_Proyecto.Repositories.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> ObtenerTodosAsync(string? busqueda = null);
        Task<IReadOnlyList<OrdenTrabajo>> ObtenerOrdenesAsync(int idCliente);
        Task<Cliente?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Cliente cliente);
        Task ActualizarAsync(Cliente cliente);
        Task EliminarAsync(int id);
    }
}