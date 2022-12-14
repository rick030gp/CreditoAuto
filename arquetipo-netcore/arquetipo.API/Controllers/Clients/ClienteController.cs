using arquetipo.Domain.Interfaces.Services.Clientes;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net;

namespace arquetipo.API.Controllers.Clients
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteInfraestructura _clienteInfraestructura;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(
            IClienteInfraestructura clienteInfraestructura,
            ILogger<ClienteController> logger)
        {
            _clienteInfraestructura = clienteInfraestructura;
            _logger = logger;
        }

        [HttpGet]
        [Route("ConsultarTodos001")]
        public async Task<IEnumerable<ECliente>> ConsultarClientesAsync()
        {
            _logger.LogInformation("Consultando todos los clientes");
            return await _clienteInfraestructura.ConsultarClientesAsync();
        }

        [HttpGet]
        [Route("Consultar002")]
        public async Task<IActionResult> ConsultarClientePorIdentificacionAsync(string identificacion)
        {
            try
            {
                _logger.LogInformation("Consultando cliente {identificacion}", identificacion);
                var cliente = await _clienteInfraestructura.ConsultarClientePorIdentificacionAsync(identificacion);
                return Ok(cliente);
            }
            catch (CrAutoExcepcion ex)
            {
                _logger.LogError(ex, "Cliente {identificacion} no encontrado", identificacion);
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Accepted;
                return result;
            }
        }

        [HttpPost]
        [Route("Registrar203")]
        public async Task<IActionResult> CrearClienteAsync(ECrearClienteDto input)
        {
            try
            {
                _logger.LogInformation("Insertando cliente {identificacion}", input.Identificacion);
                var cliente = await _clienteInfraestructura.CrearClienteAsync(input);
                return Ok(cliente);
            }
            catch (CrAutoExcepcion ex)
            {
                _logger.LogError(ex, "Inserción de cliente {identificacion} fallida", input.Identificacion);
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpPatch]
        [Route("Actualizar304/{identificacion}")]
        public async Task<IActionResult> ActualizarClienteAsync(string identificacion, JsonPatchDocument<ECliente> input)
        {
            try
            {
                _logger.LogInformation("Actualizando cliente {identificacion}", identificacion);
                var cliente = await _clienteInfraestructura.ActualizarClienteAsync(identificacion, input);
                return Ok(cliente);
            }
            catch (CrAutoExcepcion ex)
            {
                _logger.LogError(ex, "Actualización de cliente {identificacion} fallida", identificacion);
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpDelete]
        [Route("Eliminar405/{identificacion}")]
        public async Task<IActionResult> EliminarClienteAsync(string identificacion)
        {
            try
            {
                _logger.LogInformation("Eliminando cliente {identificacion}", identificacion);
                var result = await _clienteInfraestructura.EliminarClienteAsync(identificacion);
                return Ok(result);
            }
            catch (CrAutoExcepcion ex)
            {
                _logger.LogError(ex, "Eliminación de cliente {identificacion} fallida", identificacion);
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }
    }
}
