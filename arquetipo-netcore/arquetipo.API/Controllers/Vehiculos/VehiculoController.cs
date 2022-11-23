using arquetipo.Domain.Interfaces.Services.Vehiculos;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace arquetipo.API.Controllers.Vehiculos
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VehiculoController : ControllerBase
    {
        private readonly IVehiculoInfraestructura _vehiculoInfraestructura;

        public VehiculoController(IVehiculoInfraestructura vehiculoInfraestructura)
        {
            _vehiculoInfraestructura = vehiculoInfraestructura;
        }

        [HttpGet]
        [Route("ConsultarTodos001")]
        public async Task<IEnumerable<EVehiculo>> ConsultarVehiculosAsync()
        {
            return await _vehiculoInfraestructura.ConsultarVehiculosAsync();
        }

        [HttpGet]
        [Route("Consultar002")]
        public async Task<IActionResult> ConsultarVehiculoPorPlacaAsync(string placa)
        {
            try
            {
                var vehiculo = await _vehiculoInfraestructura.ConsultarVehiculoPorPlacaAsync(placa);
                return Ok(vehiculo);
            }
            catch (CrAutoExcepcion ex)
            {
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
                return result;
            }
        }

        [HttpPost]
        [Route("Registrar203")]
        public async Task<IActionResult> CrearVehiculoAsync(ECrearVehiculoDto input)
        {
            try
            {
                var vehiculo = await _vehiculoInfraestructura.CrearVehiculoAsync(input);
                return Ok(vehiculo);
            }
            catch (CrAutoExcepcion ex)
            {
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpPatch]
        [Route("Actualizar304/{placa}")]
        public async Task<IActionResult> ActualizarPatioAsync(string placa, JsonPatchDocument<EVehiculo> input)
        {
            try
            {
                var vehiculo = await _vehiculoInfraestructura.ActualizarVehiculoAsync(placa, input);
                return Ok(vehiculo);
            }
            catch (CrAutoExcepcion ex)
            {
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpDelete]
        [Route("Eliminar405/{placa}")]
        public async Task<IActionResult> EliminarPatioAsync(string placa)
        {
            try
            {
                var result = await _vehiculoInfraestructura.EliminarVehiculoAsync(placa);
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
