using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DAS_Proyecto.Models;
using DAS_Proyecto.Data;
using DAS_Proyecto.Repositories.Interfaces;
using DAS_Proyecto.Services.Interfaces;
using DAS_Proyecto.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Services.Implementations
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepository;
        private readonly PagoValidacionService _validacionService;
        private readonly InformeMensualService _informeService;
        private readonly IUnitOfWork? _unitOfWork;

        // Se conservan constructores cortos para las pruebas unitarias.
        public PagoService(
            IPagoRepository pagoRepository,
            PagoValidacionService validacionService)
            : this(pagoRepository, validacionService, new InformeMensualService(), null)
        {
        }

        public PagoService(
            IPagoRepository pagoRepository,
            PagoValidacionService validacionService,
            InformeMensualService informeService)
            : this(pagoRepository, validacionService, informeService, null)
        {
        }

        public PagoService(
            IPagoRepository pagoRepository,
            PagoValidacionService validacionService,
            InformeMensualService informeService,
            IUnitOfWork? unitOfWork)
        {
            _pagoRepository = pagoRepository;
            _validacionService = validacionService;
            _informeService = informeService;
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<Pago>> ObtenerTodosAsync()
            => _pagoRepository.ObtenerTodosAsync();

        public Task<Pago?> ObtenerPorIdAsync(int idPago)
            => _pagoRepository.ObtenerPorIdAsync(idPago);

        public Task<IEnumerable<Pago>> ObtenerPorOrdenAsync(int idOrdenTrabajo)
            => _pagoRepository.ObtenerPorOrdenAsync(idOrdenTrabajo);

        public Task<decimal> ObtenerTotalPagadoAsync(int idOrdenTrabajo)
            => _pagoRepository.ObtenerTotalPagadoAsync(idOrdenTrabajo);

        public async Task<PagoListadoViewModel> ObtenerListadoAsync()
        {
            var pagos = (await _pagoRepository.ObtenerTodosAsync()).ToList();
            var ordenes = (await _pagoRepository.ObtenerOrdenesParaPagoAsync()).ToList();

            return new PagoListadoViewModel
            {
                TotalPagado = pagos.Sum(p => p.MontoPago),
                TotalAnticipos = pagos
                    .Where(p => p.TipoPago == TipoPago.Anticipo)
                    .Sum(p => p.MontoPago),
                TotalSaldosPendientes = ordenes.Sum(o =>
                    Math.Max(0m, (o.Cotizacion?.TotalCotizacion ?? 0m)
                        - o.Pagos.Sum(p => p.MontoPago))),
                Pagos = pagos.Select(p => new PagoItemViewModel
                {
                    CodigoPago = p.CodigoPago,
                    CodigoOrden = p.OrdenTrabajo?.CodigoOrden ?? string.Empty,
                    NombreCliente = p.OrdenTrabajo?.Cotizacion?.Cliente?.Nombre ?? string.Empty,
                    MontoPago = p.MontoPago,
                    TipoPago = p.TipoPago,
                    FechaPago = p.FechaPago
                }).ToList()
            };
        }

        public async Task<List<OrdenPagoOpcionViewModel>> ObtenerOrdenesDisponiblesAsync()
        {
            var ordenes = await _pagoRepository.ObtenerOrdenesParaPagoAsync();

            return ordenes.Select(o =>
            {
                decimal total = o.Cotizacion?.TotalCotizacion ?? 0m;
                decimal pagado = o.Pagos.Sum(p => p.MontoPago);
                decimal saldo = Math.Max(0m, total - pagado);

                return new OrdenPagoOpcionViewModel
                {
                    IdOrdenTrabajo = o.IdOrdenTrabajo,
                    CodigoOrden = o.CodigoOrden,
                    NombreCliente = o.Cotizacion?.Cliente?.Nombre ?? "Cliente no disponible",
                    TotalOrden = total,
                    TotalPagado = pagado,
                    SaldoPendiente = saldo
                };
            })
            .Where(o => o.SaldoPendiente > 0m)
            .OrderBy(o => o.CodigoOrden)
            .ToList();
        }

        public async Task<DetallePagoViewModel?> ObtenerDetallePorCodigoAsync(string codigoPago)
        {
            var pago = await _pagoRepository.ObtenerPorCodigoAsync(codigoPago);
            if (pago == null)
                return null;

            return new DetallePagoViewModel
            {
                CodigoPago = pago.CodigoPago,
                TipoPago = pago.TipoPago,
                MontoPago = pago.MontoPago,
                FechaPago = pago.FechaPago,
                CodigoOrden = pago.OrdenTrabajo?.CodigoOrden ?? string.Empty,
                NombreCliente = pago.OrdenTrabajo?.Cotizacion?.Cliente?.Nombre ?? string.Empty,
                EstadoOrden = pago.OrdenTrabajo?.Estado.ToString() ?? string.Empty
            };
        }

        public async Task<InformeMensualViewModel> ObtenerInformeMensualAsync(int mes, int anio)
        {
            var periodo = _informeService.ObtenerPeriodo(mes, anio);
            var pagos = (await _pagoRepository.ObtenerPorPeriodoAsync(
                periodo.Inicio, periodo.Fin)).ToList();
            var ordenes = (await _pagoRepository.ObtenerOrdenesPorPeriodoAsync(
                periodo.Inicio, periodo.Fin)).ToList();

            var pedidos = ordenes
                .Select(o => (o.FechaOrden, o.Estado.ToString()))
                .ToList();

            return new InformeMensualViewModel
            {
                Mes = mes,
                Anio = anio,
                TotalIngresos = _informeService.CalcularTotalIngresos(pagos, mes, anio),
                CantidadPedidos = _informeService.ContarPedidos(pedidos, mes, anio),
                IngresosSemanales = _informeService
                    .CalcularIngresosSemanales(pagos, mes, anio)
                    .Select(s => new IngresoSemanalViewModel
                    {
                        Semana = s.Semana,
                        Inicio = s.Inicio,
                        FinExclusivo = s.FinExclusivo,
                        TotalIngresos = s.TotalIngresos
                    })
                    .ToList(),
                PedidosPorEstado = _informeService
                    .ContarPedidosPorEstado(pedidos, mes, anio)
            };
        }

        public async Task<ResultadoPago> RegistrarPagoAsync(
            int idOrdenTrabajo,
            decimal monto,
            TipoPago tipoPago,
            DateTime fechaPago)
        {
            if (idOrdenTrabajo <= 0)
                return new ResultadoPago(false, "Debe seleccionar una orden válida.");

            if (fechaPago == default || fechaPago.Date > DateTime.Today)
                return new ResultadoPago(false, "La fecha del pago no es válida.");

            // La transacción serializable evita que dos operaciones lean el mismo saldo
            // y registren simultáneamente pagos que superen el total de la orden.
            await using var transaction = _unitOfWork == null
                ? null
                : await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                decimal? totalOrden = await _pagoRepository.ObtenerTotalOrdenAsync(idOrdenTrabajo);
                if (totalOrden == null)
                    return new ResultadoPago(false, "La orden de trabajo no existe.");

                decimal totalPagado = await _pagoRepository.ObtenerTotalPagadoAsync(idOrdenTrabajo);
                bool anticipoRegistrado = await _pagoRepository.ExisteAnticipoAsync(idOrdenTrabajo);

                string? error = _validacionService.ValidarPago(
                    totalOrden.Value, totalPagado, tipoPago, monto, anticipoRegistrado);

                if (error != null)
                    return new ResultadoPago(false, error);

                string codigoPago = await GenerarCodigoPagoAsync();
                var pago = new Pago
                {
                    CodigoPago = codigoPago,
                    IdOrdenTrabajo = idOrdenTrabajo,
                    MontoPago = monto,
                    FechaPago = fechaPago,
                    TipoPago = tipoPago
                };

                await _pagoRepository.AgregarAsync(pago);
                if (transaction != null)
                    await transaction.CommitAsync();

                return new ResultadoPago(true, "Pago registrado correctamente.", pago.CodigoPago);
            }
            catch (DbUpdateException)
            {
                // Los índices únicos de SQL Server protegen frente a duplicados.
                return new ResultadoPago(false,
                    "No se pudo registrar el pago. Verifique que no exista un anticipo duplicado y que el saldo siga disponible.");
            }
        }

        private async Task<string> GenerarCodigoPagoAsync()
        {
            string codigo;
            do
            {
                string identificador = Guid.NewGuid()
                    .ToString("N")[..12]
                    .ToUpperInvariant();
                codigo = $"PAG-{identificador}";
            }
            while (await _pagoRepository.ExisteCodigoAsync(codigo));

            return codigo;
        }
    }
}
