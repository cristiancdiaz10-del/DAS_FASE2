using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using DAS_Proyecto.Services.Interfaces;
using DAS_Proyecto.ViewModels;

namespace DAS_Proyecto.Services.Implementations;

public class MaterialService : IMaterialService
{
    private readonly IMaterialRepository _repository;

    public MaterialService(IMaterialRepository repository) => _repository = repository;

    public Task<IReadOnlyList<Material>> ObtenerListadoAsync(string? busqueda, CancellationToken cancellationToken = default)
        => _repository.ObtenerListadoAsync(busqueda, cancellationToken);

    public Task<Material?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        => _repository.ObtenerPorIdAsync(id, cancellationToken);

    public async Task CrearAsync(MaterialViewModel modelo, CancellationToken cancellationToken = default)
    {
        Validar(modelo);
        modelo.CodigoMaterial = modelo.CodigoMaterial.Trim().ToUpperInvariant();
        if (await _repository.ExisteCodigoAsync(modelo.CodigoMaterial, null, cancellationToken))
            throw new InvalidOperationException("Ya existe un material con ese código.");

        var material = new Material
        {
            CodigoMaterial = modelo.CodigoMaterial,
            NombreMaterial = modelo.NombreMaterial.Trim(),
            CantidadDisponible = modelo.CantidadDisponible,
            UnidadMedida = modelo.UnidadMedida.Trim(),
            CostoUnitario = modelo.CostoUnitario,
            Descripcion = string.IsNullOrWhiteSpace(modelo.Descripcion) ? null : modelo.Descripcion.Trim()
        };
        await _repository.AgregarAsync(material, cancellationToken);
    }

    public async Task ActualizarAsync(MaterialViewModel modelo, CancellationToken cancellationToken = default)
    {
        Validar(modelo);
        var material = await _repository.ObtenerPorIdAsync(modelo.IdMaterial, cancellationToken)
            ?? throw new KeyNotFoundException("El material ya no existe.");
        var codigo = modelo.CodigoMaterial.Trim().ToUpperInvariant();
        if (await _repository.ExisteCodigoAsync(codigo, modelo.IdMaterial, cancellationToken))
            throw new InvalidOperationException("Otro material ya utiliza ese código.");

        material.CodigoMaterial = codigo;
        material.NombreMaterial = modelo.NombreMaterial.Trim();
        material.CantidadDisponible = modelo.CantidadDisponible;
        material.UnidadMedida = modelo.UnidadMedida.Trim();
        material.CostoUnitario = modelo.CostoUnitario;
        material.Descripcion = string.IsNullOrWhiteSpace(modelo.Descripcion) ? null : modelo.Descripcion.Trim();
        await _repository.GuardarCambiosAsync(cancellationToken);
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        var material = await _repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("El material no existe.");
        if (await _repository.TieneDetallesAsociadosAsync(id, cancellationToken))
            throw new InvalidOperationException("No se puede eliminar el material porque forma parte del historial de una orden de trabajo.");
        await _repository.EliminarAsync(material, cancellationToken);
    }

    private static void Validar(MaterialViewModel modelo)
    {
        if (string.IsNullOrWhiteSpace(modelo.CodigoMaterial) || string.IsNullOrWhiteSpace(modelo.NombreMaterial) || string.IsNullOrWhiteSpace(modelo.UnidadMedida))
            throw new InvalidOperationException("Completa el código, nombre y unidad del material.");
        if (modelo.CantidadDisponible < 0 || modelo.CostoUnitario < 0)
            throw new InvalidOperationException("Las cantidades y costos no pueden ser negativos.");
    }
}
