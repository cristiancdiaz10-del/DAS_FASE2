using System.ComponentModel.DataAnnotations;

namespace DAS_Proyecto.ViewModels;

public class MaterialViewModel
{
    public int IdMaterial { get; set; }

    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(30)]
    [Display(Name = "Código")]
    public string CodigoMaterial { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del material es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Material")]
    public string NombreMaterial { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "La existencia no puede ser negativa.")]
    [Display(Name = "Existencias")]
    public decimal CantidadDisponible { get; set; }

    [Required(ErrorMessage = "Selecciona una unidad de medida.")]
    [StringLength(40)]
    [Display(Name = "Unidad de medida")]
    public string UnidadMedida { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "El costo no puede ser negativo.")]
    [Display(Name = "Costo unitario")]
    public decimal CostoUnitario { get; set; }

    [StringLength(500)]
    public string? Descripcion { get; set; }
}
