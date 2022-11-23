using arquetipo.Domain.Interfaces.Services.Clientes;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Infrastructure.Services.Clientes
{
    public class ClienteInfraestructura : IClienteInfraestructura
    {
        private readonly IClienteRepositorio _clienteRepositorio;

        public ClienteInfraestructura(IClienteRepositorio clienteRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
        }

        public async Task<ECliente> ActualizarClienteAsync(string identificacion, JsonPatchDocument<ECliente> input)
        {
            if (!input.Operations.Any())
                throw new CrAutoExcepcion(CrAutoErrores.ActualizacionDatosVaciosError);

            var cliente = await _clienteRepositorio.ObtenerPorIdentificacionAsync(identificacion);
            if (cliente == null)
                throw new CrAutoExcepcion(CrAutoErrores.ClienteNoExisteError);

            input.ApplyTo(cliente);

            return await _clienteRepositorio.ActualizarAsync(cliente);
        }

        public async Task<ECliente> ConsultarClientePorIdentificacionAsync(string identificacion)
        {
            var cliente = await _clienteRepositorio.ObtenerPorIdentificacionAsync(identificacion);
            if (cliente == null)
                throw new CrAutoExcepcion(CrAutoErrores.ClienteNoExisteError);

            return cliente;
        }

        public async Task<IEnumerable<ECliente>> ConsultarClientesAsync()
        {
            return await _clienteRepositorio.ObtenerTodoAsync();
        }

        public async Task<ECliente> CrearClienteAsync(ECrearClienteDto input)
        {
            var clienteExistente = await _clienteRepositorio.ObtenerPorIdentificacionAsync(input.Identificacion);
            if (clienteExistente != null)
                throw new CrAutoExcepcion(CrAutoErrores.ClienteYaExisteError);

            var cliente = new ECliente(
                Guid.NewGuid(),
                input.Identificacion,
                input.Nombres,
                input.Apellidos,
                input.Edad,
                input.FechaNacimiento,
                input.Direccion,
                input.Telefono,
                input.EstadoCivil,
                input.IdentificacionConyugue,
                input.NombreConyugue,
                input.EsSujetoCredito);

            return await _clienteRepositorio.InsertarAsync(cliente);
        }

        public async Task<string> EliminarClienteAsync(string identificacion)
        {
            var cliente = await _clienteRepositorio.ObtenerPorIdentificacionAsync(identificacion);
            if (cliente == null)
                throw new CrAutoExcepcion(CrAutoErrores.ClienteNoExisteError);

            try
            {
                await _clienteRepositorio.EliminarAsync(cliente);
                return EConstante.CLIENTE_ELIMINADO;
            }
            catch (DbUpdateException ex)
            {
                throw new CrAutoExcepcion(CrAutoErrores.EliminarRelacionesExistentesError, ex);
            }
        }
    }
}
