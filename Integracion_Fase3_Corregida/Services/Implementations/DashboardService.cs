using DAS_Proyecto.Repositories.Interfaces;
using DAS_Proyecto.Services.Interfaces;
using DAS_Proyecto.ViewModels;
namespace DAS_Proyecto.Services.Implementations;
public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repository;
    public DashboardService(IDashboardRepository repository) => _repository = repository;
    public Task<DashboardViewModel> ObtenerResumenAsync(CancellationToken cancellationToken = default)
        => _repository.ObtenerResumenAsync(cancellationToken);
}
