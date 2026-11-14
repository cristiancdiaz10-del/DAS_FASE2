using DAS_Proyecto.Models;
using DAS_Proyecto.ViewModels;

namespace DAS_Proyecto.Services.Interfaces;

public interface IMaterialService
{
    Task<IReadOnlyList<Material>> ObtenerListadoAsync(string? busqueda, CancellationToken cancellationToken = default);
    Task<Material?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task CrearAsync(MaterialViewModel modelo, CancellationToken cancellationToken = default);
    Task ActualizarAsync(MaterialViewModel modelo, CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}
