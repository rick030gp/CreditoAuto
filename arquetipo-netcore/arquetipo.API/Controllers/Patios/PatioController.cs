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

        public PatioController(IPatioInfraestructura patioInfraestructura)
        {
            _patioInfraestructura = patioInfraestructura;
        }

        [HttpGet]
        [Route("Consultar001")]
        public async Task<IEnumerable<EPatio>> ConsultarPatiosAsync()
        {
            return await _patioInfraestructura.ConsultarPatiosAsync();
        }

        [HttpGet]
        [Route("Consultar001/{numeroPuntoVenta}")]
        public async Task<IActionResult> ConsultarPatioPorPuntoVentaAsync(short numeroPuntoVenta)
        {
            try
            {
                var patio = await _patioInfraestructura.ConsultarPatioPorPuntoVentaAsync(numeroPuntoVenta);
                return Ok(patio);
            }
            catch (CrAutoExcepcion ex)
            {
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
                return result;
            }
        }

        [HttpPost]
        [Route("Registrar202")]
        public async Task<IActionResult> CrearPatioAsync(ECrearPatioDto input)
        {
            try
            {
                var patio = await _patioInfraestructura.CrearPatioAsync(input);
                return Ok(patio);
            }
            catch (CrAutoExcepcion ex)
            {
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpPatch]
        [Route("Actualizar303/{numeroPuntoVenta}")]
        public async Task<IActionResult> ActualizarPatioAsync(short numeroPuntoVenta, JsonPatchDocument<EPatio> input)
        {
            try
            {
                var patio = await _patioInfraestructura.ActualizarPatioAsync(numeroPuntoVenta, input);
                return Ok(patio);
            }
            catch (CrAutoExcepcion ex)
            {
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpDelete]
        [Route("Eliminar404")]
        public async Task<IActionResult> EliminarPatioAsync(short numeroPuntoVenta)
        {
            try
            {
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
