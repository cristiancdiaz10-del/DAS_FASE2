using DAS_FASE2.Models;

namespace DAS_FASE2.Repositories.Interfaces
{
    public interface IServicioRepository
    {
        Task<IEnumerable<Servicio>> ObtenerTodosAsync();
        Task<Servicio?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Servicio servicio);
        Task ActualizarAsync(Servicio servicio);
        Task EliminarAsync(int id);
    }
}
