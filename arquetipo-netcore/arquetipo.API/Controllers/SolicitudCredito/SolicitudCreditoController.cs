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

        public SolicitudCreditoController(ISolicitudCreditoInfraestructura solicitudCreditoInfraestructura)
        {
            _solicitudCreditoInfraestructura = solicitudCreditoInfraestructura;
        }

        [HttpPost]
        [Route("Registrar201")]
        public async Task<IActionResult> CrearSolicirudCreditoAsync(ECrearSolicitudCreditoDto input)
        {
            try
            {
                var solicitud = await _solicitudCreditoInfraestructura.CrearSolicitudCreditoAsync(input);
                return Ok(solicitud);
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
