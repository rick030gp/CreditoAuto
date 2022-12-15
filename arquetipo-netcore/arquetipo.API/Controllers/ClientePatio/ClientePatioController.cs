using arquetipo.Domain.Interfaces.Services.ClientePatio;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace arquetipo.API.Controllers.ClientePatio
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ClientePatioController : ControllerBase
    {
        private readonly IClientePatioInfraestructura _clientePatioInfraestructura;
        private readonly ILogger<ClientePatioController> _logger;

        public ClientePatioController(
            IClientePatioInfraestructura clientePatioInfraestructura,
            ILogger<ClientePatioController> logger)
        {
            _clientePatioInfraestructura = clientePatioInfraestructura;
            _logger = logger;
        }

        [HttpPost]
        [Route("Registrar201")]
        public async Task<IActionResult> AsociarClientePatioAsync(EAsociarClientePatioDto input)
        {
            try
            {
                _logger.LogInformation("Asociación de cliente {identificacion} a patio", input.IdentificacionCliente);
                var clientePatio = await _clientePatioInfraestructura.AsociarClientePatioAsync(input);
                return Ok(clientePatio);
            }
            catch (CrAutoExcepcion ex)
            {
                _logger.LogError(ex, "Error al asociar cliente {identificacion} a patio", input.IdentificacionCliente);
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpPatch]
        [Route("Actualizar302/{id}")]
        public async Task<IActionResult> ActualizarAsociacionClientePatioAsync(Guid id, JsonPatchDocument<EClientePatio> input)
        {
            try
            {
                _logger.LogInformation("Actualización de asociación de cliente {id} a patio", id);
                var clientePatio = await _clientePatioInfraestructura.ActualizarAsociacionClientePatioAsync(id, input);
                return Ok(clientePatio);
            }
            catch (CrAutoExcepcion ex)
            {
                _logger.LogError(ex, "Error al actualizar asociación de cliente {id} a patio", id);
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpDelete]
        [Route("Eliminar403/{id}")]
        public async Task<IActionResult> EliminarAsociacionClientePatioAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Eliminando asociación de cliente {id} a patio", id);
                var result = await _clientePatioInfraestructura.EliminarAsociacionClientePatioAsync(id);
                return Ok(result);
            }
            catch (CrAutoExcepcion ex)
            {
                _logger.LogError(ex, "Error al eliminar asociación de cliente {id} a patio", id);
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }
    }
}
