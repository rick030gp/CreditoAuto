using arquetipo.Domain.Interfaces.Services.SolicitudCredito;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace arquetipo.API.Controllers.SolicitudCredito
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SolicitudCreditoController : ControllerBase
    {
        private readonly ISolicitudCreditoInfraestructura _solicitudCreditoInfraestructura;
        private readonly ILogger<SolicitudCreditoController> _logger;

        public SolicitudCreditoController(
            ISolicitudCreditoInfraestructura solicitudCreditoInfraestructura,
            ILogger<SolicitudCreditoController> logger)
        {
            _solicitudCreditoInfraestructura = solicitudCreditoInfraestructura;
            _logger = logger;
        }

        [HttpPost]
        [Route("Registrar201")]
        public async Task<IActionResult> CrearSolicirudCreditoAsync(ECrearSolicitudCreditoDto input)
        {
            try
            {
                _logger.LogInformation("Registro de solicitud de crédito: Cliente: {identificacion}, Patio {numeroPuntoVenta}, Vehiculo: {placa}", input.IdentificacionCliente, input.NumeroPuntoVentaPatio, input.PlacaVehiculo);
                var solicitud = await _solicitudCreditoInfraestructura.CrearSolicitudCreditoAsync(input);
                return Ok(solicitud);
            }
            catch (CrAutoExcepcion ex)
            {
                _logger.LogInformation(ex, "Registro de solicitud de crédito fallida: Cliente: {identificacion}, Patio {numeroPuntoVenta}, Vehiculo: {placa}", input.IdentificacionCliente, input.NumeroPuntoVentaPatio, input.PlacaVehiculo);
                var result = Content(JsonSerializer.Serialize(new Error(ex)));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }
    }
}
