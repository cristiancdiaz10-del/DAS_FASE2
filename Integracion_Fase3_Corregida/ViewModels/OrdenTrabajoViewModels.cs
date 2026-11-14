using DAS_Proyecto.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DAS_Proyecto.ViewModels;

public class OrdenTrabajoListadoViewModel
{
    public IReadOnlyList<OrdenTrabajo> Ordenes { get; set; } = Array.Empty<OrdenTrabajo>();
    public string? Busqueda { get; set; }
    public string? EstadoSeleccionado { get; set; }
}

public class OrdenTrabajoDetalleViewModel
{
    public OrdenTrabajo Orden { get; set; } = null!;
    public IReadOnlyList<Material> Materiales { get; set; } = Array.Empty<Material>();
    public IReadOnlyList<DetalleOrdenMaterial> Detalles { get; set; } = Array.Empty<DetalleOrdenMaterial>();
    public decimal TotalPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public int IdMaterial { get; set; }
    [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "La cantidad debe ser mayor que cero.")]
    public decimal CantidadUtilizada { get; set; }
}

public class CrearOrdenViewModel
{
    public int IdCotizacion { get; set; }
    public string CodigoCotizacion { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string Trabajo { get; set; } = string.Empty;
    public decimal Total { get; set; }
    [DataType(DataType.Date)]
    public DateTime? FechaEntregaEstimada { get; set; }
}
