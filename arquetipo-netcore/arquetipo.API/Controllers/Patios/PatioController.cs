using arquetipo.Domain.Interfaces.Services.Patios;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace arquetipo.API.Controllers.Patios
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PatioController : ControllerBase
    {
        private readonly IPatioInfraestructura _patioInfraestructura;
        private readonly ILogger<PatioController> _logger;

        public PatioController(
            IPatioInfraestructura patioInfraestructura,
            ILogger<PatioController> logger)
        {
            _patioInfraestructura = patioInfraestructura;
            _logger = logger;
        }

        [HttpGet]
        [Route("ConsultarTodos001")]
        public async Task<IEnumerable<EPatio>> ConsultarPatiosAsync()
        {
            _logger.LogInformation("Consultando todos los patios");
            return await _patioInfraestructura.ConsultarPatiosAsync();
        }

        [HttpGet]
        [Route("Consultar002")]
        public async Task<IActionResult> ConsultarPatioPorPuntoVentaAsync(short numeroPuntoVenta)
        {
            try
            {
                _logger.LogInformation("Consultando patio {numeroPuntoVenta}", numeroPuntoVenta);                
                var patio = await _patioInfraestructura.ConsultarPatioPorPuntoVentaAsync(numeroPuntoVenta);
                return Ok(patio);
            }
            catch (CrAutoExcepcion ex)
            {
                _logger.LogError(ex, "Patio {numeroPuntoVenta} no encontrado", numeroPuntoVenta);
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
                return result;
            }
        }

        [HttpPost]
        [Route("Registrar203")]
        public async Task<IActionResult> CrearPatioAsync(ECrearPatioDto input)
        {
            try
            {
                _logger.LogInformation("Insertando patio {numeroPuntoVenta}", input.NumeroPuntoVenta);
                var patio = await _patioInfraestructura.CrearPatioAsync(input);
                return Ok(patio);
            }
            catch (CrAutoExcepcion ex)
            {
                _logger.LogError(ex, "Inserción de patio {numeroPuntoVenta} fallida", input.NumeroPuntoVenta);
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpPatch]
        [Route("Actualizar304/{numeroPuntoVenta}")]
        public async Task<IActionResult> ActualizarPatioAsync(short numeroPuntoVenta, JsonPatchDocument<EPatio> input)
        {
            try
            {
                _logger.LogInformation("Actualizando patio {numeroPuntoVenta}", numeroPuntoVenta);
                var patio = await _patioInfraestructura.ActualizarPatioAsync(numeroPuntoVenta, input);
                return Ok(patio);
            }
            catch (CrAutoExcepcion ex)
            {
                _logger.LogError(ex, "Actualización de patio {numeroPuntoVenta} fallida", numeroPuntoVenta);
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpDelete]
        [Route("Eliminar405/{numeroPuntoVenta}")]
        public async Task<IActionResult> EliminarPatioAsync(short numeroPuntoVenta)
        {
            try
            {
                _logger.LogInformation("Eliminando patio {numeroPuntoVenta}", numeroPuntoVenta);
                var result = await _patioInfraestructura.EliminarPatioAsync(numeroPuntoVenta);
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
