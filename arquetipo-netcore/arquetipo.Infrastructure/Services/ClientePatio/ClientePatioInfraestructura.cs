using arquetipo.Domain.Interfaces.Services.ClientePatio;
using arquetipo.Domain.Interfaces.Services.Clientes;
using arquetipo.Domain.Interfaces.Services.Patios;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.JsonPatch;

namespace arquetipo.Infrastructure.Services.ClientePatio
{
    public class ClientePatioInfraestructura : IClientePatioInfraestructura
    {
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly IPatioRepositorio _patioRepositorio;
        private readonly IClientePatioRepositorio _clientePatioRepositorio;

        public ClientePatioInfraestructura(
            IClienteRepositorio clienteRepositorio,
            IPatioRepositorio patioRepositorio,
            IClientePatioRepositorio clientePatioRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
            _patioRepositorio = patioRepositorio;
            _clientePatioRepositorio = clientePatioRepositorio;
        }

        public async Task<EClientePatio> ActualizarAsociacionClientePatioAsync(Guid id, JsonPatchDocument<EClientePatio> input)
        {
            throw new NotImplementedException();
        }

        public async Task<EClientePatio> AsociarClientePatioAsync(EAsociarClientePatioDto input)
        {
            var cliente = await _clienteRepositorio.ObtenerPorIdentificacionAsync(input.IdentificacionCliente);
            if (cliente == null)
                throw new CrAutoExcepcion(CrAutoErrores.ClienteNoExisteError);

            var patio = await _patioRepositorio.ObtenerPorPuntoVentaAsync(input.NumeroPuntoVenta);
            if (patio == null)
                throw new CrAutoExcepcion(CrAutoErrores.PatioNoExisteError);

            var clientePatioExistente = await _clientePatioRepositorio.ObtenerPorParametrosAsync(cliente.Id, patio.Id);
            if (clientePatioExistente != null)
                return clientePatioExistente;

            var clientePatio = new EClientePatio(Guid.NewGuid(), cliente.Id, patio.Id, input.FechaAsignacion);

            return await _clientePatioRepositorio.InsertarAsync(clientePatio);
        }

        public async Task<string> EliminarAsociacionClientePatioAsync(Guid id)
        {
            var clientePatio = await _clientePatioRepositorio.ObtenerPorId(id);
            if (clientePatio == null)
                throw new CrAutoExcepcion(CrAutoErrores.AsociacionClientePatioNoExiste);

            await _clientePatioRepositorio.EliminarAsync(clientePatio);
            return EConstante.CLIENTE_PATIO_ELIMINADO;
        }
    }
}
