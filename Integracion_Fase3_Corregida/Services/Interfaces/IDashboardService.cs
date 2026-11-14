using DAS_Proyecto.ViewModels;
namespace DAS_Proyecto.Services.Interfaces;
public interface IDashboardService
{
    Task<DashboardViewModel> ObtenerResumenAsync(CancellationToken cancellationToken = default);
}
