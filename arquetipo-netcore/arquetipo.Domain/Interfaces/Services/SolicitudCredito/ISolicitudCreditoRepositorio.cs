using arquetipo.Entity.Models;

namespace arquetipo.Domain.Interfaces.Services.SolicitudCredito
{
    public interface ISolicitudCreditoRepositorio : IGenericRepositorio<ESolicitudCredito, Guid>
    {
        Task<ESolicitudCredito?> ObtenerActivaPorClienteFechaAsync(Guid clienteId, DateTime fecha);
        Task<ESolicitudCredito?> ObtenerActivaPorVehiculoAsync(Guid vehiculoId);
    }
}
