using DAS_Proyecto.Data;
using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Repositories.Implementations;

public class MaterialRepository : IMaterialRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public MaterialRepository(ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<Material>> ObtenerListadoAsync(string? busqueda, CancellationToken cancellationToken = default)
    {
        IQueryable<Material> query = _context.Materiales.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();
            query = query.Where(m => m.CodigoMaterial.Contains(termino) ||
                                     m.NombreMaterial.Contains(termino) ||
                                     m.UnidadMedida.Contains(termino) ||
                                     (m.Descripcion != null && m.Descripcion.Contains(termino)));
        }
        return await query.OrderBy(m => m.NombreMaterial).ToListAsync(cancellationToken);
    }

    public Task<Material?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.Materiales.FirstOrDefaultAsync(m => m.IdMaterial == id, cancellationToken);

    public Task<bool> ExisteCodigoAsync(string codigo, int? excluirId = null, CancellationToken cancellationToken = default)
        => _context.Materiales.AnyAsync(m => m.CodigoMaterial == codigo && (!excluirId.HasValue || m.IdMaterial != excluirId.Value), cancellationToken);

    public Task<bool> TieneDetallesAsociadosAsync(int id, CancellationToken cancellationToken = default)
        => _context.DetalleOrdenMateriales.AnyAsync(d => d.IdMaterial == id, cancellationToken);

    public async Task AgregarAsync(Material material, CancellationToken cancellationToken = default)
    {
        await _context.Materiales.AddAsync(material, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
        => _unitOfWork.SaveChangesAsync(cancellationToken);

    public async Task EliminarAsync(Material material, CancellationToken cancellationToken = default)
    {
        _context.Materiales.Remove(material);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
