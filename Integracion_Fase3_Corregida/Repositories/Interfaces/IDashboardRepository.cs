using DAS_Proyecto.Models;
using DAS_Proyecto.ViewModels;

namespace DAS_Proyecto.Repositories.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardViewModel> ObtenerResumenAsync(CancellationToken cancellationToken = default);
}
