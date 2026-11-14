using System.Data;
using DAS_Proyecto.Data;
using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using DAS_Proyecto.Services.Interfaces;
using DAS_Proyecto.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Services.Implementations;

public class OrdenTrabajoService : IOrdenTrabajoService
{
    private readonly IOrdenTrabajoRepository _repository;
    private readonly IMaterialRepository _materialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrdenTrabajoService(IOrdenTrabajoRepository repository, IMaterialRepository materialRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _materialRepository = materialRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyList<OrdenTrabajo>> ObtenerListadoAsync(string? busqueda, EstadoOrden? estado, CancellationToken cancellationToken = default)
        => _repository.ObtenerListadoAsync(busqueda, estado, cancellationToken);

    public async Task<OrdenTrabajoDetalleViewModel?> ObtenerDetalleAsync(int id, CancellationToken cancellationToken = default)
    {
        var orden = await _repository.ObtenerPorIdAsync(id, cancellationToken);
        if (orden == null) return null;
        var total = orden.Cotizacion?.TotalCotizacion ?? 0m;
        var pagado = orden.Pagos.Sum(p => p.MontoPago);
        return new OrdenTrabajoDetalleViewModel
        {
            Orden = orden,
            Detalles = await _repository.ObtenerDetallesAsync(id, cancellationToken),
            Materiales = await _materialRepository.ObtenerListadoAsync(null, cancellationToken),
            TotalPagado = pagado,
            SaldoPendiente = Math.Max(0m, total - pagado)
        };
    }

    public async Task<CrearOrdenViewModel> ObtenerDatosParaCrearAsync(int idCotizacion, CancellationToken cancellationToken = default)
    {
        var cotizacion = await _repository.ObtenerCotizacionAsync(idCotizacion, cancellationToken)
            ?? throw new KeyNotFoundException("La cotización no existe.");
        if (cotizacion.Estado != EstadoCotizacion.Aceptada)
            throw new InvalidOperationException("Solo se puede generar una orden desde una cotización aceptada.");
        if (await _repository.ExistePorCotizacionAsync(idCotizacion, cancellationToken))
            throw new InvalidOperationException("Esta cotización ya tiene una orden de trabajo.");

        return new CrearOrdenViewModel
        {
            IdCotizacion = cotizacion.IdCotizacion,
            CodigoCotizacion = cotizacion.CodigoCotizacion,
            Cliente = cotizacion.Cliente?.Nombre ?? string.Empty,
            Trabajo = cotizacion.DescripcionTrabajo,
            Total = cotizacion.TotalCotizacion
        };
    }

    public async Task CrearDesdeCotizacionAsync(CrearOrdenViewModel modelo, CancellationToken cancellationToken = default)
    {
        var cotizacion = await _repository.ObtenerCotizacionAsync(modelo.IdCotizacion, cancellationToken)
            ?? throw new KeyNotFoundException("La cotización no existe.");
        if (cotizacion.Estado != EstadoCotizacion.Aceptada)
            throw new InvalidOperationException("La cotización debe estar aceptada para generar una orden.");
        if (await _repository.ExistePorCotizacionAsync(cotizacion.IdCotizacion, cancellationToken))
            throw new InvalidOperationException("Esta cotización ya tiene una orden de trabajo.");

        string codigo;
        do { codigo = $"OT-{Guid.NewGuid().ToString("N")[..16].ToUpperInvariant()}"; }
        while (await _repository.ExisteCodigoAsync(codigo, cancellationToken));

        var orden = new OrdenTrabajo
        {
            CodigoOrden = codigo,
            IdCotizacion = cotizacion.IdCotizacion,
            Cotizacion = cotizacion,
            FechaOrden = DateTime.Today,
            FechaEntregaEstimada = modelo.FechaEntregaEstimada,
            Estado = EstadoOrden.PendienteAnticipo
        };
        orden.CalcularAnticipo();
        await _repository.AgregarAsync(orden, cancellationToken);
    }

    public async Task AgregarMaterialAsync(int idOrden, int idMaterial, decimal cantidad, CancellationToken cancellationToken = default)
    {
        if (cantidad <= 0) throw new InvalidOperationException("La cantidad debe ser mayor que cero.");
        var orden = await _repository.ObtenerPorIdAsync(idOrden, cancellationToken)
            ?? throw new KeyNotFoundException("La orden no existe.");
        if (orden.Estado == EstadoOrden.EnProceso || orden.Estado == EstadoOrden.Listo || orden.Estado == EstadoOrden.Entregado)
            throw new InvalidOperationException("No se pueden cambiar los materiales después de iniciar la producción.");

        var material = await _materialRepository.ObtenerPorIdAsync(idMaterial, cancellationToken)
            ?? throw new KeyNotFoundException("El material seleccionado no existe.");
        if (cantidad > material.CantidadDisponible)
            throw new InvalidOperationException($"La existencia actual de {material.NombreMaterial} es {material.CantidadDisponible:N2} {material.UnidadMedida}.");

        var detalle = new DetalleOrdenMaterial
        {
            IdOrdenTrabajo = idOrden,
            IdMaterial = idMaterial,
            CantidadUtilizada = cantidad,
            PrecioUnitario = material.CostoUnitario
        };
        detalle.CalcularSubtotal();
        await _repository.AgregarDetalleAsync(detalle, cancellationToken);
    }

    public async Task ActualizarEstadoAsync(int idOrden, EstadoOrden nuevoEstado, CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(typeof(EstadoOrden), nuevoEstado))
            throw new InvalidOperationException("El estado seleccionado no es válido.");

        await using var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var orden = await _repository.ObtenerPorIdAsync(idOrden, cancellationToken)
                ?? throw new KeyNotFoundException("La orden no existe.");

            if (orden.Estado == nuevoEstado)
                throw new InvalidOperationException("La orden ya se encuentra en ese estado.");

            var total = orden.Cotizacion.TotalCotizacion;
            var totalPagado = orden.Pagos.Sum(p => p.MontoPago);

            if (orden.Estado == EstadoOrden.PendienteAnticipo && nuevoEstado == EstadoOrden.Recepcion)
            {
                if (totalPagado < orden.AnticipoRequerido)
                    throw new InvalidOperationException($"Debe registrarse el anticipo de {orden.AnticipoRequerido:C2} antes de recibir la orden.");
            }
            else if (orden.Estado == EstadoOrden.Recepcion && nuevoEstado == EstadoOrden.EnProceso)
            {
                var detalles = await _repository.ObtenerDetallesAsync(idOrden, cancellationToken);
                if (detalles.Count == 0)
                    throw new InvalidOperationException("Agrega los materiales requeridos antes de iniciar la producción.");

                // Primero se valida la disponibilidad de todos los materiales. No se descuenta parcialmente.
                var agrupados = detalles
                    .GroupBy(d => d.IdMaterial)
                    .Select(g => new { IdMaterial = g.Key, Cantidad = g.Sum(x => x.CantidadUtilizada) })
                    .ToList();
                var materiales = new List<(Material Material, decimal Cantidad)>();
                foreach (var item in agrupados)
                {
                    var material = await _materialRepository.ObtenerPorIdAsync(item.IdMaterial, cancellationToken)
                        ?? throw new InvalidOperationException("Uno de los materiales de la orden ya no existe.");
                    if (material.CantidadDisponible < item.Cantidad)
                        throw new InvalidOperationException($"Stock insuficiente de {material.NombreMaterial}. Disponible: {material.CantidadDisponible:N2}; requerido: {item.Cantidad:N2}.");
                    materiales.Add((material, item.Cantidad));
                }
                foreach (var item in materiales)
                    item.Material.CantidadDisponible -= item.Cantidad;
            }
            else if (orden.Estado == EstadoOrden.EnProceso && nuevoEstado == EstadoOrden.Listo)
            {
                // Transición normal al finalizar la fabricación.
            }
            else if (orden.Estado == EstadoOrden.Listo && nuevoEstado == EstadoOrden.Entregado)
            {
                if (totalPagado < total)
                    throw new InvalidOperationException($"No se puede entregar la orden. Saldo pendiente: {Math.Max(0m, total - totalPagado):C2}.");
                orden.FechaEntregaReal = DateTime.Today;
            }
            else
            {
                throw new InvalidOperationException($"No se permite cambiar de {orden.Estado} a {nuevoEstado}.");
            }

            orden.Estado = nuevoEstado;
            await _repository.GuardarCambiosAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
