using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using DAS_Proyecto.Services.Interfaces;

namespace DAS_Proyecto.Services.Implementations
{
    public class CotizacionService : ICotizacionService
    {
        private readonly ICotizacionRepository _repository;

        public CotizacionService(ICotizacionRepository repository)
        {
            _repository = repository;
        }

        public Task<IReadOnlyList<Cotizacion>> ObtenerListadoAsync(
            string? busqueda,
            EstadoCotizacion? estado,
            CancellationToken cancellationToken = default)
        {
            return _repository.ObtenerListadoAsync(busqueda, estado, cancellationToken);
        }

        public Task<Cotizacion?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _repository.ObtenerPorIdAsync(id, cancellationToken);
        }

        public async Task CrearAsync(Cotizacion cotizacion, CancellationToken cancellationToken = default)
        {
            ValidarDatos(cotizacion);
            await ValidarReferenciasAsync(cotizacion, cancellationToken);

            cotizacion.CodigoCotizacion = await GenerarCodigoAsync(cancellationToken);
            cotizacion.FechaCotizacion = cotizacion.FechaCotizacion == default
                ? DateTime.Today
                : cotizacion.FechaCotizacion.Date;
            cotizacion.Estado = EstadoCotizacion.Pendiente;
            cotizacion.CalcularTotal();

            await _repository.AgregarAsync(cotizacion, cancellationToken);
        }

        public async Task ActualizarAsync(int id, Cotizacion datos, CancellationToken cancellationToken = default)
        {
            var existente = await _repository.ObtenerPorIdAsync(id, cancellationToken);
            if (existente == null)
            {
                throw new KeyNotFoundException("La cotización que intentas editar ya no existe.");
            }

            if (await _repository.TieneOrdenAsociadaAsync(id, cancellationToken))
            {
                throw new InvalidOperationException("No se puede modificar una cotización que ya tiene una orden de trabajo asociada, para conservar el importe histórico de la orden.");
            }

            ValidarDatos(datos);
            await ValidarReferenciasAsync(datos, cancellationToken);

            if (!Enum.IsDefined(typeof(EstadoCotizacion), datos.Estado))
            {
                throw new InvalidOperationException("El estado seleccionado no es válido.");
            }

            existente.IdCliente = datos.IdCliente;
            existente.IdUsuario = datos.IdUsuario;
            existente.IdServicio = datos.IdServicio;
            existente.FechaCotizacion = datos.FechaCotizacion.Date;
            existente.DescripcionTrabajo = datos.DescripcionTrabajo.Trim();
            existente.CostoMateriales = datos.CostoMateriales;
            existente.CostoManoObra = datos.CostoManoObra;
            existente.Estado = datos.Estado;
            existente.CalcularTotal();

            await _repository.ActualizarAsync(existente, cancellationToken);
        }

        public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var cotizacion = await _repository.ObtenerPorIdAsync(id, cancellationToken);
            if (cotizacion == null)
            {
                throw new KeyNotFoundException("La cotización que intentas eliminar no existe.");
            }

            if (await _repository.TieneOrdenAsociadaAsync(id, cancellationToken))
            {
                throw new InvalidOperationException("No se puede eliminar esta cotización porque ya tiene una orden de trabajo asociada.");
            }

            await _repository.EliminarAsync(cotizacion, cancellationToken);
        }

        private static void ValidarDatos(Cotizacion cotizacion)
        {
            if (cotizacion.IdCliente <= 0 || cotizacion.IdUsuario <= 0 || cotizacion.IdServicio <= 0)
            {
                throw new InvalidOperationException("Selecciona un cliente, un usuario responsable y un servicio válidos.");
            }

            if (string.IsNullOrWhiteSpace(cotizacion.DescripcionTrabajo))
            {
                throw new InvalidOperationException("La descripción del trabajo es obligatoria.");
            }

            if (cotizacion.CostoMateriales < 0 || cotizacion.CostoManoObra < 0)
            {
                throw new InvalidOperationException("Los costos no pueden ser negativos.");
            }

            if (cotizacion.CostoMateriales + cotizacion.CostoManoObra <= 0)
            {
                throw new InvalidOperationException("El total de la cotización debe ser mayor que cero.");
            }
        }

        private async Task ValidarReferenciasAsync(Cotizacion cotizacion, CancellationToken cancellationToken)
        {
            var referenciasValidas = await _repository.ReferenciasExistenAsync(
                cotizacion.IdCliente,
                cotizacion.IdUsuario,
                cotizacion.IdServicio,
                cancellationToken);

            if (!referenciasValidas)
            {
                throw new InvalidOperationException("El cliente, usuario o servicio seleccionado ya no existe. Actualiza el formulario e inténtalo nuevamente.");
            }
        }

        private async Task<string> GenerarCodigoAsync(CancellationToken cancellationToken)
        {
            // El prefijo más 16 caracteres hexadecimales cabe en CodigoCotizacion (20 caracteres).
            // Se verifica contra la base antes de guardar para evitar reutilizar un código existente.
            string codigo;
            do
            {
                codigo = $"COT-{Guid.NewGuid().ToString("N")[..16].ToUpperInvariant()}";
            }
            while (await _repository.ExisteCodigoAsync(codigo, cancellationToken));

            return codigo;
        }
    }
}
