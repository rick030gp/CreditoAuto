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

        public ClienteController(IClienteInfraestructura clienteInfraestructura)
        {
            _clienteInfraestructura = clienteInfraestructura;
        }

        [HttpGet]
        [Route("ConsultarTodos001")]
        public async Task<IEnumerable<ECliente>> ConsultarClientesAsync()
        {
            return await _clienteInfraestructura.ConsultarClientesAsync();
        }

        [HttpGet]
        [Route("Consultar002")]
        public async Task<IActionResult> ConsultarClientePorIdentificacionAsync(string identificacion)
        {
            try
            {
                var cliente = await _clienteInfraestructura.ConsultarClientePorIdentificacionAsync(identificacion);
                return Ok(cliente);
            }
            catch (CrAutoExcepcion ex)
            {
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
                var cliente = await _clienteInfraestructura.CrearClienteAsync(input);
                return Ok(cliente);
            }
            catch (CrAutoExcepcion ex)
            {
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
                var cliente = await _clienteInfraestructura.ActualizarClienteAsync(identificacion, input);
                return Ok(cliente);
            }
            catch (CrAutoExcepcion ex)
            {
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
                var result = await _clienteInfraestructura.EliminarClienteAsync(identificacion);
                return Ok(result);
            }
            catch (CrAutoExcepcion ex)
            {
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }
    }
}
