using DAS_Proyecto.Models;

namespace DAS_Proyecto.ViewModels;

public class ClienteDetalleViewModel
{
    public Cliente Cliente { get; set; } = null!;
    public IReadOnlyList<OrdenTrabajo> Ordenes { get; set; } = Array.Empty<OrdenTrabajo>();
}
