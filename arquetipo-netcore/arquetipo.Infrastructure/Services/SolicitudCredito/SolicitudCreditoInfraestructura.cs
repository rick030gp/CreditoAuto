using arquetipo.Domain.Interfaces.Services.ClientePatio;
using arquetipo.Domain.Interfaces.Services.Clientes;
using arquetipo.Domain.Interfaces.Services.Patios;
using arquetipo.Domain.Interfaces.Services.SolicitudCredito;
using arquetipo.Domain.Interfaces.Services.Vehiculos;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;

namespace arquetipo.Infrastructure.Services.SolicitudCredito
{
    public class SolicitudCreditoInfraestructura : ISolicitudCreditoInfraestructura
    {
        private readonly ISolicitudCreditoRepositorio _solicitudCreditoRepositorio;
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly IPatioRepositorio _patioRepositorio;
        private readonly IVehiculoRepositorio _vehiculoRepositorio;
        private readonly IClientePatioInfraestructura _clientePatioInfraestructura;

        public SolicitudCreditoInfraestructura(
            ISolicitudCreditoRepositorio solicitudCreditoRepositorio,
            IClienteRepositorio clienteRepositorio,
            IPatioRepositorio patioRepositorio,
            IVehiculoRepositorio vehiculoRepositorio,
            IClientePatioInfraestructura clientePatioInfraestructura)
        {
            _solicitudCreditoRepositorio = solicitudCreditoRepositorio;
            _clienteRepositorio = clienteRepositorio;
            _patioRepositorio = patioRepositorio;
            _vehiculoRepositorio = vehiculoRepositorio;
            _clientePatioInfraestructura = clientePatioInfraestructura;
        }

        public async Task<ESolicitudCredito> CrearSolicitudCreditoAsync(ECrearSolicitudCreditoDto input)
        {
            var cliente = await _clienteRepositorio.ObtenerPorIdentificacionAsync(input.IdentificacionCliente);
            if (cliente == null)
                throw new CrAutoExcepcion(CrAutoErrores.ClienteNoExisteError);

            var solicitudExistente = await _solicitudCreditoRepositorio.ObtenerActivaPorClienteFechaAsync(
                cliente.Id, input.FechaElaboracion);

            if (solicitudExistente != null)
                throw new CrAutoExcepcion(CrAutoErrores.ClienteYaTieneSolicitudError);

            var patio = await _patioRepositorio.ObtenerPorPuntoVentaConEjecutivosAsync(
                input.NumeroPuntoVentaPatio);

            if (patio == null)
                throw new CrAutoExcepcion(CrAutoErrores.PatioNoExisteError);

            var ejecutivo = patio.Ejecutivos.FirstOrDefault(e => e.Id == input.EjecutivoId);
            if (ejecutivo == null)
                throw new CrAutoExcepcion(CrAutoErrores.EjecutivoNoDisponibleEnPatioError);

            var vehiculo = await _vehiculoRepositorio.ObtenerPorPlacaAsync(input.PlacaVehiculo);
            if (vehiculo == null)
                throw new CrAutoExcepcion(CrAutoErrores.VehiculoNoExisteError);

            var vehiculoReservado = await _solicitudCreditoRepositorio.ObtenerActivaPorVehiculoAsync(vehiculo.Id);
            if (vehiculoReservado != null)
                throw new CrAutoExcepcion(CrAutoErrores.VehiculoEnReservaError);

            var solicitudCredito = new ESolicitudCredito(
                Guid.NewGuid(),
                input.FechaElaboracion,
                cliente.Id,
                patio.Id,
                vehiculo.Id,
                input.MesesPlazo,
                input.Cuotas,
                input.Entrada,
                input.EjecutivoId,
                input.Observacion);

            await _solicitudCreditoRepositorio.InsertarAsync(solicitudCredito);

            try
            {
                var clientePatioInput = new EAsociarClientePatioDto()
                {
                    IdentificacionCliente = input.IdentificacionCliente,
                    NumeroPuntoVenta = input.NumeroPuntoVentaPatio
                };
                _ = await _clientePatioInfraestructura.AsociarClientePatioAsync(clientePatioInput);
            }
            catch (CrAutoExcepcion)
            {
                throw;
            }

            return solicitudCredito;
        }
    }
}
