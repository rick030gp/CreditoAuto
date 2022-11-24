using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;

namespace arquetipo.Domain.Interfaces.Services.SolicitudCredito
{
    public interface ISolicitudCreditoInfraestructura
    {
        Task<ESolicitudCredito> CrearSolicitudCreditoAsync(ECrearSolicitudCreditoDto input);
    }
}
