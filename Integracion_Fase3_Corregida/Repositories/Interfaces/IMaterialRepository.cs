using DAS_Proyecto.Models;

namespace DAS_Proyecto.Repositories.Interfaces;

public interface IMaterialRepository
{
    Task<IReadOnlyList<Material>> ObtenerListadoAsync(string? busqueda, CancellationToken cancellationToken = default);
    Task<Material?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExisteCodigoAsync(string codigo, int? excluirId = null, CancellationToken cancellationToken = default);
    Task<bool> TieneDetallesAsociadosAsync(int id, CancellationToken cancellationToken = default);
    Task AgregarAsync(Material material, CancellationToken cancellationToken = default);
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
    Task EliminarAsync(Material material, CancellationToken cancellationToken = default);
}
