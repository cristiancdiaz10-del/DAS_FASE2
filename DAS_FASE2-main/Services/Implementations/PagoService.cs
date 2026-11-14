using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using DAS_Proyecto.Services.Interfaces;
using DAS_Proyecto.Services;

namespace DAS_Proyecto.Services.Implementations
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepository;
        private readonly PagoValidacionService _validacionService;

        public PagoService(
            IPagoRepository pagoRepository,
            PagoValidacionService validacionService)
        {
            _pagoRepository = pagoRepository;
            _validacionService = validacionService;
        }

        public Task<IEnumerable<Pago>> ObtenerTodosAsync()
        {
            return _pagoRepository.ObtenerTodosAsync();
        }

        public Task<Pago?> ObtenerPorIdAsync(int idPago)
        {
            return _pagoRepository.ObtenerPorIdAsync(idPago);
        }

        public Task<IEnumerable<Pago>> ObtenerPorOrdenAsync(
            int idOrdenTrabajo)
        {
            return _pagoRepository.ObtenerPorOrdenAsync(idOrdenTrabajo);
        }

        public Task<decimal> ObtenerTotalPagadoAsync(
            int idOrdenTrabajo)
        {
            return _pagoRepository.ObtenerTotalPagadoAsync(
                idOrdenTrabajo);
        }

        // ============================================
        // REGISTRAR PAGO
        // ============================================

        public async Task<ResultadoPago> RegistrarPagoAsync(
            int idOrdenTrabajo,
            decimal monto,
            TipoPago tipoPago,
            DateTime fechaPago)
        {
            if (idOrdenTrabajo <= 0)
            {
                return new ResultadoPago(
                    false,
                    "Debe seleccionar una orden válida.");
            }

            decimal? totalOrden =
                await _pagoRepository.ObtenerTotalOrdenAsync(
                    idOrdenTrabajo);

            if (totalOrden == null)
            {
                return new ResultadoPago(
                    false,
                    "La orden de trabajo no existe.");
            }

            decimal totalPagado =
                await _pagoRepository.ObtenerTotalPagadoAsync(
                    idOrdenTrabajo);

            bool anticipoRegistrado =
                await _pagoRepository.ExisteAnticipoAsync(
                    idOrdenTrabajo);

            string? error = _validacionService.ValidarPago(
                totalOrden.Value,
                totalPagado,
                tipoPago,
                monto,
                anticipoRegistrado);

            if (error != null)
            {
                return new ResultadoPago(false, error);
            }

            // Generar código único.
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

            return new ResultadoPago(
                true,
                "Pago registrado correctamente.",
                pago.CodigoPago);
        }

        // ============================================
        // GENERAR CÓDIGO ÚNICO DEL PAGO
        // ============================================

        private async Task<string> GenerarCodigoPagoAsync()
        {
            string codigo;

            do
            {
                string identificador = Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 12)
                    .ToUpperInvariant();

                codigo = $"PAG-{identificador}";

            } while (
                await _pagoRepository.ExisteCodigoAsync(codigo)
            );

            return codigo;
        }
    }
}