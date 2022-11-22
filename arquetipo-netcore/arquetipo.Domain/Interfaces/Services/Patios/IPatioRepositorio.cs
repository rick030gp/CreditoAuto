using arquetipo.Entity.Models;

namespace arquetipo.Domain.Interfaces.Services.Patios
{
    public interface IPatioRepositorio : IGenericRepositorio<EPatio, Guid>
    {
        Task<EPatio?> ObtenerPorPuntoVentaAsync(short numeroPuntoVenta);
    }
}
