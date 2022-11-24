using arquetipo.Domain.Interfaces.Services.Clientes;
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
        private readonly IVehiculoRepositorio _vehiculoRepositorio;

        public SolicitudCreditoInfraestructura(
            ISolicitudCreditoRepositorio solicitudCreditoRepositorio,
            IClienteRepositorio clienteRepositorio,
            IVehiculoRepositorio vehiculoRepositorio)
        {
            _solicitudCreditoRepositorio = solicitudCreditoRepositorio;
            _clienteRepositorio = clienteRepositorio;
            _vehiculoRepositorio = vehiculoRepositorio;
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

            var solicitudCredito = new ESolicitudCredito(
                Guid.NewGuid(),
                input.FechaElaboracion,
                cliente.Id,
                input.PatioId,
                Guid.NewGuid(),
                input.MesesPlazo,
                input.Cuotas,
                input.Entrada,
                input.EjecutivoId,
                input.Observacion);

            return await _solicitudCreditoRepositorio.InsertarAsync(solicitudCredito);
        }
    }
}
